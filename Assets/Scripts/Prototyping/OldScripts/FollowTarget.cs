using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour {

    public Transform m_target;
    public float m_zDistance;
    public float m_deathYMovement;
    public float m_deathXMovement;
    public float m_targetOrthographicSize;


    private Vector3 m_pos;
    private Vector3 m_startPos;

	// Use this for initialization
	void Start () {
        GetComponent<Camera>().orthographicSize = m_targetOrthographicSize;
        m_startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (GameManager.S.IsPlayerAlive)
        {
            m_pos = m_target.position;
            m_pos.x += 10f;
            m_pos.y = m_startPos.y;
            m_pos.z = m_zDistance;
        }
    }

    void LateUpdate()
    {
        if (GameManager.S.IsPlayerAlive)
            transform.position = m_pos;
        else
            GameOverCamera();
    }

    void GameOverCamera ()
    {
        Vector3 pos = m_pos;
        pos.y -= 1f;
        pos.y += Mathf.Sin(Time.time * m_deathYMovement);
        pos.x += Mathf.Cos(Time.time * m_deathXMovement);
        pos.z = m_zDistance;

        transform.position = pos;
    }
}
