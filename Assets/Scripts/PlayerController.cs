using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState { Idle, Running, Jumping, Falling, Dead }

public class PlayerController : MonoBehaviour {

    public float m_jumpHeight = 10f;
    public float m_jumpSpeed = 10f;
    public float m_fallSpeed = 10f;
    public float threshold = 10f;   // delta threshold for Screen Input to be considered

    // Mobile Control variables
    private Vector2 startPos;
    private Vector2 endPos;
    private Vector2 direction;
    private bool b_directionChosen = false;
    // b_canPerformAction is needed as an intermediary value to allow input for actions. Once
    // the action is performed we set this to false to prevent further input.
    private bool b_canPerformAction = false;

    private Animator m_anim;
    private PlayerState m_playerState = PlayerState.Idle;
    private Vector3 m_startPos;

	// Use this for initialization
	void Start () {
        m_startPos = transform.position;
        m_anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (GameManager.S.IsGameActive)
            m_anim.SetBool("IsGameActive", true);

        if (m_anim.GetBool ("IsGameActive"))
        {
            SetPlayerState();
            PerformAction (m_playerState);
        }
	}


    /***************************
     *   Custom Functions      *
     **************************/

    void SetPlayerState ()
    {
        // Check if we are on the "ground"
        if (transform.position.y <= m_startPos.y)
        {
            transform.position = new Vector3(transform.position.x, m_startPos.y, transform.position.z);    //REFACTOR THIS LINE
            m_playerState = PlayerState.Running;
        }

        // Changing our Jump State for PC and Mobile
        #if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            if (m_playerState == PlayerState.Running && Input.GetButtonDown("Jump"))
                m_playerState = PlayerState.Jumping;
        #endif

        #if UNITY_ANDROID || UNITY_IOS
            if (m_playerState == PlayerState.Running) { CalculateTouchDelta(); }

            if (m_playerState == PlayerState.Running && direction.y > threshold && b_canPerformAction)
            {
                m_playerState = PlayerState.Jumping;
                b_canPerformAction = false;
            }
        #endif

        // If the player has reached or passed the jumpHeight then he is in the falling state
        if (transform.position.y >= m_startPos.y + m_jumpHeight)
            m_playerState = PlayerState.Falling;
    }

    // Performs certain actions based on the players current state
    void PerformAction (PlayerState currentState)
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Running:
                m_anim.SetBool("IsGameActive", true);
                break;
            case PlayerState.Jumping:
                transform.Translate(transform.up * m_jumpSpeed * Time.deltaTime);
                break;
            case PlayerState.Falling:
                transform.Translate(-transform.up * m_fallSpeed * Time.deltaTime);
                break;
            case PlayerState.Dead:
                break;
            default:
                break;
        }
    }

    void CalculateTouchDelta()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Handle finger based on the phase
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startPos = touch.position;
                    b_directionChosen = false;
                    break;
                case TouchPhase.Moved:
                    direction = touch.position - startPos;
                    break;
                case TouchPhase.Ended:
                    print("Ended");
                    endPos = touch.position;
                    b_directionChosen = true;
                    break;
                default:
                    break;
            }
        }

        if (b_directionChosen)
        {
            direction = endPos - startPos;
            b_canPerformAction = true;
            b_directionChosen = false;
        }
    }

}
