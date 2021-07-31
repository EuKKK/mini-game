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
    public GameObject charactor ;
    

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
    public void enter_team(int x, int y, sheep enter_sheep)
    {
        int world_pos_x = MapMgr.Instance.GetPosition(x);
        int world_pos_y = MapMgr.Instance.GetPosition(y);

        int id = enter_sheep.get_id();
        if(sheep_formation.ContainsKey(id))
            leave_team(id);
        enter_sheep.this_sheep = Instantiate(charactor);
        enter_sheep.set_pos(world_pos_x, world_pos_y);
        sheep_formation[id] = enter_sheep;
    }
    public void leave_team(int id)
    {
        sheep_formation[id].destroy_self();
        sheep_formation[id] = null;
        
    }

    // public void enter_test(Dictionary<int, sheep> map)
    // {
    //     foreach(int id in map.Keys)
    //     {
    //         enter_team(id, map[id]);
    //     }
    // }

}
