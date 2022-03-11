﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    public Enemy[] enemies;
    public static IObjectPool<Enemy>[] EnemyPools = new IObjectPool<Enemy>[6];
    public Player player;
    public ItemManager itemManager;
    public float maxSpawnDelay;
    public float curSpawnDelay;
    public int spawnPointRate = 8;
    public List<Vector3> spawnPoints = new();
    [Header("SpawnDelaySettings", order = 2)]
    public float maxDelay;
    public float minDelay;

    private void Awake()
    {
        EnemyPools[0] = new ObjectPool<Enemy>(() => Instantiate(enemies[0]),
            enemy =>
            {
                enemy.gameObject.SetActive(true);
                enemy.Setup();
            }, enemy => { enemy.gameObject.SetActive(false); },
            enemy => { Destroy(enemy.gameObject); }, false, 20, 5000);
        
        EnemyPools[1] = new ObjectPool<Enemy>(() => Instantiate(enemies[1]),
            enemy =>
            {
                enemy.gameObject.SetActive(true);
                enemy.Setup();
            }, enemy => { enemy.gameObject.SetActive(false); },
            enemy => { Destroy(enemy.gameObject); }, false, 20, 5000);
        
        EnemyPools[2] = new ObjectPool<Enemy>(() => Instantiate(enemies[2]),
            enemy =>
            {
                enemy.gameObject.SetActive(true);
                enemy.Setup();
            }, enemy => { enemy.gameObject.SetActive(false); },
            enemy => { Destroy(enemy.gameObject); }, false, 20, 5000);
        
        EnemyPools[3] = new ObjectPool<Enemy>(() => Instantiate(enemies[3]),
            enemy =>
            {
                enemy.gameObject.SetActive(true);
                enemy.Setup();
            }, enemy => { enemy.gameObject.SetActive(false); },
            enemy => { Destroy(enemy.gameObject); }, false, 20, 5000);

        EnemyPools[4] = new ObjectPool<Enemy>(() => Instantiate(enemies[4]),
            enemy =>
            {
                enemy.gameObject.SetActive(true);
                enemy.Setup();
            }, enemy => { enemy.gameObject.SetActive(false); },
            enemy => { Destroy(enemy.gameObject); }, false, 20, 5000);
        
        EnemyPools[5] = new ObjectPool<Enemy>(() => Instantiate(enemies[5]),
            enemy =>
            {
                enemy.gameObject.SetActive(true);
                enemy.Setup();
            }, enemy => { enemy.gameObject.SetActive(false); },
            enemy => { Destroy(enemy.gameObject); }, false, 20, 5000);
        
        SetupSpawn();
    }

    private void SetupSpawn()
    {
        var rate = 5f / spawnPointRate;
        var nextSpawnPointPos = -2.5f;

        for (var i = 0; i < spawnPointRate; i++)
        {
            spawnPoints.Add(new Vector3(nextSpawnPointPos, 7, 0));
            nextSpawnPointPos += rate;
        }
    }

    private void Update()
    {
        curSpawnDelay += Time.deltaTime;

        if (curSpawnDelay > maxSpawnDelay)
        {
            Spawn(Random.Range(0, enemies.Length), spawnPoints[Random.Range(0, spawnPoints.Count)]);
            /*Spawn(0, spawnPoints[Random.Range(0, spawnPoints.Count)]);
            Spawn(1, spawnPoints[Random.Range(0, spawnPoints.Count)]);
            Spawn(2, spawnPoints[Random.Range(0, spawnPoints.Count)]);
            Spawn(3, spawnPoints[Random.Range(0, spawnPoints.Count)]);
            Spawn(4, spawnPoints[Random.Range(0, spawnPoints.Count)]);*/
            maxSpawnDelay = Random.Range(minDelay, maxDelay);
            curSpawnDelay = 0;
        }
    }

    private void Spawn(int type, Vector3 position)
    {
        var i = EnemyPools[type].Get();
        i.transform.position = position;
        i.player = player;
        i.itemManager = itemManager;
    }
}