using DG.Tweening;
using UnityEngine;
using TMPro;
public class PopupText : MonoBehaviour
{
    public static PopupText Instance;

    [SerializeField] private TextMeshProUGUI popupText;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float moveY = 40f;

    private Vector3 originalPosition;

    private void Awake()
    {
        Instance = this;

        originalPosition = popupText.rectTransform.anchoredPosition;
        popupText.alpha = 0f;
        popupText.gameObject.SetActive(true);
    }
    public void ShowPopup(string message)
    {
        popupText.text = message;
        popupText.DOKill();
        popupText.alpha = 1f;
        popupText.rectTransform.anchoredPosition = originalPosition;

        popupText.rectTransform.DOAnchorPosY(originalPosition.y + moveY, duration);
        popupText.DOFade(0f, duration).SetDelay(0.2f);
    }
}
