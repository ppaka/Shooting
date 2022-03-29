using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    public Enemy[] enemies;
    public Player player;
    public ItemManager itemManager;
    public float maxSpawnDelay;
    public float curSpawnDelay;
    public int spawnPointRate = 8;
    public List<Vector3> spawnPoints = new List<Vector3>();

    public int enemyCount = 0, maxStageOneEnemyCount = 20, maxStageTwoEnemyCount = 40;

    [Header("SpawnDelaySettings", order = 2)]
    public float maxDelay;

    public float minDelay;

    public List<Enemy> spawnedEnemies = new List<Enemy>();
    public List<Enemy> spawnedNpc = new List<Enemy>();
    public Bullet enemyBulletPrefab;
    public BossOne bossOne;
    public BossTwo bossTwo;
    public bool bossOneDead, bossTwoDead;

    public Image[] images;
    private float _waitingDelay;
    public GameObject resultCanvas;
    public Text nextStageText;
    private Coroutine _coroutine;

    private void Awake()
    {
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
        KillAll();
        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(StageDelay(stageNum));
    }

    private void KillAll()
    {
        foreach (var enemy in spawnedEnemies)
        {
            if (enemy != null)
                Destroy(enemy.gameObject);
        }

        spawnedEnemies.Clear();
        foreach (var npc in spawnedNpc)
        {
            if (npc != null)
                Destroy(npc.gameObject);
        }

        spawnedNpc.Clear();
    }

    public void GameEnd()
    {
        RankSaver.Instance.recentScore = player.score;
        SceneManager.LoadScene("GameEnd");
    }

    private IEnumerator StageDelay(int stageNum)
    {
        resultCanvas.SetActive(true);
        player.stage = 0;
        nextStageText.text = "Stage " + stageNum;
        _waitingDelay = 1;
        while (_waitingDelay >= 0)
        {
            _waitingDelay -= Time.deltaTime * 0.25f;
            images[0].fillAmount = _waitingDelay;
            images[1].fillAmount = _waitingDelay;
            yield return null;
        }

        resultCanvas.SetActive(false);

        enemyCount = 0;
        player.stage = stageNum;

        if (stageNum == 2)
        {
            bossOneDead = true;
            bossTwoDead = false;
            player.hp = player.maxHp;
            player.stage = 2;
            player.SetAltHp();
        }
        else
        {
            bossOneDead = false;
            bossTwoDead = false;
            player.hp = player.maxHp;
            player.stage = 1;
            player.SetAltHp();
        }

        yield return null;
    }

    private void Update()
    {
        if (!player.gameStarted) return;
        if (player.canInput)
        {
            if (Input.GetKeyDown(KeyCode.Alpha8)) NextStage(1);
            if (Input.GetKeyDown(KeyCode.Alpha9)) NextStage(2);
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                if (bossOne.gameObject.activeSelf) bossOne.OnHit(int.MaxValue);
                if (bossTwo.gameObject.activeSelf) bossTwo.OnHit(int.MaxValue);
                KillAll();
            }
            if (Input.GetKeyDown(KeyCode.O)) Spawn(4, spawnPoints[Random.Range(0, spawnPoints.Count)]);
            if (Input.GetKeyDown(KeyCode.P)) Spawn(5, spawnPoints[Random.Range(0, spawnPoints.Count)]);
        }
        curSpawnDelay += Time.deltaTime;

        if (player.stage == 1 && curSpawnDelay > maxSpawnDelay && enemyCount < maxStageOneEnemyCount)
        {
            Spawn(Random.Range(0, enemies.Length), spawnPoints[Random.Range(0, spawnPoints.Count)]);
            maxSpawnDelay = Random.Range(minDelay, maxDelay);
            curSpawnDelay = 0;
            enemyCount++;
        }
        else if (player.stage == 2 && curSpawnDelay > maxSpawnDelay && enemyCount < maxStageTwoEnemyCount)
        {
            Spawn(Random.Range(0, enemies.Length), spawnPoints[Random.Range(0, spawnPoints.Count)]);
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
        else if (player.stage == 2 && enemyCount >= maxStageTwoEnemyCount)
        {
            if (!bossTwoDead)
            {
                bossTwo.gameObject.SetActive(true);
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
                    var bullet = Instantiate(enemyBulletPrefab);
                    bullet.stat.atk = (int) enemy.stat.enemyAtk;
                    bullet.transform.position = enemy.transform.position;
                }
            }
        }
    }

    public void Spawn(int type, Vector3 position)
    {
        var i = Instantiate(enemies[type]);
        i.Setup();
        i.transform.position = position;
        i.player = player;
        i.itemManager = itemManager;
        i.enemyManager = this;
        if (i.type != EnemyType.Npc1 && i.type != EnemyType.Npc2)
        {
            spawnedEnemies.Add(i);
        }
        else
        {
            spawnedNpc.Add(i);
        }
    }
}