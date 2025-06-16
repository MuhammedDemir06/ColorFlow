using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class AnimatedMessagePanel : MonoBehaviour
{
    public static AnimatedMessagePanel Instance;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float displayDuration = 1f;

    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Image panelImage;

    [Header("Color")]
    [SerializeField] private Color errorMessageColor;
    [SerializeField] private Color messageColor;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        canvasGroup.alpha = 0;
        canvasGroup.gameObject.SetActive(false);
    }
    public void ShowMessage(string message,bool isError)
    {
        canvasGroup.gameObject.SetActive(true);
        messageText.text = message;
        if (isError)
            panelImage.color = errorMessageColor;
        else
            panelImage.color = messageColor;

        // Önceki animasyonu iptal et
        DOTween.Kill(canvasGroup);

        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, fadeDuration).OnComplete(() =>
        {
            DOVirtual.DelayedCall(displayDuration, () =>
            {
                canvasGroup.DOFade(0, fadeDuration).OnComplete(() =>
                {
                    canvasGroup.gameObject.SetActive(false);
                });
            });
        });
    }
}
