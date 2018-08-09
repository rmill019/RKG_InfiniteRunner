using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacle : MonoBehaviour {

    [Tooltip("Should this obstacle move vertically or horizontally")]
    public bool b_moveVertically = true;

    [Range(0.5f, 5f)]
    public float m_distance = 0.5f;

    [Range(1, 10)]
    public float m_speed = 1f;

    private Vector3 m_targetPos1; // Above/Right of original Position
    private Vector3 m_targetPos2; // Below/Left of original Position
    private bool b_movingToTarget1 = true;

	// Use this for initialization
	void Start () {
        // Setting the target positions based on b_moveVertically
        // Moving up and down
        if (b_moveVertically)
        {
            m_targetPos1 = transform.position + new Vector3(0, m_distance, 0);
            m_targetPos2 = transform.position - new Vector3(0, m_distance, 0);
        }
        // Moving left and right
        else
        {
            m_targetPos1 = transform.position + new Vector3(m_distance, 0, 0);
            m_targetPos2 = transform.position - new Vector3(m_distance, 0, 0);
        }
	}
	
	// Update is called once per frame
	void Update () {

        if (b_moveVertically)
            VerticalMovment();
        else
            HorizontalMovement();
	}

    void VerticalMovment ()
    {
        // If we are moving up
        if (b_movingToTarget1)
        {
            if (transform.position.y < m_targetPos1.y)
                transform.Translate(Vector2.up * m_speed * Time.deltaTime);
            else
                b_movingToTarget1 = false;
        }
        // We are moving down
        else
        {
            if (transform.position.y > m_targetPos2.y)
                transform.Translate(Vector2.down * m_speed * Time.deltaTime);
            else
                b_movingToTarget1 = true;
        }
    }

    void HorizontalMovement ()
    {
        // If we are moving right
        if (b_movingToTarget1)
        {
            if (transform.position.x < m_targetPos1.x)
                transform.Translate(Vector2.right * m_speed * Time.deltaTime);
            else
                b_movingToTarget1 = false;
        }
        // Else we are moving left
        else
        {
            if (transform.position.x > m_targetPos2.x)
                transform.Translate(Vector2.left * m_speed * Time.deltaTime);
            else
                b_movingToTarget1 = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        
    }
}
