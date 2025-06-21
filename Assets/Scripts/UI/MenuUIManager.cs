using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MenuUIManager : MonoBehaviour
{
    public static MenuUIManager Instance;
    [Header("Menu UI Manager")]
    [Space(20)]
    [Header("Settings")]
    [SerializeField] private SettingsPanel settingsPanel;
    [Header("Levels")]
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private Transform contentParent;
    private void Start()
    {
        LoadLevelButtons();
    }
    private void LoadLevelButtons()
    {
        LevelData[] allLevels = Resources.LoadAll<LevelData>("Levels");

        var sortedLevels = allLevels.OrderBy(lv =>
        {
            Match match = Regex.Match(lv.name, @"\d+");
            return match.Success ? int.Parse(match.Value) : 0;
        }).ToArray();

        for (int i = 0; i < sortedLevels.Length; i++)
        {
            GameObject btnObj = Instantiate(levelButtonPrefab, contentParent);
            int levelIndex = i + 1;

            if (i < PlayerDataManager.Instance.CurrentPlayerData.CurrentLevel)
            {
                btnObj.GetComponentInChildren<TextMeshProUGUI>().text = levelIndex.ToString();

                int capturedIndex = levelIndex;
                btnObj.GetComponent<Button>().onClick.AddListener(()=> NewLevelButton(capturedIndex));
            }
            else
            {
                btnObj.GetComponentInChildren<TextMeshProUGUI>().text = " ";

                Transform lockTransform = btnObj.transform.Find("Lock Icon");

                lockTransform.gameObject.SetActive(true);
            }
        }
        ResizeUI(sortedLevels.Length);
    }
    private void NewLevelButton(int currentLevel)
    {
        PlayerDataManager.Instance.CurrentPlayerData.DesiredLevel = currentLevel;

        PlayerDataManager.Instance.SaveData();

        SceneTransitionEffect.Instance.LoadScene("Game");
    }
    private void ResizeUI(int levelCount)
    {
        if(levelCount>30)
        {
            GridLayoutGroup grid = contentParent.GetComponent<GridLayoutGroup>();
            RectTransform rt = contentParent.GetComponent<RectTransform>();

            int columns = 4; //How many button in the line
            int rows = Mathf.CeilToInt(levelCount / (float)columns);

            float width = grid.cellSize.x * columns + grid.spacing.x * (columns - 1);
            float height = grid.cellSize.y * rows + grid.spacing.y * (rows - 1);

            rt.sizeDelta = new Vector2(width, height);
        }
    }
    //Buttons
    public void OpenURLButton(string link)
    {
        Application.OpenURL(link);
    }
    public void StoreButton()
    {
        settingsPanel.SpendCash(100, true);
        settingsPanel.SaveCash(false);
        PopupText.Instance.ShowPopup("Cash Earned");
    }
    public void ExitButton()
    {
        SceneTransitionEffect.Instance.Exit();
    }
}
