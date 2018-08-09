using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager S;
    public float m_countdownLength = 3f;
    public GameObject m_player;
    public Transform m_ground;

    private int m_currentScore;
    private bool b_isPlayerAlive = true;
    private bool b_isGameActive = false;

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
    }

    private void Update()
    {
        // Keep track to see if the countdown is over
        if (m_countdownLength <= 0f)
        {
            b_isGameActive = true;
            m_player.GetComponent<Animator>().SetBool("GameStarted", true);
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
}
