using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteerToCenter : MonoBehaviour
{
    // Start is called before the first frame update
    //public TrackingSpace vrTrackingOrigin;
    
    public GameObject trackingSpace;
    public Camera myCam;
    Vector3 A;
    Vector3 B;
    Vector3 C; 
    Vector3 prevForwardVector;
    float prevYawRelativeToCenter;
    void Start() {
        prevForwardVector = myCam.gameObject.transform.forward;
        prevYawRelativeToCenter = angleBetweenVectors(prevForwardVector, trackingSpace.transform.position - myCam.transform.position);
    }
    
    void Update()
    {
        Vector3 currentForwardVector = myCam.gameObject.transform.forward;
        
        float howMuchUserRotated = angleBetweenVectors(prevForwardVector, currentForwardVector);
        //float directionUserRotated = getDirection();
        
        prevForwardVector = myCam.gameObject.transform.forward;
    }

    
    float getDirection(Vector3 testPoint, Vector3 sourcePoint, Vector3 destinationPoint) {
        float finalVal = ((testPoint.x - sourcePoint.x) * (destinationPoint.y - sourcePoint.y)) - ((testPoint.y - sourcePoint.y) * (destinationPoint.x - sourcePoint.x));
        return finalVal;
    }

    float angleBetweenVectors(Vector3 testPoint, Vector3 sourcePoint) {
        float dotProduct = Vector3.Dot(testPoint.normalized, sourcePoint.normalized);
        dotProduct = Mathf.Clamp(dotProduct, -1, 1);
        return Mathf.Acos(dotProduct);
    }
    
}
