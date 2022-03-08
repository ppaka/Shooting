using UnityEngine;

public class Bullet : MonoBehaviour
{
    public BulletStat stat;
    public int level;

    private void Update()
    {
        transform.position += new Vector3(0, 1, 0) * stat.speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BulletBorder"))
        {
            Player.BulletPools[level - 1].Release(this);
        }
        else if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().OnHit(stat.atk);
        }
    }
}