using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;

    Resolution[] resolutions;
    List<Resolution> filterResolution = new List<Resolution>();

    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        
        float currentHz = Screen.currentResolution.refreshRate;
        for (int i=0; i<resolutions.Length; i++)
        {
            if (resolutions[i].refreshRate == currentHz && resolutions[i].height >= 1024 && resolutions[i].width >= 1600)
            {
                filterResolution.Add(resolutions[i]);
            }
        }

        List<string> options = new List<string>();
        int currentIndex = 0;
        for (int i=0; i<filterResolution.Count; i++)
        {
            string option = filterResolution[i].width + " x " + filterResolution[i].height;
            options.Add(option);

            if (filterResolution[i].width == Screen.currentResolution.width &&
                filterResolution[i].height == Screen.currentResolution.height)
            {
                currentIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = filterResolution[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }    

    public void PlayEasyGame() 
    {
        PlayerPrefs.SetInt("mode",0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
    }

    public void PlayHardGame() 
    {
        PlayerPrefs.SetInt("mode",1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
