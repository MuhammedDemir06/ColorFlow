using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelData", menuName = "Custom/Level Data")]
public class LevelData : ScriptableObject
{
    public int BestMove;            
    public int TotalColors;         

    [SerializeField]
    private Color[] tilesColor;     
    [SerializeField]
    private int[] tilesID;         

    [SerializeField]
    private List<int> savedWays;  

    [SerializeField]
    private List<int> currentSavedTilesID; 

    // Dışarıdan erişim için property'ler
    public Color[] TilesColor => tilesColor;
    public int[] TilesID => tilesID;
    public List<int> SavedWays => savedWays;
    public List<int> CurrentSavedTilesID => currentSavedTilesID;
    public void SetTileData(Color[] colors, int[] ids, List<int> ways, List<int> currentIDs)
    {
        tilesColor = colors;
        tilesID = ids;
        savedWays = new List<int>(ways);
        currentSavedTilesID = new List<int>(currentIDs);

        TotalColors = currentSavedTilesID.Count;
    }
}
