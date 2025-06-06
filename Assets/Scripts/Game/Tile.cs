using UnityEngine;

public class Tile : MonoBehaviour
{
    public int TileID;
    public bool IsHaveBall;
    public Color TileColor;
    [SerializeField] private LineRenderer colorLine;

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
}
