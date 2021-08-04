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
    public GameObject stay_btn_ob;
    public GameObject skill_btn_ob;
    public GameObject charac_tab;
    public GameObject end_btn_ob;
    public GameObject round;
    public GameObject gold;
    public Button menu;
    private int clickSetting = 0;

    Dictionary<int, GameObject> sheep_prefabs = new Dictionary<int, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        register_btn_click();

    }
    //注册按钮点击监听
    void register_btn_click()
    {
        //Button battle_btn = battle_btn_ob.GetComponent<Button>();
        Button skill_btn = skill_btn_ob.GetComponent<Button>();
        Button stay_btn = stay_btn_ob.GetComponent<Button>();
        Button end_btn = end_btn_ob.GetComponent<Button>();
        //battle_btn.onClick.AddListener(battle_end);
        skill_btn.onClick.AddListener(skillBtn);
        stay_btn.onClick.AddListener(stayBtn);
        end_btn.onClick.AddListener(endBtn);
        //menu.onClick.AddListener(active_menu);
    }

    //结束战斗
    void battle_end()
    {
        //WindowMgr.Instance.active_window("Result");
    }
    void skillBtn()
    {
        MapMgr.Instance.SkillUsed();

    }
    void stayBtn()
    {
        BattleMgr.Instance.Stay();
    }
    void endBtn()
    {
        BattleMgr.Instance.CampChange();
    }
    void Click1()
    {
        skill_btn_ob.SetActive(true);
        stay_btn_ob.SetActive(true);
        charac_tab.SetActive(true);

        string name = "";
        int[] info = new int[5];
        BattleMgr.Instance.GetInfo(ref name, ref info);
        charac_tab.transform.Find("Name").GetComponent<Text>().text = name;
        charac_tab.transform.Find("hpNumber").GetComponent<Text>().text = info[0].ToString() + "/" + info[1].ToString();
        charac_tab.transform.Find("attackNumber").GetComponent<Text>().text = info[2].ToString();
        charac_tab.transform.Find("attackRangeNumber").GetComponent<Text>().text = info[3].ToString();
        charac_tab.transform.Find("moveRangeNumber").GetComponent<Text>().text = info[4].ToString();

    }
    void Click2()
    {
        skill_btn_ob.SetActive(false);
        stay_btn_ob.SetActive(false);
        charac_tab.SetActive(false);

    }
    void Click3()
    {
        charac_tab.SetActive(true);
        string name = "";
        int[] info = new int[5];
        BattleMgr.Instance.GetInfo(ref name, ref info);
        charac_tab.transform.Find("Name").GetComponent<Text>().text = name;
        charac_tab.transform.Find("hpNumber").GetComponent<Text>().text = info[0].ToString() + "/" + info[1].ToString();
        charac_tab.transform.Find("attackNumber").GetComponent<Text>().text = info[2].ToString();
        charac_tab.transform.Find("attackRangeNumber").GetComponent<Text>().text = info[3].ToString();
        charac_tab.transform.Find("moveRangeNumber").GetComponent<Text>().text = info[4].ToString();

    }
    // Update is called once per frame
    void Update()
    {
        round.transform.Find("roundNumber").GetComponent<Text>().text = "round" + " " + BattleMgr.Instance.GetRound().ToString();
        gold.transform.Find("goldNumber").GetComponent<Text>().text = "gold" + " " + BattleMgr.Instance.GetGold().ToString();

        if (BattleMgr.Instance.click == 1)
        {
            Click1();
        }
        if (BattleMgr.Instance.click == 2)
        {
            Click2();
        }
        if (BattleMgr.Instance.click == 3)
        {

            Click3();
        }

    }

    public override void redraw(GameObject window = null)
    {
        skill_btn_ob.SetActive(false);
        stay_btn_ob.SetActive(false);
        charac_tab.SetActive(false);
        if (!window)
            window = this.gameObject;
        base.redraw(window);

        //获取所有的羊
        foreach (int id in User.Instance.get_sheeps().Keys)
        {
            draw_sheep(id, User.Instance.get_sheeps()[id]);
        }
        Game.Instance.switch_music("battle");
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
