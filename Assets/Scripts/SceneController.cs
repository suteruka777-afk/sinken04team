using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void OnClickStartButton()
    {
        SceneManager.LoadScene("Ingame");
    }
    public void OnClickRetryButton()
    {
        SceneManager.LoadScene("Ingame");
    }
    public void OnClickEndButton()
    {
        SceneManager.LoadScene("Title");
    }
}