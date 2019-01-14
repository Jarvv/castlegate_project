using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.EventSystems;

public class Movement : MonoBehaviour{

    public GameObject cameraLimits;
    public BuildControl buildControl;
    public MobileMovement mobMovLeft;
    public MobileMovement mobMovRight;
    public float lookSpeedH = 2f;
    public float lookSpeedV = 2f;
    public float zoomSpeed = 2f;
    public float dragSpeed = 6f;
    public bool isStatic = false;

    private float yaw;
    private float pitch;
    private CharacterController cont;
    private Vector3 moveDirection = Vector3.zero;
    private bool mobile;
    
    void Start(){
        cont = transform.gameObject.GetComponent<CharacterController>();
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    void Update(){
        // Desktop
        if (!buildControl.onMobile)
        { 
            //Look around with Left Mouse
            if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                yaw += lookSpeedH * Input.GetAxis("Mouse X");
                pitch -= lookSpeedV * Input.GetAxis("Mouse Y");
                transform.eulerAngles = new Vector3(pitch, yaw, 0f);
            }

            //Zoom in and out with Mouse Wheel
            moveDirection = new Vector3(0, 0, Input.GetAxis("Mouse ScrollWheel") * zoomSpeed);
            moveDirection = transform.TransformDirection(moveDirection);
            cont.Move(moveDirection);

            //Fly with arrows
            if (Input.GetKey(KeyCode.UpArrow))
            {
                moveDirection = new Vector3(0, 0, 0.005f * zoomSpeed);
                moveDirection = transform.TransformDirection(moveDirection);
                cont.Move(moveDirection);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                moveDirection = new Vector3(0, 0, -0.005f * zoomSpeed);
                moveDirection = transform.TransformDirection(moveDirection);
                cont.Move(moveDirection);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                moveDirection = new Vector3(-0.005f * zoomSpeed, 0, 0);
                moveDirection = transform.TransformDirection(moveDirection);
                cont.Move(moveDirection);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                moveDirection = new Vector3(0.005f * zoomSpeed, 0, 0);
                moveDirection = transform.TransformDirection(moveDirection);
                cont.Move(moveDirection);
            }
        }
        // Mobile
        else
        {
            // Movement
            moveDirection = mobMovLeft.Direction();
            // Set y value on canvas equal to z value in world space
            moveDirection.z = moveDirection.y/100;
            moveDirection.x = moveDirection.x / 100;
            moveDirection.y = 0;
            moveDirection = transform.TransformDirection(moveDirection);
            cont.Move(moveDirection);

            // Rotation
            moveDirection = mobMovRight.Direction();
            yaw += moveDirection.x/50;
            pitch -= moveDirection.y/50;
            transform.eulerAngles = new Vector3(pitch, yaw, 0f);
        }
    }     
}

