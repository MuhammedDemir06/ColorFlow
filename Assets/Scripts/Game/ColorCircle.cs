using UnityEngine;

public class ColorCircle : MonoBehaviour
{
    public static ColorCircle Instance;

    [Header("Selected Circle")]
    [SerializeField] private Transform selectedCircle;
    public bool EnableCircle;
    private void Awake()
    {
        Instance = this;
    }
    public void SetSelectedCircleColor(Color newColor)
    {
        if (EnableCircle)
            selectedCircle.GetComponent<SpriteRenderer>().color = newColor;
    }
    private void GetSelectedCircle()
    {
        if (EnableCircle)
        {
            selectedCircle.gameObject.SetActive(InputManager.Instance.CanSelect);

            Vector3 mousePosition = Input.mousePosition;

            mousePosition.z = Camera.main.nearClipPlane;

            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            selectedCircle.position = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
        }
    }
    private void Update()
    {
        GetSelectedCircle();
    }
}
