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
            if (owner == BulletOwner.Player) Player.BulletPools[level - 1].Release(this);
            else if(owner == BulletOwner.Enemy) EnemyManager.BulletPool.Release(this);
        }
        else if (other.CompareTag("Enemy"))
        {
            if (owner == BulletOwner.Player) other.GetComponent<Enemy>().OnHit(stat.atk);
        }
        else if (other.CompareTag("Player"))
        {
            if (owner == BulletOwner.Enemy || owner == BulletOwner.BossOne || owner == BulletOwner.BossTwo) other.GetComponent<Player>().OnDamaged(stat.atk);
        }
    }
}