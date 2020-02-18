using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleTreasure : MonoBehaviour
{
    public int value;
    public string collectibleType;

    void setValue(int givenValue) {
        this.value = givenValue;
    }
    int getValue() {
        return value;
    }

    public string getType() {
        return collectibleType;
    }
}
