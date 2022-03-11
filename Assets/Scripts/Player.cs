using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
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

    public Bullet[] bullets;
    public float[] fireDelays = { 0.18f, 0.23f };
    public float timeSinceLastFire;
    public static IObjectPool<Bullet>[] BulletPools = new IObjectPool<Bullet>[5];

    private float _offset;
    public float scrollSpeed = 0.5f;
    public RawImage img, img2, img3;

    public Image hpImage, altHpImage;

    private void Awake()
    {
        BulletPools[0] = new ObjectPool<Bullet>(() => Instantiate(bullets[0]),
            bullet => { bullet.gameObject.SetActive(true); }, bullet => { bullet.gameObject.SetActive(false); },
            bullet => { Destroy(bullet.gameObject); }, false, 20, 10000);
        BulletPools[1] = new ObjectPool<Bullet>(() => Instantiate(bullets[1]),
            bullet => { bullet.gameObject.SetActive(true); }, bullet => { bullet.gameObject.SetActive(false); },
            bullet => { Destroy(bullet.gameObject); }, false, 20, 10000);

        hp = maxHp;
        if (stage == 1) altHp = maxAltHp - maxAltHp * 0.1f;
        else if (stage == 2) altHp = maxAltHp - maxAltHp * 0.3f;

        _camera = Camera.main;
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) UpgradeWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) UpgradeWeapon(2);

        Clock();
        if (Input.GetKey(KeyCode.X)) Fire(weaponLevel);
        Move();
        StayInCamera();
        BgScroll();

        hpImage.fillAmount = hp / maxHp;
        altHpImage.fillAmount = Mathf.Abs(altHp - maxAltHp) / maxAltHp;
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