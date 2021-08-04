using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sheeps;
using UnityEngine.EventSystems;
public class MapMgr : MonoBehaviour
{
    public const int MAX_NUMBER = 30;
    public int[,] MapInfo;
    public int[,] GamePlayer;
    public Dictionary<int, GameObject> landformPos;
    public Dictionary<int, GameObject> enemyPos;
    public Dictionary<int, GameObject> characterPos;
    public Dictionary<GameObject, sheep> enemy_GO = new Dictionary<GameObject, sheep>();
    List<GameObject> highLightS = new List<GameObject>();
    public GameObject mapInfo;
    public GameObject map;
    public GameObject landform;
    public GameObject highLightSheep;
    public GameObject ex;
    public int locationX;
    public int locationY;
    public int maxX;
    public int maxY;
    public float InitX;
    public float InitY;
    private float rotX;
    private float rotY;
    private float rotZ;

    public static MapMgr Instance { get; private set; }
    public Camera BattleCamera;


    private bool isMouseDown;
    private Vector3 lastMousePosition;
    public bool Playing = false;
    public bool isWin = false;
    public bool isLose = false;
    List<float[]> play_pos;
    public int skill;

    GameObject target;
    bool first_third = true;

    void Start()
    {
        Instance = this;

    }

    public void GetMap()
    {

        //初始化地图数据
        map_init();
        isWin = false;
        isLose = false;
        InitX = mapInfo.transform.position.x;
        InitY = mapInfo.transform.position.y;
        rotX = ex.transform.eulerAngles.x;
        rotY = ex.transform.eulerAngles.y;
        rotZ = ex.transform.eulerAngles.z;
        highLightSheep = (GameObject)Resources.Load("Prefab/HighLightSheep");
        for (int i = 0; i < 35; i++)
        {
            for (int j = 0; j < 35; j++)
            {
                MapInfo[i, j] = 0;
                GamePlayer[i, j] = 0;
            }
        }
        for (int i = 0; i < map.transform.childCount; i++)
        {
            GameObject g = map.transform.GetChild(i).gameObject;
            int x = GetLocationX(g.transform.position.x);
            int y = GetLocationY(g.transform.position.y);
            if (x > maxX) maxX = x;
            if (y > maxY) maxY = y;
            MapInfo[x, y] = 1;
        }
        enemyPos.Clear();
        for (int i = 0; i < landform.transform.childCount; i++)
        {
            GameObject g = landform.transform.GetChild(i).gameObject;
            string tag = g.tag;
            int x = GetLocationX(g.transform.position.x);
            int y = GetLocationY(g.transform.position.y);

            if (MapInfo[x, y] == 1)
            {
                switch (tag)
                {

                    case "character":
                        float[] pos = new float[2];
                        pos[0] = GetPositionX(x);
                        pos[1] = GetPositionY(y);
                        play_pos.Add(pos);
                        Destroy(g);
                        GameObject HS = Instantiate(highLightSheep, new Vector3(pos[0], pos[1], -0.5f), new Quaternion(0, 0, 0, 0));
                        HS.transform.eulerAngles = new Vector3(rotX, rotY, rotZ);
                        HS.layer = 9;
                        highLightS.Add(HS);
                        HS.transform.SetParent(mapInfo.transform);

                        break;

                    case "enemy":
                        GamePlayer[x, y] = -1;
                        g.transform.position = g.transform.position + new Vector3(0, 0, -0.9f);
                        enemyPos.Add(getDic(x, y), g);

                        break;
                    case "barrier":
                        MapInfo[x, y] = 900;
                        landformPos.Add(getDic(x, y), g);
                        break;
                    case "jungle":
                        MapInfo[x, y] = 2;
                        landformPos.Add(getDic(x, y), g);
                        break;
                    case "water":
                        MapInfo[x, y] = 9999;
                        landformPos.Add(getDic(x, y), g);
                        break;
                    case "trap":
                        MapInfo[x, y] = 500;
                        landformPos.Add(getDic(x, y), g);
                        break;
                }
            }
        }

        int t = 0;
        //敌人gameobject修改和enemyGo添加
        for (int j = 0; j < 31; j++)
        {
            for (int i = 0; i < 31; i++)
            {
                if (enemyPos.ContainsKey(getDic(i, j)))
                {
                    t++;
                    GameObject enemy_ob = enemyPos[getDic(i, j)];
                    sheep enemy_sheep = new sheep(true);
                    string class_id = ExcMgr.Instance.get_array_data("position", User.Instance.level.ToString(), "魔物id", t);
                    GameObject sprite_ob = enemy_ob.transform.Find("test1").gameObject;
                    GlobalFuncMgr.set_model_sprite(sprite_ob, ExcMgr.Instance.get_data("character", class_id, "人物图片"));
                    sprite_ob.transform.localScale = new Vector3(0.67f, 0.9f, 1);
                    enemy_sheep.load_data(class_id);
                    enemy_GO[enemy_ob] = enemy_sheep;

                }
            }
        }

        //特殊处理第一关和第二关的不选人
        if (User.Instance.level == 6001)
        {
            for (int i = 1; i <= 3; i++)
            {
                FormationMgr.Instance.enter_team(play_pos[i - 1][0], play_pos[i - 1][1], User.Instance.get_sheep_by_id(i));
            }
            GetSheep();
            WindowMgr.Instance.switch_window("Battle");
            StoryMgr.Instance.start_story(1);
            BattleMgr.Instance.GameStart();
        }
        if (User.Instance.level == 6002)
        {
            //增加跳关容错
            if (User.Instance.sheep_map.Count < 4)
            {
                sheep new_sheep = new sheep();
                new_sheep.load_data("1008");
                User.Instance.sheep_map[4] = new_sheep;
            }
            for (int i = 1; i <= 4; i++)
            {
                FormationMgr.Instance.enter_team(play_pos[i - 1][0], play_pos[i - 1][1], User.Instance.get_sheep_by_id(i));
            }
            GetSheep();
            WindowMgr.Instance.switch_window("Battle");
            StoryMgr.Instance.start_story(3);
            BattleMgr.Instance.GameStart();
        }
        if (User.Instance.level == 6003 && first_third)
        {
            first_third = false;
            StoryMgr.Instance.start_story(5);
        }

    }

    public void GetSheep()
    {
        foreach (KeyValuePair<sheep, int> this_sheep in FormationMgr.Instance.sheep_position)
        {
            int x = DicX(this_sheep.Value);
            int y = DicY(this_sheep.Value);

            GamePlayer[x, y] = 1;
            characterPos.Add(this_sheep.Value, this_sheep.Key.this_sheep);

        }
        Playing = true;
        foreach (GameObject g in highLightS)
        {
            Destroy(g);
        }
        highLightS.Clear();
    }

    void map_init()
    {

        string map_name = ExcMgr.Instance.get_data("stage", User.Instance.level.ToString(), "地图");

        mapInfo = Instantiate((GameObject)Resources.Load("MapPrefab/" + map_name));
        //mapInfo = Instantiate((GameObject)Resources.Load("MapPrefab/" + "ceshi1"));
        Transform[] father = mapInfo.GetComponentsInChildren<Transform>();
        foreach (Transform child in father)
            child.gameObject.layer = 9;

        map = mapInfo.transform.Find("Map(Clone)").gameObject;
        landform = mapInfo.transform.Find("Landform(Clone)").gameObject;
        landformPos = new Dictionary<int, GameObject>();
        enemyPos = new Dictionary<int, GameObject>();
        characterPos = new Dictionary<int, GameObject>();

        maxX = 0;
        maxY = 0;
        //monster_num = 1;

        MapInfo = new int[35, 35];
        GamePlayer = new int[35, 35];
        play_pos = new List<float[]>();
        User.Instance.reduct_hp();
    }

    public int GetLocationX(float pos)
    {
        float mathX = pos + InitX - mapInfo.transform.position.x;
        return ((int)(mathX) - 20) / 40 + 1;
    }
    public int GetLocationY(float pos)
    {
        float mathY = pos + InitY - mapInfo.transform.position.y;
        return ((int)(mathY) - 20) / 40 + 1;
    }
    public float GetPositionX(int loc)
    {
        return (loc - 1) * 40 + 20 - InitX + mapInfo.transform.position.x;
    }
    public float GetPositionY(int loc)
    {
        return (loc - 1) * 40 + 20 - InitY + mapInfo.transform.position.y;
    }
    public int getDic(int x, int y)
    {
        return y * MAX_NUMBER + x;
    }
    public int DicY(int dicNumber)
    {
        return (dicNumber - 1) / MAX_NUMBER;
    }
    public int DicX(int dicNumber)
    {
        return dicNumber - DicY(dicNumber) * MAX_NUMBER;
    }


    void Update()
    {
        if (Playing)
        {
            OnMouseMove();
        }
        if (!GetComponent<BattleMgr>().ScreenLock)
        {
            ScreenMove();
        }
    }
    private void OnMouseMove()
    {

        if (Input.GetMouseButtonDown(0))
        {

            if (EventSystem.current.IsPointerOverGameObject())
            {

            }
            else
            {
                Ray ray = BattleCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit rh;
                bool hit = Physics.Raycast(ray, out rh);

                if (hit)
                {
                    target = rh.collider.gameObject;

                    locationX = GetLocationX(target.transform.position.x);
                    locationY = GetLocationY(target.transform.position.y);

                    /*
                    Debug.Log(target.transform.position.x + "," + target.transform.position.y);
                    Debug.Log(InitX);
                    Debug.Log(mapInfo.transform.position.x);
                    Debug.Log(locationX + "," + locationY);

                    */
                    BattleMgr.Instance.CenterManager(locationX, locationY, ref target, skill);

                    skill = 0;
                }
            }
        }
    }
    public void SkillUsed()
    {
        skill = 1;
        BattleMgr.Instance.CenterManager(locationX, locationY, ref target, skill);
    }

    public int[] GetChracPos(Vector3 pos)
    {
        int[] map_pos = null;
        Ray ray = BattleCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit rh;
        bool hit = Physics.Raycast(ray, out rh);
        if (hit)
        {
            map_pos = new int[2];
            GameObject target = rh.collider.gameObject;

            if (target.layer != 9) return null;
            if (!target.name.Contains("HighLightSheep")) return null;

            locationX = GetLocationX(target.transform.position.x);
            locationY = GetLocationY(target.transform.position.y);
            map_pos[0] = locationX;
            map_pos[1] = locationY;
        }
        return map_pos;
    }


    private void ScreenMove()
    {
        if (Input.GetMouseButtonDown(2))
        {
            isMouseDown = true;
        }
        if (Input.GetMouseButtonUp(2))
        {
            isMouseDown = false;
            lastMousePosition = Vector3.zero;
        }
        if (isMouseDown)
        {
            if (lastMousePosition != Vector3.zero)
            {
                Vector3 offset = BattleCamera.ScreenToWorldPoint(Input.mousePosition) - lastMousePosition;
                mapInfo.transform.position += offset;
            }
            lastMousePosition = BattleCamera.ScreenToWorldPoint(Input.mousePosition);
        }
    }
    public void leave_battle()
    {
        if (mapInfo)
            Destroy(mapInfo);

        mapInfo = null;
        GamePlayer = null;
        landformPos = null;
        enemyPos = null;
        characterPos = null;
        map = null;
    }
}
