using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MenuUIManager : MonoBehaviour
{
    public static MenuUIManager Instance;

    [Header("Player Cash")]
    [SerializeField] private int currentCash;
    [SerializeField] private TextMeshProUGUI currentCashText;
    [Header("Sound Slider")]
    [SerializeField] private Slider soundSlider;
    [SerializeField] private SoundManager soundManager;
    [Header("Color Circle")]
    [SerializeField] private Image circleButtonImage;
    [SerializeField] private TextMeshProUGUI circleEnableText;
    [SerializeField] private Color redColor;
    [SerializeField] private Color greenColor;
    private bool isActive;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        SoundSlider(true);
        SaveCash(true);

        CircleUpdate(true);
    }
    private void SaveCash(bool isGettingData)
    {
        if(isGettingData)
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
    private void CircleUpdate(bool isGettingData)
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
    //Buttons
    public void SoundSlider(bool isGettingData)
    {
        if (isGettingData)
            soundManager.GetSoundData(soundSlider);
        else
            soundManager.SetSoundData(soundSlider);
    }

    public void CircleButton()
    {
        isActive = !isActive;
        CircleUpdate(false);
    }
    public void OpenURLButton(string link)
    {
        Application.OpenURL(link);
    }
    public void StoreButton()
    {
        currentCash += 100;
        SaveCash(false);
        PopupText.Instance.ShowPopup("Cash Earned");
    }
    public void ExitButton()
    {
        SceneTransitionEffect.Instance.Exit();
    }
    public void PlayButton()
    {
        SceneTransitionEffect.Instance.LoadScene("Game");
    }
}
