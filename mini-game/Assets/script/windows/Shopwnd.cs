using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BaseObject;
using sheeps;
using UnityEngine.EventSystems;


public class Shopwnd : window, IPointerEnterHandler
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
   bool need_refresh = true;
   Dictionary<int, GameObject> shop_sheep_map = new Dictionary<int, GameObject>();
   Dictionary<int, sheep> shop_user_sheep_map = new Dictionary<int, sheep>();
   Dictionary<int, string> buttons = new Dictionary<int, string>();
   int [] shop_units = new int[3];
   public List<GameObject> sell_sheeps = new List<GameObject>();

    void Awake()
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
        buy_sheep1.GetComponent<Button>().onClick.AddListener(delegate () { this.buy_sheep_btn(0); });
        buy_sheep2.GetComponent<Button>().onClick.AddListener(delegate () { this.buy_sheep_btn(1); });
        buy_sheep3.GetComponent<Button>().onClick.AddListener(delegate () { this.buy_sheep_btn(2); });
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
        User.Instance.user_level++;
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
        if(shop_units[num] == -1 || shop_units[num] == 0 || User.Instance.sheep_map.Count >= 14) return;
      
        sheep new_sheep = new sheep();
        new_sheep.load_data(shop_units[num].ToString());
        User.Instance.add_sheep(new_sheep);
        redraw();
        SheepMgr.dele_id(shop_units[num]);

        shop_units[num] = -1;
        GlobalFuncMgr.set_image(sell_sheeps[num], "白");
    }
    override public void redraw(GameObject window = null)
    {
        if(!window)
            window = this.gameObject;
        window.SetActive(true);

        if(need_refresh)
        {
            refresh_shop();
            need_refresh = false;
        }

        refresh_user_sheeps();
    }

    void refresh_shop()
    {
        shop_units = SheepMgr.get_random_percent();
        for(int i=0;i<3;i++)
        {
            GlobalFuncMgr.set_image(sell_sheeps[i], ExcMgr.Instance.get_data("character", shop_units[i].ToString(), "人物图片"));
        }        
    }
    void refresh_user_sheeps()
    {
        try_level_up();
        int index = 0;
        foreach(int i in User.Instance.sheep_map.Keys)
        {
            index ++;
            GlobalFuncMgr.set_image(shop_sheep_map[index], ExcMgr.Instance.get_data("character", User.Instance.sheep_map[i].class_id, "人物图片"));
            if(User.Instance.sheep_map[i].star == 2)
                shop_sheep_map[index].transform.Find("star").gameObject.SetActive(true);
            else
                shop_sheep_map[index].transform.Find("star").gameObject.SetActive(false);

            shop_user_sheep_map[index] = User.Instance.sheep_map[i];
        }
        for(int i = index + 1;i<=14;i++)
        {
            GlobalFuncMgr.set_image(shop_sheep_map[i], "白");
            shop_sheep_map[i].transform.Find("star").gameObject.SetActive(false);
            shop_user_sheep_map[i] = null;
        }

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        List<RaycastResult> list = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, list);
        if (list[0].gameObject.tag == "shopcharac")
        {
            string ob_name = list[0].gameObject.name;
            int num = int.Parse(ob_name);
        }
    }

}
