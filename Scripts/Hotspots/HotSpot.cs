using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR;

public class HotSpot : MonoBehaviour {

    public GameObject userInterface;
    public Animator sphereAnim;
    public Text physicalTitle;
    public Text physicalText;
    public GameObject hotSpotTitle;
    public GameObject informationCanvas;
    public TextFollower tf;
    public GameObject hotspotMain;

    private bool open = false;
    private InformationController info;
    private Vector3 origin;
   
    private void Start()
    {
        info = userInterface.GetComponent<InformationController>();
        origin = informationCanvas.transform.position;
    }

    public void OpenHotSpot()
    {
        // Desktop/Mobile input
        if (UnityEngine.XR.XRSettings.isDeviceActive != true)
        {
            info.OpenInfoPanel(this.name);
        }
        // VR input
        else
        {
            hotspotMain.SetActive(false);
            open = true;
            informationCanvas.SetActive(true);
            hotSpotTitle.SetActive(false);
            tf.enabled = false;
        }              
    }

    public void CloseHotSpot()
    {
        // Desktop/Mobile input
        if (UnityEngine.XR.XRSettings.isDeviceActive != true)
        {
            info.CloseInfoPanel();
        }
        // VR input
        else
        {
            hotspotMain.SetActive(true);
            open = false;
            informationCanvas.SetActive(false);
            hotSpotTitle.SetActive(true);
            tf.enabled = true;
            CloseVRPanel();
        }
    }

    public void StartHover()
    {
        sphereAnim.SetBool("hovering", true);
    }

    public void EndHover()
    {
        sphereAnim.SetBool("hovering", false);
    }

    public void HotSpotControl()
    {
        if (open)
        {
            EndHover();
            CloseHotSpot();
            open = false;
        }     
        else{
            StartHover();
            OpenHotSpot();
            open = true;
        }            
    }

    public string GetHotSpotNumber()
    {
        return this.name;
    }

    public void OpenVRPanel()
    {
        // Change the direction of the hotspot
        Vector3 dir = Vector3.zero;
        switch (this.name) {
            case "1":
                dir = new Vector3(0, 0, -4);
                break;
            case "2":
                dir = new Vector3(-3.83f, 0, 0.32f);
                break;
            case "3":
                dir = new Vector3(0, 0, 4);
                break;
            case "4":
                dir = new Vector3(0, 0, -4);
                break;
            case "5":
                dir = new Vector3(0, 0, -4);
                break;
        }

        informationCanvas.transform.position += dir;
        Vector3 v = origin - informationCanvas.transform.position;
        v.x = v.z = 0.0f;
        informationCanvas.transform.LookAt(origin - v);
        informationCanvas.transform.Rotate(0, 180, 0);
    }

    private void CloseVRPanel()
    {
        informationCanvas.transform.position = origin;
    }
}