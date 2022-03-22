using System;
using System.Collections.Generic;
using UnityEngine;

public class RankSaver : MonoBehaviour
{
    private static RankSaver instance;

    public static RankSaver Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<RankSaver>();
                if (obj != null)
                {
                    instance = obj;
                }
                else
                {
                    var newSingleton = new GameObject("RankSaver").AddComponent<RankSaver>();
                    instance = newSingleton;
                }
            }

            return instance;
        }
        private set { instance = value; }
    }

    private void Awake()
    {
        var objs = FindObjectsOfType<RankSaver>();
        if (objs.Length != 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public List<(string name, int score)> scores = new List<(string name, int score)>();

    public int recentScore;
    
    public void Save(string name, int score)
    {
        scores.Add((name, score));
        scores.Sort((tuple, valueTuple) => valueTuple.score.CompareTo(tuple.score));
    }

    public void Clean()
    {
        var newList = new List<(string name, int score)>();
        for (int i = 0; i < scores.Count; i++)
        {
            if (i >= 5)
            {
                break;
            }
            newList.Add(scores[i]);
        }

        scores = newList;
    }
}