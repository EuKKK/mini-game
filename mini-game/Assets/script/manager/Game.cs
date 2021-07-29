using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sheeps;
using BaseObject;
using UnityEditor;
public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; }
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(Instance.gameObject);
    }
    void Start()
    {
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
        int cnt = PlayerPrefs.GetInt("sheep_count");
        string sheep_ex = "sheep_";
        string sheep_id = "";
        for(int i = 1;i < cnt; i++)
        {
            sheep_id = sheep_ex + i.ToString();
            sheep u_sheep = JsonUtility.FromJson<sheep>(PlayerPrefs.GetString(sheep_id));
            User.Instance.add_sheep(i, u_sheep);
        }

    }
    public void test_asset()
    {
        holder asset = Resources.Load("doc/test") as holder;
        List<HolderData> map = asset.maps;
        Dictionary<string, Dictionary<string , string >> m =new Dictionary<string, Dictionary<string, string>>();
        for (int i = 0; i< map.Count; i++)
        {
            string m_id = map[i].out_map.key;
            Dictionary<string, string > m_map = new Dictionary<string, string>(); 
            for(int k = 0; k < map[i].out_map.imaps.Count; k++)
                m_map[map[i].out_map.imaps[k].key] = map[i].out_map.imaps[k].val;
            m[m_id] = m_map;
        }
        foreach(string id in m.Keys)
        {
            foreach(string id_2 in m[id].Keys)
            {
                Debug.Log(m[id][id_2]);
            }
        }
    }
}
