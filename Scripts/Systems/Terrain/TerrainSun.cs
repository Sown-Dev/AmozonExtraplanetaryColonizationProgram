using UnityEngine;
using UnityEngine.Rendering.Universal;

public partial class TerrainManager : MonoBehaviour
{
    // Time configuration
    private const int TICKS_PER_SECOND = 20;
    private const float REAL_SECONDS_PER_GAME_DAY = 5f * 60f;
    private float dayProgress = 0f;

    // Solar curve configuration
    [SerializeField] private AnimationCurve sunIntensityCurve;
    private const float DAYTIME_START = 0f;  // 6 AM
    private const float DAYTIME_END = 0.6f;    // 6 PM
    private const float SUNRISE_DURATION = 0.1f;
    private const float SUNSET_DURATION = 0.1f;
    
    //light
    [SerializeField] private Light2D globalLight;
    [SerializeField] private Color nightColor;
    [SerializeField] private Color dayColor;
    [SerializeField] private CanvasGroup nightFX;

    
    private void InitializeSunCurve()
    {
        sunIntensityCurve = new AnimationCurve();
        

        // Nighttime (0 intensity)
        sunIntensityCurve.AddKey(0f, 1f);
        sunIntensityCurve.AddKey(DAYTIME_START - SUNRISE_DURATION, 0f);

        // Sunrise
        sunIntensityCurve.AddKey(DAYTIME_START, 0f);
        sunIntensityCurve.AddKey(DAYTIME_START + SUNRISE_DURATION, 1f);

        // Daylight plateau
        sunIntensityCurve.AddKey(0.5f, 1f);

        // Sunset
        sunIntensityCurve.AddKey(DAYTIME_END - SUNSET_DURATION, 1f);
        sunIntensityCurve.AddKey(DAYTIME_END, 0f);

        // Nighttime
        sunIntensityCurve.AddKey(1f, 0f);

        // Smooth the curve
        for (int i = 0; i < sunIntensityCurve.length; i++)
        {
            sunIntensityCurve.SmoothTangents(i, 0.5f);
        }
    }

    private void UpdateTime()
    {
        // Calculate day progress (0-1)
        dayProgress = ((totalTicksElapsed/TICKS_PER_SECOND) % REAL_SECONDS_PER_GAME_DAY) / REAL_SECONDS_PER_GAME_DAY;
        
        
        // Update the global light intensity based on the sun intensity curve
        /*globalLight.intensity = 0.4f + GetSolarIntensity() * 0.6f; // Adjust the base intensity as needed
        globalLight.color = Color.Lerp(nightColor, dayColor, intensity);*/
        nightFX.alpha = 1 - GetSolarIntensity(); // Fade out night effects based on intensity
    }

    public float GetSolarIntensity()
    {
        // Evaluate curve with wrap-around for day/night cycle
        return sunIntensityCurve.Evaluate(dayProgress);
    }

    // Optional helper for time display
    public string GetFormattedGameTime()
    {
        float totalHours = dayProgress * 24f;
        int hours = Mathf.FloorToInt(totalHours);
        int minutes = Mathf.FloorToInt((totalHours - hours) * 60f);
        return $"{hours:00}:{minutes:00}";
    }
}