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
    string file_path = "Assets/Resources/doc/";

    public static ExcMgr Instance { get; private set; }
    void Start()
    {
        Instance = this;
        init();
    }
    void init()
    {
        DirectoryInfo direction = new DirectoryInfo(file_path);  
        FileInfo[] files = direction.GetFiles("*",SearchOption.AllDirectories);
        for(int p=0;p<files.Length;p++)
        {
            string name = files[p].Name;
            if(name.EndsWith(".asset"))
            {
                name = name.Replace(".asset", "");
                holder asset = Resources.Load("doc/"+name) as holder;
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
    }
    public string get_data(string asset_name, string key_name, string val_type)
    {
        return assets[asset_name][key_name][val_type];
    }

}
