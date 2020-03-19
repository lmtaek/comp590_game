using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseRotateView : MonoBehaviour
{
    //public OVRPlayerController player;
    //float speed = player.Acceleration;

    void Update()
    {
        float mouseInput = Input.GetAxis("Mouse X");
        if (mouseInput != null) {
            Vector3 lookHere = new Vector3(0, mouseInput * 5f, 0);
            transform.Rotate(lookHere);
        }
        
    }
}
