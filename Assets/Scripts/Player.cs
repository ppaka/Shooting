using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int score;

    public float speed = 3;
    public int stage = 1;
    public int weaponLevel = 1;
    public float hp, altHp;
    public float maxHp, maxAltHp;
    public float timeSinceLastHit;
    public float invincibleTime = 1.5f;
    public float invincibleEffectTime = 1.5f;
    private Camera _camera;
    private Animator _animator;
    private SpriteRenderer _sr;
    public EnemyManager enemyManager;

    public Bullet[] bullets;
    public float[] fireDelays = {0.18f, 0.23f, 0.18f, 0.2f, 0.03f};
    public float timeSinceLastFire;

    private float _offset;
    public float scrollSpeed = 0.5f;
    public RawImage img, img2, img3;

    public GameObject uiCanvas, introCanvas;
    public Image hpImage, altHpImage;
    public Text scoreText, hpText, altHpText;

    public bool gameStarted;

    private void Awake()
    {
        Time.timeScale = 0;

        SetAltHp();
        hp = maxHp;
        timeSinceLastHit = 10;

        _camera = Camera.main;
        _animator = GetComponent<Animator>();
        _sr = GetComponent<SpriteRenderer>();
    }

    public void SetAltHp()
    {
        if (stage == 1) altHp = maxAltHp - maxAltHp * 0.1f;
        else if (stage == 2) altHp = maxAltHp - maxAltHp * 0.3f;
    }

    private void Update()
    {
        if (!gameStarted)
        {
            if (Input.GetKeyDown(KeyCode.Space)) StartGame();
            else return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) UpgradeWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) UpgradeWeapon(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) UpgradeWeapon(3);
        if (Input.GetKeyDown(KeyCode.Alpha4)) UpgradeWeapon(4);
        if (Input.GetKeyDown(KeyCode.Alpha5)) UpgradeWeapon(5);

        Clock();
        InvincibleEffect();
        Move();
        StayInCamera();
        BgScroll();

        hpText.text = (hp / maxHp * 100).ToString();
        altHpText.text = (Mathf.Abs(altHp - maxAltHp) / maxAltHp * 100).ToString();
        hpImage.fillAmount = hp / maxHp;
        altHpImage.fillAmount = Mathf.Abs(altHp - maxAltHp) / maxAltHp;
    }

    private void StartGame()
    {
        gameStarted = true;
        uiCanvas.SetActive(true);
        introCanvas.SetActive(false);
        Time.timeScale = 1;
        enemyManager.NextStage(1);
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.X)) Fire(weaponLevel);
    }

    private void InvincibleEffect()
    {
        if (timeSinceLastHit <= invincibleEffectTime)
        {
            var i = Mathf.Sin(timeSinceLastHit * 3.14f * 10) * 255;
            var srColor = _sr.color;
            srColor.a = i / 255;
            _sr.color = srColor;
        }
        else
        {
            var srColor = _sr.color;
            srColor.a = 1;
            _sr.color = srColor;
        }
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
        score += Mathf.FloorToInt(value);
        scoreText.text = score.ToString();
    }

    public void OnDamaged(float damage)
    {
        if (timeSinceLastHit <= invincibleTime) return;
        timeSinceLastHit = 0;
        invincibleEffectTime = 1.5f;
        invincibleTime = 1.5f;
        hp -= damage;
        if (hp <= 0)
        {
            Debug.Log("죽음");
            RankSaver.Instance.recentScore = score;
            SceneManager.LoadScene("GameEnd");
        }
    }

    public void OnDamagedAlt(float damage)
    {
        altHp -= damage;
        _camera.GetComponent<CameraShake>().VibrateForTime(0.4f);
        if (altHp <= 0)
        {
            Debug.Log("보조 체력 죽음");
            RankSaver.Instance.recentScore = score;
            SceneManager.LoadScene("GameEnd");
        }
    }

    private void Clock()
    {
        timeSinceLastFire += Time.deltaTime;
        timeSinceLastHit += Time.deltaTime;
    }

    public void UpgradeWeapon(int level)
    {
        if (level >= 1 && level <= bullets.Length) weaponLevel = level;
    }

    private void Fire(int level)
    {
        if (timeSinceLastFire > fireDelays[level - 1])
        {
            timeSinceLastFire = 0;
            var bullet = Instantiate(bullets[level - 1]);
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
        _animator.SetInteger("Input", (int) value);
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

    public void FireBomb()
    {
        var oneShooting = 60f;
        var speed = 1f;
        for (var i = 0; i < oneShooting; i++)
        {
            var obj = Instantiate(bullets[1], transform.position, Quaternion.identity);
            obj.direction = new Vector3(speed * Mathf.Cos(Mathf.PI * 2 * i / oneShooting),
                speed * Mathf.Sin(Mathf.PI * i * 2 / oneShooting));
            obj.transform.Rotate(new Vector3(0, 0, 360 * i / oneShooting - 90));
        }
    }
}