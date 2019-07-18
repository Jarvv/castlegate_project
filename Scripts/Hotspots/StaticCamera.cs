using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine;

public class StaticCamera : MonoBehaviour {

    public GameObject staticCameras;

    private int activeCamera;
    private int cameras;

    void Start () {
        cameras = staticCameras.transform.childCount;
        activeCamera = 1;
	}

    public void Next(){
        DeactivateCamera();

        activeCamera += 1;
        if (activeCamera > cameras)
            activeCamera = 1;

        ActivateCamera();
    }

    public Transform ThisSpot(int hotSpot)
    {
        return staticCameras.transform.GetChild(hotSpot - 1).transform;
    }

    public Transform NextSpot()
    {
        activeCamera += 1;
        if (activeCamera > cameras)
            activeCamera = 1;


        return staticCameras.transform.GetChild(activeCamera - 1).transform;
    }

    public Transform PrevSpot()
    {
        activeCamera -= 1;
        if (activeCamera < 1)
            activeCamera = cameras;


        return staticCameras.transform.GetChild(activeCamera - 1).transform;
    }

    public void Prev(){
        DeactivateCamera();

        activeCamera -= 1;
        if (activeCamera < 1)
            activeCamera = cameras;

        ActivateCamera();
    }

    public void ActivateCamera(){
        staticCameras.transform.GetChild(activeCamera-1).gameObject.SetActive(true);
    }

    void DeactivateCamera(){
        staticCameras.transform.GetChild(activeCamera-1).gameObject.SetActive(false);
    }

    public void DeactivateAll(){
        for(int i = 0; i < cameras; i++){
            staticCameras.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}