using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    public static LineDrawer Instance;

    private Dictionary<int, LineRenderer> selectedTilesPerColor = new Dictionary<int, LineRenderer>();
    private void Awake()
    {
        Instance = this;
    }
    public void UpdateLine(LineRenderer colorLine,List<Tile> selectedTiles)
    {
        for (int i = 0; i < selectedTiles.Count; i++)
        {
            colorLine.SetPosition(i, selectedTiles[i].transform.position);
        }
    }
    public void LineEraser(int currentID)
    {
        selectedTilesPerColor[currentID].positionCount = 0;
    }
    public void LineRemover(int currentID)
    {
        if (selectedTilesPerColor.ContainsKey(currentID))
            selectedTilesPerColor.Remove(currentID);
        else
            DebugManager.Instance.DebugLogError("Line Not Found");
    }
    public LineRenderer GetLine(int currentID)
    {
        return selectedTilesPerColor[currentID];
    }
    public void AddLine(int currentID,LineRenderer colorLine)
    {
        selectedTilesPerColor.Add(currentID, colorLine);
    }
}
