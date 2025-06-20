using UnityEngine;
using System.IO;
[System.Serializable]
public class PlayerData
{
    [Header("Game")]
    public int PlayerCash;
    public int CurrentLevel;
    public int DesiredLevel;
    [Header("Settings")]
    public float GameSound;
    public bool CircleEnable;
    public bool BackgroundEnable;
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
            CurrentPlayerData = NewPlayerData();
            return CurrentPlayerData;
        }
    }
    private PlayerData NewPlayerData()
    {
        PlayerData newPlayerData = new PlayerData
        {
            PlayerCash = 100,
            CurrentLevel = 1,
            DesiredLevel = 0,
            CircleEnable = true,
            GameSound = 40,
            BackgroundEnable = true
        };

        string json = JsonUtility.ToJson(newPlayerData, true);
        File.WriteAllText(filePath, json);

        return newPlayerData;
    }
    public static void DeleteData()
    {
        if (File.Exists(Path.Combine(Application.persistentDataPath, "playerData.json")))
        {
            File.Delete(Path.Combine(Application.persistentDataPath, "playerData.json"));
            Debug.Log("Data Deleted.");
        }
    }
}
