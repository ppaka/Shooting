using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossTwo : MonoBehaviour
{
    public List<Bullet> spawnedBullet = new List<Bullet>();
    public Bullet prefab;
    public GameObject canon1, canon2;
    public GameObject moveEnd1;
    public EnemyManager enemyManager;
    public float gameTime;
    public bool startPattern;
    
    public int hp, maxHp;
    public Image hpImage;

    private Coroutine _fireRoutine;
    
    private void Awake()
    {
        hp = maxHp;
    }
    
    private void OnEnable()
    {
        var pos = transform.position;
        pos.y = 8;
        transform.position = pos;
    }
    
    public void OnHit(int dmg)
    {
        if (!startPattern) return;
        hp -= dmg;
        if (hp <= 0)
        {
            gameObject.SetActive(false);
            foreach (var bullet in spawnedBullet)
            {
                Destroy(bullet.gameObject);
            }
            StopCoroutine(_fireRoutine);
            StopAllCoroutines();
            enemyManager.GameEnd();
        }
    }
    
    private void Update()
    {
        gameTime += Time.deltaTime;

        if (transform.position.y <= 6 && !startPattern)
        {
            startPattern = true;
            _fireRoutine = StartCoroutine(Pattern());
        }
        else
        {
            var i = Vector3.Lerp(new Vector3(0, 8, 0), new Vector3(0, 6, 0), gameTime / 5);
            transform.position = new Vector3(0, i.y, 0);
        }
        
        for (int j = 0; j < spawnedBullet.Count; j++)
        {
            var bullet = spawnedBullet[j];
            var time = (gameTime - bullet.startTime) * bullet.speed;
            bullet.transform.position = Vector3.Lerp(bullet.startPos, bullet.endPos, time);

            if (time > 1)
            {
                spawnedBullet.Remove(bullet);
                Destroy(bullet.gameObject);
            }
        }

        hpImage.fillAmount = (float) hp / maxHp;
    }
    
    private IEnumerator Pattern()
    {
        while (startPattern)
        {
            yield return StartCoroutine(nameof(Fire));
            enemyManager.Spawn(1, enemyManager.spawnPoints[1]);
            yield return new WaitForSeconds(0.2f);
            enemyManager.Spawn(0, enemyManager.spawnPoints[2]);
            yield return new WaitForSeconds(0.2f);
            enemyManager.Spawn(1, enemyManager.spawnPoints[3]);
            yield return new WaitForSeconds(0.2f);
            enemyManager.Spawn(0, enemyManager.spawnPoints[4]);
            
            yield return new WaitForSeconds(0.4f);
            enemyManager.Spawn(1, enemyManager.spawnPoints[9]);
            enemyManager.Spawn(0, enemyManager.spawnPoints[10]);
            enemyManager.Spawn(0, enemyManager.spawnPoints[11]);
            enemyManager.Spawn(1, enemyManager.spawnPoints[12]);
            yield return new WaitForSeconds(1);
            
            yield return StartCoroutine(nameof(Fire));
            enemyManager.Spawn(0, enemyManager.spawnPoints[19]);
            yield return new WaitForSeconds(0.2f);
            enemyManager.Spawn(1, enemyManager.spawnPoints[18]);
            yield return new WaitForSeconds(0.2f);
            enemyManager.Spawn(0, enemyManager.spawnPoints[17]);
            yield return new WaitForSeconds(0.2f);
            enemyManager.Spawn(1, enemyManager.spawnPoints[16]);
            
            yield return new WaitForSeconds(0.4f);
            enemyManager.Spawn(0, enemyManager.spawnPoints[9]);
            enemyManager.Spawn(0, enemyManager.spawnPoints[10]);
            enemyManager.Spawn(0, enemyManager.spawnPoints[11]);
            enemyManager.Spawn(0, enemyManager.spawnPoints[12]);
            yield return new WaitForSeconds(1);

            StartCoroutine(Fire3());
            yield return StartCoroutine(nameof(Fire2));
            yield return new WaitForSeconds(1.5f);
        }
        yield break;
    }

    private IEnumerator Fire()
    {
        float delay = 0;
        float speed = 4f;
        for (int j = 0; j < 30; j++)
        {
            var i = Instantiate(prefab);
            i.transform.position = canon1.transform.position;
            var k = Instantiate(prefab);
            k.transform.position = canon2.transform.position;
            
            i.direction = Vector3.down;
            k.direction = Vector3.down;
            
            i.startPos = canon1.transform.position;
            k.startPos = canon2.transform.position;

            i.endPos = new Vector3(canon1.transform.position.x, moveEnd1.transform.position.y);
            k.endPos = new Vector3(canon2.transform.position.x, moveEnd1.transform.position.y);

            i.startTime = gameTime + delay;
            i.speed = speed;
            
            k.startTime = gameTime + delay;
            k.speed = speed;

            delay += 0.03f;
            
            spawnedBullet.Add(i);
            spawnedBullet.Add(k);
        }
        
        for (int j = 0; j < 120; j++)
        {
            var i = Instantiate(prefab);
            var startPos1 = canon1.transform.position +
                            new Vector3(Mathf.Abs(canon1.transform.position.x - canon2.transform.position.x)*
                                        ((float)j/120), 0, 0);
            i.transform.position = startPos1;
            
            var k = Instantiate(prefab);
            var startPos2 = canon2.transform.position -
                            new Vector3(Mathf.Abs(canon1.transform.position.x - canon2.transform.position.x)*
                                        ((float)j/120), 0, 0);
            k.transform.position = startPos2;
            
            i.direction = Vector3.down;
            k.direction = Vector3.down;
            
            i.startPos = startPos1;
            k.startPos = startPos2;

            i.endPos = new Vector3(startPos1.x, moveEnd1.transform.position.y);
            k.endPos = new Vector3(startPos2.x, moveEnd1.transform.position.y);

            i.startTime = gameTime + delay;
            i.speed = speed;
            
            k.startTime = gameTime + delay;
            k.speed = speed;

            delay += 0.03f;
            
            spawnedBullet.Add(i);
            spawnedBullet.Add(k);
        }
        
        yield return new WaitForSeconds(1f);
        
        enemyManager.Spawn(0, enemyManager.spawnPoints[9]);
        enemyManager.Spawn(0, enemyManager.spawnPoints[10]);
        enemyManager.Spawn(0, enemyManager.spawnPoints[11]);
        enemyManager.Spawn(0, enemyManager.spawnPoints[12]);
        
        yield return new WaitForSeconds(1f);
        
        enemyManager.Spawn(0, enemyManager.spawnPoints[9]);
        enemyManager.Spawn(1, enemyManager.spawnPoints[10]);
        enemyManager.Spawn(1, enemyManager.spawnPoints[11]);
        enemyManager.Spawn(0, enemyManager.spawnPoints[12]);
        
        yield return new WaitForSeconds(1f);

        yield return new WaitForSeconds(delay);
    }

    private IEnumerator Fire2()
    {
        float delay = 0;
        float speed = 2f;
        for (int j = 0; j < 30; j++)
        {
            var i = Instantiate(prefab);
            i.transform.position = canon1.transform.position;
            var k = Instantiate(prefab);
            k.transform.position = canon2.transform.position;
            
            i.direction = Vector3.down;
            k.direction = Vector3.down;
            
            i.startPos = canon1.transform.position;
            k.startPos = canon2.transform.position;

            i.endPos = new Vector3(canon1.transform.position.x, moveEnd1.transform.position.y);
            k.endPos = new Vector3(canon2.transform.position.x, moveEnd1.transform.position.y);

            i.startTime = gameTime + delay;
            i.speed = speed;
            
            k.startTime = gameTime + delay;
            k.speed = speed;

            delay += 0.25f;
            
            spawnedBullet.Add(i);
            spawnedBullet.Add(k);
        }

        yield return new WaitForSeconds(delay);
    }

    private IEnumerator Fire3()
    {
        var oneShooting = 60f;
        var speed = 1f;
        for (var i = 0; i < oneShooting; i++)
        {
            var obj = Instantiate(prefab, new Vector3(0, 4, 0), Quaternion.identity);
            obj.direction = new Vector3(speed * Mathf.Cos(Mathf.PI * 2 * i / oneShooting),
                speed * Mathf.Sin(Mathf.PI * 2 * i / oneShooting));
            obj.transform.Rotate(new Vector3(0, 0, 360 * i / oneShooting - 90));
            obj.owner = BulletOwner.Enemy;
            obj.speed = speed;
        }

        yield return new WaitForSeconds(1);
        
        for (var i = 0; i < oneShooting; i++)
        {
            var obj = Instantiate(prefab, new Vector3(0, 4, 0), Quaternion.identity);
            obj.direction = new Vector3(speed * Mathf.Cos(Mathf.PI * 2 * i / oneShooting),
                speed * Mathf.Sin(Mathf.PI * 2 * i / oneShooting));
            obj.transform.Rotate(new Vector3(0, 0, 360 * i / oneShooting - 90));
            obj.owner = BulletOwner.Enemy;
            obj.speed = speed;
        }

        yield return new WaitForSeconds(1);
        
        for (var i = 0; i < oneShooting; i++)
        {
            var obj = Instantiate(prefab, new Vector3(0, 4, 0), Quaternion.identity);
            obj.direction = new Vector3(speed * Mathf.Cos(Mathf.PI * 2 * i / oneShooting),
                speed * Mathf.Sin(Mathf.PI * 2 * i / oneShooting));
            obj.transform.Rotate(new Vector3(0, 0, 360 * i / oneShooting - 90));
            obj.owner = BulletOwner.Enemy;
            obj.speed = speed;
        }
        
        yield return new WaitForSeconds(0.5f);
    }
}