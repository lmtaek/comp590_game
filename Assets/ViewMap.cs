using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewMap : MonoBehaviour
{

    public GameObject map;
    public bool isMapOpen;

    void Start() {
        map.gameObject.active = true;
        isMapOpen = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("k")) {
            switch (isMapOpen) {
                case true:
                    map.gameObject.active = false;
                    isMapOpen = false;
                    Debug.Log("Map display closed.");
                    break;
                case false:
                    map.gameObject.active = true;
                    isMapOpen = true;
                    Debug.Log("Map display opened.");
                    break;
            }
        }
        
    }
}
