using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BossOne : MonoBehaviour
{
    public static ObjectPool<Bullet> BulletPool;
    public List<Bullet> spawnedBullet = new();
    public Bullet prefab;
    public GameObject canon1, canon2;
    public GameObject moveEnd1;
    public float gameTime;
    private bool _startPattern;

    private void Awake()
    {
        BulletPool = new ObjectPool<Bullet>(() => Instantiate(prefab),
            bullet => { bullet.gameObject.SetActive(true); }, bullet => { bullet.gameObject.SetActive(false); },
            bullet => { Destroy(bullet.gameObject); }, true, 20);
    }

    private void OnEnable()
    {
        var pos = transform.position;
        pos.y = 8;
        transform.position = pos;
    }

    private void Update()
    {
        gameTime += Time.deltaTime;

        if (transform.position.y <= 6 && !_startPattern)
        {
            _startPattern = true;
            StartCoroutine(Pattern());
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
                BulletPool.Release(bullet);
            }
        }
    }

    private IEnumerator Pattern()
    {
        StartCoroutine(Fire());
        yield break;
    }

    private IEnumerator Fire()
    {
        float delay = 0;
        float speed = 1f;
        for (int j = 0; j < 30; j++)
        {
            var i = BulletPool.Get();
            i.transform.position = canon1.transform.position;
            var k = BulletPool.Get();
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

            delay += 0.05f;
            
            spawnedBullet.Add(i);
            spawnedBullet.Add(k);
        }
        
        for (int j = 0; j < 120; j++)
        {
            var i = BulletPool.Get();
            var startPos1 = canon1.transform.position +
                            new Vector3(Mathf.Abs(canon1.transform.position.x - canon2.transform.position.x)*
                                        ((float)j/120), 0, 0);
            i.transform.position = startPos1;
            
            var k = BulletPool.Get();
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

            delay += 0.05f;
            
            spawnedBullet.Add(i);
            spawnedBullet.Add(k);
        }

        yield return null;
    }
}
