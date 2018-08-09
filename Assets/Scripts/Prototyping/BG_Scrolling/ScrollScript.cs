using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollScript : MonoBehaviour {

    public bool b_shouldMove = true;
    private float speed = 0;

    private void Start()
    {
        speed = GameManager.S.m_backgroundSpeed;
    }

    // Update is called once per frame
    void Update () {

        speed = GameManager.S.m_backgroundSpeed;

        if (GameManager.S.IsGameActive)
        {
            if (b_shouldMove)
            {
                Vector3 pos = transform.position;
                pos.x = GameManager.S.m_player.transform.position.x + 10f;
                transform.position = pos;
            }

            GetComponent<Renderer>().material.mainTextureOffset = new Vector2((Time.time * speed) % 1, 0f);
        }
	}
}
