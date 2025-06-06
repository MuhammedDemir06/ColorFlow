using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance;

    [Header("Grid")]
    [SerializeField] private int gridWidth = 4;
    [SerializeField] private int gridHeight = 4;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform gridParent;

    [Header("Test")]
    [SerializeField] private Color[] tilesColor;
    [SerializeField] private int[] tilesID;
    private List<Tile> spawnedTiles = new List<Tile>();
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
        SetTiles();
    }
    private void TileSpawner()
    {
        for (int x = 0; x < gridWidth; x++)
            for (int y = 0; y < gridHeight; y++)
            {
                var newTile = Instantiate(tilePrefab, new Vector2(x, y), Quaternion.identity);
                spawnedTiles.Add(newTile.GetComponent<Tile>());
                newTile.name = $"Tile x:{x} y:{y}";
                newTile.transform.SetParent(gridParent);
            }

        cam.position = new Vector3((float)gridWidth / 2 - 0.5f, (float)gridHeight / 2 - 0.5f, -10f);
    }
    private void SetTiles()
    {
        for (int i = 0; i < spawnedTiles.Count; i++)
        {
            if (tilesID[i] != 0)
            {
                var tileColorBall = spawnedTiles[i].transform.Find("Color Ball");
                tileColorBall.GetComponent<SpriteRenderer>().color = tilesColor[i];
                tileColorBall.gameObject.SetActive(true);

                spawnedTiles[i].TileID = tilesID[i];
                spawnedTiles[i].IsHaveBall = true;
                spawnedTiles[i].SetColor(tilesColor[i]);
            }
        }
    }
}
