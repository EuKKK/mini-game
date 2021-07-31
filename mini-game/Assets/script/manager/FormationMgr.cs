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
    public Dictionary<int, sheep> sheep_position;
    public Dictionary<GameObject, sheep> sheep_GO;
    public GameObject charactor;


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
        sheep_position = new Dictionary<int, sheep>();
        sheep_GO = new Dictionary<GameObject, sheep>();
    }
    public void enter_team(int x, int y, sheep enter_sheep)
    {
        int world_pos_x = MapMgr.Instance.GetLocation(x);
        int world_pos_y = MapMgr.Instance.GetLocation(y);
        int PosID = MapMgr.Instance.getDic(world_pos_x, world_pos_y);

        int id = enter_sheep.get_id();
        if (sheep_formation.ContainsKey(id))
            leave_team(id);
        enter_sheep.this_sheep = Instantiate(charactor);
        enter_sheep.this_sheep.layer = 9;
        enter_sheep.this_sheep.transform.SetParent(MapMgr.Instance.mapInfo.transform);
        Transform[] father = enter_sheep.this_sheep.GetComponentsInChildren<Transform>();
        foreach (Transform child in father)
            child.gameObject.layer = 9;
        enter_sheep.set_pos(x, y);

        sheep_position[PosID] = enter_sheep;
        sheep_GO[enter_sheep.this_sheep] = enter_sheep;
        sheep_formation[id] = enter_sheep;
    }
    public void leave_team(int id)
    {
        sheep_formation[id].destroy_self();
        sheep_formation[id] = null;

    }


}
