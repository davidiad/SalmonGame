using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float timeBetweenSpawns = 3f;
    public int startingNumNPF;
    public NPFish[] fishPrefabs;
    public int maxNum;
    Transform NPFparent;

    float timeSinceLastSpawn;

    // initial spawning (does not control eventual max number)
    void Start() 
    {
        for (int i = 0; i < startingNumNPF; i++) {
            SpawnFish();
        }
        GameObject oneFish = GameObject.FindGameObjectWithTag("NPF");
        NPFparent = oneFish.transform.parent;
    }

    void FixedUpdate()
    {
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= timeBetweenSpawns)
        {
            timeSinceLastSpawn -= timeBetweenSpawns;
            if (NPFparent.childCount < maxNum) {
                SpawnFish();
            }
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