using UnityEngine;

public enum EnemyType
{
    Enemy1,
    Enemy2,
    Enemy3,
    Enemy4,
    Npc1,
    Npc2
}

public class Enemy : MonoBehaviour
{
    public EnemyStat stat;
    private float _hp;
    public float timeSinceFire;
    public EnemyType type;
    public Player player;
    public ItemManager itemManager;
    public EnemyManager enemyManager;

    private void Update()
    {
        transform.position -= new Vector3(0, stat.moveSpeed, 0) * Time.deltaTime;
    }

    public void Setup()
    {
        _hp = stat.maxHp;
    }

    public void OnHit(int damage)
    {
        _hp -= damage;
        if (_hp <= 0)
        {
            if (type == EnemyType.Npc1)
            {
                var random = Random.Range(1f, 100f);
                if (random >= 50) itemManager.RandomItem(transform.position);
            }
            if (type == EnemyType.Npc2)
            {
                player.OnDamagedAlt(stat.enemyAtk);
            }
            else
            {
                player.AddScore(stat.maxHp * 100);
            }

            enemyManager.spawnedEnemies.Remove(this);
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            if (type == EnemyType.Npc1)
            {
                var random = Random.Range(1f, 100f);
                if (random >= 50) itemManager.RandomItem(transform.position);
                Destroy(gameObject);
            }
            else if (type == EnemyType.Npc2)
            {
                player.OnDamagedAlt(stat.enemyAtk);
                Destroy(gameObject);
            }
            else
            {
                player.OnDamaged(stat.enemyAtk / 2);
            }
        }
        else if (col.CompareTag("EnemyBorder"))
        {
            if (type == EnemyType.Npc1 || type == EnemyType.Npc2)
            {
                Destroy(gameObject);
            }
            else
            {
                player.OnDamagedAlt(stat.enemyAtk / 2);
                enemyManager.spawnedEnemies.Remove(this);
                Destroy(gameObject);
            }
        }
    }
}