using System;
using UnityEngine;

public partial class TerrainManager : MonoBehaviour
{
    public int TICKS_PER_SECOND = 20;
    public float realSecondsPerGameDay = 300f;
    [Range(0f, 1f)]
    public float startTimeOffset = 0f;
    public Light globalLight;
    public Color nightColor = Color.blue;
    public Color dayColor = Color.white;
    
    [SerializeField] private CanvasGroup nightFX;

    public float GetDayProgress()
    {
        float gameTimeSeconds = totalTicksElapsed / (float)TICKS_PER_SECOND;
        float rawProgress = gameTimeSeconds / realSecondsPerGameDay;
        float progress = (rawProgress + startTimeOffset) % 1f;
        return progress;
    }

    public float GetSolarIntensity()
    {
        float progress = GetDayProgress();
        float intensity = 0.5f * (Mathf.Cos(2f * Mathf.PI * progress) + 1f);
        return intensity;
    }

    protected void UpdateTime()
    {
        float intensity = GetSolarIntensity();
        if (globalLight != null)
        {
            globalLight.intensity = intensity;
            globalLight.color = Color.Lerp(nightColor, dayColor, intensity);
        }
        nightFX.alpha = 1f - intensity;
        
    }
}