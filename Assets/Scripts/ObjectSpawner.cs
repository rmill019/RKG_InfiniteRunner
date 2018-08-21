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

    [Tooltip("The columns spawned will be between 1 and this value")]
    [Range(2,20)]
    public int m_columnsMax;

    [Tooltip("The rows spawned will be between 1 and this value")]
    [Range(2, 20)]
    public int m_rowsMax;

    public int numRows;
    public int numCols;
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
                // Set the CanMove flag to true so that it will update it's position while active
                obstacleToSpawn.GetComponent<Obstacle>().CanMove = true;
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

        int numCols = Random.Range(1, m_columnsMax);
        int numRows = Random.Range(1, m_rowsMax);

        // Storing coins in a one-dimensional array to make processing it easier
        //GameObject[] coins = new GameObject[numRows * numCols];
        GameObject[] coins = new GameObject[numRows * numCols];
        //print("Coins Length: " + coins.Length);
        //print("Spawning " + numCols + " X " + numRows + " coins");

        Vector3 prevCoinPos = Vector3.zero;
        Vector3 nextCoinPos = Vector3.zero;
        Vector3 firstCoinPos = Vector3.zero;
        Vector3 targetPos = Vector3.zero;

        // Spawn the coins
        for (int i = 0; i < coins.Length; i++)
        {
            GameObject coin = GetPooledCoin();
            coin.SetActive(true);
            coins[i] = coin;

            // If we are at the beginning of a row
            if (i % numCols == 0 && i != 0)
            {
                targetPos = firstCoinPos;
                targetPos.y += m_coinSpacing * (i / numCols);
                coins[i].transform.position = targetPos;
                prevCoinPos = coins[i].transform.position;
                //print("Coin #: " + (i) + " Spawning modulus of 4 at: " + targetPos.ToString());
            }
            // else we are on the next column of the row
            else if (i != 0)
            {
                targetPos = prevCoinPos;
                targetPos.x += m_coinSpacing;
                coins[i].transform.position = targetPos;
                prevCoinPos = targetPos;
                //print("Coin # " + (i) + " location: " + targetPos.ToString());
            }

            // Leaving this as a seperate if statement to make sure it gets fired for i = 0
            if (i == 0)
            {
                //print("Spawning first coint at: " + coins[0].transform.position.ToString());
                firstCoinPos = coins[0].transform.position;
                prevCoinPos = firstCoinPos;
            }
           
        }
        // Once we finish all the spawning of coins set our next Obstacle spawn time
        GameManager.S.ObstacleSpawnTime = Time.time + GameManager.S.m_timeBetweenSpawns;

        // Tell each coin it can now move since they have all spawned
        for (int i = 0; i < coins.Length; i++)
        {
            coins[i].GetComponent<Obstacle>().CanMove = true;
        }
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
