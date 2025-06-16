using UnityEngine;
public class Tile : MonoBehaviour
{
    [HideInInspector] public int TileID;
    [HideInInspector] public bool IsHaveBall;
    [HideInInspector] public Color TileColor;
    [SerializeField] private LineRenderer colorLine;
    [Header("If there is a Saved Tile")]
    [HideInInspector] public int SavedTileLineID;
    [HideInInspector] public bool HasLine;
    [HideInInspector] public int LineID;
    [Header("Saved Way ID")]
    [HideInInspector] public int SavedPathID;

    public void SetColor(Color newColor)
    {
        colorLine.startColor = newColor;
        colorLine.endColor = newColor;

        TileColor = newColor;
    }
    private void OnMouseDown()
    {
        if (InputManager.Instance.CanSelect || GameUIManager.Instance.GamePaused ||GameUIManager.Instance.LevelFinished)
            return;
        InputManager.Instance.StartTileSelection(GetComponent<Tile>());
    }
    private void OnMouseEnter()
    {
        if (!InputManager.Instance.CanSelect || GameUIManager.Instance.GamePaused || GameUIManager.Instance.LevelFinished)
            return;

        InputManager.Instance.TileSelection(GetComponent<Tile>());
    }
    private void OnMouseUp()
    {
        if (!InputManager.Instance.CanSelect || GameUIManager.Instance.GamePaused || GameUIManager.Instance.LevelFinished)
            return;
        InputManager.Instance.EndTileSelection();
    }
    public void UpdateLineState(bool hasLine, int lineID)
    {
        if (IsHaveBall)
            return;

        HasLine = hasLine;
        LineID = hasLine ? lineID : 0;
    }
}
