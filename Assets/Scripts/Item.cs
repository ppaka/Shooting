using UnityEngine;

public enum ItemType
{
    Upgrade,
    Invincible,
    Heal,
    AltHeal
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
        }
    }
}