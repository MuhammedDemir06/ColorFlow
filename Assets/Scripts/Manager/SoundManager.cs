using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public void GetSoundData(Slider newSlider)
    {
        
        newSlider.value = PlayerDataManager.Instance.LoadData().GameSound;
    }
    public void SetSoundData(Slider newSlider)
    {
        float number = newSlider.value / 100;
        PlayerDataManager.Instance.CurrentPlayerData.GameSound = number;
        PlayerDataManager.Instance.SaveData();
    }
}
