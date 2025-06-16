using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Manager")]
    [Space(10)]
    [Header("Message Panel Prefab")]
    [SerializeField] private GameObject canvasPanelPrefab;
    void Start()
    {
        if (AnimatedMessagePanel.Instance == null && SceneTransitionEffect.Instance == null)
        {
            Instantiate(canvasPanelPrefab);
        }
        else
            DebugManager.Instance.DebugLogError("Instances Not Found");
    }
}
