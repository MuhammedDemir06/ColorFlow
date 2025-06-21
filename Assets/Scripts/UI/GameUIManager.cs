using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;

    [Header("Game UI Manager")]
    public bool GamePaused = false;
    public bool LevelFinished;
    [Header("Color Find")]
    [SerializeField] private int findAmount = 100;
    [SerializeField] private TextMeshProUGUI colorFindText;
    [Header("Move Count")]
    [SerializeField] private TextMeshProUGUI moveCountText;
    [SerializeField] private int moveCount;
    [Header("Settings")]
    [SerializeField] private AnimatedPanel settingsAnimatedPanel;
    [SerializeField] private SettingsPanel settingsPanel;
    [Header("Level Finish || Next Level")]
    [SerializeField] private AnimatedPanel nextLevelAnimatedPanel;
    [SerializeField]private Image[] starImages;
    [SerializeField] private Sprite whiteStarSprite;
    [SerializeField] private int earnedCash;
    [SerializeField] private TextMeshProUGUI earnedCashText;
    [Header("Current Level")]
    [SerializeField] private TextMeshProUGUI desiredLevelText;
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
        UpdateMoveCount(0);

        colorFindText.text = findAmount.ToString();

        desiredLevelText.text = $"Level {PlayerDataManager.Instance.CurrentPlayerData.DesiredLevel}";
    }
    public void UpdateMoveCount(int amount)
    {
        moveCount += amount;
        moveCountText.text = "Total Move: " + moveCount.ToString();
    }
    public void LevelFinish(bool level)
    {
        if (PlayerDataManager.Instance.CurrentPlayerData.CurrentLevel == PlayerDataManager.Instance.CurrentPlayerData.DesiredLevel)
        {
            PlayerDataManager.Instance.CurrentPlayerData.CurrentLevel += 1;
            PlayerDataManager.Instance.SaveData();
        }
        LevelFinished = level;
        nextLevelAnimatedPanel.Show();

        var newValue = moveCount - TileManager.Instance.BestMove;
        int whiteStarCount;

        if (newValue < 1)
        {
            whiteStarCount = 3;
            earnedCash = 50;
            settingsPanel.SpendCash(earnedCash, true);
        }
        else if (newValue < 3)
        {
            whiteStarCount = 2;
            earnedCash = 25;
            settingsPanel.SpendCash(earnedCash, true);
        }
        else
        {
            whiteStarCount = 1;
            earnedCash = 10;
            settingsPanel.SpendCash(earnedCash, true);
        }
        earnedCashText.text = $"Earned Cash {earnedCash}";

        StartCoroutine(StarCalculate(whiteStarCount));
    }
    private IEnumerator StarCalculate(int whiteStarCount)
    {
        for (int i = 0; i < starImages.Length; i++)
        {
            yield return new WaitForSeconds(0.4f);

            if (i < whiteStarCount)
                starImages[i].sprite = whiteStarSprite;

            starImages[i].gameObject.SetActive(true);
        }
    }
    //Buttons
    public void ReturnButton()
    {
        if (LevelFinished)
            return;

        InputManager.Instance.ReturnLine();
    }
    public void FindButton()
    {
        if (LevelFinished)
            return;

        if (settingsPanel.CurrentCash >= findAmount)
        {
            settingsPanel.CurrentCash -= findAmount;
            settingsPanel.SaveCash(false);

            InputManager.Instance.FindLine();
            AnimatedMessagePanel.Instance.ShowMessage("Find Color is Purchased!", false);
            DebugManager.Instance.DebugLog("Find Color is Purchased!");
        }
        else
        {
            AnimatedMessagePanel.Instance.ShowMessage("Find Color is not Purchased!",true);  //error Panel
            DebugManager.Instance.DebugLogWarning("Find Color is not Purchased!");
        }  
    }
    public void SetGameMode(bool mode)
    {
        if (!LevelFinished)
            GamePaused = mode;
    }
    public void SettingsButton()
    {
        if (LevelFinished)
            return;

        settingsAnimatedPanel.Show();
    }
    public void NextScene(string sceneName)
    {
        SceneTransitionEffect.Instance.LoadScene(sceneName);
    }
    public void NextLevelButton()
    {
        PlayerDataManager.Instance.CurrentPlayerData.DesiredLevel += 1;

        PlayerDataManager.Instance.SaveData();
    }
}