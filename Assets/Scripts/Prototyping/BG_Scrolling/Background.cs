using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour {

    float edgeOfColl = 0f;
    Vector3 startPos;
    public float threshold = 5f;

	// Use this for initialization
	void Start () {
        startPos = transform.position;
        edgeOfColl = transform.GetChild(0).position.x + GetComponent<MeshCollider>().bounds.extents.x;
        print("Edge: " + edgeOfColl);
	}
	
	// Update is called once per frame
	void Update () {
        if (GameManager.S.m_player.transform.position.x >= edgeOfColl - threshold)
        {
            print("Reset!!");
            transform.position = new Vector3(transform.GetChild(0).position.x - threshold, startPos.y, startPos.z);
            edgeOfColl = transform.GetChild(0).position.x + GetComponent<MeshCollider>().bounds.extents.x;
        }
	}

    private void OnDrawGizmos()
    {
        // Draw a cube where the threshold for the background will be;
    }
}
