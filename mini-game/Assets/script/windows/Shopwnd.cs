using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BaseObject;
using sheeps;


public class Shopwnd : window
{
   public Text money;
   public GameObject sheep_panel;
   public Button refresh;
   public Button level_up;
   public GameObject user_sheep_panel;
   public Button back;
   public Button next_level;
   public Button sell;
   public Button cancel;
   public Button buy_sheep1;
   public Button buy_sheep2;
   public Button buy_sheep3;
   public bool need_refresh = false;
   Dictionary<int, GameObject> shop_sheep_map = new Dictionary<int, GameObject>();

    void Start()
    {
        register_btn_click();
        initshop();
    }
    //注册按钮点击监听
    void register_btn_click()
    {
        refresh.GetComponent<Button>().onClick.AddListener(refresh_func);
        level_up.GetComponent<Button>().onClick.AddListener(level_up_func);
        back.GetComponent<Button>().onClick.AddListener(back_func);
        next_level.GetComponent<Button>().onClick.AddListener(next_level_func);
        sell.GetComponent<Button>().onClick.AddListener(sell_func);
        cancel.GetComponent<Button>().onClick.AddListener(cancel_func);
        buy_sheep1.GetComponent<Button>().onClick.AddListener(delegate () { this.buy_sheep_btn(1); });
        buy_sheep2.GetComponent<Button>().onClick.AddListener(delegate () { this.buy_sheep_btn(2); });
        buy_sheep3.GetComponent<Button>().onClick.AddListener(delegate () { this.buy_sheep_btn(3); });
    }
    void initshop()
    {
        for(int i = 1; i <= 14; i++)
            shop_sheep_map[i] = this.gameObject.transform.Find("userscroll/" + "sheep" + i.ToString()).gameObject;
    }

    void refresh_func()
    {
        refresh_shop();
    }
    void level_up_func()
    {
        
    }
    void back_func()
    {
        WindowMgr.Instance.switch_window("Mode");
    }

    void next_level_func()
    {
        WindowMgr.Instance.switch_window("Battlestandby");
        MapMgr.Instance.GetMap();
    }
    void sell_func()
    {
        
    }
    void cancel_func()
    {
        
    }
    void buy_sheep_btn(int num)
    {
        sheep new_sheep = new sheep();
        new_sheep.load_data("1201");
        User.Instance.add_sheep(new_sheep);
        redraw();
    }
    override public void redraw(GameObject window = null)
    {
        if(!window)
            window = this.gameObject;
        window.SetActive(true);

        if(need_refresh)
            refresh_shop();

        refresh_user_sheeps();
    }

    void refresh_shop()
    {
        
    }
    void refresh_user_sheeps()
    {
        try_level_up();
        int index = 0;
        foreach(int i in User.Instance.sheep_map.Keys)
        {
            index ++;
            GlobalFuncMgr.set_image(shop_sheep_map[index], ExcMgr.Instance.get_data("character", User.Instance.sheep_map[i].class_id, "人物图片"));
        }
        for(int i = index + 1;i<=14;i++)
            GlobalFuncMgr.set_image(shop_sheep_map[i], "白");

    }
    void try_level_up()
    {
        Dictionary<string, Dictionary<int, sheep>> star_map = new Dictionary<string, Dictionary<int, sheep>>();
        foreach(int i in User.Instance.sheep_map.Keys)
        {
            string class_id = User.Instance.sheep_map[i].class_id;
            Dictionary<int, sheep> sheep_map = new Dictionary<int, sheep>();
            star_map[class_id] = sheep_map;
        }
        
        foreach(int i in User.Instance.sheep_map.Keys)
        {
            if(User.Instance.sheep_map[i].star == 1)
            {
                string class_id = User.Instance.sheep_map[i].class_id;
                star_map[class_id][i] = User.Instance.sheep_map[i];
            }
        }

        foreach(string class_id in star_map.Keys)
        {
            if(star_map[class_id].Count>=3)
            {
                foreach(int id in star_map[class_id].Keys)
                {
                    User.Instance.dele_sheep(star_map[class_id][id]);
                }
                sheep new_sheep = new sheep();
                new_sheep.load_data(class_id, 2);
                User.Instance.add_sheep(new_sheep);
            }
        }
    }

}
