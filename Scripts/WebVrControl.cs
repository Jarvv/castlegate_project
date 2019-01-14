using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebVrControl : MonoBehaviour
{
    public GameObject webvrCamera;
    public GameObject webvrEye;
    public Transform webvrRightHandAnchor;

    public GameObject staticCameras;
    public GameObject castlegate;
    public GameObject userInterface;
    public GameObject reticle;
    public LineRenderer lineRenderer;
    public LayerMask teleportMask;
    public GameObject mouseControl;
    public GameObject dimmer;

    private HotSpot currentHotspot = null;
    private OVRScreenFade fader;
    private StaticCamera stat;
    private bool hotSpotOpen = false;

    // UI
    public GameObject staticLeft;
    public GameObject staticRight;
    public GameObject toggle;

    // How long the user can gaze at this before the button is clicked.
    public float timerDuration = 3f;

    // Count time the player has been gazing at the button.
    private float lookTimer = 0f;

    private Vector3 offset;
    private string device;

    private void Start()
    {
        fader = webvrEye.GetComponent<OVRScreenFade>();
        stat = staticCameras.GetComponent<StaticCamera>();
        offset = new Vector3(0, 0.05f, 0);
    }

    private void Update()
    {
        // No controller present, cast from eye
        Ray eyeRay = new Ray(webvrEye.transform.position, webvrEye.transform.forward);
        EyeRayMove(eyeRay);
        HotSpotControl(eyeRay);
    }

    private void EyeRayMove(Ray eyeRay)
    {
        RaycastHit hit;

        if (Physics.Raycast(eyeRay, out hit, 50f, LayerMask.GetMask("UI")))
        {
            // Increment the gaze timer.
            lookTimer += Time.deltaTime;

            // Gaze time exceeded limit - button is considered clicked.
            if (lookTimer > timerDuration)
            {
                lookTimer = 0f;

                if (hit.transform.name == "Right")
                    staticRight.GetComponent<Button>().onClick.Invoke();
                else if (hit.transform.name == "Left")
                    staticLeft.GetComponent<Button>().onClick.Invoke();
                else if (hit.transform.name == "Toggle")
                    toggle.GetComponent<Toggle>().isOn = !toggle.GetComponent<Toggle>().isOn;

            }
        }
    }

    private void HotSpotControl(Ray pointer)
    {
        // Cast a ray from the centre eye position.
        RaycastHit hit;
        // If the ray hits a hotspot
        if (Physics.Raycast(pointer, out hit, 20.0f) && hit.transform.tag == "HotSpot")
        {
            // Make the current hotspot active
            if (currentHotspot == null)
                currentHotspot = hit.transform.GetComponentInParent<HotSpot>();
            // If already active
            else
            {
                // Hover the active HotSpot
                currentHotspot.StartHover();

                // Increment the gaze timer.
                lookTimer += Time.deltaTime;

                // If trigger is pressed, open it
                if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.5f)
                {
                    currentHotspot.OpenHotSpot();
                    hotSpotOpen = true;
                }
                else if (lookTimer > timerDuration)
                {
                    lookTimer = 0f;
                    currentHotspot.OpenHotSpot();
                    hotSpotOpen = true;

                }
            }
        }
        // Else if the current hotspot is faced away from, deactivate it
        else if (currentHotspot != null && !hotSpotOpen)
        {
            currentHotspot.EndHover();
            currentHotspot = null;
        }
        // Close the open hotspot when the tigger is pressed
        else if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.5f && hotSpotOpen)
        {
            currentHotspot.CloseHotSpot();
            currentHotspot = null;
            hotSpotOpen = false;
            dimmer.SetActive(false);
        }
        // Close the hotspot if faced away when open for a certain amout of time
        else if (currentHotspot != null && hotSpotOpen)
        {
            // Increment the gaze timer.
            lookTimer += Time.deltaTime;

            if (lookTimer > 5f)
            {
                lookTimer = 0f;
                currentHotspot.CloseHotSpot();
                currentHotspot = null;
                hotSpotOpen = false;
                dimmer.SetActive(false);
            }
        }
    }

    public void ToggleCastlegate(bool active)
    {
        castlegate.SetActive(active);
    }

    public void MoveCamera(string direction)
    {
        fader.FadeOut();
        Transform tran = transform;

        if (direction == "Right")
        {
            tran = stat.NextSpot();
            webvrCamera.transform.parent.position = tran.position + new Vector3(0, -1.5f, 0);
            webvrCamera.transform.parent.rotation = tran.rotation;
        }
            
        else if (direction == "Left")
        {
            tran = stat.PrevSpot();
            webvrCamera.transform.parent.position = tran.position + new Vector3(0, -1.5f, 0);
            webvrCamera.transform.parent.rotation = tran.rotation;
        }
            
        // User has selected the hotspot
        else if (direction == "HotSpot")
        {
            tran = currentHotspot.transform;
            webvrCamera.transform.position = tran.position;
            currentHotspot.OpenVRPanel();
            dimmer.SetActive(true);
        }
        
        fader.FadeIn();
    }
}