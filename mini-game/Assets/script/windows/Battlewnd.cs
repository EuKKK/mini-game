using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BaseObject;
using sheeps;

public class Battlewnd : window
{
    public GameObject battle_btn_ob;
    public GameObject sheep_prefab;
    public Button menu;
    
    Dictionary<int, GameObject> sheep_prefabs = new Dictionary<int, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        register_btn_click();
    }
    //注册按钮点击监听
    void register_btn_click()
    {
        Button battle_btn = battle_btn_ob.GetComponent<Button>();
        battle_btn.onClick.AddListener(battle_end);
        //menu.onClick.AddListener(active_menu);
    }

    //结束战斗
    void battle_end()
    {
        WindowMgr.Instance.active_window("Result");
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public override void redraw(GameObject window = null)
    {
        if(!window)
            window = this.gameObject;
        base.redraw(window);
        
        //获取所有的羊
        foreach (int id in User.Instance.get_sheeps().Keys)
        {
            draw_sheep(id, User.Instance.get_sheeps()[id]);
        }
    }

    void draw_sheep(int id, sheep u_sheep)
    {
        GameObject new_sheep_ob = GameObject.Instantiate(sheep_prefab);
        new_sheep_ob.name = "sheep" + id.ToString();
        new_sheep_ob.transform.SetParent(this.gameObject.transform.Find("sheeps/Viewport/Content"));
        sheep_prefabs[id] = new_sheep_ob;
        GlobalFuncMgr.set_image(new_sheep_ob, ExcMgr.Instance.get_data("character", u_sheep.class_id, "人物图片"));
    }
    public override void close(GameObject window = null)
    {
        if (!window)
            window = this.gameObject;
        base.close(window);

        foreach (int id in sheep_prefabs.Keys)
        {
            Destroy(sheep_prefabs[id]);
        }
    }
    void active_menu()
    {
        WindowMgr.Instance.active_window("BattleMenu");
    }
}
