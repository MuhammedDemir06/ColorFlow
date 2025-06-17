using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] private string[] popupMessages;

    private void Awake()
    {
        Instance = this;
    }
    public bool IsLevelCompleted(Dictionary<int,List<Tile>> selectedTilesPerID)
    {
        for (int colorID = 1; colorID <= TileManager.Instance.TotalColors; colorID++)
        {
            if (!selectedTilesPerID.ContainsKey(colorID))
                return false;

            var tiles = selectedTilesPerID[colorID];
            int ballCount = tiles.Count(t => t.IsHaveBall);

            if (ballCount < 2)
            {
                DebugManager.Instance.DebugLog($"Line {colorID} not Matched");
                return false;
            }
        }
        GameUIManager.Instance.LevelFinish(true);

        var random = Random.Range(0, popupMessages.Length);
        PopupText.Instance.ShowPopup(popupMessages[random]);

        DebugManager.Instance.DebugLogError("All Lines Matched, Level Finished!");
        return true;
    }

    public bool ColorMatched(int lastTileID, Tile lastPassedTile, Dictionary<int, bool> colorMatched, Dictionary<int, List<Tile>> selectedTilesPerID, Dictionary<int, Tile> savedLines, int currentID)
    {
        if (selectedTilesPerID.ContainsKey(currentID) && lastTileID == currentID)
        {
            if (colorMatched.ContainsKey(currentID))
                colorMatched[currentID] = true;

            DebugManager.Instance.DebugLog("Color Matched");
            GameUIManager.Instance.UpdateMoveCount(1);
            return true;
        }
        else
        {
            //that means, last Tile dont have a line or ball
            SaveLine(lastPassedTile, savedLines, currentID);
        }
        return false;
    }
    private void SaveLine(Tile lastPassedTile, Dictionary<int, Tile> savedLines, int currentID)
    {
        if (lastPassedTile != null)
            lastPassedTile.SavedTileLineID = currentID;
        if (!savedLines.ContainsKey(currentID))
            savedLines.Add(currentID, lastPassedTile);

        DebugManager.Instance.DebugLogWarning("Saved Line");
    }
}
