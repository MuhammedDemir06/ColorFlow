using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [Header("Input Manager")]
    public bool CanSelect;
    public int CurrentID;
    private int lastTileID;

    [SerializeField] private List<Tile> selectedTiles = new List<Tile>();
    private Dictionary<int, List<Tile>> selectedTilesPerID = new Dictionary<int, List<Tile>>();
    private Dictionary<int, LineRenderer> selectedTilesPerColor = new Dictionary<int, LineRenderer>();
    private LineRenderer colorLine;
    private void Awake()
    {
        Instance = this;
    }
    public void StartTileSelection(Tile newTile)
    {
        if (newTile.IsHaveBall)
        {
            CanSelect = true;
            CurrentID = newTile.TileID;

            if (!selectedTilesPerID.ContainsKey(CurrentID))
            {
                selectedTilesPerID.Add(CurrentID, selectedTiles);
                selectedTiles = new List<Tile>();

                colorLine = newTile.GetComponent<LineRenderer>();
                selectedTilesPerColor.Add(CurrentID, colorLine);
                print("0");
            }
            else
            {
                print("1");

                selectedTilesPerID[CurrentID].Clear();
                selectedTilesPerColor[CurrentID].positionCount = 0;

                selectedTiles = new List<Tile>();
            }

            TileSelection(newTile);

            Debug.Log("Tile Selection Started!");
        }
        else
        {
            Debug.LogWarning("Tile Has No A Color Ball");
        }
    }
    private void LineDraw(Tile newTile)
    {
        colorLine.positionCount = selectedTiles.Count;
        LineDrawer.Instance.UpdateLine(colorLine, selectedTiles);
    }
    public void TileSelection(Tile newTile)
    {
        lastTileID = newTile.TileID;

        TryUndo(newTile);

        if (selectedTiles.Contains(newTile))
            return;

        selectedTilesPerID[CurrentID].Add(newTile);
        selectedTiles.Add(newTile);
        LineDraw(newTile);

        Debug.Log("Tile Selection Contiunes");
    }
    public void EndTileSelection()
    {
        CanSelect = false;
        if (selectedTilesPerID[CurrentID].Count <= 1)
        {
            selectedTilesPerID.Remove(CurrentID);
            selectedTilesPerColor.Remove(CurrentID);
        }

        //level Finished?
        if (lastTileID == CurrentID)
        {
            Debug.LogWarning("Color Matched");
        }

        ShowList();
        CurrentID = 0;
        Debug.Log("Tile Selection Ended!");
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
