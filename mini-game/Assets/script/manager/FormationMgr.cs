using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseObject;
using System.IO;
using sheeps;


/*
/////////////////
定义阵容管理器

*/
public class FormationMgr : MonoBehaviour
{

    public static FormationMgr Instance { get; private set; }
    public Dictionary<string, sheep> user_sheeps;
    

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {

    }
    
    public void init_formation()
    {
        user_sheeps = User.Instance.get_sheeps();

    }
}
