using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{
    public void Play()
    {
        SceneLoader.Instance.LoadScene("Play");
        //SceneManager.LoadScene("Play");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
