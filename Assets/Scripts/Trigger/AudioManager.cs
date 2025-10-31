using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("Audio Clips")]
    public AudioClip[] footstepSounds;
    public AudioClip[] background; // Ahora es un array

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
    private float sfxVolume = 1f;

    private void Start()
    {
        backgroundVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        musicSource.volume = backgroundVolume;
        SFXSource.volume = sfxVolume;

        background = Resources.LoadAll<AudioClip>("Sounds/Music");
        PlayRandomBackgroundMusic();
    }

    public void PlayRandomBackgroundMusic()
    {
        if (background.Length > 0)
        {
            AudioClip randomClip = background[Random.Range(0, background.Length)];
            musicSource.clip = randomClip;
            musicSource.loop = false;
            musicSource.Play();

            Invoke(nameof(PlayRandomBackgroundMusic), randomClip.length);
        }
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

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (SFXSource != null)
            SFXSource.volume = sfxVolume;
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }

    public float GetSFXVolume() => sfxVolume;
}