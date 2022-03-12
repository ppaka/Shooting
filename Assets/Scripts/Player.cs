using System;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float score;
    
    public float speed = 3;
    public int stage = 1;
    public int weaponLevel = 1;
    public float hp, altHp;
    public float maxHp, maxAltHp;
    public float timeSinceLastHit;
    public float invincibleTime = 1.5f;
    private float _invincibleEffectTime = 2.5f;
    private Camera _camera;
    private Animator _animator;

    public GameObject[] bullets;
    public float[] fireDelays = { 0.18f, 0.23f };
    public float timeSinceLastFire;
    public static IObjectPool<GameObject>[] BulletPools = new IObjectPool<GameObject>[5];

    private float _offset;
    public float scrollSpeed = 0.5f;
    public RawImage img, img2, img3;

    public Image hpImage, altHpImage;
    public Text scoreText;

    private void Awake()
    {
        BulletPools[0] = new ObjectPool<GameObject>(() => Instantiate(bullets[0]),
            go => { go.SetActive(true); }, go => { go.SetActive(false); },
            go => { Destroy(go); }, true, 20, 10000);
        BulletPools[1] = new ObjectPool<GameObject>(() => Instantiate(bullets[1]),
            go => { go.SetActive(true); }, go => { go.SetActive(false); },
            go => { Destroy(go); }, true, 20, 10000);
        BulletPools[2] = new ObjectPool<GameObject>(() => Instantiate(bullets[2]),
            go => { go.SetActive(true); }, go => { go.SetActive(false); },
            go => { Destroy(go); }, true, 20, 10000);
        BulletPools[3] = new ObjectPool<GameObject>(() => Instantiate(bullets[3]),
            go => { go.SetActive(true); }, go => { go.SetActive(false); },
            go => { Destroy(go); }, true, 20, 10000);
        BulletPools[4] = new ObjectPool<GameObject>(() => Instantiate(bullets[4]),
            go => { go.SetActive(true); }, go => { go.SetActive(false); },
            go => { Destroy(go); }, true, 20, 10000);
        
        if (stage == 1) altHp = maxAltHp - maxAltHp * 0.1f;
        else if (stage == 2) altHp = maxAltHp - maxAltHp * 0.3f;

        hp = maxHp;

        _camera = Camera.main;
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) UpgradeWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) UpgradeWeapon(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) UpgradeWeapon(3);
        if (Input.GetKeyDown(KeyCode.Alpha4)) UpgradeWeapon(4);
        if (Input.GetKeyDown(KeyCode.Alpha5)) UpgradeWeapon(5);

        Clock();
        Move();
        StayInCamera();
        BgScroll();

        hpImage.fillAmount = hp / maxHp;
        altHpImage.fillAmount = Mathf.Abs(altHp - maxAltHp) / maxAltHp;
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.X)) Fire(weaponLevel);
    }

    private void BgScroll()
    {
        _offset += Time.deltaTime * scrollSpeed;

        var imgUVRect = img.uvRect;
        imgUVRect.y = _offset / 1.3f;
        img.uvRect = imgUVRect;

        var img2UVRect = img2.uvRect;
        img2UVRect.y = _offset / 1.4f;
        img2.uvRect = img2UVRect;

        var img3UVRect = img3.uvRect;
        img3UVRect.y = _offset / 1.5f;
        img3.uvRect = img3UVRect;
    }

    public void AddScore(float value)
    {
        score += value;
        scoreText.text = score.ToString();
    }
    
    public void OnDamaged(float damage)
    {
        if (timeSinceLastHit <= invincibleTime) return;
        timeSinceLastHit = 0;
        hp -= damage;
        if (hp <= 0)
        {
            Debug.Log("죽음");
        }
    }

    public void OnDamagedAlt(float damage)
    {
        altHp -= damage;
        if (altHp <= 0)
        {
            Debug.Log("보조 체력 죽음");
        }
    }

    private void Clock()
    {
        timeSinceLastFire += Time.deltaTime;
        timeSinceLastHit += Time.deltaTime;
    }

    public void UpgradeWeapon(int level)
    {
        if (level >= 1 && level <= BulletPools.Length) weaponLevel = level;
    }

    private void Fire(int level)
    {
        if (timeSinceLastFire > fireDelays[level-1])
        {
            timeSinceLastFire = 0;
            var bullet = BulletPools[level-1].Get();
            bullet.transform.position = transform.position;
        }
    }

    private void Move()
    {
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");

        transform.position += new Vector3(h, v, 0).normalized * speed * Time.deltaTime;

        SetAnimation(h);
    }

    private void SetAnimation(float value)
    {
        _animator.SetInteger("Input", (int)value);
    }

    private void StayInCamera()
    {
        var curPoint = _camera.WorldToViewportPoint(transform.position);
        curPoint.x = Mathf.Clamp(curPoint.x, 0.35f, 0.65f);
        curPoint.y = Mathf.Clamp01(curPoint.y);

        var result = _camera.ViewportToWorldPoint(curPoint);
        result.z = 0;
        transform.position = result;
    }
}