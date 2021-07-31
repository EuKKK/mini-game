using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sheeps;
using BaseObject;
using UnityEditor;
public class SheepMgr : MonoBehaviour
{
    public static SheepMgr Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }
    
 
}
