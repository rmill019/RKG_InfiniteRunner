using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObstacleType { Grounded, InAir }

public class Obstacle : MonoBehaviour {

    public static float SPEED_X = 20f;

    [SerializeField]
    private ObstacleType m_obstacleType;

    [SerializeField]
    [Range(0.5f, 5f)]
    private float m_lifeSpan = 0.5f;

    // These range variables will be removed once we find the appropriate range for spawning items
    [Range(10f, 20f)]
    [SerializeField]
    private float m_xMinDistance;

    [Range(21f, 40f)]
    [SerializeField]
    private float m_xMaxDistance;


    [SerializeField]
    private float m_yMinDistance = 0.1f;

    [SerializeField]
    private float m_yMaxDistance = .15f;

    private float m_timeToDelete;

    private Vector3 m_spawnLocation = Vector3.zero;

    private void OnEnable()
    {
        SetSpawnLocation (m_obstacleType);
        m_timeToDelete = Time.time + m_lifeSpan;
    }

    // Update is called once per frame
    void Update () {
        
        if (gameObject.activeInHierarchy)
        {
            transform.Translate(-transform.right * SPEED_X * Time.deltaTime);
        }

        // Once we have reached or passed our lifespan then deactivate this obstacle and send it back
        // to the Obstacle Pool.
        if (Time.time >= m_timeToDelete)
            gameObject.SetActive(false);

	}

    void SetSpawnLocation (ObstacleType type)
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
                float rand = Choose(m_yMinDistance, m_yMaxDistance);
                // We add rand to groundPosition.y because if we try to assign while the player is jumping our positioning could be off
                yPos = GameManager.S.m_ground.position.y + rand;    
                break;
            default:
                break;
        }

        m_spawnLocation = new Vector3(xPos, yPos, 0);
    }

    // Randomly choose one of the given values
    T Choose<T> (T x, T y)
    {
        float random = Random.Range(0f, 1f);

        if (random < 0.5f)
        {
            return x;
        }
        else
        {
            return y;
        }
    }

    // Properties
    public Vector3 SpawnLocation
    {
        get { return m_spawnLocation; }
    }
}
