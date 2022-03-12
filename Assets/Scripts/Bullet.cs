using UnityEngine;

public class Bullet : MonoBehaviour
{
    public BulletStat stat;
    public int level;
    public bool triggered;

    private void OnEnable()
    {
        triggered = false;
    }

    private void Update()
    {
        transform.position += new Vector3(0, 1, 0) * stat.speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BulletBorder"))
        {
            if (!triggered)
            {
                triggered = true;
                Player.BulletPools[level - 1].Release(gameObject);
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            if (!triggered)
            {
                triggered = true;
                other.GetComponent<Enemy>().OnHit(stat.atk);
            }
        }
    }
}