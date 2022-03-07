using UnityEngine;
using UnityEngine.Pool;

public class Player : MonoBehaviour
{
    public float speed = 3;
    public int weaponLevel = 1;
    private Camera _camera;
    private Animator _animator;

    public Bullet[] bullets;
    public float[] fireDelays = { 0.18f, 0.23f };
    public float timeSinceLastFire;
    public static IObjectPool<Bullet>[] BulletPools = new IObjectPool<Bullet>[5];

    private void Awake()
    {
        BulletPools[0] = new ObjectPool<Bullet>(() => Instantiate(bullets[0]),
            bullet => { bullet.gameObject.SetActive(true); }, bullet => { bullet.gameObject.SetActive(false); },
            bullet => { Destroy(bullet.gameObject); }, false, 20, 10000);
        BulletPools[1] = new ObjectPool<Bullet>(() => Instantiate(bullets[1]),
            bullet => { bullet.gameObject.SetActive(true); }, bullet => { bullet.gameObject.SetActive(false); },
            bullet => { Destroy(bullet.gameObject); }, false, 20, 10000);

        _camera = Camera.main;
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            weaponLevel = 1;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            weaponLevel = 2;
        }
        
        Clock();
        if (Input.GetKey(KeyCode.X)) Fire(weaponLevel-1);
        Move();
        StayInCamera();
    }

    private void Clock()
    {
        timeSinceLastFire += Time.deltaTime;
    }

    private void Fire(int level)
    {
        if (timeSinceLastFire > fireDelays[level])
        {
            timeSinceLastFire = 0;
            var bullet = BulletPools[level].Get();
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
        curPoint.x = Mathf.Clamp(curPoint.x, 0.35f, 0.66f);
        curPoint.y = Mathf.Clamp01(curPoint.y);

        var result = _camera.ViewportToWorldPoint(curPoint);
        result.z = 0;
        transform.position = result;
    }
}