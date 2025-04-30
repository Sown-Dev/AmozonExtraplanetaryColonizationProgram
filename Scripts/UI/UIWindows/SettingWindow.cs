using System;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingWindow : UIWindow{
    public Button resetTutorialButton;

    public Slider masterVolumeSlider;
    public Slider soundVolumeSlider;
    public Slider musicVolumeSlider;

    public Toggle terrainInfoToggle;
    public Toggle oreInfoToggle;
    public Toggle blockInfoToggle;

    // Reference to the main AudioMixer that contains your subgroups.
    public AudioMixer mainMixer;


    public Toggle fullscreenToggle;
    public TMP_Dropdown resolutionDropdown;

    private readonly Vector2Int[] allowedResolutions = new[]{
        new Vector2Int(640, 360),
        new Vector2Int(1280, 720),
        new Vector2Int(1920, 1080),
    };

    private List<Resolution> resolutionsList = new List<Resolution>();

    public override void Awake(){
        base.Awake();

        soundVolumeSlider.value = GameManager.Instance.settings.sfxVolume;
        musicVolumeSlider.value = GameManager.Instance.settings.musicVolume;
        masterVolumeSlider.value = GameManager.Instance.settings.masterVolume;
        masterVolumeSlider.onValueChanged.AddListener(f => {
            GameManager.Instance.settings.masterVolume = f;
            SetVolume(GameManager.Instance.settings.masterVolume, "Master");
        });
        soundVolumeSlider.onValueChanged.AddListener(f => {
            GameManager.Instance.settings.sfxVolume = f;
            SetVolume(GameManager.Instance.settings.sfxVolume, "SFX");
        });
        musicVolumeSlider.onValueChanged.AddListener(f => {
            GameManager.Instance.settings.musicVolume = f;
            SetVolume(GameManager.Instance.settings.musicVolume, "Music");
        });

        terrainInfoToggle.isOn = GameManager.Instance.settings.TerrainInfo;
        terrainInfoToggle.onValueChanged.AddListener(f => { GameManager.Instance.settings.TerrainInfo = f; });
        oreInfoToggle.isOn = GameManager.Instance.settings.OreInfo;
        oreInfoToggle.onValueChanged.AddListener(f => { GameManager.Instance.settings.OreInfo = f; });
        blockInfoToggle.isOn = GameManager.Instance.settings.BlockInfo;

        blockInfoToggle.onValueChanged.AddListener(f => { GameManager.Instance.settings.BlockInfo = f; });


        fullscreenToggle.isOn = Screen.fullScreen; 
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggle);

        resolutionDropdown.ClearOptions();
        var optionLabels = new List<string>();
        for (int i = 0; i < allowedResolutions.Length; i++)
        {
            var v = allowedResolutions[i];
            optionLabels.Add($"{v.x} x {v.y}");
            resolutionsList.Add(new Resolution { width = v.x, height = v.y });
        }
        resolutionDropdown.AddOptions(optionLabels);

        // Set dropdown to current resolution if in our list
        var current = Screen.currentResolution;
        int idx = resolutionsList.FindIndex(r => r.width == current.width && r.height == current.height);
        resolutionDropdown.value = idx >= 0 ? idx : 0;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(OnResolutionChange);       
        
    }

    private void Start(){
        SetVolume(GameManager.Instance.settings.sfxVolume, "SFX");
        SetVolume(GameManager.Instance.settings.musicVolume, "Music");
        SetVolume(GameManager.Instance.settings.masterVolume, "Master");
        
        
    }

    public void ResetTutorial(){
        GameManager.Instance.settings.completedTutorials = new List<string>();
    }

    public override void Refresh(){
        base.Refresh();
    }

    private void Update(){
        resetTutorialButton.interactable = GameManager.Instance.settings.completedTutorials.Count > 0;
    }

    public void SetVolume(float value, string key){
        // If value is 0 or less, mute the group (set to -80 dB, a common mute level)
        float dB = (value <= 0f) ? -80f : Mathf.Log10(value) * 20;
        mainMixer.SetFloat(key, dB);
        Refresh();
    }


    private void OnFullscreenToggle(bool isFullscreen){
        // Toggle fullscreen/windowed mode :contentReference[oaicite:3]{index=3}
        Screen.fullScreen = isFullscreen;
    }

    private void OnResolutionChange(int index){
        var sel = resolutionsList[index];
        // Apply selected resolution, preserve fullscreen state :contentReference[oaicite:4]{index=4}
        Screen.SetResolution(sel.width, sel.height, Screen.fullScreen);
        resolutionDropdown.RefreshShownValue();         

    }
}