using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceController : MonoBehaviour {

    // UI
    public Button leftCycleButton;
    public Button rightCycleButton;
    public Button castlegateOnButton;
    public Button castlegateOffButton;
    public Button flyButton;
    public Button walkButton;
    public GameObject walkingMode;
    public CanvasGroup desktopCanvasGroup;
    public GameObject rmb;

    private Text rmbText;
    private bool walking = false;

    private void Start()
    {
        rmbText = rmb.transform.Find("Text").gameObject.GetComponent<Text>();
    }

    void Update () {
        InterfaceControl();
	}

    private void InterfaceControl()
    {
        // cycle hotspot
        if (Input.GetKeyDown("h"))
        {
            leftCycleButton.onClick.Invoke();
            ModeControl("Tour");
        }
        else if (Input.GetKeyDown("g"))
        {
            rightCycleButton.onClick.Invoke();
            ModeControl("Tour");
        }
        // Toggle castlegate model
        else if (Input.GetKeyDown("c"))
        {
            if (castlegateOnButton.IsActive())
            {
                castlegateOnButton.onClick.Invoke();
            }
            else
            {
                castlegateOffButton.onClick.Invoke();
            }
            
        }
        // Flying/Walking
        else if (Input.GetKeyDown("f"))
        {
            walkButton.onClick.Invoke();
            ModeControl("Flying");
        }
        else if (Input.GetKeyDown("w"))
        {
            flyButton.onClick.Invoke();
            ModeControl("Walking");
        }
    }
    
    public void ModeControl(string mode)
    {
        // What mode the user is currently in
        switch (mode)
        {
            case "Walking":
                walking = true;
                //rmb.SetActive(true);
                break;
            case "Flying":
                WalkingInterfaceControl(false);
                walking = false;
                rmb.SetActive(false);
                break;
            case "Tour":
                WalkingInterfaceControl(false);
                walking = false;
                rmb.SetActive(false);
                break;
        }

    }

    // Called from FirstPersonController with the value of the cursorlock or switching to flying (cursor not locked).
    public void WalkingInterfaceControl(bool locked)
    {
        // If the user is walking, reduce the alpha of the UI and display
        // control to let them know how to unlock cursor.
        if (locked && walking)
        {
            desktopCanvasGroup.alpha = 0.4f;
            rmb.SetActive(true);
            rmbText.text = "Unlock Cursor";

        }
        // The user is not in walking mode
        else if(!locked && !walking)
        {
            desktopCanvasGroup.alpha = 1f;
            rmb.SetActive(false);
        }
        // If the user is in walking mode and unlocked cursor
        else if(!locked && walking)
        {
            desktopCanvasGroup.alpha = 1f;
            rmbText.text = "Lock Cursor";
        }

    }

}
