using UnityEngine;

public class DontDestroyOnLoadObject : MonoBehaviour
{
    private void Awake()
    {
        if (transform.parent != null)
            transform.parent = null;

        DontDestroyOnLoad(gameObject);
    }
}
