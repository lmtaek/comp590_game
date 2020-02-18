using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleCapsule : CollectibleTreasure
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
