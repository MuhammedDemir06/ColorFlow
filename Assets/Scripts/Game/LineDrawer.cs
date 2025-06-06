using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    public static LineDrawer Instance;

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
}
