using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyStat stat;
    private float _hp;
    public int type;
    public Player player;

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
            EnemyManager.EnemyPools[type].Release(this);
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            player.OnDamaged(stat.enemyAtk / 2);
        }
        else if (col.CompareTag("EnemyBorder"))
        {
            player.OnDamagedAlt(stat.enemyAtk / 2);
            EnemyManager.EnemyPools[type].Release(this);
        }
    }
}