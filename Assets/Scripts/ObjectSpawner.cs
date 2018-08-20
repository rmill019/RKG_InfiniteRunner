using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This acts as Spawner and Object Pooler for Obstacles. Coins have been delegated to a Coin Spawner
public class ObjectSpawner : MonoBehaviour {

    public static ObjectSpawner S;

    [Tooltip("How many obstacles should we create in our pool")]
    public int m_obstacleAmountToPool = 5;
    public int m_coinAmountToPool = 100;

    public GameObject m_pipePrefab;
    public GameObject m_blockPrefab;
    public GameObject m_coinPrefab;

    [Header("Information about coin formation spawning")]
    [Range(2,10)]
    public int m_columnAndRowsMin;
    [Range(10, 20)]
    public int m_columnAndRowsMax;
    public float m_coinSpacing = 1f;

    private List<GameObject> m_pooledBlockObstacles;
    private List<GameObject> m_pooledPipeObstacles;
    private List<GameObject> m_pooledCoins;

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
        m_pooledCoins = new List<GameObject>();

		// Create the list of pooled obstacles
        for (int i = 0; i < m_obstacleAmountToPool; i++)
        {
            GameObject pipe = (GameObject)Instantiate(m_pipePrefab);
            GameObject block = (GameObject)Instantiate(m_blockPrefab);
            
            pipe.SetActive(false);
            block.SetActive(false);

            m_pooledPipeObstacles.Add(pipe);
            m_pooledBlockObstacles.Add(block);
        }

        // Create a List of pooled coins
        for (int i = 0; i < m_coinAmountToPool; i++)
        {
            GameObject coin = (GameObject)Instantiate(m_coinPrefab);
            coin.SetActive(false);
            m_pooledCoins.Add(coin);
        }
	}
	
	// Update is called once per frame
	void Update () {

        //if (GameManager.S.IsGameActive && Time.time >= m_nextSpawnTime)
        //{
        //    // Pick Random # of Objects to spawn
        //    // spawn / activate prefab
        //    //SpawnObject();

        //    // Reset the next spawn time
        //    m_nextSpawnTime = Time.time + m_timeBetweenSpawns;
        //}
	}

    public IEnumerator SpawnObstacles ()
    {
        // Setting this at the start of the Coroutine so that it won't get called in Update: Hack!!
        GameManager.S.ObstacleSpawnTime = Time.time + 1000f;

        int amountOfObstacles = Random.Range(1, 10);
        //print("Spawning obstacles " + amountOfObstacles + " times");
        GameObject obstacleToSpawn;

        for (int i = 0; i < amountOfObstacles - 1; i++)
        {
            //print("Spawning object # " + (i + 1));
            float random = Random.Range(0, 1f);

            if (random > 0.5f)
                obstacleToSpawn = GetPooledBlockObstacle();
            else
                obstacleToSpawn = GetPooledPipeObstacle();

            if (obstacleToSpawn != null)
            {
                obstacleToSpawn.SetActive(true);
                obstacleToSpawn.transform.position = obstacleToSpawn.GetComponent<Obstacle>().SpawnLocation;
            }

            yield return new WaitForSeconds(GameManager.S.m_timeBetweenSpawns);
        }
        // Once we finish all the spawning of obstacles set our coin spawn time
        GameManager.S.CoinSpawnTime = Time.time + GameManager.S.m_timeBetweenSpawns;
        
    }

    public void SpawnCoins()
    {
        // Setting this at the start of the Coroutine so that it won't get called in Update: Hack!!
        GameManager.S.CoinSpawnTime = Time.time + 1000f;

        int numCols = Random.Range(m_columnAndRowsMin, m_columnAndRowsMax);
        int numRows = Random.Range(m_columnAndRowsMin, m_columnAndRowsMax);

        // Storing coins in a one-dimensional array to make processing it easier
        GameObject[] coins = new GameObject[numRows * numCols];
        //print("Spawning " + numCols + " X " + numRows + " coins");

        Vector3 prevCoinPos = Vector3.zero;
        Vector3 nextCoinPos = Vector3.zero;
        Vector3 firstCoinPos = Vector3.zero;
        Vector3 targetPos = Vector3.zero;

        for (int i = 0; i < coins.Length - 1; i++)
        {
            GameObject coin = GetPooledCoin();
            coin.SetActive(true);
            coins[i] = coin;

            // If we are at the beginning of a row
            if (i % numCols == 0)
            {
                targetPos = firstCoinPos;
                targetPos.y -= m_coinSpacing;
                coins[i].transform.position = targetPos;
                prevCoinPos = coins[i].transform.position;
            }
            // else we are on the next column of the row
            else
            {
                targetPos = prevCoinPos;
                targetPos.x += m_coinSpacing;
                coins[i].transform.position = targetPos;
                prevCoinPos = targetPos;
            }

            if (i == 0)
            {
                firstCoinPos = coins[i].transform.position;
                prevCoinPos = firstCoinPos;
            }
           
        }
        // Once we finish all the spawning of coins set our next Obstacle spawn time
        GameManager.S.ObstacleSpawnTime = Time.time + GameManager.S.m_timeBetweenSpawns;

        //// This whole spawn and positioning process needs to be re-worked
        //for (int r = 0; r < numRows; r++)
        //{
        //    for (int c = 0; c < numCols; c++)
        //    {
        //        GameObject coin = GetPooledCoin();
        //        coin.SetActive(true);

        //    }

        //    //GameManager.S.CanSpawnObstacles = true;
        //    yield return new WaitForSeconds(GameManager.S.m_timeBetweenSpawns);
        //}
        //// Once we finish all the spawning of coins set our next Obstacle spawn time
        //GameManager.S.ObstacleSpawnTime = Time.time + GameManager.S.m_timeBetweenSpawns;

    }


    // We are using object Pooling so we are actually activating rather than "spawning"
    void SpawnObject ()
    {
        GameObject objectToSpawn;
        float random = Random.Range(0f, 1f);

        // This can be Refactored to something clearer
        if (random > 0.5f)
            objectToSpawn = GetPooledBlockObstacle();
        else
            objectToSpawn = GetPooledPipeObstacle();
        //else
        //    objectToSpawn = GetPooledCoin();

        // Position the object
        if (objectToSpawn != null)
        {
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = objectToSpawn.GetComponent<Obstacle>().SpawnLocation;
        }
            
    }

    #region Grab From Object Pool Methods
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

    public GameObject GetPooledCoin()
    {
        for (int i = 0; i < m_pooledCoins.Count; i++)
        {
            // If the object in the list is not active in the hierarchy then it is available. So we return it
            if (!m_pooledCoins[i].activeInHierarchy)
                return m_pooledCoins[i];
        }

        // Return null if there were no available objects in our pool to use
        return null;
    }
    #endregion

}
