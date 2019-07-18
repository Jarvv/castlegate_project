using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseControl : MonoBehaviour
{
    public GameObject hotSpots;
    public GameObject castlegate;
    public GameObject bridge;
    public GameObject walkCam;
    public GameObject staticCams;
    
    private HotSpot currentHotspot = null;
    private bool hotSpotOpen = false;
    private bool viewCastlegate = true;
    private bool mouseLocked;

    void Update()
    {
        HotSpotControl();
    }

    public void MouseLocked(bool locked)
    {
        mouseLocked = locked;
    }

    public void HotSpotControl()
    {
        // Cast a ray from the mouse pointer, if in walk mode the ray will be cast from the
        // centre of the screen.
        RaycastHit hit;
        Ray ray;
        if (!mouseLocked)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        }
        else
        {
            ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        }
        
        if (Physics.Raycast(ray, out hit, 200f) && hit.collider.tag == "HotSpot" && !EventSystem.current.IsPointerOverGameObject())
        {
            
            // Make the current hotspot active
            if (currentHotspot == null && !hotSpotOpen)
            {
                currentHotspot = hit.transform.GetComponentInParent<HotSpot>();
            }
            // If a different hotspot hovered (when cycling through)
            else if(currentHotspot.name != hit.collider.transform.parent.name)
            {
                currentHotspot = hit.transform.GetComponentInParent<HotSpot>();
            }
            // If already active
            else
            {
                // Hover the active HotSpot
                currentHotspot.StartHover();

                // If click is and not on ui, open
                if (Input.GetMouseButtonDown(0) && !hotSpotOpen)
                {
                    // Move the walking camera to the hotspot
                    MoveCamera();
                    OpenInfoPanel();

                }
                // If it is open, close
                else if (Input.GetMouseButtonDown(0) && hotSpotOpen)
                {
                    InfoPanelClose();
                }
            }
        }
        // Else if the current hotspot is not moused over, deactivate it
        else if (currentHotspot != null && !hotSpotOpen)
        {
            currentHotspot.EndHover();
            currentHotspot = null;
        }
        else if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            InfoPanelClose();
        }
    }

    public void ToggleInfoPanel()
    {
        // If using the open button, (from static view) assign the hotspot
        if (currentHotspot == null)
        {
            currentHotspot = hotSpots.transform.Find(Camera.main.name.ToString()).GetComponent<HotSpot>();
            hotSpotOpen = true;
            currentHotspot.HotSpotControl();
        }
        // If the hotspot is already open
        else
        {
            InfoPanelClose();
        }
    }

    public void OpenInfoPanel()
    {
        hotSpotOpen = true;
        currentHotspot.HotSpotControl();
    }

    public void InfoPanelClose()
    {
        // If the current hotspot is open
        if (currentHotspot != null && hotSpotOpen)
        {
            currentHotspot.EndHover();
            currentHotspot.HotSpotControl();
            currentHotspot = null;
            hotSpotOpen = false;
        }   
    }

    public void ToggleCastlegate()
    {
        bridge.SetActive(viewCastlegate);
        viewCastlegate = !viewCastlegate;
        castlegate.SetActive(viewCastlegate);
        // If the castlegate model is off, the bridge needs to be on
        if(castlegate.active)
        { 

            // If the camera is stuck under the castlegate model, teleport on top
            RaycastHit hit;
            if (Physics.Raycast(walkCam.transform.position, walkCam.transform.up, out hit, 40f))
            {
                if(hit.transform.gameObject.layer == LayerMask.NameToLayer("CanTeleport"))
                {
                    // Transform to the ground + the height of the player character
                    walkCam.transform.position = hit.point + new Vector3(0, 2, 0);
                }
                else if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Building"))
                {

                    Vector3 min = Vector3.positiveInfinity;
     
                    Vector3 pos = walkCam.transform.position;
                    int m = 0;

                    // Transfrom to the nearest hotspot
                    for (int i=1; i <= staticCams.transform.childCount; i++)
                    {
                        Vector3 distance = staticCams.transform.Find((i).ToString()).transform.position - pos;

                        if(distance.sqrMagnitude < min.sqrMagnitude)
                        {
                            m = i;
                            min = distance;
                        }
                    }

                    walkCam.transform.position = staticCams.transform.Find(m.ToString()).transform.position;
                }
                
            }

        }
        
    }

    public void ToggleFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    public void MoveCamera()
    {
        // Transform in the direction the walking camera is to the hotspot
        Transform tran = currentHotspot.transform;
        Vector3 dir =  2 * Vector3.Normalize(walkCam.transform.position - tran.position) + tran.position;
        walkCam.transform.position = dir;  

    }


}