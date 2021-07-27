using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BaseObject;
using sheeps;

public class Battlestandbywnd : window
{
    public GameObject menu;
    public GameObject battle_btn_ob;
    public GameObject sheep_prefab;
    Dictionary<int, sheep> user_sheeps;
    ArrayList sheep_prefabs = new ArrayList();

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
    }

    //开始战斗
    void battle_start()
    {
        WindowMgr.Instance.switch_window("Battle");
    }

    void back()
    {
    }

    public override void redraw(GameObject window = null)
    {
        if(!window)
            window = this.gameObject;
        
        window.SetActive(true);
        FormationMgr.Instance.init_formation();

        //获取所有的羊
        user_sheeps = User.Instance.get_sheeps();
        foreach(int id in user_sheeps.Keys)
        {
            draw_sheep(user_sheeps[id]);
        }

        FormationMgr.Instance.enter_test(user_sheeps);
    }

    public override void close(GameObject window = null)
    {
        if(!window)
            window = this.gameObject;
        base.close(window);

        foreach(GameObject u_sheep in sheep_prefabs)
            Destroy(u_sheep);
    }

    void draw_sheep(sheep u_sheep)
    {
        GameObject new_sheep_ob = GameObject.Instantiate(sheep_prefab);
        new_sheep_ob.transform.SetParent(this.gameObject.transform.Find("sheeps/Viewport/Content"));
        sheep_prefabs.Add(new_sheep_ob);
    }
}
