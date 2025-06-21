using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public void SetSound(float value)
    {
        float number = value / 100;
        AudioListener.volume = number;
    }
    public void GetSoundData(Slider newSlider)
    {
        newSlider.value = PlayerDataManager.Instance.LoadData().GameSound;
        AudioListener.volume = PlayerDataManager.Instance.LoadData().GameSound / 100;
    }
    public void SetSoundData(Slider newSlider)
    {
        PlayerDataManager.Instance.CurrentPlayerData.GameSound = newSlider.value;
        PlayerDataManager.Instance.SaveData();
    }
}
