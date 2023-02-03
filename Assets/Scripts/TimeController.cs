using System;
using TMPro;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    /// Controls how fast time flows in the game.
    [SerializeField]
    private float timeMultiplier;

    /// The hour we want the game to start at.
    [SerializeField]
    private float startHour;


    /// Display the current time.
    [SerializeField]
    private TextMeshProUGUI timeText;

    /// Keep track of the current time.
    private DateTime _currentTime;

    private void Start()
    {
        _currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);
    }

    private void Update()
    {
        UpdateTimeOfDay();
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
}