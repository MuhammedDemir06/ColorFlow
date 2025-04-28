using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [Header("Grid")]
    [SerializeField] private int gridWidth = 4;
    [SerializeField] private int gridHeight = 4;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform gridParent;
    [Header("Tile")]
    public bool CanSelect = false;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        Init();
    }
    private void Init()
    {
        TileSpawner();
    }
    private void TileSpawner()
    {
        for (int x = 0;  x < gridWidth;  x++)
            for (int y = 0; y < gridHeight; y++)
            {
                var newTile = Instantiate(tilePrefab, new Vector2(x, y), Quaternion.identity);
                newTile.name = $"Tile x:{x} y:{y}";
                newTile.transform.SetParent(gridParent);
            }

        cam.position = new Vector3((float)gridWidth / 2 - 0.5f, (float)gridHeight / 2 - 0.5f, -10f);
    }
    public void StartTileSelection()
    {
        CanSelect = true;
    }
    public void EndTileSelection()
    {
        CanSelect = false;
    }
}