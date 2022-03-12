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
    public EnemyType type;
    public Player player;
    public ItemManager itemManager;

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
                if (random >= 70) itemManager.RandomItem(transform.position);
            }
            if (type == EnemyType.Npc2)
            {
                var random = Random.Range(85f, 100f)* 0.01f;
                player.OnDamagedAlt(stat.enemyAtk * random);
            }
            EnemyManager.EnemyPools[(int)type].Release(this);
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            if (type == EnemyType.Npc1)
            {
                itemManager.RandomItem(transform.position);
                EnemyManager.EnemyPools[(int)type].Release(this);
            }
            else if (type == EnemyType.Npc2)
            {
                player.OnDamagedAlt(stat.enemyAtk / 2);
            }
            else
            {
                player.OnDamaged(stat.enemyAtk / 2);
            }
        }
        else if (col.CompareTag("EnemyBorder"))
        {
            if (type != EnemyType.Npc2)
            {
                player.OnDamagedAlt(stat.enemyAtk / 2);
                EnemyManager.EnemyPools[(int)type].Release(this);
            }
        }
    }
}