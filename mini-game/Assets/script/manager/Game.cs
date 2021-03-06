using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sheeps;
using BaseObject;
using UnityEditor;
public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; }
    public AudioClip normal;
    public AudioClip battle;
    public AudioSource back;
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(Instance.gameObject);
    }
    void Start()
    {
        SheepMgr.init_store();
        back.loop = true;
        back.clip = normal;
        back.Play(); //播放背景音乐

        //test_asset();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void player_save()
    {
        PlayerPrefs.SetInt("player_money", User.Instance.money);
        PlayerPrefs.SetInt("player_level", User.Instance.level);
        PlayerPrefs.SetInt("player_character_num", User.Instance.character_num);
        PlayerPrefs.SetInt("player_id", GlobalFuncMgr.sheep_id);
        Dictionary<int,sheep> sheeps = User.Instance.get_sheeps();
        string sheep_ex = "sheep_";
        int cnt = 1;
        foreach(int id  in sheeps.Keys)
        {
            string sheep_json = "";
            string sheep_id = sheep_ex + cnt.ToString();
            cnt = cnt + 1;
            sheep_json = JsonUtility.ToJson(sheeps[id]);
            PlayerPrefs.SetString(sheep_id, sheep_json);
        }
        PlayerPrefs.SetInt("sheep_count", cnt);
        PlayerPrefs.Save();
    }
    public void load_saves()
    {
        User.Instance.money = PlayerPrefs.GetInt("player_money");
        User.Instance.level = PlayerPrefs.GetInt("player_level");
        User.Instance.character_num = PlayerPrefs.GetInt("player_character_num");
        GlobalFuncMgr.sheep_id = PlayerPrefs.GetInt("player_id");
        int cnt = PlayerPrefs.GetInt("sheep_count");
        string sheep_ex = "sheep_";
        string sheep_id = "";
        for(int i = 1;i < cnt; i++)
        {
            sheep_id = sheep_ex + i.ToString();
            sheep u_sheep = JsonUtility.FromJson<sheep>(PlayerPrefs.GetString(sheep_id));
            User.Instance.add_sheep(u_sheep);
        }

    }
    public void switch_music(string name)
    {
        if(name == "battle")
        {
            back.clip = battle;
            back.Play(); //播放背景音乐
        }
        if(name == "normal")
        {
            back.clip = normal;
            back.Play(); //播放背景音乐
        }
    }
 
}
