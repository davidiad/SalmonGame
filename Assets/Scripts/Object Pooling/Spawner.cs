using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float timeBetweenSpawns = 3f;
    public int numNPF = 20;
    public NPFish[] fishPrefabs;


    float timeSinceLastSpawn;

    void Start() 
    {
        for (int i = 0; i < numNPF; i++) {
            SpawnFish();
        }

    }

    void FixedUpdate()
    {
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= timeBetweenSpawns)
        {
            timeSinceLastSpawn -= timeBetweenSpawns;
            SpawnFish();
        }
    }

    void SpawnFish()
    {
        NPFish prefab = fishPrefabs[Random.Range(0, fishPrefabs.Length)];
        NPFish spawn = prefab.GetPooledInstance<NPFish>();

        //NPFish spawn = Instantiate<NPFish>(prefab);
        //spawn.transform.localPosition = transform.position;
    }
}