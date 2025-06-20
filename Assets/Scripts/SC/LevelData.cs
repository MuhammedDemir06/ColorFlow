using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelData", menuName = "Custom/Level Data")]
public class LevelData : ScriptableObject
{
    public int Width;
    public int Height;

    public int BestMove;
    public int TotalColors;

    public List<Color> TilesColor;
    public List<int> TilesID;

    public List<int> SavedWays;
}
