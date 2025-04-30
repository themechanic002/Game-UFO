using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    public void OnClickPlay()
    {
        SceneManager.LoadScene("MainGame");
    }
}
