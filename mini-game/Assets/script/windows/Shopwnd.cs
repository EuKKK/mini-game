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

    void Start()
    {
        register_btn_click();
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
        buy_sheep1.GetComponent<Button>().onClick.AddListener(buy_sheep_btn_1);
        buy_sheep2.GetComponent<Button>().onClick.AddListener(buy_sheep_btn_2);
        buy_sheep3.GetComponent<Button>().onClick.AddListener(buy_sheep_btn_3);
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
        
    }

    void next_level_func()
    {
        
    }
    void sell_func()
    {
        
    }
    void cancel_func()
    {
        
    }
    void buy_sheep_btn_1()
    {
        sheep sheep1 = new sheep();
        sheep1.load_data("1201");
        User.Instance.add_sheep(sheep1);
    }
    void buy_sheep_btn_2()
    {
        sheep sheep2 = new sheep();
        sheep2.load_data("1005");
        User.Instance.add_sheep(sheep2);
    }
    void buy_sheep_btn_3()
    {
        sheep sheep3 = new sheep();
        sheep3.load_data("1007");   
        User.Instance.add_sheep(sheep3);
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

    }

}
