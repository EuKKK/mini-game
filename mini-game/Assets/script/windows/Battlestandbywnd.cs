using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BaseObject;
using sheeps;
using UnityEngine.EventSystems;

public class Battlestandbywnd : window, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public GameObject menu;
    public GameObject battle_btn_ob;
    public GameObject sheep_prefab;
    Dictionary<int, sheep> user_sheeps;
    GameObject in_drag_ob = null;
    Vector3 pre_pos;
    ArrayList sheep_prefabs = new ArrayList();
    public Camera main_camera;
    public Button left;
    public Button right;
    int left_num = 0;

    // Start is called before the first frame update
    void Start()
    {
        register_btn_click();
    }
    //注册按钮点击监听
    void register_btn_click()
    {
        Button battle_btn = battle_btn_ob.GetComponent<Button>();

        battle_btn.onClick.AddListener(battle_start);
        left.onClick.AddListener(left_btn);
        right.onClick.AddListener(right_btn);
    }

    //开始战斗
    void battle_start()
    {
        MapMgr.Instance.GetSheep();
        WindowMgr.Instance.switch_window("Battle");
        BattleMgr.Instance.GameStart();
    }

    void back()
    {
    }

    public override void redraw(GameObject window = null)
    {
        if (!window)
            window = this.gameObject;

        window.SetActive(true);
        FormationMgr.Instance.init_formation();

        redraw_sheeps();

        //FormationMgr.Instance.enter_test(user_sheeps);
    }
    void redraw_sheeps()
    {
        foreach (GameObject u_sheep in sheep_prefabs)
            Destroy(u_sheep);
        //获取所有的羊
        user_sheeps = User.Instance.get_sheeps();
        int now_sheep_cnt = 0;
        foreach (int id in user_sheeps.Keys)
        {
            if (left_num > 0) left_num--;
            else
            {
                if (now_sheep_cnt < 4)
                {
                    now_sheep_cnt++;
                    draw_sheep(id.ToString(), user_sheeps[id]);
                }
            }
        }
    }

    public override void close(GameObject window = null)
    {
        if (!window)
            window = this.gameObject;
        base.close(window);

        foreach (GameObject u_sheep in sheep_prefabs)
            Destroy(u_sheep);
    }

    void draw_sheep(string name, sheep u_sheep)
    {
        GameObject new_sheep_ob = GameObject.Instantiate(sheep_prefab);
        new_sheep_ob.name = "sheep" + name;
        new_sheep_ob.transform.SetParent(this.gameObject.transform.Find("characterscroll/Viewport/Content"));
        sheep_prefabs.Add(new_sheep_ob);
        GlobalFuncMgr.set_image(new_sheep_ob, ExcMgr.Instance.get_data("character", u_sheep.class_id, "人物图片"));
    }

    void Update()
    {

    }
    public void OnDrag(PointerEventData eventData)
    {
        if (in_drag_ob)
            in_drag_ob.transform.position = eventData.position;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        in_drag_ob = null;
        pre_pos = new Vector3(0, 0, 0);
        List<RaycastResult> list = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, list);
        if (list[0].gameObject.name.Contains("sheep"))
        {
            in_drag_ob = list[0].gameObject;
            pre_pos = in_drag_ob.transform.position;
        }

    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (in_drag_ob)
        {
            in_drag_ob.transform.position = pre_pos;
            int[] pos = MapMgr.Instance.GetChracPos(eventData.position);

            if (pos != null)
            {
                float x = MapMgr.Instance.GetPositionX(pos[0]);
                float y = MapMgr.Instance.GetPositionY(pos[1]);
                string sheep_id = in_drag_ob.name.Replace("sheep", "");
                FormationMgr.Instance.enter_team(x, y, User.Instance.get_sheep_by_id(int.Parse(sheep_id)));
            }
        }
    }
    public void left_btn()
    {
        if (left_num - 4 > 0) left_num -= 4;
        redraw_sheeps();
    }
    public void right_btn()
    {
        if (left_num + 4 < User.Instance.sheep_map.Count) left_num += 4;
        redraw_sheeps();
    }
}
