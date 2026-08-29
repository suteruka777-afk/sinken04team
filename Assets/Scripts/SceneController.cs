using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void OnClickStartButton()
    {
        SceneManager.LoadScene("Ingame");
    }
}