using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour {

    public float m_jumpForce;
    public float m_speed;
    public float m_maxSpeed;
    public float m_rayLength;
    public float m_descentMult;
    public Transform m_groundCheck;
    public float m_groundRadius;
    [Tooltip("Layers that count as being grounded")]
    public LayerMask m_groundLayer;

    private bool b_onGround = false;
    private Animator m_anim;
    private Rigidbody2D m_rigid;
    private BoxCollider2D m_boxColl;

	// Use this for initialization
	void Start () {
        
        m_boxColl = GetComponent<BoxCollider2D>();
        m_rigid = GetComponent<Rigidbody2D>();
        m_anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        print("OnGround: " + b_onGround);
	}

    private void FixedUpdate()
    {
        //b_onGround = CheckOnGround();
        b_onGround = Physics2D.OverlapCircle(m_groundCheck.position, m_groundRadius, m_groundLayer);


        //Jump();

        //// If we are on the ground then apply our speed to x and normal rigid body velocity
        //if (b_onGround)
        //    m_rigid.velocity = new Vector2(m_speed, m_rigid.velocity.y) * Time.deltaTime;
        //else
        //    m_rigid.velocity = new Vector2(m_rigid.velocity.x, m_rigid.velocity.y);
    }


    bool CheckOnGround ()
    {
        RaycastHit2D groundRayHit;
        Ray groundCheckRay = new Ray(m_groundCheck.position, -transform.up);
        Debug.DrawRay(groundCheckRay.origin, groundCheckRay.direction, Color.green);
        groundRayHit = Physics2D.Raycast(groundCheckRay.origin, groundCheckRay.direction, m_rayLength, m_groundLayer);
        if (groundRayHit.collider != null && 1 << groundRayHit.collider.gameObject.layer == m_groundLayer.value)
        {
            //print("Player landed on: " + groundRayHit.collider.gameObject.name);
            return true;
        }

        print("Not hitting ground");
        return false;
    }

    void Jump ()
    {
        if (Input.GetButton("Jump") && b_onGround)
            m_rigid.AddForce(new Vector2(m_rigid.velocity.x, m_jumpForce));
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        print("Collision");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("Trigger");
    }
}
