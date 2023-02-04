using System;
using TMPro;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    /// Display the current time.
    [SerializeField]
    private TextMeshProUGUI timeText;

    /// Light representing the sun.
    [SerializeField]
    private Light sunLight;

    /// A light representing the moon.
    [SerializeField]
    private Light moonLight;

    /// Hour when the sun rises.
    /// Ambient light color during the day.
    [SerializeField]
    private Color dayAmbientLight;

    /// Ambient light color during the night.
    [SerializeField]
    private Color nightAmbientLight;

    /// Transition between the ambient light colors smoothly.
    [SerializeField]
    private AnimationCurve lightChangeCurve;

    /// Maximum intensity of the sun light during the day..
    [SerializeField]
    public float maxSunLightIntensity;

    /// Maximum intensity of the moon light at night.
    [SerializeField]
    public float maxMoonLightIntensity;

    [SerializeField]
    private float sunriseHour;

    /// Hour when the sun sets.
    [SerializeField]
    private float sunsetHour;

    /// Hour we want the game to start at.
    [SerializeField]
    private float startHour;

    /// Controls how fast time flows in the game.
    [SerializeField]
    private float timeMultiplier;

    /// Time when the sun sets.
    private TimeSpan _sunsetTime;

    /// Time when the sun rises.
    private TimeSpan _sunriseTime;

    /// Keep track of the current time.
    private DateTime _currentTime;
    
    private void Start()
    {
        _currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);
        _sunriseTime = TimeSpan.FromHours(sunriseHour);
        _sunsetTime = TimeSpan.FromHours(sunsetHour);
    }

    private void Update()
    {
        UpdateTimeOfDay();
        RotateSun();
        UpdateLightSettings();
    }

    /// Update the current time at the desired rate.
    private void UpdateTimeOfDay()
    {
        _currentTime = _currentTime.AddSeconds(Time.deltaTime * timeMultiplier);

        if (timeText != null)
        {
            timeText.text = _currentTime.ToString("HH:mm");
        }
    }

    /// Rotate the sun based on the calculated time difference.
    private void RotateSun()
    {
        // Holds the value of the rotation of the sun
        float sunLightRotation;

        // Check if we are in daytime
        if (_currentTime.TimeOfDay > _sunriseTime && _currentTime.TimeOfDay < _sunsetTime)
        {
            // Calculate the total time between sunrise and sunset
            TimeSpan sunriseToSunsetDuration = CalculateTimeDifference(_sunriseTime, _sunsetTime);

            // Calculate how much time has passed since sunrise
            TimeSpan timeSinceSunrise = CalculateTimeDifference(_sunriseTime, _currentTime.TimeOfDay);

            // Determine which percentage of the daytime has passed
            var percentage = timeSinceSunrise.TotalMinutes / sunriseToSunsetDuration.TotalMinutes;

            // Set the rotation value to 0 at sunrise and will  progress to 180 at sunset
            sunLightRotation = Mathf.Lerp(0, 180, (float)percentage);
        }
        else
        {
            // Calculate the total time between sunset and sunrise
            TimeSpan sunsetToSunriseDuration = CalculateTimeDifference(_sunsetTime, _sunriseTime);

            // Calculate how much time has passed since sunset
            TimeSpan timeSinceSunset = CalculateTimeDifference(_sunsetTime, _currentTime.TimeOfDay);

            // Determine which percentage of the night has passed
            var percentage = timeSinceSunset.TotalMinutes / sunsetToSunriseDuration.TotalMinutes;

            // Set the rotation value to 180 at sunset and will progress to 360 at sunrise
            sunLightRotation = Mathf.Lerp(180, 360, (float)percentage);
        }

        // Apply the rotation to the sun light
        // Pass in vector3.right to have it rotate on the x-axis
        sunLight.transform.rotation = Quaternion.AngleAxis(sunLightRotation, Vector3.right);
    }

    /// Change between daytime and nighttime settings
    private void UpdateLightSettings()
    {
        // Get value between -1 and 1 and if sun is pointing down we get 1, horizontal we get 0, up we get -1
        var dotProduct = Vector3.Dot(sunLight.transform.forward, Vector3.down);

        // Create a non-linear transition of the light 

        // Adjust the intensity of the sun by transitioning off (0 -> max)
        sunLight.intensity = Mathf.Lerp(0, maxSunLightIntensity, lightChangeCurve.Evaluate(dotProduct));

        // Adjust the intensity of the moon by transitioning on (max -> 0)
        moonLight.intensity = Mathf.Lerp(maxMoonLightIntensity, 0, lightChangeCurve.Evaluate(dotProduct));

        // Transition from nighttime ambient light to daytime ambient light
        RenderSettings.ambientLight = Color.Lerp(nightAmbientLight, dayAmbientLight, lightChangeCurve.Evaluate(dotProduct));
    }

    /// Calculate the difference between times to determine how long until sunset or sunrise.
    /// <param name="fromTime"> </param>
    /// <param name="toTime"> </param>
    /// <returns> The difference of the two given times. </returns>
    private TimeSpan CalculateTimeDifference(TimeSpan fromTime, TimeSpan toTime)
    {
        TimeSpan difference = toTime - fromTime;

        // Check if the two times have spanned the transition from one day to another
        if (difference.TotalSeconds < 0)
        {
            difference += TimeSpan.FromHours(24);
        }

        return difference;
    }
    
    
}