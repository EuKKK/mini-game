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
    public Dictionary<int, sheep> sheep_formation;
    

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {

    }
    
    public void init_formation()
    {
        sheep_formation = new Dictionary<int, sheep>();
    }
    public void enter_team(int id, sheep enter_sheep)
    {
        if(sheep_formation.ContainsKey(id))
            leave_team(id);
        sheep_formation[id] = enter_sheep;
    }
    public void leave_team(int id)
    {
        sheep_formation[id] = null;
    }

    public void enter_test(Dictionary<int, sheep> map)
    {
        foreach(int id in map.Keys)
        {
            enter_team(id, map[id]);
        }
    }

}
