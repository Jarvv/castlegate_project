using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class StaticMovement : MonoBehaviour{

    public MobileMovement mobMovRight;
    public BuildControl buildControl;
    public Button walkButton;
    public GameObject walkCam;
    public FirstPersonController fpc;

    private float lookSpeedH = 2f;
    private float lookSpeedV = 2f;
    private float yaw;
    private float pitch;

    void Start(){
        yaw = transform.localEulerAngles.y;
        pitch = transform.localEulerAngles.x;
    }

    void Update(){

        // If desktop
        if (!buildControl.onMobile)
        {
            //Look around with Left Mouse
            if (Input.GetMouseButton(0))
            {
                yaw += lookSpeedH * Input.GetAxis("Mouse X");
                pitch -= lookSpeedV * Input.GetAxis("Mouse Y");

                Camera.main.transform.eulerAngles = new Vector3(pitch, yaw, 0f);
            }

            // If the player moves, walk   
            if(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.W))
            {
                Transform tran = this.transform;
                fpc.SetCameraPos(tran);
                walkCam.transform.position = tran.position;
                walkButton.onClick.Invoke();
             
            }
            
        }
        // Mobile
        else
        {
            // Rotation
            Vector3 moveDirection = mobMovRight.Direction();
            yaw += moveDirection.x / 50;
            pitch -= moveDirection.y / 50;
            transform.eulerAngles = new Vector3(pitch, yaw, 0f);
        }
        
    }
}
