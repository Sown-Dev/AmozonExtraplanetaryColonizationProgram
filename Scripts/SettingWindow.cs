using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingWindow : UIWindow {
    public Button resetTutorialButton;
    
    public Slider masterVolumeSlider;
    public Slider soundVolumeSlider;
    public Slider musicVolumeSlider;

    // Reference to the main AudioMixer that contains your subgroups.
    public AudioMixer mainMixer;

    public override void Awake() {
        base.Awake();
        
        soundVolumeSlider.value = GameManager.Instance.settings.sfxVolume;
        musicVolumeSlider.value = GameManager.Instance.settings.musicVolume;
        masterVolumeSlider.value = GameManager.Instance.settings.masterVolume;
        masterVolumeSlider.onValueChanged.AddListener( f => {
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
        
    }

    private void Start(){
        
        SetVolume(GameManager.Instance.settings.sfxVolume, "SFX");
        SetVolume(GameManager.Instance.settings.musicVolume, "Music");
        SetVolume(GameManager.Instance.settings.masterVolume, "Master");

    }

    public void ResetTutorial() {
        GameManager.Instance.settings.completedTutorials = new List<string>();
    }

    public override void Refresh() {
        base.Refresh();
        
    }

    private void Update() {
        resetTutorialButton.interactable = GameManager.Instance.settings.completedTutorials.Count > 0;
    }

    public void SetVolume(float value, string key) {
        // If value is 0 or less, mute the group (set to -80 dB, a common mute level)
        float dB = (value <= 0f) ? -80f : Mathf.Log10(value) * 20;
        mainMixer.SetFloat(key, dB);
        Refresh();
    }

    
}