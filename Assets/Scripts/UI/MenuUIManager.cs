using Unity.VisualScripting;
using UnityEngine;
public class MenuUIManager : MonoBehaviour
{
    public void OpenURLButton(string link)
    {
        Application.OpenURL(link);
    }
    public void StoreButton()
    {
        PopupText.Instance.ShowPopup("Cash Earned");
    }
    public void ExitButton()
    {
        SceneTransitionEffect.Instance.Exit();
    }
    public void PlayButton()
    {
        SceneTransitionEffect.Instance.LoadScene("Game");
    }
}
