using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
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

    public int enemyCount = 0, maxStageOneEnemyCount = 20, maxStageTwoEnemyCount = 40;

    [Header("SpawnDelaySettings", order = 2)]
    public float maxDelay;
    public float minDelay;

    public List<Enemy> spawnedEnemies = new();
    public static IObjectPool<Bullet> BulletPool;
    public Bullet enemyBulletPrefab;
    public BossOne bossOne;
    public bool bossOneDead, bossTwoDead;

    public Image[] images;
    private float _waitingDelay;
    public GameObject resultCanvas;

    private void Awake()
    {
        EnemyPools[0] = new ObjectPool<Enemy>(() => Instantiate(enemies[0]),
            enemy =>
            {
                enemy.gameObject.SetActive(true);
                enemy.Setup();
            }, enemy =>
            {
                spawnedEnemies.Remove(enemy);
                enemy.gameObject.SetActive(false);
            },
            enemy =>
            {
                spawnedEnemies.Remove(enemy);
                Destroy(enemy.gameObject);
            }, false, 20, 5000);

        EnemyPools[1] = new ObjectPool<Enemy>(() => Instantiate(enemies[1]),
            enemy =>
            {
                enemy.gameObject.SetActive(true);
                enemy.Setup();
            }, enemy =>
            {
                spawnedEnemies.Remove(enemy);
                enemy.gameObject.SetActive(false);
            },
            enemy =>
            {
                spawnedEnemies.Remove(enemy);
                Destroy(enemy.gameObject);
            }, false, 20, 5000);

        EnemyPools[2] = new ObjectPool<Enemy>(() => Instantiate(enemies[2]),
            enemy =>
            {
                enemy.gameObject.SetActive(true);
                enemy.Setup();
            }, enemy =>
            {
                spawnedEnemies.Remove(enemy);
                enemy.gameObject.SetActive(false);
            },
            enemy =>
            {
                spawnedEnemies.Remove(enemy);
                Destroy(enemy.gameObject);
            }, false, 20, 5000);

        EnemyPools[3] = new ObjectPool<Enemy>(() => Instantiate(enemies[3]),
            enemy =>
            {
                enemy.gameObject.SetActive(true);
                enemy.Setup();
            }, enemy =>
            {
                spawnedEnemies.Remove(enemy);
                enemy.gameObject.SetActive(false);
            },
            enemy =>
            {
                spawnedEnemies.Remove(enemy);
                Destroy(enemy.gameObject);
            }, false, 20, 5000);

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

        BulletPool = new ObjectPool<Bullet>(() => Instantiate(enemyBulletPrefab),
            bullet => { bullet.gameObject.SetActive(true); }, bullet => { bullet.gameObject.SetActive(false); },
            bullet => { Destroy(bullet.gameObject); }, true, 20);

        SetupSpawn();
    }

    private void SetupSpawn()
    {
        var rate = 5f / spawnPointRate;
        var nextSpawnPointPos = -2.5f;

        for (var i = 0; i < spawnPointRate; i++)
        {
            spawnPoints.Add(new Vector3(nextSpawnPointPos, 5.2f, 0));
            nextSpawnPointPos += rate;
        }
    }

    public void NextStage(int stageNum)
    {
        bossOneDead = true;
        bossTwoDead = true;
        StartCoroutine(StageDelay(stageNum));
    }

    private IEnumerator StageDelay(int stageNum)
    {
        resultCanvas.SetActive(true);
        _waitingDelay = 1;
        while (_waitingDelay >= 0)
        {
            _waitingDelay -= Time.deltaTime * 0.25f;
            images[0].fillAmount = _waitingDelay;
            images[1].fillAmount = _waitingDelay;
            yield return new WaitForEndOfFrame();
        }
        resultCanvas.SetActive(false);

        player.stage = stageNum;
        bossOneDead = false;
        bossTwoDead = false;
        yield return null;
    }

    private void Update()
    {
        curSpawnDelay += Time.deltaTime;

        if (player.stage == 1 && curSpawnDelay > maxSpawnDelay && enemyCount < maxStageOneEnemyCount)
        {
            Spawn(Random.Range(0, enemies.Length), spawnPoints[Random.Range(0, spawnPoints.Count)]);
            /*Spawn(0, spawnPoints[Random.Range(0, spawnPoints.Count)]);
            Spawn(1, spawnPoints[Random.Range(0, spawnPoints.Count)]);
            Spawn(2, spawnPoints[Random.Range(0, spawnPoints.Count)]);
            Spawn(3, spawnPoints[Random.Range(0, spawnPoints.Count)]);
            Spawn(4, spawnPoints[Random.Range(0, spawnPoints.Count)]);*/
            maxSpawnDelay = Random.Range(minDelay, maxDelay);
            curSpawnDelay = 0;
            enemyCount++;
        }else if (player.stage == 2 && curSpawnDelay > maxSpawnDelay && enemyCount < maxStageTwoEnemyCount)
        {
            Spawn(Random.Range(0, enemies.Length), spawnPoints[Random.Range(0, spawnPoints.Count)]);
            /*Spawn(0, spawnPoints[Random.Range(0, spawnPoints.Count)]);
            Spawn(1, spawnPoints[Random.Range(0, spawnPoints.Count)]);
            Spawn(2, spawnPoints[Random.Range(0, spawnPoints.Count)]);
            Spawn(3, spawnPoints[Random.Range(0, spawnPoints.Count)]);
            Spawn(4, spawnPoints[Random.Range(0, spawnPoints.Count)]);*/
            maxSpawnDelay = Random.Range(minDelay, maxDelay);
            curSpawnDelay = 0;
            enemyCount++;
        }

        if (player.stage == 1 && enemyCount >= maxStageOneEnemyCount)
        {
            if (!bossOneDead)
            {
                bossOne.gameObject.SetActive(true);
            }
        }
        else if(player.stage == 2 && enemyCount >= maxStageTwoEnemyCount)
        {
            if (!bossTwoDead)
            {
                bossOne.gameObject.SetActive(true);
            }
        }

        foreach (var enemy in spawnedEnemies)
        {
            if (enemy.type != EnemyType.Enemy1)
            {
                enemy.timeSinceFire += Time.deltaTime;
                if (enemy.timeSinceFire >= enemy.stat.fireDelay)
                {
                    enemy.timeSinceFire = 0;
                    var bullet = BulletPool.Get();
                    bullet.stat.atk = (int)enemy.stat.enemyAtk;
                    bullet.transform.position = enemy.transform.position;
                }
            }
        }
    }

    private void Spawn(int type, Vector3 position)
    {
        var i = EnemyPools[type].Get();
        i.transform.position = position;
        i.player = player;
        i.itemManager = itemManager;
        if (i.type != EnemyType.Npc1 && i.type != EnemyType.Npc2)
        {
            spawnedEnemies.Add(i);
        }
    }
}