using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{

    [Header("Player Cash")]
    public int CurrentCash;
    [SerializeField] private TextMeshProUGUI currentCashText;
    [Header("Sound Slider")]
    [SerializeField] private Slider soundSlider;
    [SerializeField] private SoundManager soundManager;
    [Header("Color Circle")]
    [SerializeField] private Image circleButtonImage;
    [SerializeField] private TextMeshProUGUI circleEnableText;
    [SerializeField] private Color redColor;
    [SerializeField] private Color greenColor;
    private bool circleIsActive;
    [Header("Background Effect")]
    [SerializeField] private GameObject backgroundEffect;
    [SerializeField] private Image backgroundButtonImage;
    [SerializeField] private TextMeshProUGUI backgroundEnableText;
    private bool backgroundIsActive;
    private void Start()
    {
        SoundSlider(true);
        SaveCash(true);

        CircleUpdate(true);
        BackgroundUpdate(true);
    }
    public void SpendCash(int spendAmount,bool isIncrease)
    {
        if(isIncrease)
            CurrentCash += spendAmount;
        else
            CurrentCash -= spendAmount;
        SaveCash(false);
    }
    public void SaveCash(bool isGettingData)
    {
        if (isGettingData)
        {
            CurrentCash = PlayerDataManager.Instance.CurrentPlayerData.PlayerCash;
        }
        else
        {
            PlayerDataManager.Instance.CurrentPlayerData.PlayerCash = CurrentCash;
            PlayerDataManager.Instance.SaveData();
        }

        currentCashText.text = CurrentCash.ToString();
    }
    public void CircleUpdate(bool isGettingData)
    {
        if (isGettingData)
        {
            circleIsActive = PlayerDataManager.Instance.CurrentPlayerData.CircleEnable;
        }
        else
        {
            PlayerDataManager.Instance.CurrentPlayerData.CircleEnable = circleIsActive;
            PlayerDataManager.Instance.SaveData();
        }

        if (circleIsActive)
        {
            circleButtonImage.color = greenColor;
            circleEnableText.text = "Enable";
        }
        else
        {
            circleButtonImage.color = redColor;
            circleEnableText.text = "Disable";
        }

        if (ColorCircle.Instance != null)
            ColorCircle.Instance.EnableCircle = circleIsActive;
    }
    private void BackgroundUpdate(bool isGettingData)
    {
        if(isGettingData)
        {
            backgroundIsActive = PlayerDataManager.Instance.CurrentPlayerData.BackgroundEnable;
        }
        else
        {
            PlayerDataManager.Instance.CurrentPlayerData.BackgroundEnable = backgroundIsActive;
            PlayerDataManager.Instance.SaveData();
        }

        if(backgroundIsActive)
        {
            
            backgroundButtonImage.color = greenColor;
            backgroundEnableText.text = "Enable";
        }
        else
        {
            backgroundButtonImage.color = redColor;
            backgroundEnableText.text = "Disable";
        }
        backgroundEffect.SetActive(backgroundIsActive);
    }
    public void SoundSlider(bool isGettingData)
    {
        if (isGettingData)
            soundManager.GetSoundData(soundSlider);
        else
            soundManager.SetSoundData(soundSlider);
    }
    //Buttons
    public void PlaySound(AudioSource audio)
    {
        if (audio != null)
            audio.Play();
    }
    public void SetSound()
    {
        soundManager.SetSound(soundSlider.value);
    }
    public void CircleButton()
    {
        circleIsActive = !circleIsActive;
        CircleUpdate(false);
    }
    public void BackgroundButton()
    {
        backgroundIsActive = !backgroundIsActive;
        BackgroundUpdate(false);
    }
}
