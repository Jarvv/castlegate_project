using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using System.Runtime.InteropServices;
using System;

public class BuildControl : MonoBehaviour
{

    public GameObject ovrCamera;
    public GameObject flyCam;
    public GameObject walkCam;
    public GameObject mouseControl;
    public GameObject webvrCamera;
    public GameObject viveCam;
    public GameObject userInterface;
    public GameObject desktopInterface;
    public GameObject mobileInterface;
    public GameObject gazeControls;
    public LineRenderer lineRenderer;
    public bool onMobile = false;

    // From mobile.jslib plugin
    [DllImport("__Internal")]
    private static extern bool IsMobile();

    [DllImport("__Internal")]
    private static extern void CloseApplicationWindow();

    void Awake()
    {
        // Change scene depending on what platform is being used
        // If using the standalone build
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            // OVR
            if (OVRManager.isHmdPresent)
            {
                walkCam.SetActive(false);
                webvrCamera.SetActive(false);
                userInterface.SetActive(false);
                mouseControl.SetActive(false);
                ovrCamera.SetActive(true);

                transform.GetComponent<WebVrControl>().enabled = false;
            }
            // SteamVR
            else if (SteamVR.enabled)
            {
                walkCam.SetActive(false);
                viveCam.SetActive(true);
                webvrCamera.SetActive(false);
                userInterface.SetActive(false);
                mouseControl.SetActive(false);
                transform.GetComponent<WebVrControl>().enabled = false;

            }
            // Else use standard desktop controls
            else
            {
                // Init the walkCam
                walkCam.SetActive(true);
                desktopInterface.SetActive(true);
            }
        }
        // If using the WebGL build
        else if (Application.platform == RuntimePlatform.WebGLPlayer)
        {

            // Using the jslib plugin, check if on mobile.        
            onMobile = IsMobile();

            // If running WebVr
            if (false)
            {
                flyCam.gameObject.SetActive(false);
                userInterface.SetActive(false);
                mouseControl.SetActive(false);
                webvrCamera.SetActive(true);
                gazeControls.SetActive(true);
                transform.GetComponent<VRControl>().enabled = false;
            }
            // If running on mobile browser
            else if (onMobile)
            {
                walkCam.SetActive(true);
                desktopInterface.SetActive(false);
                mobileInterface.SetActive(true);
            }
            // Desktop browser
            else
            {
                walkCam.SetActive(true);
                mobileInterface.SetActive(false);
                desktopInterface.SetActive(true);
            }
        }
    }

    public void CloseApplication()
    {
        // If using the standalone build
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            Application.Quit();
        }

        // If using the WebGL build
        else if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            CloseApplicationWindow();
        }
    }
}
