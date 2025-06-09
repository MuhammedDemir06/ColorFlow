using UnityEngine;
public class Tile : MonoBehaviour
{
    public int TileID;
    public bool IsHaveBall;
    public Color TileColor;
    [SerializeField] private LineRenderer colorLine;
    [Header("If there is a Saved Tile")]
    public int SavedTileLineID;
    public bool HasLine;
    public int LineID;

    public void SetColor(Color newColor)
    {
        colorLine.startColor = newColor;
        colorLine.endColor = newColor;

        TileColor = newColor;
    }
    private void OnMouseDown()
    {
        if (InputManager.Instance.CanSelect)
            return;
        InputManager.Instance.StartTileSelection(GetComponent<Tile>());
    }
    private void OnMouseEnter()
    {
        if (!InputManager.Instance.CanSelect)
            return;

        InputManager.Instance.TileSelection(GetComponent<Tile>());
    }
    private void OnMouseUp()
    {
        if (!InputManager.Instance.CanSelect)
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
