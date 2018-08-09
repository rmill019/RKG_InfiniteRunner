using UnityEngine;

public class AudioManager : MonoBehaviour {

    public static AudioManager S;
    public AudioClip deathClip;
    public AudioClip coinCollectedClip;
    public AudioClip spikeClip;

    private AudioSource m_audioSource;

    private void Awake()
    {
        if (S == null)
            S = this;
        else
            Destroy(this.gameObject);
    }

    // Use this for initialization
    void Start () {
        m_audioSource = GetComponent<AudioSource>();
	}

    public void PlayClip(AudioClip clipToPlay, float volume = 1.0f)
    {
        m_audioSource.PlayOneShot(clipToPlay, volume);
    }
}
