using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager S;
    public ObjectSpawner m_objSpawner;
    //public CoinSpawner m_coinSpawner;

    public float m_countdownLength = 3f;
    public float m_backgroundSpeed = 0.2f;
    public float m_timeBetweenSpawns = 2f;
    public GameObject m_player;
    public Transform m_ground;

    private int m_currentScore;
    private bool b_isPlayerAlive = true;
    private bool b_isGameActive = false;
    private bool b_CanSpawnObstacles = false; //TODO Do we need this after moving to a time based system
    private bool firstTimeSpawn = true;
    private float m_nextObstacleSpawnTime;
    private float m_nextCoinFormationSpawnTime;

    private void Awake()
    {
        if (S == null)
            S = this;
        else
            Destroy(this.gameObject);

        // Validate Countdown Length
        if (m_countdownLength < 3f)
            m_countdownLength = 3f;
    }

    private void Start()
    {
        m_currentScore = 0;

        if (m_player == null)
            m_player = GameObject.FindGameObjectWithTag("Player");

        // We cannot spawn until the initial countdown has finished. So the initial spawnTime needs to take this into account
        m_nextObstacleSpawnTime = Time.time + GameManager.S.m_countdownLength + m_timeBetweenSpawns;

        // Set Coin spawn time to a ridiculously high number to ensure that all obstacles are spawned first;
        m_nextCoinFormationSpawnTime = Time.time + 1000f;
    }

    private void Update()
    {
        // Keep track to see if the countdown is over
        if (m_countdownLength <= 0f && b_isPlayerAlive)
        {
            b_isGameActive = true;
            m_player.GetComponent<Animator>().SetBool("GameStarted", true);
            //if (firstTimeSpawn)
            //{
            //    StartCoroutine(ObjectSpawner.S.SpawnCoins());
            //    firstTimeSpawn = false;
            //}
        }

        if (Time.time >= m_nextObstacleSpawnTime)
        {
            print("Starting Obstacle Coroutine");
            StartCoroutine(ObjectSpawner.S.SpawnObstacles());
        }

        if (Time.time >= m_nextCoinFormationSpawnTime)
        {
            print("Starting Coin Coroutine");
            ObjectSpawner.S.SpawnCoins();
        }
    }

    public void UpdateScore (int amount)
    {
        m_currentScore += amount;
        // Update the UI representation of the Score
        UIManager.S.UpdateScoreText();
    }

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

    public bool CanSpawnObstacles
    {
        get { return b_CanSpawnObstacles; }
        set { b_CanSpawnObstacles = value; }
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
}
