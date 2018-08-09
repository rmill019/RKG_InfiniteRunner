using UnityEngine.SceneManagement;
using UnityEngine;

/*
 * This GO is composed in a similar structure as DoorSwitch. The GO is composed of two GO's. The parent is the "Open" Door Sprite
 * and the child is the "Closed" Door Sprite. When opened the child GO is moved upwards to simulate a door opening.
*/
public class Door : MonoBehaviour {

    public string m_sceneToLoad;

    private SwitchState m_doorState = SwitchState.Closed;
    private Animator m_anim;
    private AudioSource m_audioSource;
    private AudioClip m_mainClip;

	// Use this for initialization
	void Start () {
        m_anim = GetComponent<Animator>();
        m_audioSource = GetComponent<AudioSource>();
        m_mainClip = m_audioSource.clip;

        if (m_anim == null)
            Debug.LogWarning("No Animator found on " + gameObject.name);
	}

    public AudioClip DoorAudioClip
    {
        get { return m_mainClip; }
    }

    public SwitchState DoorState
    {
        get { return m_doorState; }
        set { m_doorState = value; }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (m_doorState == SwitchState.Open)
                SceneManager.LoadScene(m_sceneToLoad);
            else
            {
                GameManager.S.m_player.GetComponent<RobotController>().Die();
            }

        }
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.tag == "Player")
    //        GameManager.S.m_player.GetComponent<RobotController>().Die();
    //}
}
