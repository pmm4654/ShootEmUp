using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject mainMenuHolder;
    public GameObject optionsMenuHolder;

    public Slider[] volumeSliders;
    public Toggle[] resolutionToggles;
    public Toggle fullScreenToggle;
    public int[] screenWidths;
    int activeScreenResolutionIndex;

    private void Start()
    {
        activeScreenResolutionIndex = PlayerPrefs.GetInt("ScreenResolutionIndex", activeScreenResolutionIndex);
        bool isFullScreen = (PlayerPrefs.GetInt("FullScreen", 0) == 1);
        print("\n master: " + AudioManager.instance.masterVolumePercent + "\n music: " + AudioManager.instance.musicVolumePercent + "\n sfx: " + AudioManager.instance.sfxVolumePercent);
        volumeSliders[0].value = AudioManager.instance.masterVolumePercent;
        volumeSliders[1].value = AudioManager.instance.musicVolumePercent;
        volumeSliders[2].value = AudioManager.instance.sfxVolumePercent;

        for(int i = 0; i < resolutionToggles.Length; i ++)
        {
            resolutionToggles[i].isOn = i == activeScreenResolutionIndex;
        }
        fullScreenToggle.isOn = isFullScreen;
    }

    public void Play()
    {
        SceneManager.LoadScene("Game");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OptionsMenu()
    {
        mainMenuHolder.SetActive(false);
        optionsMenuHolder.SetActive(true);
    }

    public void MainMenu()
    {
        optionsMenuHolder.SetActive(false);
        mainMenuHolder.SetActive(true);
    }

    public void SetScreenResolution(int i)
    {
        activeScreenResolutionIndex = i;
        float aspectRatio = 16 / 9;
        if(resolutionToggles[i].isOn)
        {
            Screen.SetResolution(screenWidths[i], (int)(screenWidths[i] / aspectRatio), false);
        }
        PlayerPrefs.SetInt("ScreenResolutionIndex", activeScreenResolutionIndex);
        PlayerPrefs.Save();
    }

    public void SetFullScreen(bool isFullScreen)
    {
        for (int i = 0; i < resolutionToggles.Length; i++)
        {
            resolutionToggles[i].interactable = !isFullScreen;
        }

        if(isFullScreen)
        {
            Resolution[] allResolutions = Screen.resolutions;
            Resolution maxResolution = allResolutions[allResolutions.Length - 1];
            Screen.SetResolution(maxResolution.width, maxResolution.height, true);
        } else
        {
            SetScreenResolution(activeScreenResolutionIndex);
        }
        PlayerPrefs.SetInt("FullScreen", isFullScreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetMasterVolume(float volumePercent)
    {
        AudioManager.instance.SetVolume(volumePercent, AudioManager.AudioChannel.Master);
    }

    public void SetMusicVolume(float volumePercent)
    {
        AudioManager.instance.SetVolume(volumePercent, AudioManager.AudioChannel.Music);
    }

    public void SetSfxVolume(float volumePercent)
    {
        AudioManager.instance.SetVolume(volumePercent, AudioManager.AudioChannel.Sfx);
    }

}
