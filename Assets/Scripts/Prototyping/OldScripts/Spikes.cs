using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour {

    public bool b_triggerOnLeft = true;
    public AudioSource m_audioSource;

    [Range(2, 25)]
    public float m_triggerDistance;

    private Vector2 m_gizmoSize = new Vector2 (1.5f, 2f);
    private float m_spikeTriggerLocationX;
    private float m_yTargetPos;
    private bool b_canBeActivated = true;

	// Use this for initialization
	void Start () {
        m_audioSource = GetComponent<AudioSource>();

        // Position the trigger X position to activate the spike.
        if (b_triggerOnLeft)
            m_spikeTriggerLocationX = transform.position.x - m_triggerDistance;
        else
            m_spikeTriggerLocationX = transform.position.x + m_triggerDistance;

        m_yTargetPos = CalculateTargetYPosition();
        print("Target Y: " + m_yTargetPos);
	}

    // Update is called once per frame
    void Update()
    {
        if (GameManager.S.m_player.transform.position.x >= m_spikeTriggerLocationX && b_canBeActivated)
        {
            b_canBeActivated = false;
            AudioManager.S.PlayClip(AudioManager.S.spikeClip, 10f);
           transform.position = new Vector2(transform.position.x, m_yTargetPos);
           GetComponent<BoxCollider2D>().isTrigger = false;
        }
    }

    float CalculateTargetYPosition ()
    {
        float yOffset = GetComponent<BoxCollider2D>().bounds.extents.y * 2;
        Vector3 targetPos = transform.position + new Vector3(0, yOffset, 0);

        return targetPos.y;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 gizmoDrawPos = transform.position;

        if (b_triggerOnLeft)
            gizmoDrawPos -= new Vector3(m_triggerDistance, 0, 0);
        else
            gizmoDrawPos += new Vector3(m_triggerDistance, 0, 0);

        Gizmos.DrawCube(gizmoDrawPos, new Vector3(m_gizmoSize.x, m_gizmoSize.y, 0));
        Gizmos.DrawIcon(gizmoDrawPos, "SpikeGizmo.png", true);
    }
}
