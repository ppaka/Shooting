using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader instance;

    public static SceneLoader Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            var obj = FindObjectOfType<SceneLoader>();
            if(obj == null) obj = Instantiate(Resources.Load<SceneLoader>("SceneLoader"));
            instance = obj;
            return instance;
        }
        set
        {
            instance = value;
        }
    }

    public Image image;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(Load(sceneName));
    }

    private IEnumerator Load(string sceneName)
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        image.fillAmount = 0;
        image.fillOrigin = 0;
        image.gameObject.SetActive(true);
        image.fillMethod = Image.FillMethod.Horizontal;
        while (image.fillAmount < 1)
        {
            image.fillAmount += Time.fixedDeltaTime * 0.5f;
            yield return null;
        }

        image.fillAmount = 1;
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    private IEnumerator Close()
    {
        image.fillAmount = 1;
        image.fillOrigin = 1;
        image.fillMethod = Image.FillMethod.Horizontal;
        while (image.fillAmount > 0)
        {
            image.fillAmount -= Time.fixedDeltaTime * 0.5f;
            yield return null;
        }

        image.fillAmount = 0;
        image.gameObject.SetActive(false);
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        StartCoroutine(Close());
    }
}
