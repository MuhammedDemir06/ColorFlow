using DG.Tweening.Core.Easing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;

    [Header("Game UI Manager")]
    public bool GamePaused = false;
    [Header("Cash")]
    [SerializeField] private int currentCash;
    [SerializeField] private TextMeshProUGUI currentCashText;

    [Header("Color Find")]
    [SerializeField] private int findAmount = 100;
    [SerializeField] private TextMeshProUGUI colorFindText;
    [Header("Circle")]
    [SerializeField] private Button circleEnableButton;
    [SerializeField] private TextMeshProUGUI circleEnableText;
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
        colorFindText.text = findAmount.ToString();

        UpdateCircleEnable();

        UpdateCash();
    }
    private void UpdateCash()
    {
        currentCashText.text = currentCash.ToString();
    }
    //Buttons
    public void ReturnButton()
    {
        InputManager.Instance.ReturnLine();
    }
    public void FindButton()
    {
        if (currentCash >= findAmount)
        {
            currentCash -= findAmount;
            UpdateCash();

            InputManager.Instance.FindLine();
            DebugManager.Instance.DebugLog("Find Color is Purchased!");
        }
        else
            DebugManager.Instance.DebugLogWarning("Find Color is not Purchased!");
    }
    public void RestartButton()
    {
        InputManager.Instance.RestartGrid();
    }
    public void SetGameMode(bool mode)
    {
        GamePaused = mode;
    }
    private void UpdateCircleEnable()
    {
        if (ColorCircle.Instance.EnableCircle)
        {
            circleEnableButton.GetComponent<Image>().color = Color.green;
            
            circleEnableText.text = "Enabled";
            DebugManager.Instance.DebugLogWarning("Circle Enabled");
        }
        else
        {
            circleEnableButton.GetComponent<Image>().color = Color.red;
            circleEnableText.text = "Disabled";
        }
    }
    public void CircleEnableButton()
    {
        ColorCircle.Instance.EnableCircle = !ColorCircle.Instance.EnableCircle;
        UpdateCircleEnable();
    }
}
