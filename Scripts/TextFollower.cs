using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextFollower : MonoBehaviour {

    public MeshRenderer floatingText;

	void Update () {
        Vector3 v = Camera.main.transform.position - transform.position;
        v.x = v.z = 0.0f;
        transform.LookAt(Camera.main.transform.position - v);
        transform.Rotate(0, 180, 0);

        if(Vector3.Distance(Camera.main.transform.position, transform.position) < 5f)
        {
            floatingText.enabled = false;
        }
        else
        {
            floatingText.enabled = true;
        }
    }
}
