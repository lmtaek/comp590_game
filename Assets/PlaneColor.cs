﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneColor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
