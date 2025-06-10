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
    [SerializeField] private List<int> currentTilesID;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        //Test
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
            }
            else
            {
                if (colorMatched.ContainsKey(CurrentID))
                    colorMatched[CurrentID] = false;

                SetCurrentTileLines(false,false); //HAS LINE

                selectedTilesPerID[CurrentID].Clear();

                colorLine = LineDrawer.Instance.GetLine(CurrentID);

                selectedTiles = selectedTilesPerID[CurrentID];

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
    private void LineDraw(Tile newTile)
    {
        colorLine.positionCount = selectedTiles.Count;
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
        LineDraw(newTile);

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
        }
        
        ColorMatched();
        //Current set Color Lines
        SetCurrentTileLines(true,false);

        if (DebugManager.Instance.DebugMode)
            ShowList();

        IsLevelCompleted();

        CurrentID = 0;
        // Debug.Log("Tile Selection Ended!");
        DebugManager.Instance.DebugLog("Tile Selection Ended!");
    }
    private bool IsLevelCompleted()
    {
        for (int colorID = 1; colorID <= TileManager.Instance.TotalColors; colorID++)
        {
            if (!selectedTilesPerID.ContainsKey(colorID))
                return false; 

            var tiles = selectedTilesPerID[colorID];
            int ballCount = tiles.Count(t => t.IsHaveBall);

            if (ballCount < 2)
            {
                // Debug.Log($"Line {colorID} not Matched");
                DebugManager.Instance.DebugLog($"Line {colorID} not Matched");
                return false;
            }
        }

        //  Debug.LogError("All Lines Matched, Level Finished!");
        DebugManager.Instance.DebugLogError("All Lines Matched, Level Finished!");
        return true;
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
    private bool ColorMatched()
    {
        if (selectedTilesPerID.ContainsKey(CurrentID) && lastTileID == CurrentID)
        {
            if (colorMatched.ContainsKey(CurrentID))
                colorMatched[CurrentID] = true;

            DebugManager.Instance.DebugLog("Color Matched");
            return true;
        }
        else
        {
            //that means, last Tile dont have a line or ball
            SaveLine();
        }
        return false;
    }
    private void SaveLine()
    {
        if (lastPassedTile != null)
            lastPassedTile.SavedTileLineID = CurrentID;
        if (!savedLines.ContainsKey(CurrentID))
            savedLines.Add(CurrentID, lastPassedTile);

       // Debug.LogWarning("Saved Line");
        DebugManager.Instance.DebugLogWarning("Saved Line");
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
                LineDraw(currentTile);
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

    public List<int> NewTestPath;
    public void FindLine()
    {
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
                        Debug.Log($"Path index: {i} for ColorID: {colorID}");  // En son burda kaldıkkkkkk
                    }
                }
                break;
            }
        }

        DebugManager.Instance.DebugLog("Line Found");
    }
    public void ReturnLine()
    {
        DebugManager.Instance.DebugLog("Line Returned");
    }
    public void RestartGrid()
    {
        DebugManager.Instance.DebugLog("Grid Restarted");
    }
}