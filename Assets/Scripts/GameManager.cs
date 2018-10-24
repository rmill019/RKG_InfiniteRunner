using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager S;
    public ObjectSpawner m_objSpawner;

    public float m_countdownLength = 3f;
    public float m_backgroundSpeed = 0.2f;
    public float m_timeBetweenSpawns = 2f;
    [Tooltip("How often should the difficulty increase")]
    public float m_difficultyInterval = 5f;
    public float m_minSpawnInterval = 0.75f;
    public GameObject m_player;
    public Transform m_ground;

    [Header("These setting affect the difficuly scaling")]
    public float m_backgroundSpeedInc = 0.1f;
    public float m_spawnIntervalDec = 0.025f;
    public float m_jumpAccInc = 0.25f;
    public float m_playerGravityInc = 0.25f;
    public float m_obstacleSpeedInc = 0.25f;
    public float m_animSpeedInc = 0.15f;

    private int m_currentScore;
    private bool b_isPlayerAlive = true;
    private bool b_isGameActive = false;
    private bool firstTimeSpawn = true;
    private float m_nextObstacleSpawnTime;
    private float m_nextCoinFormationSpawnTime;
    private float m_increaseDifficultyTime = 0;

    // High Score Tracking variables
    private List<int> m_highScores;
    private HighScoreTracker m_highScoreTracker;
    private PlayerController m_PlayerController;

    private void Awake()
    {
        if (S == null)
            S = this;
        else
            Destroy(this.gameObject);

        m_highScores = new List<int>();
        m_highScoreTracker = new HighScoreTracker
        {
            CanSave = true
        };

        // Validate Countdown Length
        if (m_countdownLength < 3f)
            m_countdownLength = 3f;
    }

    private void Start()
    {
        // Reset our original Obstacles speed once we start the level again
        Obstacle.SPEED_X = Obstacle.START_SPEED_X;

        m_currentScore = 0;
        // Initialize m_highScores to 0 if no file exists on Disk
        PlayerHighScores scores = m_highScoreTracker.LoadScores();
        if (scores == null)
        {
            for (int i = 0; i < 10; i++)
            {
                m_highScores.Add(0);
                //m_highScores[i] = 0;
            }
        }
        else
        {
            m_highScores = scores.HighScores;
        }

        if (m_player == null)
            m_player = GameObject.FindGameObjectWithTag("Player");

        m_PlayerController = m_player.GetComponent<PlayerController>();

        // We cannot spawn until the initial countdown has finished. So the initial spawnTime needs to take this into account
        m_nextObstacleSpawnTime = Time.time + GameManager.S.m_countdownLength + m_timeBetweenSpawns;
        //print("Setting Spawn time to: " + m_nextObstacleSpawnTime);

        // Set Coin spawn time to a ridiculously high number to ensure that all obstacles are spawned first;
        m_nextCoinFormationSpawnTime = Time.time + 1000f;

        // Set when we should increase difficulty
        m_increaseDifficultyTime = Time.time + m_difficultyInterval;
    }

    private void Update()
    {
        // Keep track to see if the countdown is over
        if (m_countdownLength <= 0f && b_isPlayerAlive)
        {
            b_isGameActive = true;
            m_player.GetComponent<Animator>().SetBool("GameStarted", true);
        }

        DetermineSpawning();
        CheckForDifficultyIncrease();

        // if the Player is not alive and our score tracker has not saved data yet
        if (!IsPlayerAlive && m_highScoreTracker.CanSave)
        {
            m_highScoreTracker.CanSave = false;
            // TODO need to make sure this fires only Once per death
            UpdateHighScoreList();
            m_highScoreTracker.SaveScores(m_highScores);
        }
    }

    public void UpdateHighScoreList ()
    {
        // Add our score to the list, sort and reverse to list in descending order
        m_highScores.Add(CurrentScore);
        m_highScores.Sort();
        m_highScores.Reverse();

        // prune scores past 10th entry
        if (m_highScores.Count > 9)
        {
            for (int i = 10; i < m_highScores.Count; i++)
            {
                m_highScores.RemoveAt(i);
            }
        }
    }

    // This is called every set amount of time to adjust parameters that will affect difficulty
    public void CheckForDifficultyIncrease ()
    {
        // Make sure our m_PlayerController is not null so we can modify values in it
        if (Time.time > m_increaseDifficultyTime && m_PlayerController != null)
        {
            //print("INCREASE DIFFICULTY");
            m_backgroundSpeed += m_backgroundSpeedInc;
            // Only decrease our time between spawns if we have not yet reached our minimum time threshold for spawn interval
            if (m_timeBetweenSpawns > m_minSpawnInterval)
                m_timeBetweenSpawns -= m_spawnIntervalDec;

            m_PlayerController.m_fallSpeed += m_playerGravityInc;
            m_PlayerController.m_jumpSpeed += m_jumpAccInc;
            m_PlayerController.IncreaseAnimationSpeed(m_animSpeedInc);
            Obstacle.SPEED_X += m_obstacleSpeedInc;
            // Reset the time to increase difficulty
            m_increaseDifficultyTime = Time.time + m_difficultyInterval;
        }
        else
            Debug.LogWarning("No PlayerController assigned to m_PlayerController");
    }

    // Checks to see if it is time to start spawning coins or obstacles
    public void DetermineSpawning ()
    {
        if (Time.time >= m_nextObstacleSpawnTime)
        {
            //print("Next Obstacle Spawn Time: " + m_nextObstacleSpawnTime);
            //print("Starting Obstacle Coroutine");
            StartCoroutine(ObjectSpawner.S.SpawnObstacles());
        }

        if (Time.time >= m_nextCoinFormationSpawnTime)
        {
            //print("Starting CoinSpawn");
            ObjectSpawner.S.SpawnCoins();
        }
    }

    public void UpdateScore (int amount)
    {
        m_currentScore += amount;
        // Update the UI representation of the Score
        UIManager.S.UpdateScoreText();
    }

    #region Properties

    public int CurrentScore
    {
        get { return m_currentScore; }
        set { m_currentScore = value; }
    }

    public bool IsPlayerAlive
    {
        get { return b_isPlayerAlive; }
        set { b_isPlayerAlive = value; }
    }

    public bool IsGameActive
    {
        get { return b_isGameActive; }
        set { b_isGameActive = value; }
    }

    public float ObstacleSpawnTime
    {
        get { return m_nextObstacleSpawnTime; }
        set { m_nextObstacleSpawnTime = value; }
    }

    public float CoinSpawnTime
    {
        get { return m_nextCoinFormationSpawnTime; }
        set { m_nextCoinFormationSpawnTime = value; }
    }
    #endregion
}
