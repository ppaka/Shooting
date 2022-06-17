using UnityEngine;
using UnityEngine.UI;

public class RankingManager : MonoBehaviour
{
    public InputField nameField;
    public Transform listParent;
    public ListItem prefab;
    
    private bool _canSave;
    
    private void Start()
    {
        _canSave = false;
        Load();
    }

    private void Load()
    {
        var children = listParent.GetComponentsInChildren<ListItem>();
        if (children.Length != 0)
        {
            foreach (var child in children)
            {
                Destroy(child.gameObject);
            }
        }

        for (int i = 0; i < RankSaver.Instance.scores.Count; i++)
        {
            if (i >= 5) break; 
            var obj = Instantiate(prefab, listParent);
            obj.nameText.text = RankSaver.Instance.scores[i].name;
            obj.scoreText.text = RankSaver.Instance.scores[i].score.ToString();

            if (RankSaver.Instance.recentScore > RankSaver.Instance.scores[i].score)
            {
                _canSave = true;
            }
        }

        if (RankSaver.Instance.scores.Count < 5)
        {
            _canSave = true;
        }
    }

    public void Save()
    {
        if (nameField.text == string.Empty || !_canSave)
        {
            return;
        }

        RankSaver.Instance.Save(nameField.text, RankSaver.Instance.recentScore);
        RankSaver.Instance.Clean();
        Load();
        _canSave = false;
    }

    public void Retry()
    {
        SceneLoader.Instance.LoadScene("Play");
    }

    public void Exit()
    {
        SceneLoader.Instance.LoadScene("Title");
    }
}