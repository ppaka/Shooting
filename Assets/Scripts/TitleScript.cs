﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("Play");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
