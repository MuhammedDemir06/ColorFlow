using UnityEngine;
using System.IO;
[System.Serializable]
public class PlayerData
{
    [Header("Game")]
    public int PlayerCash;
    public int CurrentLevel;
    [Header("Settings")]
    public float GameSound;
    public bool CircleEnable;
}
public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;

    public PlayerData CurrentPlayerData;
    private string filePath;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        filePath = Path.Combine(Application.persistentDataPath, "playerData.json");
        CurrentPlayerData = LoadData();
    }   
    public void SaveData()
    {
        string json = JsonUtility.ToJson(CurrentPlayerData, true);
        File.WriteAllText(filePath, json);
    }
    public PlayerData LoadData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            return data;
        }
        else
        {
            NewPlayerData();
            Debug.LogWarning("Data Not Found And New Data Created!");
            return null;
        }
    }
    private PlayerData NewPlayerData()
    {
        PlayerData newPlayerData = new PlayerData
        {
            PlayerCash = 100,
            CurrentLevel = 1,
            CircleEnable = true,
            GameSound = 40
        };

        string json = JsonUtility.ToJson(newPlayerData, true);
        File.WriteAllText(filePath, json);

        return newPlayerData;
    }
    public void DeleteData()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Veri dosyası silindi.");
        }
    }
}
