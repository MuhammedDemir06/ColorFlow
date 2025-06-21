using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
public class SceneTransitionEffect : MonoBehaviour
{
    public static SceneTransitionEffect Instance;

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.5f;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.gameObject.SetActive(false);
    }
    public void Exit()
    {
        fadeImage.gameObject.SetActive(true);

       fadeImage.DOFade(1f, fadeDuration);

        Invoke(nameof(ExitTimer), fadeDuration);
    }
    private void ExitTimer()
    {
        Application.Quit();
    }
    public void LoadScene(string sceneName)
    {
        StartCoroutine(DoTransition(sceneName));
    }
    private System.Collections.IEnumerator DoTransition(string sceneName)
    {
        fadeImage.gameObject.SetActive(true);

        yield return fadeImage.DOFade(1f, fadeDuration).WaitForCompletion();

        yield return SceneManager.LoadSceneAsync(sceneName);

        yield return fadeImage.DOFade(0f, fadeDuration).WaitForCompletion();

        fadeImage.gameObject.SetActive(false);
    }
}
