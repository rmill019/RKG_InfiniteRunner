using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour {

    public bool b_EndlessRunnerMode = true;
    public float maxSpeed = 10f;
    public float m_rollSpeed = 20f;
    public float m_jumpForce = 700f;
    public Transform m_groundCheck;
    public LayerMask m_whatIsGround;
    [Tooltip("Delta needed to trigger a jump or roll on swipe movment")]
    public float threshold = 10f;

    // Attached Components
    private Rigidbody2D m_rb2d;
    private Animator m_anim;

    // Mobile Control variables
    private Vector2 startPos;
    private Vector2 endPos;
    private Vector2 direction;
    private bool b_directionChosen = false;
    // b_canPerformAction is needed as an intermediary value to allow input for actions. Once
    // the action is performed we set this to false to prevent further input.
    private bool b_canPerformAction = false;    

    private bool b_grounded = false;
    private bool b_facingRight;
    private bool b_collidersModified = false;
    // How big is our groundcheck radius going to be to check if we are actually standing on the ground
    private float m_groundRadius = 0.35f;
    private float m_circleCollStartRadius;
    private float m_colliderTimer = 0f;
    private float m_speedX; // This is what is controlling our rigidbody velocity in the x-axis

    // Use this for initialization
    void Start () {
        b_facingRight = GetComponent<SpriteRenderer>().flipX = false;
        m_rb2d = GetComponent<Rigidbody2D>();
        m_anim = GetComponent<Animator>();
        m_circleCollStartRadius = GetComponent<CircleCollider2D>().radius;
        m_speedX = maxSpeed;
    }

    private void Update()
    {
        #if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            ConventionalControls();
        #endif

        #if UNITY_ANDROID || UNITY_IOS
            MobileControls();
        #endif

        // If our colliders have been modified and it is time to reset them then Reset the colliders
        if (Time.time >= m_colliderTimer && b_collidersModified)
            ResetColliders();
    }

    // Everything in FixedUpdate is set based off Input that is read in Update(). 
    void FixedUpdate () {
        if (GameManager.S.IsGameActive)
        {
            // Check if we are on the ground: OverlapCircle returns a boolean
            b_grounded = Physics2D.OverlapCircle(m_groundCheck.position, m_groundRadius, m_whatIsGround);
            m_anim.SetBool("Grounded", b_grounded);

            // Setting Vertical speed parameter of Animator to equal our Rigidbody's y velocity vector
            m_anim.SetFloat("vSpeed", m_rb2d.velocity.y);

            float move = Input.GetAxis("Horizontal");

            // This sets our Animators Speed parameter if we are on EndlessRunner mode then Speed is always 10, 
            //else it is based off of our move variable
            if (b_EndlessRunnerMode)
            {
                m_anim.SetFloat("Speed", m_speedX);
                // Set our Rigidbody's velocity
                m_rb2d.velocity = new Vector2(m_speedX, m_rb2d.velocity.y);
            }
            else
            {
                m_anim.SetFloat("Speed", Mathf.Abs(move));
                // Set our Rigidbody's velocity
                m_rb2d.velocity = new Vector2(move * m_speedX, m_rb2d.velocity.y);
            }

            // Logic to decide when we need to Flip our Sprite on the X-axis
            if (move > 0 && b_facingRight)
                Flip();
            else if (move < 0 && !b_facingRight)
                Flip();
        }
	}

    /*************************
     *   Custom Functions    *
     ************************/

    void ConventionalControls ()
    {
        // If we are on the ground and we press the Jump button then jump
        if (b_grounded && Input.GetButtonDown("Jump"))
            Jump();

        // If we are moving and grounded and press the Roll button then Roll
        if (b_grounded && m_anim.GetFloat("Speed") > 0.1 && Input.GetButtonDown("Roll"))
            Roll();
    }
    
    void MobileControls ()
    {
        // Only allow us to give input if we are on the ground
        if (b_grounded)
            CalculateTouchDelta();

        if (b_grounded && direction.y > threshold && b_canPerformAction)
        {
            Jump();
            b_canPerformAction = false;
        }

        if (b_grounded && m_anim.GetFloat("Speed") > 0.1f && direction.y < -threshold && b_canPerformAction)
        {
            Roll();
            b_canPerformAction = false;
        }
    }

    void Jump ()
    {
        m_anim.SetBool("Grounded", false);
        m_rb2d.AddForce(new Vector2(0, m_jumpForce));
    }

    void Roll ()
    {

        m_anim.SetTrigger("Rolling");
        b_collidersModified = true;
        GetComponent<CapsuleCollider2D>().isTrigger = true;
        m_speedX = m_rollSpeed;
        m_colliderTimer = Time.time + GetAnimatorStateLength("Roll");
    }

    void Flip()
    {
        b_facingRight = !b_facingRight;
        GetComponent<SpriteRenderer>().flipX = b_facingRight;
    }

    float GetAnimatorStateLength (string stateName)
    {
        if (m_anim != null)
        {
            RuntimeAnimatorController ac = m_anim.runtimeAnimatorController;
            for (int i = 0; i < ac.animationClips.Length; i++)
            {
                if (ac.animationClips[i].name == stateName)
                {
                   // print(stateName + " has a length of " + ac.animationClips[i].length);
                    return ac.animationClips[i].length;
                }
            }
        }
        // Desired Animator state not found, Logging as Warning
        Debug.LogWarning(stateName + " State not found in Animator component of " + gameObject.name);
        return -1f;
    }

    // May need better / clearer name
    void ResetColliders ()
    {
        b_collidersModified = false;
        GetComponent<CapsuleCollider2D>().isTrigger = false;
        m_speedX = maxSpeed;
    }

    public void Die()
    {
        AudioManager.S.PlayClip(AudioManager.S.deathClip);
        // Disable speed
        m_rb2d.velocity = Vector2.zero;
        m_speedX = 0f;
        // Set appropriate Animator Components
        m_anim.SetBool("IsDead", true);
        m_anim.SetFloat("Speed", 0);
        // Disable Collider
        GetComponent<CapsuleCollider2D>().isTrigger = true;

        // Tell the GameManager that we are dead
        GameManager.S.IsPlayerAlive = false;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            Die();
        }
    }


    // Properties
    public bool IsGrounded
    {
        get { return IsGrounded; }
    }
}
