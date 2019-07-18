using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRControl : MonoBehaviour {

    // OVR
    public GameObject ovrCamera;
    public GameObject ovrEye;
    public Transform ovrRightHandAnchor;
    public GameObject staticLeft;
    public GameObject staticRight;
    public GameObject toggle;
    public GameObject staticCameras;  
    public GameObject castlegate;
    public GameObject bridge;
    public GameObject userInterface;
    public GameObject reticle;  
    public GameObject mouseControl;
    public GameObject dimmer;
    public LineRenderer lineRenderer;
    public LayerMask teleportMask;

    private HotSpot currentHotspot = null;
    private OVRScreenFade fader;
    private StaticCamera stat;
    private CharacterController cont;
    private Vector3 moveDirection = Vector3.zero;
    private bool active = true;
    private Vector3 offset;
    private float teleportDelay = 1f;
    private float lastTeleport;
    private string device;
    private bool doorTeleport = false;
    private bool shouldTeleport = false;
    private Vector3 teleportPoint = Vector3.zero;
    private bool shouldOpen = false;
    private bool hotSpotOpen = false;

    private void Start(){     
        cont = ovrCamera.GetComponent<CharacterController>();
        fader = ovrEye.GetComponent<OVRScreenFade>();
        stat = staticCameras.GetComponent<StaticCamera>();
        offset = new Vector3(0, 0.05f, 0);
        lastTeleport = Time.time;
    }

    private void Update()
    {
        // For click to move and hotspot
        Ray pointer = new Ray(ovrRightHandAnchor.position, ovrRightHandAnchor.forward);

        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.1f || shouldTeleport || shouldOpen)
        {
            lineRenderer.enabled = true;
            reticle.SetActive(true);

            // Create and draw ray
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, pointer.origin);
            lineRenderer.SetPosition(1, pointer.origin + pointer.direction * 500.0f);
            
            HotSpotControl(pointer);
            ClickToMove(pointer);
        }
        else
        {
            reticle.SetActive(false);
            lineRenderer.enabled = false;
        }

        
        MovementControl();
    }

    private void ClickToMove(Ray pointer)
    {
        // If trigger is released, teleport to that location
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) < 0.1f && shouldTeleport)
        {
            fader.FadeOut();
            Teleport(teleportPoint);
            fader.FadeIn();
            lastTeleport = Time.time;
            shouldTeleport = false;
        }

        // Teleporting up the round tower
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) < 0.1f && doorTeleport)
        {
            fader.FadeOut();
            Teleport(new Vector3(-40.22f, 15f, -40.56f));
            fader.FadeIn();
            lastTeleport = Time.time;
            doorTeleport = false;
        }

        RaycastHit laserHit;

        // If the ray hits a teleportable location
        if (Physics.Raycast(pointer, out laserHit, 500.0f))
        {
            if (laserHit.transform.gameObject.layer == LayerMask.NameToLayer("CanTeleport"))
            {
                // Display a reticle at the target.
                reticle.SetActive(true);
                reticle.transform.position = laserHit.point + offset;

                if (Time.time - lastTeleport > teleportDelay)
                {
                    shouldTeleport = true;
                    teleportPoint = laserHit.point;
                }

            }
            else
            {
                reticle.SetActive(false);
                shouldTeleport = false;
            }

            // The round tower door
            if (laserHit.transform.gameObject.layer == LayerMask.NameToLayer("Door"))
            {
                doorTeleport = true;
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
                shouldOpen = true;
              
            }
        }
        // Else if the current hotspot is faced away from, deactivate it
        else if (currentHotspot != null && !hotSpotOpen)
        {
            currentHotspot.EndHover();
            currentHotspot = null;
        }
        // Close the open hotspot when the tigger is pressed
        else if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) < 0.1f && hotSpotOpen)
        {
            currentHotspot.CloseHotSpot();
            currentHotspot = null;
            hotSpotOpen = false;
            dimmer.SetActive(false);
        }
        else
        {
            shouldOpen = false;
        }

        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) < 0.1f && shouldOpen)
        {
            // Open the hotspot
            currentHotspot.OpenHotSpot();
            hotSpotOpen = true;

            // Move the camera to the hotspot
            MoveCamera("HotSpot");
            shouldOpen = false;
        }
    }

    private void MovementControl()
    {

        // Cycle through static cameras, instead of switching to the camera, teleport the player there instead.
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            // Move to the next hotspot location
            MoveCamera("Right");

            // Close the current hotspot if open
            if(currentHotspot != null)
            {
                currentHotspot.CloseHotSpot();
                currentHotspot = null;
                hotSpotOpen = false;
                dimmer.SetActive(false);
            }
        }

        // Toggle castlegate model
        if (OVRInput.GetDown(OVRInput.Button.Three))
        {
            ToggleCastlegateModel();
        }

        // Fade when rotating camera to reduce motion sickness
        if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft) || OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight))
        {
            fader.FadeOut();
            fader.FadeIn();
        }
    }

    private void Teleport(Vector3 hitPoint)
    {
        reticle.SetActive(false);
        // Difference so not to teleport into the ground
        Vector3 difference = ovrCamera.transform.position - ovrEye.transform.position;
        difference.y = 1;

        // Translate to the position and stop any velocity
        ovrCamera.transform.position = hitPoint + difference;
        cont.SimpleMove(Vector3.zero);
    }

    public void MoveCamera(string direction)
    {
        fader.FadeOut();
        Transform tran = transform;

        if (direction == "Right")
        {
            tran = stat.NextSpot();
            ovrCamera.transform.position = tran.position + new Vector3(0, -1f, 0);
            ovrCamera.transform.rotation = tran.rotation;
        }
        else if (direction == "Left")
        {
            tran = stat.PrevSpot();
            ovrCamera.transform.position = tran.position + new Vector3(0, -1f, 0);
            ovrCamera.transform.rotation = tran.rotation;
        }
        // User has selected the hotspot
        else if (direction == "HotSpot")
        {
            tran = currentHotspot.transform;
            ovrCamera.transform.position = tran.position;
            currentHotspot.OpenVRPanel();
            dimmer.SetActive(true);
        }


        fader.FadeIn();
    }

    private void ToggleCastlegateModel()
    {
        bridge.SetActive(active);
        active = !active;
        castlegate.SetActive(active);

        if (castlegate.active)
        {
            // If the camera is stuck under the castlegate model, teleport on top or back to a hotspot
            RaycastHit hit;
            if (Physics.Raycast(ovrCamera.transform.position, ovrCamera.transform.up, out hit, 40f))
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("CanTeleport"))
                {
                    // Transform to the ground + the height of the player character
                    ovrCamera.transform.position = hit.point + new Vector3(0, 2, 0);
                }
                else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Building"))
                {

                    Vector3 min = Vector3.positiveInfinity;

                    Vector3 pos = ovrCamera.transform.position;
                    int m = 0;

                    // Transfrom to the nearest hotspot
                    for (int i = 1; i <= staticCameras.transform.childCount; i++)
                    {
                        Vector3 distance = staticCameras.transform.Find((i).ToString()).transform.position - pos;

                        if (distance.sqrMagnitude < min.sqrMagnitude)
                        {
                            m = i;
                            min = distance;
                        }
                    }

                    ovrCamera.transform.position = staticCameras.transform.Find(m.ToString()).transform.position;
                }
            }
        }     
    }

}
