using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RankingManager : MonoBehaviour
{
    public InputField nameField;
    public Transform listParent;
    public ListItem prefab;
    
    private bool _isSaved;
    
    private void Start()
    {
        _isSaved = false;
        Load();
    }

    private void Load()
    {
        var children = listParent.GetComponentsInChildren<ListItem>();
        if (children.Length != 0)
            foreach (var child in children)
            {
                Destroy(child.gameObject);
            }

        foreach (var (name, score) in RankSaver.Instance.scores)
        {
            var obj = Instantiate(prefab, listParent);
            obj.nameText.text = name;
            obj.scoreText.text = score.ToString();
        }
    }

    public void Save()
    {
        if (nameField.text == String.Empty || _isSaved)
        {
            return;
        }

        RankSaver.Instance.Save(nameField.text, RankSaver.Instance.recentScore);
        _isSaved = true;
        Load();
    }

    public void Retry()
    {
        SceneManager.LoadScene("Scenes/Play");
    }
}