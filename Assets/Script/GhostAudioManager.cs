using UnityEngine;

public class GhostAudioManager : MonoBehaviour
{
    public static GhostAudioManager Instance;
    private AudioSource audioSource;

    public AudioClip hovering;
    public AudioClip stepVision;
    public AudioClip dashing;
    public AudioClip catchingRobberSuccess;
    public AudioClip ambientCreepy;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing from GhostAudioManager!");
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("Attempted to play a null AudioClip.");
        }
    }

    public void PlayHoveringSound()
    {
        PlaySound(hovering);
    }

    public void PlayStepVisionSound()
    {
        PlaySound(stepVision);
    }

    public void PlayDashingSound()
    {
        PlaySound(dashing);
    }

    public void PlayCatchingRobberSuccessSound()
    {
        PlaySound(catchingRobberSuccess);
    }

    public void PlayAmbientCreepySound()
    {
        PlaySound(ambientCreepy);
    }
}
