using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Toggle fullScreenToggle;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown qualityDropdown;
    public TextMeshProUGUI fpsText;

    private List<Resolution> filteredResolutions;
    private float deltaTime = 0.0f;

    void Start()
    {
        // Load available screen resolutions
        Resolution[] allResolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<Resolution> resolutions16_9 = new List<Resolution>();

        foreach (Resolution res in allResolutions)
        {
            float aspectRatio = (float)res.width / res.height;
            if (Mathf.Approximately(aspectRatio, 16f / 9f))
            {
                resolutions16_9.Add(res);
            }
        }

        // Sort resolutions by width and height
        resolutions16_9.Sort((a, b) => (b.width * b.height).CompareTo(a.width * a.height));

        // Supposed to limit to the top 10 resolutions for 16:9, It limits to 3 on my laptop for some reason
        if (resolutions16_9.Count > 10)
        {
            resolutions16_9 = resolutions16_9.GetRange(0, 10);
        }

        filteredResolutions = resolutions16_9;

        List<string> resolutionOptions = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            string option = filteredResolutions[i].width + " x " + filteredResolutions[i].height;
            resolutionOptions.Add(option);

            if (filteredResolutions[i].width == Screen.currentResolution.width &&
                filteredResolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        // Load resolution settings
        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Load quality settings
        qualityDropdown.ClearOptions();
        List<string> qualityOptions = new List<string>(QualitySettings.names);
        qualityDropdown.AddOptions(qualityOptions);
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();

        // Fullscreen
        fullScreenToggle.isOn = Screen.fullScreen;

        // Listeners
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        fullScreenToggle.onValueChanged.AddListener(SetFullScreen);
        qualityDropdown.onValueChanged.AddListener(SetQuality);
    }

    public void SetResolution(int index)
    {
        Resolution resolution = filteredResolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    void Update()
    {
        // FPS Counter Logic - REMOVE IN PRODUCTION
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = "FPS: " + Mathf.Ceil(fps).ToString();
    }

    public void CloseSettings()
    {
        gameObject.SetActive(false);
    }
}
