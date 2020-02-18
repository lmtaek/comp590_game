using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif
public class TreasureHunterInventory : MonoBehaviour
{

    [Serializable] //Using SerializableDictionary from the AssetStore: https://assetstore.unity.com/packages/tools/integration/serializabledictionary-90477
    public class CollectibleIntegerDictionary : SerializableDictionary<CollectibleTreasure, int>{};
    public CollectibleIntegerDictionary collectibles;

    void Start(){
        CollectibleIntegerDictionary collectibles = new CollectibleIntegerDictionary();
    }
}
