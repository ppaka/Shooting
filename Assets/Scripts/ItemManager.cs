using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public Item[] itemPrefabs;

    public void RandomItem(Vector3 pos)
    {
        Instantiate(itemPrefabs[Random.Range(0, itemPrefabs.Length)], pos, Quaternion.identity);
    }
}