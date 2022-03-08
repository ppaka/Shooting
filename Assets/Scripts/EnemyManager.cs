using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    public Enemy[] enemies;
    public static IObjectPool<Enemy>[] EnemyPools = new IObjectPool<Enemy>[3];
    public Transform[] spawns;
    public Player player;
    
    private void Awake()
    {
        EnemyPools[0] = new ObjectPool<Enemy>(() => Instantiate(enemies[0]),
            enemy => { enemy.gameObject.SetActive(true);
                enemy.Setup();
            }, enemy => { enemy.gameObject.SetActive(false); },
            enemy => { Destroy(enemy.gameObject); }, false, 20, 10000);
    }

    private void Start()
    {
        Spawn(0, new Vector3(Random.Range(spawns[0].position.x, spawns[1].position.x), 7));
        Spawn(0, new Vector3(Random.Range(spawns[0].position.x, spawns[1].position.x), 7));
        Spawn(0, new Vector3(Random.Range(spawns[0].position.x, spawns[1].position.x), 7));
        Spawn(0, new Vector3(Random.Range(spawns[0].position.x, spawns[1].position.x), 7));
    }

    private void Spawn(int type, Vector3 position)
    {
        var i = EnemyPools[type].Get();
        i.transform.position = position;
        i.player = player;
    }
}