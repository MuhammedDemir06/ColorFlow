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
    [Header("Cash")]
    [SerializeField] private int currentCash;
    [SerializeField] private TextMeshProUGUI currentCashText;
    [Header("Color Find")]
    [SerializeField] private int findAmount = 100;
    [SerializeField] private TextMeshProUGUI colorFindText;
    [Header("Color Circle")]
    [SerializeField] private Image circleButtonImage;
    [SerializeField] private TextMeshProUGUI circleEnableText;
    [SerializeField] private Color greenColor;
    [SerializeField] private Color redColor;
    private bool isActive;
    [Header("Move Count")]
    [SerializeField] private TextMeshProUGUI moveCountText;
    [SerializeField] private int moveCount;
    [Header("Settings")]
    [SerializeField] private AnimatedPanel settingsPanel;
    [Header("Level Finish || Next Level")]
    [SerializeField] private AnimatedPanel nextLevelPanel;
    [SerializeField]private Image[] starImages;
    [SerializeField] private Sprite whiteStarSprite;
    [Header("Sound Slider")]
    [SerializeField] private Slider soundSlider;
    [SerializeField] private SoundManager soundManager;
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

        UpdateCircleEnable(true);

        SaveCash(true);
    }
    private void SaveCash(bool isGettingData)
    {
        if (isGettingData)
        {
            currentCash = PlayerDataManager.Instance.LoadData().PlayerCash;
        }
        else
        {
            PlayerDataManager.Instance.CurrentPlayerData.PlayerCash = currentCash;
            PlayerDataManager.Instance.SaveData();
        }

        currentCashText.text = currentCash.ToString();
    }
    public void UpdateMoveCount(int amount)
    {
        moveCount += amount;
        moveCountText.text = "Total Move: " + moveCount.ToString();
    }
    public void LevelFinish(bool level)
    {
        LevelFinished = level;
        nextLevelPanel.Show();

        var newValue = moveCount - TileManager.Instance.BestMove;
        int whiteStarCount;

        if (newValue < 1)
            whiteStarCount = 3;
        else if (newValue < 3)
            whiteStarCount = 2;
        else
            whiteStarCount = 1;

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
    public void SoundSlider(bool isGettingData)
    {
        if (isGettingData)
            soundManager.GetSoundData(soundSlider);
        else
            soundManager.SetSoundData(soundSlider);
    }
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

        if (currentCash >= findAmount)
        {
            currentCash -= findAmount;
            SaveCash(false);

            InputManager.Instance.FindLine();
            AnimatedMessagePanel.Instance.ShowMessage("Find Color is Purchased!", false);
            DebugManager.Instance.DebugLog("Find Color is Purchased!");
        }
        else
        {
            AnimatedMessagePanel.Instance.ShowMessage("Find Color is not Purchased!",true);  //erroPanel
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

        settingsPanel.Show();
    }
    private void UpdateCircleEnable(bool isGettingData)
    {
        if (isGettingData)
        {
            isActive = PlayerDataManager.Instance.CurrentPlayerData.CircleEnable;
        }
        else
        {
            PlayerDataManager.Instance.CurrentPlayerData.CircleEnable = isActive;
        }

        if (isActive)
        {
            circleButtonImage.color = greenColor;
            circleEnableText.text = "Enable";
        }
        else
        {
            circleButtonImage.color = redColor;
            circleEnableText.text = "Disable";
        }
    }
    public void CircleEnableButton()
    {
        isActive = !isActive;
        ColorCircle.Instance.EnableCircle = isActive;
        UpdateCircleEnable(false);
    }
    public void NextScene(string sceneName)
    {
        SceneTransitionEffect.Instance.LoadScene(sceneName);
    }
}