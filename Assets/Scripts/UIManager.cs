using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static UIManager S;
    [Header("UI Container references")]
    public GameObject GameActivePanel;
    public GameObject GameOverPanel;

    [Header("Game Canvas UI References")]
    public Text m_scoreText;
    public Text m_timerText;
    public Text m_countdownText;

    private float m_timer;

    private void Awake()
    {
        if (S == null)
            S = this;
        else
            Destroy(this.gameObject);
    }

    // Use this for initialization
    void Start () {
        // Set up the proper canvas objects for Game Start
        // Does this work on Level reload?
        GameOverPanel.SetActive(false);
        GameActivePanel.SetActive(true);

        m_scoreText.text = "Score: ";
        m_timerText.text = "Timer: ";
        m_countdownText.text = GameManager.S.m_countdownLength.ToString("F0");
        m_timer = 0.0f;
	}

    private void Update()
    {
        HandleLevelCountdown();

        if (GameManager.S.IsPlayerAlive && GameManager.S.IsGameActive)
            ManageTimer();
        
        if (!GameManager.S.IsPlayerAlive)
            Invoke("EnableGameOverUI", 2f);
        
    }

    public void UpdateScoreText ()
    {
        m_scoreText.text = "Score: " + GameManager.S.CurrentScore.ToString();
    }

    private void HandleLevelCountdown ()
    {
        // While the game is not active then keep counting down
        if (!GameManager.S.IsGameActive)
        {
            GameManager.S.m_countdownLength -= Time.deltaTime;
            if (GameManager.S.m_countdownLength < .5f)
                m_countdownText.text = "GO!";
            else
                m_countdownText.text = GameManager.S.m_countdownLength.ToString("F0");
        }
        else
            m_countdownText.gameObject.SetActive(false);
    }

    private void ManageTimer ()
    {
        // Update this information while the player is alive and once the game has begun
            m_timer += Time.deltaTime;
            m_timerText.text = "Timer: " + m_timer.ToString("F2");
    }

    public void EnableGameOverUI ()
    {
        GameActivePanel.SetActive(false);
        GameOverPanel.SetActive(true);
    }
}
