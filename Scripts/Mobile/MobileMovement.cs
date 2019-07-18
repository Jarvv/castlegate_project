using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MobileMovement : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{

    public int MovementRange = 100;
    private Vector3 startPos;
    private Vector3 movement;

    // Left or Right
    public string control;

    private void Start()
    {
        startPos = transform.position;
        movement = startPos;
    }

    // When the user releases the joystick, return it to its inital position.
    public void OnPointerUp(PointerEventData data)
    {
        transform.position = startPos;
        movement = startPos;
    }

    public void OnPointerDown(PointerEventData data) {

    }

    public void OnDrag(PointerEventData data)
    {
        Vector3 newPos = Vector3.zero;

        // The change in x on the canvas
        int deltaX = (int)(data.position.x - startPos.x);
        deltaX = Mathf.Clamp(deltaX, -MovementRange, MovementRange);
        newPos.x = deltaX;
      
        // The change in y on the canvas
        int deltaY = (int)(data.position.y - startPos.y);
        deltaY = Mathf.Clamp(deltaY, -MovementRange, MovementRange);
        newPos.y = deltaY;

        // Update the movement vector and assign to the transform.
        movement = new Vector3(startPos.x + newPos.x, startPos.y + newPos.y, startPos.z + newPos.z);
        transform.position = movement;
    }

    public Vector3 Direction()
    { 
        // Return the current direction the joystick is currently in
        return movement - startPos;
    }
}
