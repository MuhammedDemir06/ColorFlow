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
    private LineRenderer colorLine = new LineRenderer();
    private void Awake()
    {
        Instance = this;
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
            }
            else
            {
                SetCurrentTileLines(false,false); //HAS LINE

                selectedTilesPerID[CurrentID].Clear();

                colorLine = LineDrawer.Instance.GetLine(CurrentID);

                selectedTiles = selectedTilesPerID[CurrentID];

                ResetSavedLineTile();
            }

            TileSelection(newTile);

            Debug.Log("Tile Selection Started!");
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

                ResetSavedLineTile();
            }
            else
            {
                Debug.LogWarning("Tile Has No A Color Ball Or Saved Line");
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
            Debug.LogWarning("Different color touched. Ending line.");
            EndTileSelection();
            return;
        }

        if (newTile.HasLine && CurrentID != newTile.LineID)
            RemoveCurrentLine();

        if (selectedTiles.Contains(newTile))
            return;

        selectedTiles.Add(newTile);
        LineDraw(newTile);

        Debug.Log("Tile Selection Contiunes");
    }
    private void RemoveCurrentLine()
    {
        if (selectedTilesPerID.ContainsKey(CurrentID))
        {
            selectedTilesPerID.Remove(CurrentID);
        }

        LineDrawer.Instance.LineRemover(CurrentID);

        CanSelect = false;
        selectedTiles.Clear();
        CurrentID = 0;

        Debug.Log("Removed Current Line");
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
        ShowList();

        IsLevelCompleted();

        CurrentID = 0;
        Debug.Log("Tile Selection Ended!");
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
                Debug.Log($"Line {colorID} not Matched");
                return false;
            }
        }

        Debug.Log("All Lines Matched, Level Finished!");
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
                tile.UpdateLineState(hasLine, CurrentID);
        }
    }
    private bool ColorMatched()
    {
        if (selectedTilesPerID.ContainsKey(CurrentID) && lastTileID == CurrentID)
        {
            Debug.LogWarning("Color Matched");
            return true;
        }
        else if (lastTileID == 0)
        {
            //that means, last Tile dont have a line or ball
            SaveLine();
            return false;
        }
        return false;
    }
    private void SaveLine()
    {
        lastPassedTile.SavedTileLineID = CurrentID;
        savedLines.Add(CurrentID, lastPassedTile);

        Debug.LogWarning("Saved Line");
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
                selectedTiles.RemoveAt(selectedTiles.Count - 1);
                LineDraw(currentTile);
                Debug.Log("Undo Done");
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
}