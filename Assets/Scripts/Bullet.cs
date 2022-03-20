using UnityEngine;

public enum BulletOwner
{
    Player,
    Enemy,
    BossOne,
    BossTwo
}

public class Bullet : MonoBehaviour
{
    public BulletStat stat;
    public int level;
    public BulletOwner owner = BulletOwner.Player;
    public Vector3 direction, startPos, endPos;
    public float startTime, speed;

    private void Update()
    {
        if (owner != BulletOwner.BossOne && owner != BulletOwner.BossTwo) transform.position += direction * stat.speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BulletBorder"))
        {
            if (owner == BulletOwner.Player) Destroy(gameObject);
            else if(owner == BulletOwner.Enemy) Destroy(gameObject);
        }
        else if (other.CompareTag("Enemy"))
        {
            if (owner == BulletOwner.Player)
            {
                other.GetComponent<Enemy>().OnHit(stat.atk);
                Destroy(gameObject);
            }
        }
        else if (other.CompareTag("Player"))
        {
            if (owner == BulletOwner.Enemy || owner == BulletOwner.BossOne || owner == BulletOwner.BossTwo)
            {
                other.GetComponent<Player>().OnDamaged(stat.atk);
            }
        }
        else if (other.CompareTag("Boss"))
        {
            if (owner == BulletOwner.Player)
            {
                if(other.TryGetComponent<BossOne>(out var bossOne)) bossOne.OnHit(stat.atk);
                else if (other.TryGetComponent<BossTwo>(out var bossTwo)) bossTwo.OnHit(stat.atk);
            }
        }
    }
}