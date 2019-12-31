using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum AudioChannel { Master, Sfx, Music }

    public float masterVolumePercent {get; private set; }
    public float sfxVolumePercent { get; private set; } // sound effects
    public float musicVolumePercent { get; private set; }

    AudioSource sfx2DSource;
    AudioSource[] musicSources;
    int activeMusicSourceIndex;

    Transform audioListener;
    Transform playerTransform;

    SoundLibrary library;

    public static AudioManager instance; // make it a singletom 
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        } else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            library = GetComponent<SoundLibrary>();
            musicSources = new AudioSource[2];
            for (int i = 0; i < 2; i++)
            {
                GameObject newMusicSource = new GameObject("MusicSource" + (i + 1));
                musicSources[i] = newMusicSource.AddComponent<AudioSource>();
                newMusicSource.transform.parent = transform;
            }
            GameObject newSfx2Dsource = new GameObject("2DSfxSource");
            sfx2DSource = newSfx2Dsource.AddComponent<AudioSource>();
            newSfx2Dsource.transform.parent = transform;

            audioListener = FindObjectOfType<AudioListener>().transform;
            if(FindObjectOfType<Player>() != null)
            {
                playerTransform = FindObjectOfType<Player>().transform;
            }

            GetVolumePreferences();
        }
    }

    void OnLevelWasLoaded(int index)
    {
        if (playerTransform == null)
        {
            if (FindObjectOfType<Player>() != null)
            {
                playerTransform = FindObjectOfType<Player>().transform;
            }
        }
    }

    private void Update()
    {
        if(playerTransform != null)
        {
            audioListener.position = playerTransform.position;
        }
    }

    public void SetVolume(float volumePercent, AudioChannel channel)
    {
        switch(channel)
        {
            case AudioChannel.Master:
                masterVolumePercent = volumePercent;
                break;
            case AudioChannel.Sfx:
                sfxVolumePercent = volumePercent;
                break;
            case AudioChannel.Music:
                musicVolumePercent = volumePercent;
                break;
        }

        musicSources[0].volume = musicVolumePercent * masterVolumePercent;
        musicSources[1].volume = musicVolumePercent * masterVolumePercent;
        SetVolumePreferences();
    }

    public void GetVolumePreferences()
    {
        masterVolumePercent = PlayerPrefs.GetFloat("master volume", 1);
        sfxVolumePercent = PlayerPrefs.GetFloat("sfx volume", 1);
        musicVolumePercent = PlayerPrefs.GetFloat("music volume", 1);
    }

    public void SetVolumePreferences()
    {
        PlayerPrefs.SetFloat("master volume", masterVolumePercent);
        PlayerPrefs.SetFloat("sfx volume", sfxVolumePercent);
        PlayerPrefs.SetFloat("music volume", musicVolumePercent);
        PlayerPrefs.Save();
    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 1)
    {
        activeMusicSourceIndex = 1 - activeMusicSourceIndex; // since it's always 2 sources this will just go back and forth between 1 and 0
        musicSources[activeMusicSourceIndex].clip = clip;
        musicSources[activeMusicSourceIndex].Play();

        StartCoroutine(AnimateMusicCrossfade(fadeDuration));
    }

    public void PlaySound(AudioClip clip, Vector3 position)
    {
        if(clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, position, sfxVolumePercent * masterVolumePercent);
        }
    }

    public void PlaySound(string soundName, Vector3 position)
    {
        PlaySound(library.GetClipFromName(soundName), position);
    }

    public void PlaySound2D(string soundName)
    {
        sfx2DSource.PlayOneShot(library.GetClipFromName(soundName), sfxVolumePercent * masterVolumePercent);
    }

    IEnumerator AnimateMusicCrossfade(float duration)
    {
        float percent = 1;
        float speed = 1 / duration;
        while (percent < 1) {
            percent += speed * Time.deltaTime;
            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, musicVolumePercent * masterVolumePercent, percent);
            musicSources[1-activeMusicSourceIndex].volume = Mathf.Lerp(musicVolumePercent * masterVolumePercent, 0, percent);
            yield return null;
        }
    }
}
