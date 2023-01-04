using System;
using UnityEngine;
using Cinemachine;

/// <summary>
/// Recenter the camera to the direction the character is facing when the specified input is pressed.
/// </summary>
public class CameraRecenter : MonoBehaviour
{
    private CinemachineFreeLook _camera;

    private void Start()
    {
        _camera = GetComponent<CinemachineFreeLook>();
    }

    private void Update()
    {
        // If left ctrl or right trigger has been pressed
        if (Math.Abs(Input.GetAxis("CameraRecenter") - 1) < 1)
        {
            _camera.m_RecenterToTargetHeading.m_enabled = true;
        }
        else
        {
            _camera.m_RecenterToTargetHeading.m_enabled = false;
        }
    }
}