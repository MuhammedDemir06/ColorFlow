using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [Header("Input Manager")]
    public bool CanSelect;
    [HideInInspector] public int CurrentID;
    private Tile lastPassedTile;
    private int lastTileID;

    private List<Tile> selectedTiles = new List<Tile>();
    private Dictionary<int, List<Tile>> selectedTilesPerID = new Dictionary<int, List<Tile>>();
    private Dictionary<int, Tile> savedLines = new Dictionary<int, Tile>();
    private Dictionary<int, bool> colorMatched = new Dictionary<int, bool>();
    private LineRenderer colorLine = new LineRenderer();

    private List<int> currentTilesID;
    private List<Tile> spawnedTiles;

    private List<int> lineOrder = new List<int>();
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        //Test
        UpdateCurrentTileID();
    }
    private void UpdateCurrentTileID()
    {
        spawnedTiles = TileManager.Instance.SpawnedTiles();
        currentTilesID = TileManager.Instance.SetCurrentSavedTilesID();

        for (int i = 0; i < currentTilesID.Count; i++)
        {
            colorMatched.Add(currentTilesID[i], false);
        }
    }
    public void StartTileSelection(Tile newTile)
    {
        if (newTile.IsHaveBall)
        {
            CurrentID = newTile.TileID;
            CanSelect = true;

            if (!selectedTilesPerID.ContainsKey(CurrentID))
            {
                selectedTiles = new List<Tile>();
                selectedTilesPerID.Add(CurrentID, selectedTiles); 

                colorLine = newTile.GetComponent<LineRenderer>();
                LineDrawer.Instance.AddLine(CurrentID, colorLine);

                ColorCircle.Instance.SetSelectedCircleColor(colorLine.startColor);

                if (!lineOrder.Contains(CurrentID))
                    lineOrder.Add(CurrentID);
            }
            else
            {
                if (lineOrder.Contains(CurrentID))
                {
                    lineOrder.Remove(CurrentID);
                    lineOrder.Add(CurrentID);
                }

                if (colorMatched.ContainsKey(CurrentID))
                    colorMatched[CurrentID] = false;

                SetCurrentTileLines(false,false);

                selectedTilesPerID[CurrentID].Clear();

                colorLine = LineDrawer.Instance.GetLine(CurrentID);

                selectedTiles = selectedTilesPerID[CurrentID];

                if (ColorCircle.Instance.EnableCircle)
                    ColorCircle.Instance.SetSelectedCircleColor(colorLine.startColor);

                ResetSavedLineTile();
            }

            TileSelection(newTile);

            //  Debug.Log("Tile Selection Started!");
            DebugManager.Instance.DebugLog("Tile Selection Started!");
        }
        else
        {
            selectedTiles = new List<Tile>();
            if(newTile.SavedTileLineID != 0)
            {
                CurrentID = newTile.SavedTileLineID;
                CanSelect = true;
                colorLine = LineDrawer.Instance.GetLine(CurrentID);
                selectedTiles = selectedTilesPerID[CurrentID];

                ColorCircle.Instance.SetSelectedCircleColor(colorLine.startColor);

                ResetSavedLineTile();
            }
            else
            {
                // Debug.LogWarning("Tile Has No A Color Ball Or Saved Line");
                DebugManager.Instance.DebugLogWarning("Tile Has No A Color Ball Or Saved Line");
            }
        }
    }
    private void ResetSavedLineTile()
    {
        if (savedLines.ContainsKey(CurrentID))
        {
            savedLines[CurrentID].SavedTileLineID = 0;
            savedLines.Remove(CurrentID);
        }
    }
    private void LineDraw()
    {
        LineDrawer.Instance.UpdateLine(colorLine, selectedTiles);
    }
    public void TileSelection(Tile newTile)
    {
        if (!newTile.IsHaveBall)
            lastPassedTile = newTile;

        lastTileID = newTile.TileID;

        TryUndo(newTile);

        if (newTile.IsHaveBall && newTile.TileID != CurrentID)
        {
            //  Debug.LogWarning("Different color touched. Ending line.");
            DebugManager.Instance.DebugLogWarning("Different color touched. Ending line.");
            EndTileSelection();
            return;
        }

        if (newTile.HasLine && CurrentID != newTile.LineID)
            RemoveCurrentLine();

        if (selectedTiles.Contains(newTile))
            return;

        selectedTiles.Add(newTile);
        LineDraw();

       // Debug.Log("Tile Selection Contiunes");
        DebugManager.Instance.DebugLog("Tile Selection Contiunes");
    }
    private Tile RemoveSavedTileID(Tile newTile)
    {
        if (newTile.SavedTileLineID > 0)
            newTile.SavedTileLineID = 0;

        return newTile;
    }
    private void RemoveCurrentLine()
    {
        if (lineOrder.Contains(CurrentID))
            lineOrder.Remove(CurrentID);

        if (selectedTilesPerID.ContainsKey(CurrentID))
            selectedTilesPerID.Remove(CurrentID);

        if (savedLines.ContainsKey(CurrentID))
            savedLines.Remove(CurrentID);

        LineDrawer.Instance.LineRemover(CurrentID);

        CanSelect = false;
        selectedTiles.Clear();
        CurrentID = 0;

       // Debug.Log("Removed Current Line");
        DebugManager.Instance.DebugLog("Removed Current Line");
    }
    public void EndTileSelection()
    {
        CanSelect = false;
        if (selectedTilesPerID[CurrentID].Count <= 1)
        {
            selectedTilesPerID.Remove(CurrentID);
            LineDrawer.Instance.LineRemover(CurrentID);

            if (lineOrder.Contains(CurrentID))
                lineOrder.Remove(CurrentID);
        }

        LevelManager.Instance.ColorMatched(lastTileID, lastPassedTile, colorMatched, selectedTilesPerID, savedLines, CurrentID);
        //Current set Color Lines
        SetCurrentTileLines(true,false);

        if (DebugManager.Instance.DebugMode)
            ShowList();

        LevelManager.Instance.IsLevelCompleted(selectedTilesPerID);

        CurrentID = 0;
        DebugManager.Instance.DebugLog("Tile Selection Ended!");
    }
    private void SetCurrentTileLines(bool hasLine, bool isUndo)
    {
        if (selectedTiles == null || selectedTiles.Count == 0)
            return;

        if (isUndo)
        {
            var lastTile = selectedTiles[selectedTiles.Count - 1];
            lastTile.UpdateLineState(false, 0);
        }
        else
        {
            foreach (var tile in selectedTiles)
            {
                tile.UpdateLineState(hasLine, CurrentID);
            }
        }
    }
    private void TryUndo(Tile currentTile)
    {
        var path = selectedTilesPerID[CurrentID];

        if (path.Count >= 2)
        {
            Tile lastTile = path[path.Count - 1];
            Tile secondLastTile = path[path.Count - 2];

            if (currentTile == secondLastTile)
            {
                SetCurrentTileLines(true, true);

                path.RemoveAt(path.Count - 1);

                RemoveSavedTileID(selectedTiles[selectedTiles.Count - 1]);

                selectedTiles.RemoveAt(selectedTiles.Count - 1);
                LineDraw();
                // Debug.Log("Undo Done");
                DebugManager.Instance.DebugLog("Undo Done");
            }
        }
    }
    private void ShowList()
    {
        foreach (var pair in selectedTilesPerID)
        {
            int id = pair.Key;
            int count = pair.Value.Count;
            Debug.Log($"ID: {id}, Tile Count: {count}");
        }
    }
    public void FindLine()
    {
        selectedTiles.Clear();

        var path = TileManager.Instance.SetPathFind();

        foreach (var kvp in colorMatched)
        {
            int colorID = kvp.Key;
            bool isMatched = kvp.Value;

            if (!isMatched && colorID != 0)
            {
                for (int i = 0; i < path.Count; i++)
                {
                    if (path[i] == colorID)
                    {
                        selectedTiles.Add(spawnedTiles[i]);

                        DebugManager.Instance.DebugLog($"Path index: {i} for ColorID: {colorID}");
                    }
                }

                CurrentID = colorID;

                if (colorMatched.ContainsKey(CurrentID))
                    colorMatched[CurrentID] = true;

                colorLine = selectedTiles[0].GetComponent<LineRenderer>();
                selectedTilesPerID.Add(CurrentID, selectedTiles);
                LineDrawer.Instance.AddLine(CurrentID, colorLine);
                SetCurrentTileLines(true, false);
                LineDraw();

                LevelManager.Instance.IsLevelCompleted(selectedTilesPerID);
                break;
            }
        }

        DebugManager.Instance.DebugLog("Line Found");
    }
    public void ReturnLine()
    {
        if (selectedTilesPerID.Count >= 1)
        {
            selectedTiles.Clear();
            CurrentID = lineOrder.Last();
            lineOrder.Remove(lineOrder.Last());

            SetCurrentTileLines(false, false);
            LineDrawer.Instance.LineEraser(CurrentID);

            selectedTilesPerID.Remove(CurrentID);
            LineDrawer.Instance.LineRemover(CurrentID);

            AnimatedMessagePanel.Instance.ShowMessage("Line Returned", false);
            DebugManager.Instance.DebugLog("Line Returned");
        }
        else
            AnimatedMessagePanel.Instance.ShowMessage("No Line Found", true);
    }
}