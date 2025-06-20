using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Manager")]
    [Space(10)]
    [Header("Message Panel Prefab")]
    [SerializeField] private GameObject canvasPanelPrefab;
    private void Start()
    {
        if (AnimatedMessagePanel.Instance == null && SceneTransitionEffect.Instance == null)
        {
            Instantiate(canvasPanelPrefab);
        }
        else
        {
            DebugManager.Instance.DebugLog("Instances Found");
            Destroy(gameObject);
        }
    }
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/Delete Game Data")]
    public static void DeleteGameData()
    {
        PlayerDataManager.DeleteData();
    }

#endif
}
