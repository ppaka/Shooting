﻿using UnityEngine;

public enum ItemType
{
    Upgrade,
    Invincible,
    Heal,
    AltHeal,
    HealAll,
    Bomb
}

public class Item : MonoBehaviour
{
    public ItemType type;
    public float speed;

    private void Update()
    {
        transform.position += Vector3.down * speed * Time.deltaTime;
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            var player = col.GetComponent<Player>();
            if (type == ItemType.Upgrade)
            {
                player.UpgradeWeapon(player.weaponLevel+1);
            }
            else if (type == ItemType.Invincible)
            {
                player.timeSinceLastHit = 0;
                player.invincibleTime = 3;
                player.invincibleEffectTime = 2.5f;
            }
            else if (type == ItemType.Heal)
            {
                player.hp += player.maxHp * 0.25f;
                player.hp = Mathf.Clamp(player.hp, 0, player.maxHp);
            }
            else if (type == ItemType.AltHeal)
            {
                player.altHp += player.maxAltHp * 0.25f;
                player.altHp = Mathf.Clamp(player.altHp, 0, player.maxAltHp);
            }
            else if (type == ItemType.HealAll)
            {
                player.hp += player.maxHp * 0.25f;
                player.hp = Mathf.Clamp(player.hp, 0, player.maxHp);
                player.altHp += player.maxAltHp * 0.25f;
                player.altHp = Mathf.Clamp(player.altHp, 0, player.maxAltHp);
            }
            else if (type == ItemType.Bomb)
            {
                player.FireBomb();
            }
            player.AddScore(100);
            Destroy(gameObject);
        }
    }
}