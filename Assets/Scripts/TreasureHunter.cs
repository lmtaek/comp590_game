using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

//equivalent to EAttachmentRule in UE4
public enum AttachmentRule{ KeepRelative, KeepWorld, SnapToTarget}

public class TreasureHunter : MonoBehaviour
{

//Variables to reference for user's movements, positioning, etc.
    public OVRCameraRig oVRCameraRig;
    public OVRManager oVRManager;
    public OVRHeadsetEmulator oVRHeadsetEmulator;
     public Camera myCam;
     Vector3 waistPosition; 

//Variables to reference for user's controllers and interactible objects
    public GameObject leftPointerObject;
    public GameObject rightPointerObject;
     Vector3 previousPointerPos;
     CollectibleTreasure thingIGrabbed;

//Inventory utilities
     public TreasureHunterInventory inventory;
     public int totalScore;
     int numberOfItemsCollected = 0;

//Text fields
     public TextMesh scoreText;
     public TextMesh centerPoint;
     public LayerMask collectiblesMask;

    // Start is called before the first frame update
    void Start() {
        oVRCameraRig = this.gameObject.GetComponent<OVRCameraRig>();
        oVRManager = this.gameObject.GetComponent<OVRManager>();
        oVRHeadsetEmulator = this.gameObject.GetComponent<OVRHeadsetEmulator>();
    }

    // Update is called once per frame
    void Update() {
        
        previousPointerPos=rightPointerObject.gameObject.transform.position;
        waistPosition = myCam.gameObject.transform.position - new Vector3(0, 1, 0);

        if (Input.GetKeyDown("j")) {
            Debug.Log("Pressed 'J' key.");
            Vector3 originPosition = myCam.gameObject.transform.position;
            Vector3 rayDirection = myCam.gameObject.transform.forward;
            RaycastHit hit;
            if (Physics.Raycast(originPosition, rayDirection, out hit)) {
                GameObject hitObject = hit.collider.gameObject;
                CollectibleTreasure objectComponent = hitObject.GetComponent<CollectibleTreasure>();
                string objectName = objectComponent.getType();
                //string assetPath = "Assets/Collectibles/" + objectName + ".prefab";
                //CollectibleTreasure prefab = (CollectibleTreasure)AssetDatabase.LoadAssetAtPath(assetPath, typeof(CollectibleTreasure));
                CollectibleTreasure prefab = (CollectibleTreasure)Resources.Load(objectName, typeof(CollectibleTreasure));
                if (!prefab){
                    Debug.Log("Prefab is null.");
                }
                addToInventory(prefab);
                Destroy(hitObject);
                }
        } else if (OVRInput.GetDown(OVRInput.RawButton.RHandTrigger)) {
            //centerPoint.text = "GRABBED ITEM!";

            Collider[] overlappingThings = Physics.OverlapSphere(rightPointerObject.transform.position, 0.1f, collectiblesMask);
            if (overlappingThings.Length > 0) {
                //centerPoint.text = "object is: " + overlappingThings[0].gameObject;
                attachGameObjectToAChildGameObject(overlappingThings[0].gameObject, rightPointerObject, AttachmentRule.KeepWorld, AttachmentRule.KeepWorld, AttachmentRule.KeepWorld, true);
                //I'm not bothering to check for nullity because layer mask should ensure I only collect collectibles.
                thingIGrabbed = overlappingThings[0].gameObject.GetComponent<CollectibleTreasure>();
            }

        } else if (OVRInput.GetUp(OVRInput.RawButton.RHandTrigger)) {
            //centerPoint.text = "DROPPED ITEM!";
            letGo();
        }
    }

    bool canPutInInventory(CollectibleTreasure item) {
        Collider[] overlappingWithWaistThings = Physics.OverlapSphere(waistPosition, 0.2f, collectiblesMask);
        if (overlappingWithWaistThings.Length > 0) {
            if (overlappingWithWaistThings[0].gameObject.GetComponent<CollectibleTreasure>() == thingIGrabbed) {
                return true;
            }
        }
        return false;

    }

    void addToInventory(CollectibleTreasure item) {
        if (inventory.collectibles.ContainsKey(item)) {
            inventory.collectibles[item] = inventory.collectibles[item] + 1;
        } else {
            inventory.collectibles.Add(item, 1);
        }
        calculateScore();
    }

    void calculateScore() {
        int currentScore = 0;
        int currentNumberOfItems = 0;
        foreach(KeyValuePair<CollectibleTreasure, int> collectible in inventory.collectibles) {
            currentScore += (collectible.Key.value * collectible.Value);
            currentNumberOfItems += collectible.Value;
        }
        totalScore = currentScore;
        numberOfItemsCollected = currentNumberOfItems;
        string scoreTextUpdate = "COLLECTIBLE NAME | # OF COLLECTIBLE | VALUE\n";
        foreach(KeyValuePair<CollectibleTreasure, int> collectible in inventory.collectibles) {
            scoreTextUpdate += collectible.Key + " | " + collectible.Value + " | " + collectible.Key.value + "\n";
        }
        scoreTextUpdate += "TOTAL SCORE: " + totalScore;
        scoreText.text = scoreTextUpdate;
    }

    public void attachGameObjectToAChildGameObject(GameObject GOToAttach, GameObject newParent, AttachmentRule locationRule, AttachmentRule rotationRule, AttachmentRule scaleRule, bool weld){
        GOToAttach.transform.parent=newParent.transform;
        handleAttachmentRules(GOToAttach,locationRule,rotationRule,scaleRule);
        if (weld){
            simulatePhysics(GOToAttach,Vector3.zero,false);
        }
    }

public static void handleAttachmentRules(GameObject GOToHandle, AttachmentRule locationRule, AttachmentRule rotationRule, AttachmentRule scaleRule){
        GOToHandle.transform.localPosition=
        (locationRule==AttachmentRule.KeepRelative)?GOToHandle.transform.position:
        //technically don't need to change anything but I wanted to compress into ternary
        (locationRule==AttachmentRule.KeepWorld)?GOToHandle.transform.localPosition:
        new Vector3(0,0,0);

        //localRotation in Unity is actually a Quaternion, so we need to specifically ask for Euler angles
        GOToHandle.transform.localEulerAngles=
        (rotationRule==AttachmentRule.KeepRelative)?GOToHandle.transform.eulerAngles:
        //technically don't need to change anything but I wanted to compress into ternary
        (rotationRule==AttachmentRule.KeepWorld)?GOToHandle.transform.localEulerAngles:
        new Vector3(0,0,0);

        GOToHandle.transform.localScale=
        (scaleRule==AttachmentRule.KeepRelative)?GOToHandle.transform.lossyScale:
        //technically don't need to change anything but I wanted to compress into ternary
        (scaleRule==AttachmentRule.KeepWorld)?GOToHandle.transform.localScale:
        new Vector3(1,1,1);
    }
    public void simulatePhysics(GameObject target,Vector3 oldParentVelocity,bool simulate){
        Rigidbody rb = target.GetComponent<Rigidbody>();
        if (rb){
            if (!simulate){
                Destroy(rb);
            } 
        } else{
            if (simulate){
                //there's actually a problem here relative to the UE4 version since Unity doesn't have this simple "simulate physics" option
                //The object will NOT preserve momentum when you throw it like in UE4.
                //need to set its velocity itself.... even if you switch the kinematic/gravity settings around instead of deleting/adding rb
                Rigidbody newRB=target.AddComponent<Rigidbody>();
                newRB.velocity=oldParentVelocity;
            }
        }
    }
    void letGo() {
        detachGameObject(thingIGrabbed.gameObject, AttachmentRule.KeepWorld, AttachmentRule.KeepWorld, AttachmentRule.KeepWorld);
        simulatePhysics(thingIGrabbed.gameObject,(rightPointerObject.gameObject.transform.position-previousPointerPos)/Time.deltaTime,true);

        if (canPutInInventory(thingIGrabbed)) {
                string objectName = thingIGrabbed.getType();
                CollectibleTreasure prefab = (CollectibleTreasure)Resources.Load(objectName, typeof(CollectibleTreasure));
                if (!prefab){
                    Debug.Log("Prefab is null.");
                }
                addToInventory(prefab);
                //centerPoint.text = "Added item";
                CollectibleTreasure temporaryThingIGrabbed = thingIGrabbed;
                thingIGrabbed = null;
                Destroy(temporaryThingIGrabbed.gameObject);

            }

        thingIGrabbed=null;
    }

    public static void detachGameObject(GameObject GOToDetach, AttachmentRule locationRule, AttachmentRule rotationRule, AttachmentRule scaleRule){
        //making the parent null sets its parent to the world origin (meaning relative & global transforms become the same)
        GOToDetach.transform.parent=null;
        handleAttachmentRules(GOToDetach,locationRule,rotationRule,scaleRule);
    }
}
