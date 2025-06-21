using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance;

    [Header("Grid")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform gridParent;
    private int gridWidth = 4;
    private int gridHeight = 4;
    [SerializeField] private LevelData levelData;
    public int BestMove;
    public int TotalColors;
    private List<Color> tilesColor = new List<Color>();
    private List<int> tilesID = new List<int>();
    private List<Tile> spawnedTiles = new List<Tile>();
    private List<int> savedWays = new List<int>();
    private List<int> currentSavedTilesID = new List<int>();

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
        GeLevelData();
        TileSpawner();
        SetTiles();

        SavedTiles();
    }
    private void GeLevelData()
    {
        LevelData[] allLevels = Resources.LoadAll<LevelData>("Levels");

        var sortedLevels = allLevels.OrderBy(lv =>
        {
            Match match = Regex.Match(lv.name, @"\d+");
            return match.Success ? int.Parse(match.Value) : 0;
        }).ToArray();

        levelData = sortedLevels[PlayerDataManager.Instance.CurrentPlayerData.DesiredLevel];

        //Values
        gridHeight = levelData.Height;
        gridWidth = levelData.Width;
        BestMove = levelData.BestMove;
        TotalColors = levelData.TotalColors;

        tilesColor = levelData.TilesColor;
        tilesID = levelData.TilesID;
        savedWays = levelData.SavedWays;
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

        // Kamera pozisyonu
        float camX = (gridWidth - 1) / 2f;
        float camY = (gridHeight - 1) / 2f;
        cam.position = new Vector3(camX, camY, cam.position.z);

        // Kamera zoom ayarı (ortografik varsayılarak)
        Camera cameraComponent = cam.GetComponent<Camera>();

        if (cameraComponent.orthographic)
        {
            float aspect = (float)Screen.width / Screen.height;
            float verticalSize = gridHeight / 2f + 1f; // +1 bir miktar boşluk için
            float horizontalSize = (gridWidth / 2f + 1f) / aspect;

            cameraComponent.orthographicSize = Mathf.Max(verticalSize, horizontalSize);
        }
    }
    private void SetTiles()
    {
        for (int i = 0; i < spawnedTiles.Count; i++)
        {
            spawnedTiles[i].SavedPathID = savedWays[i];

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
    private void SavedTiles()
    {
        currentSavedTilesID.Clear();

        HashSet<int> addedIDs = new HashSet<int>();

        for (int i = 0; i < tilesID.Count; i++)
        {
            int id = tilesID[i];

            if (id != 0 && !addedIDs.Contains(id))
            {
                currentSavedTilesID.Add(id);
                addedIDs.Add(id);
            }
        }
    }
    public List<Tile> SpawnedTiles()
    {
        return spawnedTiles;
    }
    public List<int> SetCurrentSavedTilesID()
    {
        return currentSavedTilesID;
    }
    public List<int> SetPathFind()
    {
        return savedWays;
    }
}
