using UnityEngine;

public class Tile : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (!GridManager.Instance.CanSelect) return;
        GridManager.Instance.StartTileSelection();
    }

    private void OnMouseEnter()
    {
        if (!GridManager.Instance.CanSelect) return;
        // Tile highlight vs.
    }

    private void OnMouseUp()
    {
        if (!GridManager.Instance.CanSelect) return;
        GridManager.Instance.EndTileSelection();
    }
}