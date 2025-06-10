using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public static DebugManager Instance;

    public bool DebugMode;

    private void Awake()
    {
        Instance = this;
    }
    public void DebugLog(string message)
    {
        if (DebugMode)
            Debug.Log(message);
    }
    public void DebugLogWarning(string message)
    {
        if (DebugMode)
            Debug.LogWarning(message);
    }
    public void DebugLogError(string message)
    {
        if (DebugMode)
            Debug.LogError(message);
    }
}
