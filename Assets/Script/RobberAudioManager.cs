using UnityEngine;

public class RobberAudioManager : MonoBehaviour
{
    public static RobberAudioManager Instance;
    private AudioSource audioSource;

    public AudioClip pickUpItem;
    public AudioClip interacting;
    public AudioClip walking;
    public AudioClip bumpIntoWall;
    public AudioClip flashlightToggle;
    public AudioClip nightVisionToggle;
    public AudioClip timeRunsOut;
    public AudioClip seeGhostUnderNightVision;
    public AudioClip successEscape;
    public AudioClip whiteNoise;
    public AudioClip ventsInteract;

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
            Debug.LogError("AudioSource component is missing from RobberAudioManager!");
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

    public void PlayPickUpItemSound()
    {
        PlaySound(pickUpItem);
    }

    public void PlayInteractingSound()
    {
        PlaySound(interacting);
    }

    public void PlayWalkingSound()
    {
        PlaySound(walking);
    }

    public void PlayBumpIntoWallSound()
    {
        PlaySound(bumpIntoWall);
    }

    public void PlayFlashlightToggleSound()
    {
        PlaySound(flashlightToggle);
    }

    public void PlayNightVisionToggleSound()
    {
        PlaySound(nightVisionToggle);
    }

    public void PlayTimeRunsOutSound()
    {
        PlaySound(timeRunsOut);
    }

    public void PlaySeeGhostUnderNightVisionSound()
    {
        PlaySound(seeGhostUnderNightVision);
    }

    public void PlaySuccessEscapeSound()
    {
        PlaySound(successEscape);
    }

    public void PlayWhiteNoiseSound()
    {
        PlaySound(whiteNoise);
    }

    public void PlayVentsInteractSound()
    {
        PlaySound(ventsInteract);
    }
}
