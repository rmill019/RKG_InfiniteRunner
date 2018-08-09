using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This acts as Spawner and Object Pooler
public class ObstacleSpawner : MonoBehaviour {

    public static ObstacleSpawner S;

    [Tooltip("How many obstacles should we create in our pool")]
    public int m_amountToPool = 5;

    public GameObject m_pipePrefab;
    public GameObject m_blockPrefab;
    public float m_timeBetweenSpawns = 5f;

    private List<GameObject> m_pooledBlockObstacles;
    private List<GameObject> m_pooledPipeObstacles;
    private float m_nextSpawnTime;

    void Awake()
    {
        if (S == null)
            S = this;
        else
            Destroy(this.gameObject);
    }

    // Use this for initialization
    void Start () {
        m_pooledPipeObstacles = new List<GameObject>();
        m_pooledBlockObstacles = new List<GameObject>();

		// Create the list of pooled pipe obstacles
        for (int i = 0; i < m_amountToPool; i++)
        {
            GameObject pipe = (GameObject)Instantiate(m_pipePrefab);
            GameObject block = (GameObject)Instantiate(m_blockPrefab);
            
            pipe.SetActive(false);
            block.SetActive(false);

            m_pooledPipeObstacles.Add(pipe);
            m_pooledBlockObstacles.Add(block);
        }

        // We cannot spawn until the initial countdown has finished. So the initial spawnTime needs to take this into account
        m_nextSpawnTime = Time.time + GameManager.S.m_countdownLength + m_timeBetweenSpawns;
	}
	
	// Update is called once per frame
	void Update () {
        if (GameManager.S.IsGameActive && Time.time >= m_nextSpawnTime)
        {
            // spawn / activate prefab
            SpawnObstacle();

            // Reset the next spawn time
            m_nextSpawnTime = Time.time + m_timeBetweenSpawns;
        }
	}



    // We are using object Pooling so we are actually activating rather than "spawning"
    void SpawnObstacle ()
    {
        GameObject objectToSpawn;
        float random = Random.Range(0f, 1f);

        if (random < 0.5f)
            objectToSpawn = GetPooledBlockObstacle();
        else
            objectToSpawn = GetPooledPipeObstacle();

        ////For now only test returning pipes
        //objectToSpawn = GetPooledBlockObstacle();

        // Position the object
        if (objectToSpawn != null)
        {
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = objectToSpawn.GetComponent<Obstacle>().SpawnLocation;
        }
            
    }

    // Might be able to refactor this to include all possible obstacles
    public GameObject GetPooledPipeObstacle ()
    {
        for (int i = 0; i < m_pooledPipeObstacles.Count; i++)
        {
            // If the object in the list is not active in the hierarchy then it is available. So we return it
            if (!m_pooledPipeObstacles[i].activeInHierarchy)
                return m_pooledPipeObstacles[i];
        }

        // Return null if there were no available objects in our pool to use
        return null;
    }

    public GameObject GetPooledBlockObstacle ()
    {
        for (int i = 0; i < m_pooledBlockObstacles.Count; i++)
        {
            // If the object in the list is not active in the hierarchy then it is available. So we return it
            if (!m_pooledBlockObstacles[i].activeInHierarchy)
                return m_pooledBlockObstacles[i];
        }

        // Return null if there were no available objects in our pool to use
        return null;
    }

    

    
}
