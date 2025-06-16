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
    [Header("Circle")]
    [SerializeField] private Button circleEnableButton;
    [SerializeField] private TextMeshProUGUI circleEnableText;
    [SerializeField] private Color greenColor;
    [SerializeField] private Color redColor;
    [Header("Move Count")]
    [SerializeField] private TextMeshProUGUI moveCountText;
    [SerializeField] private int moveCount;
    [Header("Settings")]
    [SerializeField] private AnimatedPanel settingsPanel;
    [Header("Level Finish || Next Level")]
    [SerializeField] private AnimatedPanel nextLevelPanel;
    [SerializeField]private Image[] starImages;
    [SerializeField] private Sprite whiteStarSprite;
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

        UpdateCircleEnable();

        UpdateCash();
    }
    private void UpdateCash()
    {
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
            UpdateCash();

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
    private void UpdateCircleEnable()
    {
        if (ColorCircle.Instance.EnableCircle)
        {
            circleEnableButton.GetComponent<Image>().color = greenColor;
            
            circleEnableText.text = "Enabled";
            DebugManager.Instance.DebugLogWarning("Circle Enabled");
        }
        else
        {
            circleEnableButton.GetComponent<Image>().color = redColor;
            circleEnableText.text = "Disabled";
        }
    }
    public void CircleEnableButton()
    {
        ColorCircle.Instance.EnableCircle = !ColorCircle.Instance.EnableCircle;
        UpdateCircleEnable();
    }
    public void NextScene(string sceneName)
    {
        SceneTransitionEffect.Instance.LoadScene(sceneName);
    }
}
[System.Serializable]
public class Star
{
    public string[] Subtitles;
}