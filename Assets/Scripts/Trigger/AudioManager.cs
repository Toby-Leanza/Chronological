using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]

    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("Audio Clips")]

    public AudioClip[] footstepSounds;
    
    public AudioClip background;
    public AudioClip grabClock;
    public AudioClip fallSound;
    public AudioClip fstFwdTime;
    public AudioClip freeze;
    public AudioClip unfreeze;
    public AudioClip metalBoxHitSound;
    public AudioClip gravitySound;
    public AudioClip lightBeam;
    public AudioClip switchSound;
    public AudioClip clickSound;
    public AudioClip hoverSound;
    public AudioClip paperNote;
    public AudioClip pressUnpressButton;
    public AudioClip movingPlatform;
    public AudioClip openDoor;
    public AudioClip closedDoor;
    public AudioClip rewindTime;
    public AudioClip spawnCopy;
    private float backgroundVolume;

    private void Start()
    {
        musicSource.clip = background;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && SFXSource != null)
        {
            SFXSource.PlayOneShot(clip);
        }
    }

    public void PlayRandomFootstep()
    {
        if (footstepSounds.Length > 0)
        {
            AudioClip randomClip = footstepSounds[Random.Range(0, footstepSounds.Length)];
            PlaySFX(randomClip);
        }
    }

    public void PlayFallSound()
    {
        if (fallSound != null)
        {
            PlaySFX(fallSound);
        }
    }

    public void PlayButtonHover()
    {
        if (hoverSound != null && SFXSource != null)
        {
            SFXSource.PlayOneShot(hoverSound);
        }
    }

    public void PlayButtonClick()
    {
        if (clickSound != null && SFXSource != null)
        {
            SFXSource.PlayOneShot(clickSound);
        }
    }

    public void SetMusicVolume(float volume)
    {
        backgroundVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
            musicSource.volume = backgroundVolume;
        PlayerPrefs.SetFloat("MusicVolume", backgroundVolume);
    }

    public float GetMusicVolume() => backgroundVolume;

}
