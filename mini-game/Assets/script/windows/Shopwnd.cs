using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BaseObject;
using sheeps;
using UnityEngine.EventSystems;


public class Shopwnd : window, IPointerDownHandler
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
   public sheep last_down_sheep;
   bool need_refresh = true;
   Dictionary<int, GameObject> shop_sheep_map = new Dictionary<int, GameObject>();
   Dictionary<int, sheep> shop_user_sheep_map = new Dictionary<int, sheep>();
   Dictionary<int, string> buttons = new Dictionary<int, string>();
   int [] shop_units = new int[3];
   public List<GameObject> sell_sheeps = new List<GameObject>();
   public List<GameObject> sheep_infos = new List<GameObject>();
   bool first_enter = true;

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
        if(User.Instance.money>=3)
        {
            User.Instance.money-=3;
            refresh_shop();
            money.text = "金币"+User.Instance.money.ToString();
        }
    }
    void level_up_func()
    {
        if(User.Instance.money>=10)
        {
            User.Instance.money-=10;
            User.Instance.user_level++;
            money.text = "金币"+User.Instance.money.ToString();
        }
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
        if(last_down_sheep!=null)
        {
            User.Instance.dele_sheep(last_down_sheep);
            refresh_user_sheeps();
            User.Instance.money += last_down_sheep.sell;
            last_down_sheep = null;
        }
    }
    void cancel_func()
    {
        
    }
    void buy_sheep_btn(int num)
    {
        if(shop_units[num] == -1 || shop_units[num] == 0 || User.Instance.sheep_map.Count >= 14) return;
        int sell_money = int.Parse(ExcMgr.Instance.get_data("character", shop_units[num].ToString(), "费用"));
        if(User.Instance.money>=sell_money)
        {
            User.Instance.money -= sell_money;
            sheep new_sheep = new sheep();
            new_sheep.load_data(shop_units[num].ToString());
            User.Instance.add_sheep(new_sheep);
            redraw();
            SheepMgr.dele_id(shop_units[num]);

            shop_units[num] = -1;
            GlobalFuncMgr.set_image(sell_sheeps[num], "白");
        }
        
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
        if(first_enter)
        {
            first_enter = false;
            StartCoroutine(start_story_async());
        }

        refresh_user_sheeps();
        money.text = "金币"+User.Instance.money.ToString();
    }

    IEnumerator start_story_async()
    {
        yield return null;
        StoryMgr.Instance.start_story(4);
    }

    void refresh_shop()
    {
        shop_units = SheepMgr.get_random_percent();
        for(int i=0;i<3;i++)
        {
            if(shop_units[i]!=0)
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
            shop_user_sheep_map.Remove(i);
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
    void Update()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;
        List<RaycastResult> list = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, list);
        if (list.Count>0&&list[0].gameObject.tag == "shopcharac")
        {
            string ob_name = list[0].gameObject.name;
            ob_name = ob_name.Replace("sheep", "");
            int num = int.Parse(ob_name);
            if(num > User.Instance.sheep_map.Count) return ;
            sheep sheep_info = shop_user_sheep_map[num];
            sheep_infos[0].GetComponent<Text>().text = ExcMgr.Instance.get_data("character", sheep_info.class_id, "角色名字");
            sheep_infos[1].GetComponent<Text>().text = "生命值:" + ExcMgr.Instance.get_data("character", sheep_info.class_id, "hp");
            sheep_infos[2].GetComponent<Text>().text = "攻击范围:" + ExcMgr.Instance.get_data("character", sheep_info.class_id, "攻击范围");
            sheep_infos[3].GetComponent<Text>().text = "移动范围:" + ExcMgr.Instance.get_data("character", sheep_info.class_id, "移动范围");
            sheep_infos[5].GetComponent<Text>().text = "售价:" + ExcMgr.Instance.get_data("character", sheep_info.class_id, "售价");
            if (sheep_info.skill == "attack")
                sheep_infos[4].GetComponent<Text>().text = "攻击:" + ExcMgr.Instance.get_data("character", sheep_info.class_id, "攻击");
            else
                sheep_infos[4].GetComponent<Text>().text = "技能效果:" + ExcMgr.Instance.get_data("skill", sheep_info.skill_id, "技能效果");

        }
        if (list.Count>0&&list[0].gameObject.tag == "sellcharac")
        {
            string ob_name = list[0].gameObject.name;
            ob_name = ob_name.Replace("sheep", "");
            int num = int.Parse(ob_name) - 1;
            if(shop_units[num] == -1 || shop_units[num] == 0 ) return;
            string class_id = shop_units[num].ToString();
            sheep_infos[0].GetComponent<Text>().text = ExcMgr.Instance.get_data("character", class_id, "角色名字");
            sheep_infos[1].GetComponent<Text>().text = "生命值:" + ExcMgr.Instance.get_data("character", class_id, "hp");
            sheep_infos[2].GetComponent<Text>().text = "攻击范围:" + ExcMgr.Instance.get_data("character", class_id, "攻击范围");
            sheep_infos[3].GetComponent<Text>().text = "移动范围:" + ExcMgr.Instance.get_data("character", class_id, "移动范围");
            sheep_infos[5].GetComponent<Text>().text = "售价:" + ExcMgr.Instance.get_data("character", class_id, "费用");
            string skill = ExcMgr.Instance.get_data("character", class_id, "技能id");
            if (skill != null && skill != "")
                sheep_infos[4].GetComponent<Text>().text = "技能效果:" + ExcMgr.Instance.get_data("skill", skill, "技能效果");
            else
                sheep_infos[4].GetComponent<Text>().text =  "攻击:" + ExcMgr.Instance.get_data("character", class_id, "攻击");

        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        List<RaycastResult> list = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, list);
        last_down_sheep = null;
        if (list.Count>0&&list[0].gameObject.tag == "shopcharac")
        {
            string ob_name = list[0].gameObject.name;
            ob_name = ob_name.Replace("sheep", "");
            int num = int.Parse(ob_name);
            if(num > User.Instance.sheep_map.Count) return ;
            last_down_sheep = shop_user_sheep_map[num];
        }
    }
    

}
