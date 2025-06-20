using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class TileData
{
    public bool TileEnabled;
    public Color BallColor;
    public Color NormalColor;
    public int BallID;
    public int NormalID;
}
public class LevelEditorWindow : EditorWindow
{
    private int gridHeight = 1;
    private int gridWidth = 1;
    private float tileSize = 40f;
    private float tileSpacing = 2f;
    private int lastLevel = 1;
    private bool autoSetBestMove;

    private Color currentColor = Color.blue;
    private Color colorArea = Color.red;

    private TileData[,] gridData;

    private bool isDrawing = false;
    private List<Vector2Int> currentPath = new List<Vector2Int>();

    private int bestMove = 0;

    private Dictionary<Color, int> colorToID = new Dictionary<Color, int>();

    private void OnEnable()
    {
        lastLevel = EditorPrefs.GetInt("MyGame_LastLevel", 0);
    }

    [MenuItem("Tools/Level Creator")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorWindow>("Level Creator");
    }
    private void InitializeTiles()
    {
        if (gridData == null || gridData.GetLength(0) != gridWidth || gridData.GetLength(1) != gridHeight)
        {
            gridData = new TileData[gridWidth, gridHeight];
            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    gridData[x, y] = new TileData();
                }
            }
        }
    }
    private void ShowTitle()
    {
        GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontSize = 25;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.normal.textColor = Color.white;

        GUILayout.Space(10);
        GUILayout.Label("Level Creator", titleStyle);
        GUILayout.Space(10);
    }
    private void ShowVariables()
    {
        GUILayout.Label("Grid Dimensions", EditorStyles.boldLabel);
        GUILayout.Space(20);
        gridHeight = EditorGUILayout.IntSlider("Grid Height", gridHeight, 1, 12);
        GUILayout.Space(10);
        gridWidth = EditorGUILayout.IntSlider("Grid Width", gridWidth, 1, 12);
        GUILayout.Space(20);

        GUILayout.Label("Set it yourself or have it set automatically with Auto Best Move.", EditorStyles.boldLabel);

        EditorGUI.BeginDisabledGroup(autoSetBestMove);
        bestMove = EditorGUILayout.IntField("Best Move", bestMove);
        EditorGUI.EndDisabledGroup();

        autoSetBestMove = EditorGUILayout.Toggle("Auto Best Move", autoSetBestMove);

        GUILayout.Space(20);
        GUILayout.Label("Attention: Please check the Resources/Levels section, if there are levels, write the last level there here.", EditorStyles.boldLabel);
        lastLevel = EditorGUILayout.IntField("Last Level", lastLevel);
        GUILayout.Space(20);
        SetColor();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear Grid", GUILayout.Height(30)))
        {
            ClearGrid();
        }

        if (GUILayout.Button("Save Level", GUILayout.Height(30)))
        {
            SaveLevel();
        }
        GUILayout.EndHorizontal();
    }
    private void SetColor()
    {
        GUILayout.Space(10);
        GUILayout.Label("Select Color", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Special Color:", GUILayout.Width(85));
        Color newColor = EditorGUILayout.ColorField(GUIContent.none, colorArea, false, false, false, GUILayout.Width(40), GUILayout.Height(40));
        if (newColor != currentColor)
        {
            currentColor = newColor;
            colorArea = newColor;
        }

        GUIStyle previewColor = new GUIStyle(GUI.skin.box);
        previewColor.normal.background = Texture2D.whiteTexture;

        GUILayout.FlexibleSpace();
        GUILayout.Label("Selected Color:", GUILayout.Width(90));
        GUI.color = currentColor;
        GUILayout.Box("", previewColor, GUILayout.Width(40), GUILayout.Height(40));
        GUI.color = Color.white;
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(25);

        Color[] colorPalette = new Color[]
        {
            Color.red, Color.black, Color.green, Color.white, Color.gray, Color.blue, Color.cyan, Color.magenta
        };

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleRight;
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 14;
        GUILayout.Label("Already Colors:", style);

        foreach (var color in colorPalette)
        {
            GUI.backgroundColor = color;
            if (GUILayout.Button("", GUILayout.Width(40), GUILayout.Height(40)))
            {
                currentColor = color;
                colorArea = color;
            }
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;
    }
    private void DrawGrid()
    {
        float totalWidth = gridWidth * (tileSize + tileSpacing);
        float totalHeight = gridHeight * (tileSize + tileSpacing);

        GUILayout.Space(80);
        Rect gridRect = GUILayoutUtility.GetRect(totalWidth, totalHeight, GUILayout.ExpandWidth(false));

        float xOffset = (position.width - totalWidth) / 2;
        float yOffset = gridRect.y;

        Event e = Event.current;

        Vector2 mousePos = e.mousePosition;
        int tileX = (int)((mousePos.x - xOffset) / (tileSize + tileSpacing));
        int tileY = (int)((mousePos.y - yOffset) / (tileSize + tileSpacing));

        bool isInsideGrid = tileX >= 0 && tileX < gridWidth && tileY >= 0 && tileY < gridHeight;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Rect rect = new Rect(
                    xOffset + x * (tileSize + tileSpacing),
                    yOffset + y * (tileSize + tileSpacing),
                    tileSize, tileSize);

                Color drawColor = gridData[x, y].BallColor != default ? gridData[x, y].BallColor :
                                   gridData[x, y].NormalColor != default ? gridData[x, y].NormalColor :
                                   Color.gray;

                EditorGUI.DrawRect(rect, drawColor);

                // Ara noktalar için geçici görsel renk gösterimi
                if (isDrawing && currentPath.Contains(new Vector2Int(x, y)))
                {
                    // Eğer baş ya da son değilse (ara noktaysa)
                    if (currentPath.Count >= 2)
                    {
                        if (new Vector2Int(x, y) != currentPath[0] && new Vector2Int(x, y) != currentPath[currentPath.Count - 1])
                        {
                            Color previewColor = currentColor;
                            previewColor.a = 0.4f; // Şeffaf renk
                            EditorGUI.DrawRect(rect, previewColor);
                        }
                    }
                }
            }
        }
        if (isInsideGrid)
        {
            Vector2Int pos = new Vector2Int(tileX, tileY);

            if (e.type == EventType.MouseDown)
            {
                isDrawing = true;
                currentPath.Clear();
                currentPath.Add(pos);
                e.Use();
            }
            else if (e.type == EventType.MouseDrag && isDrawing)
            {
                if (!currentPath.Contains(pos))
                {
                    currentPath.Add(pos);
                    Repaint();
                    e.Use();
                }
            }
            else if (e.type == EventType.MouseUp && isDrawing)
            {
                isDrawing = false;

                if (currentPath.Count >= 2)
                {
                    ApplyPathColors();
                    Repaint();
                }

                currentPath.Clear();
                e.Use();
            }
        }
    }
    private void ApplyPathColors()
    {
        // Color Check (default)
        if (currentColor.a == 0f)
        {
            Debug.LogError("Please select a color before drawing.");
            return;
        }

        if (!colorToID.ContainsKey(currentColor))
        {
            colorToID[currentColor] = colorToID.Count + 1;
        }

        int assignedID = colorToID[currentColor];

        // Clear All Points
        foreach (var pos in currentPath)
        {
            gridData[pos.x, pos.y].BallColor = default;
            gridData[pos.x, pos.y].BallID = 0;
            gridData[pos.x, pos.y].NormalColor = default;
            gridData[pos.x, pos.y].NormalID = 0;
            gridData[pos.x, pos.y].TileEnabled = false;
        }

        if (currentPath.Count >= 2)
        {
            Vector2Int start = currentPath[0];
            Vector2Int end = currentPath[currentPath.Count - 1];

            gridData[start.x, start.y].BallColor = currentColor;
            gridData[start.x, start.y].BallID = assignedID;
            gridData[start.x, start.y].TileEnabled = true;

            gridData[end.x, end.y].BallColor = currentColor;
            gridData[end.x, end.y].BallID = assignedID;
            gridData[end.x, end.y].TileEnabled = true;
        }

        for (int i = 1; i < currentPath.Count - 1; i++)
        {
            var pos = currentPath[i];
            gridData[pos.x, pos.y].TileEnabled = true;
        }

        for (int i = 1; i < currentPath.Count - 1; i++)
        {
            var pos = currentPath[i];
            gridData[pos.x, pos.y].NormalID = assignedID;
            gridData[pos.x, pos.y].NormalColor = currentColor;
        }

        currentPath.Clear();
    }
    private void ClearGrid()
    {
        if (!EditorUtility.DisplayDialog(
            "Clear Grid?",
            "This will reset all tiles and remove all data. Are you sure?",
            "Yes, clear everything",
            "Cancel"))
        {
            return;
        }

        // Clear ALL
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                gridData[x, y].BallColor = default;
                gridData[x, y].NormalColor = default;
                gridData[x, y].BallID = 0;
                gridData[x, y].NormalID = 0;
                gridData[x, y].TileEnabled = false;
            }
        }

        currentPath.Clear();
        colorToID.Clear();

        Repaint();
    }
    private void ResetLevel()
    {
        GUILayout.Space(20);
        GUILayout.Label("Advanced", EditorStyles.boldLabel);

        if (GUILayout.Button("🔁 Reset Last Level (EditorPrefs)", GUILayout.Height(25)))
        {
            if (EditorUtility.DisplayDialog(
                "Reset Last Level?",
                "This will reset the saved last level counter. Are you sure?",
                "Yes, reset it",
                "Cancel"))
            {
                EditorPrefs.DeleteKey("MyGame_LastLevel");
                lastLevel = 0;
                Debug.Log("Last Level reset.");
            }
        }
    }
    private void SaveLevel()
    {
        if (!EditorUtility.DisplayDialog(
             "Save Level?",
             "This will overwrite or create a new level file. Are you sure you want to save?",
             "Yes, save it",
             "Cancel"))
        {
            return;
        }
        if (gridHeight < 3 || gridWidth < 3)
        {
            Debug.LogError("Minimum 3");
            return;
        }

        LevelData levelAsset = CreateInstance<LevelData>();

        levelAsset.Width = gridWidth;
        levelAsset.Height = gridHeight;

        levelAsset.TilesColor = new List<Color>();
        levelAsset.TilesID = new List<int>();
        levelAsset.SavedWays = new List<int>();

        HashSet<Color> uniqueColors = new HashSet<Color>();

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                var data = gridData[x, y];

                if(!data.TileEnabled)
                {
                    Debug.LogError("Please fill in all tiles");
                    return;
                }

                //   levelAsset.TilesColor.Add(data.BallColor != default ? data.BallColor : data.NormalColor);
                levelAsset.TilesColor.Add(data.BallColor);
                levelAsset.TilesID.Add(data.BallID);
                levelAsset.SavedWays.Add(data.BallID != 0 ? data.BallID : data.NormalID);

                if (data.BallColor != default)
                    uniqueColors.Add(data.BallColor);
                else if (data.NormalColor != default)
                    uniqueColors.Add(data.NormalColor);
            }
        }
        levelAsset.TotalColors = uniqueColors.Count;

        if (autoSetBestMove)
            bestMove = uniqueColors.Count;

        levelAsset.BestMove = bestMove;

        lastLevel += 1;
        EditorPrefs.SetInt("MyGame_LastLevel", lastLevel);

        string folderPath = "Assets/Resources/Levels";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets/Resources", "Levels");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(folderPath + $"/Level_{lastLevel}.asset");
        AssetDatabase.CreateAsset(levelAsset, assetPathAndName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Level saved at: " + assetPathAndName);
    }
    private void OnGUI()
    {
        ShowTitle();
        ShowVariables();
        InitializeTiles();
        DrawGrid();
        ResetLevel();
    }
}