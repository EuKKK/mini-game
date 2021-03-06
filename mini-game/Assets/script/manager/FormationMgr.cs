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
    public Dictionary<sheep, int> sheep_position;
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
        sheep_position = new Dictionary<sheep, int>();
        sheep_GO = new Dictionary<GameObject, sheep>();
    }
    public void enter_team(float x, float y, sheep enter_sheep)
    {
        int id = enter_sheep.get_id();
        if (sheep_formation.ContainsKey(id))
            leave_team(id);
        int this_map_charac_count = Mathf.Min(5 + User.Instance.user_level - 1, int.Parse(ExcMgr.Instance.get_data("stage", User.Instance.level.ToString(), "上阵人数")));
        if (sheep_formation.Count >= this_map_charac_count) return;

        int world_pos_x = MapMgr.Instance.GetLocationX(x);
        int world_pos_y = MapMgr.Instance.GetLocationY(y);
        int PosID = MapMgr.Instance.getDic(world_pos_x, world_pos_y);

        enter_sheep.this_sheep = Instantiate(charactor);


        string sheep_class_id = enter_sheep.class_id;
        GlobalFuncMgr.set_model_sprite(enter_sheep.this_sheep.transform.Find("characterTexture").gameObject, ExcMgr.Instance.get_data("character", sheep_class_id, "人物图片"));


        enter_sheep.this_sheep.layer = 9;
        enter_sheep.this_sheep.transform.SetParent(MapMgr.Instance.mapInfo.transform);
        Transform[] father = enter_sheep.this_sheep.GetComponentsInChildren<Transform>();
        foreach (Transform child in father)
            child.gameObject.layer = 9;
        enter_sheep.set_pos(x, y);

        sheep_position[enter_sheep] = PosID;
        sheep_GO[enter_sheep.this_sheep] = enter_sheep;
        sheep_formation[id] = enter_sheep;
    }
    public void leave_team(int id)
    {
        sheep_position.Remove(sheep_formation[id]);
        sheep_formation[id].destroy_self();
        sheep_formation.Remove(id);

    }


}
