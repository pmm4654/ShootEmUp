using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public AudioClip mainTheme;
    public AudioClip menuTheme;

    string sceneName;
    private void Start()
    {
        AudioManager.instance.PlayMusic(menuTheme, 2);
    }

    private void OnLevelWasLoaded(int sceneIndex)
    {
        string newSceneName = SceneManager.GetActiveScene().name;
        if(newSceneName != sceneName)
        {
            sceneName = newSceneName;
            // instead of calling it directly, we want this to be called after the object with the audio listener component is destroyed - so this keeps it from being called twice
            Invoke("PlayMusic", .2f); 
        }
    }

    void PlayMusic()
    {
        AudioClip clipToPlay = null;
        if(sceneName == "Menu")
        {
            clipToPlay = menuTheme;
        } else if (sceneName == "Game")
        {
            clipToPlay = mainTheme;
        }
        if(clipToPlay != null)
        {
            AudioManager.instance.PlayMusic(clipToPlay, 2);
            Invoke("PlayMusic", clipToPlay.length);
        }
    }
}
