using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseObject;
using System.IO;


/*
/////////////////
定义表数据管理类

*/
public class ExcMgr : MonoBehaviour
{
    Dictionary<string, Dictionary<string, Dictionary<string , string >>> assets = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
    public List<holder> lists;

    public static ExcMgr Instance { get; private set; }
    void Awake()
    {
        Instance = this;
        init();
    }
    void init()
    {
        for(int p=0;p<7;p++)
        {
            holder asset = lists[p];
            string name = asset.file_name;
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
            assets[name] = m;
        }  
    }
    public string get_data(string asset_name, string key_name, string val_type)
    {
        var t1 = assets[asset_name];
        if(t1!=null)
        {
            var t2 = t1[key_name];
            if(t2!=null)
            {
                var t3 = t2[val_type];
                if(t3!=null)
                return t3;
            }
            return "";
        }
        return "";
    }

    public string get_array_data(string asset_name, string key_name, string val_type, int num)
    {
        var t1 = assets[asset_name];
        if(t1!=null)
        {
            var t2 = t1[key_name];
            if(t2!=null)
            {
                var t3 = t2[val_type];
                if(t3!=null)
                {   
                    string res = "";
                    int point = 0;
                    for(int i = 0; i < t3.Length;i++)
                    {
                        if(t3[i] != ',')
                            res += t3[i];
                        else{
                            point ++;
                            if(point == num) return res;
                            res = "";
                        }
                    }
                    return res;
                }

            }
            return "";
        }
        return "";
    }

}
