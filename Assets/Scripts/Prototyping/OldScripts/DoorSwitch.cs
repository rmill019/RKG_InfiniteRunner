using UnityEngine;

public enum SwitchState { Closed, Open };

// Basic Switches are constructed by 2 GO's. The parent is the Off Switch and the child is the
// On Switch. To activate the switch we disable the SpriteRenderer of the Parent GO.
public class DoorSwitch : MonoBehaviour {

    private AudioSource m_audioSource;
    private SwitchState m_state = SwitchState.Closed;

    [SerializeField]
    private Door m_targetDoor;

	// Use this for initialization
	void Start () {
        m_audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
       
		
	}

    void SetTargetDoorState (bool isOpen, SwitchState doorState)
    {
        m_audioSource.PlayOneShot(m_audioSource.clip);
        SetActivationState(doorState);
        m_targetDoor.GetComponent<Animator>().SetBool("IsOpen", isOpen);
        m_targetDoor.GetComponent<Animator>().SetBool("Idle", false);
        m_targetDoor.GetComponent<AudioSource>().PlayOneShot(m_targetDoor.DoorAudioClip);
        m_targetDoor.DoorState = doorState;
    }

    // Disable the SpriteRenderer of the parent GO. 
    void SetActivationState (SwitchState state)
    {
        m_state = state;

        // Make sure there is a SpriteRenderer Component Attached
        if (GetComponent<SpriteRenderer>() != null)
        {
            if (state == SwitchState.Closed)
                GetComponent<SpriteRenderer>().enabled = true;
            else
                GetComponent<SpriteRenderer>().enabled = false;
        }
        else
            Debug.LogError("Attempting to call SetActivationState() on " + gameObject.name
                + " but it does not contain a SpriteRenderer");
    }

    public SwitchState State
    {
        get { return m_state; }
        set { m_state = value; }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            SetTargetDoorState(true, SwitchState.Open);
        }
    }
}
