using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViveControl : MonoBehaviour
{

    public GameObject viveEye;
    public GameObject viveCam;
    public GameObject reticle;
    public GameObject doorTeleportLocation;
    public GameObject staticCameras;
    public GameObject castlegate;
    public GameObject bridge;
    public Transform rightHandAnchor;
    public LineRenderer lineRenderer;

    private CharacterController cont;
    private OVRScreenFade fader;
    private StaticCamera stat;
    private HotSpot currentHotspot = null;
    private bool shouldTeleport = false;
    private bool active = true;
    private bool hotSpotOpen = false;
    private bool doorTeleport = false;
    private Vector3 teleportPoint = Vector3.zero;
    private Vector3 offset;
    private float teleportDelay = 1f;
    private float lastTeleport;

    // Vive Controller Setup
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }
    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private void Start()
    {
        cont = viveCam.GetComponent<CharacterController>();
        fader = viveEye.GetComponent<OVRScreenFade>();
        stat = staticCameras.GetComponent<StaticCamera>();
        offset = new Vector3(0, 0.05f, 0);
        lastTeleport = Time.time;
    }

    void Update()
    {
        Ray pointer = new Ray(rightHandAnchor.position, rightHandAnchor.forward);
        HotSpotControl(pointer);
        ClickToMove(pointer);

        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            MoveCamera("Right");
        }
        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            ToggleCastlegateModel();
        }
    }

    private void ClickToMove(Ray pointer)
    {
        if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            lineRenderer.enabled = true;
            reticle.SetActive(true);

            // Create and draw ray
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, pointer.origin);
            lineRenderer.SetPosition(1, pointer.origin + pointer.direction * 500.0f);
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
                }

                // The round tower door
                if (laserHit.transform.gameObject.layer == LayerMask.NameToLayer("Door"))
                {
                    doorTeleport = true;
                }
            }
        }
        else
        {
            reticle.SetActive(false);
            lineRenderer.enabled = false;
        }

        // If trigger is pressed, teleport to that location
        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) && shouldTeleport)
        {
            fader.FadeOut();
            Teleport(teleportPoint);
            fader.FadeIn();
            lastTeleport = Time.time;
            shouldTeleport = false;
        }

        // Teleporting up the round tower
        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) && doorTeleport)
        {
            fader.FadeOut();
            Teleport(doorTeleportLocation.transform.position);
            fader.FadeIn();
            lastTeleport = Time.time;
            doorTeleport = false;
        }
    }

    private void Teleport(Vector3 hitPoint)
    {
        reticle.SetActive(false);
        // Difference so not to teleport into the ground
        Vector3 difference = viveCam.transform.position - viveEye.transform.position;
        difference.y = 1;
        // Translate to the position and stop any velocity
        viveCam.transform.position = hitPoint + difference;
        cont.SimpleMove(Vector3.zero);
    }

    public void MoveCamera(string direction)
    {
        fader.FadeOut();
        Transform tran = transform;

        if (direction == "Right")
        {
            tran = stat.NextSpot();
            viveCam.transform.position = tran.position + new Vector3(0, -1.5f, 0);
            viveCam.transform.rotation = tran.rotation;
        }
        else if (direction == "Left")
        {
            tran = stat.PrevSpot();
            viveCam.transform.position = tran.position + new Vector3(0, -1.5f, 0);
            viveCam.transform.rotation = tran.rotation;
        }
        // User has selected the hotspot
        else if (direction == "HotSpot")
        {
            tran = currentHotspot.transform;
            viveCam.transform.position = tran.position;
            currentHotspot.OpenVRPanel();
        }


        fader.FadeIn();
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

                // If trigger is pressed, open it
                if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) && !hotSpotOpen)
                {
                    currentHotspot.OpenHotSpot();
                    hotSpotOpen = true;
                    MoveCamera("HotSpot");
                }
                else if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) && hotSpotOpen)
                {
                    currentHotspot.CloseHotSpot();
                    currentHotspot = null;
                    hotSpotOpen = false;
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
        else if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) && hotSpotOpen)
        {
            currentHotspot.CloseHotSpot();
            currentHotspot = null;
            hotSpotOpen = false;
        }
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
            if (Physics.Raycast(viveCam.transform.position, viveCam.transform.up, out hit, 40f))
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("CanTeleport"))
                {
                    // Transform to the ground + the height of the player character
                    viveCam.transform.position = hit.point + new Vector3(0, 2, 0);
                }
                else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Building"))
                {

                    Vector3 min = Vector3.positiveInfinity;

                    Vector3 pos = viveCam.transform.position;
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

                    viveCam.transform.position = staticCameras.transform.Find(m.ToString()).transform.position;
                }
            }
        }
    }

}
