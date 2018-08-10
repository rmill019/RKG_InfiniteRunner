using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour {

    public int m_value;

    void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.gameObject.tag == "Player")
        {
            AudioManager.S.PlayClip(AudioManager.S.coinCollectedClip);
            GameManager.S.UpdateScore(m_value);
            gameObject.SetActive(false);
        }
    }

}
