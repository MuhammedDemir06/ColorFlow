using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class TileData
{
    public Color TileColor = Color.white;
    public int ID = 0;
    public bool IsColored = false;
}
public class LevelEditorWindow : EditorWindow
{
    //Variables
    private int gridHeight;
    private int gridWidth;

    private Color currentColor = Color.blue;

    private Color colorArea = Color.red;

    private TileData[,] tiles;

    private Vector2 scrollPos;


    [MenuItem("Tools/Level Creator")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorWindow>("Level Creator");
    }
    private void InitializeTiles()
    {
        tiles = new TileData[gridWidth, gridHeight];
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                tiles[x, y] = new TileData();
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
        gridHeight = EditorGUILayout.IntSlider("Grid Height",gridHeight, 1, 12);

        GUILayout.Space(10);
        gridWidth = EditorGUILayout.IntSlider("Grid Width", gridWidth, 1, 12);

        GUILayout.Space(20);
        SetColor();
    }
    private void SetColor()
    {
        GUILayout.Space(10);
        GUILayout.Label("Select Color", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();

        GUILayout.Label("Special Color:", GUILayout.Width(55));
        Color newColor = EditorGUILayout.ColorField(GUIContent.none, colorArea, false, false, false, GUILayout.Width(40), GUILayout.Height(40));
        if (newColor != currentColor)
        {
            currentColor = newColor;
            colorArea = newColor;
        }

        // Ortadaki kutuda seçili rengi göster
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
        Color.red,
        Color.black,
        Color.green,
        Color.white,
        Color.gray,
        Color.blue,
        Color.cyan,
        Color.magenta
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
        GUILayout.Space(20);
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 20;
        GUILayout.Label("Grid:", style);

        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(600)); 

        GUIStyle tileStyle = new GUIStyle(GUI.skin.button);
        tileStyle.margin = new RectOffset(2, 2, 2, 2);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace(); 
        GUILayout.BeginVertical();

        for (int y = 0; y < gridHeight; y++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < gridWidth; x++)
            {
                GUI.backgroundColor = tiles[x, y].TileColor;

                if (GUILayout.Button("", GUILayout.Width(40), GUILayout.Height(40)))
                {
                    tiles[x, y].TileColor = currentColor;
                    tiles[x, y].IsColored = true;
                 //   tiles[x, y].ID = GetColorID(currentColor);
                }
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.EndScrollView(); // Scroll alanını kapat
        GUI.backgroundColor = Color.white;
    }
    private void OnGUI()
    {
        ShowTitle();

        ShowVariables();

        InitializeTiles();
        DrawGrid();
    }
   
}
