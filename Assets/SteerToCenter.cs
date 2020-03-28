using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteerToCenter : MonoBehaviour
{
    // Start is called before the first frame update
    //public TrackingSpace vrTrackingOrigin;
    
    public GameObject trackingSpace;
    public Camera myCam;
    Vector3 prevForwardVector;
    float prevYawRelativeToCenter;
    float longestDimensionOfPE = 3f;
    float decelerateThreshold = 0.13f;
    float accelerateThreshold = 0.30f;

    public TextMesh valueCheck;

    void Start() {
        prevForwardVector = myCam.gameObject.transform.forward;
        prevYawRelativeToCenter = angleBetweenVectors(prevForwardVector, trackingSpace.transform.position - myCam.transform.position);
    }
    
    void Update()
    {
        Vector3 currentForwardVector = myCam.gameObject.transform.forward;
        Vector3 currentPosition = myCam.gameObject.transform.position;
        
        float howMuchUserRotated = angleBetweenVectors(prevForwardVector, currentForwardVector);
        float directionUserRotated = (getDirection(currentPosition+prevForwardVector, currentPosition, currentPosition+currentForwardVector)<0)?-1:1;
        
        float deltaYawRelativeToCenter = prevYawRelativeToCenter - angleBetweenVectors(currentForwardVector, trackingSpace.transform.position - currentPosition);
        float distanceFromCenter = myCam.transform.localPosition.magnitude;

        float howMuchToAccelerate = ((deltaYawRelativeToCenter<0)? -decelerateThreshold : accelerateThreshold) * howMuchUserRotated * directionUserRotated * Mathf.Clamp(distanceFromCenter/longestDimensionOfPE/2, 0, 1);
        
        if (Mathf.Abs(howMuchToAccelerate) > 0) {
            trackingSpace.transform.RotateAround(currentPosition, new Vector3(0, 1, 0), howMuchToAccelerate);
        }
        /*
        valueCheck.text = "current forward vector: " + currentForwardVector + "\ncurrent position: " + currentPosition
        + "\nhowMuchUserRotated: " + howMuchUserRotated + "\ndirectionUserRotated: " + directionUserRotated
        + "\ndeltaYawRelativeToCenter: " + deltaYawRelativeToCenter + "\ndistanceFromCenter: " + distanceFromCenter 
        + "\nhowMuchToAccelerate: " + howMuchToAccelerate;*/

        prevForwardVector = currentForwardVector;
        prevYawRelativeToCenter = angleBetweenVectors(currentForwardVector, trackingSpace.transform.position - currentPosition);
    }

    
    float getDirection(Vector3 testPoint, Vector3 sourcePoint, Vector3 destinationPoint) {
        float finalVal = ((testPoint.x - sourcePoint.x) * (destinationPoint.z - sourcePoint.z)) - ((testPoint.z - sourcePoint.z) * (destinationPoint.x - sourcePoint.x));
        return finalVal;
    }

    float angleBetweenVectors(Vector3 testPoint, Vector3 sourcePoint) {
        Vector3 tempTestPoint = new Vector3(testPoint.x, 0, testPoint.z);
        Vector3 tempSourcePoint = new Vector3(sourcePoint.x, 0, sourcePoint.z);

        float dotProduct = Vector3.Dot(tempTestPoint.normalized, tempSourcePoint.normalized);
        dotProduct = Mathf.Clamp(dotProduct, -1, 1);
        float arcCosDotProdRadians = Mathf.Acos(dotProduct);
        return arcCosDotProdRadians * Mathf.Rad2Deg;
    }
    
}
