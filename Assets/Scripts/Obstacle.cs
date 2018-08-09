using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObstacleType { Grounded, InAir }

public class Obstacle : MonoBehaviour {

    [SerializeField]
    private ObstacleType m_obstacleType;

    [SerializeField]
    [Range(0.5f, 2f)]
    private float m_lifeSpan = 0.5f;

    [SerializeField]
    [Range(1, 10)]
    private float m_speedX;

    // These range variables will be removed once we find the appropriate range for spawning items
    [Range(10f, 20f)]
    [SerializeField]
    private float m_xMinDistance;

    [Range(21f, 40f)]
    [SerializeField]
    private float m_xMaxDistance;

    [Range(10f, 20f)]
    [SerializeField]
    private float m_yMinDistance;

    [Range(21f, 40f)]
    [SerializeField]
    private float m_yMaxDistance;

    private float m_timeToDelete;

    private Vector3 m_spawnLocation;

    // Use this for initialization
    void Start () {
        //SetObstacleLocation(m_obstacleType);
        //m_lifeSpan += Time.time;
    }

    private void OnEnable()
    {
        SetObstacleLocation (m_obstacleType);
        m_timeToDelete = Time.time + m_lifeSpan;
    }

    // Update is called once per frame
    void Update () {

        // Once we have reached or passed our lifespan then deactivate this obstacle and send it back
        // to the Obstacle Pool.
        if (Time.time >= m_timeToDelete)
            gameObject.SetActive(false);
		
	}

    void SetObstacleLocation (ObstacleType type)
    {
        float xPos = 0;
        float yPos = 0;

        switch (type)
        {
            case ObstacleType.Grounded:
                xPos =  GameManager.S.m_player.transform.position.x + Random.Range(m_xMinDistance, m_xMaxDistance);
                yPos = GameManager.S.m_ground.position.y + GetComponent<BoxCollider2D>().bounds.extents.y;
                break;
            case ObstacleType.InAir:
                xPos = GameManager.S.m_player.transform.position.x + Random.Range(m_xMinDistance, m_xMaxDistance);
                yPos = GameManager.S.m_player.transform.position.y + Random.Range(m_yMinDistance, m_yMaxDistance);
                break;
            default:
                break;
        }

        m_spawnLocation = new Vector3(xPos, yPos, 0);
    }

    // Properties
    public Vector3 SpawnLocation
    {
        get { return m_spawnLocation; }
    }
}
