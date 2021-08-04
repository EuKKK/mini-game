using UnityEngine;
using ExcelDataReader;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System;
 
namespace BaseObject
{
    /////////////////////
    //存放读表数据
    [Serializable]
    public class holder:ScriptableObject
    {
        public List<HolderData> maps = new List<HolderData>();
        public string file_name = "";
        public void init(Dictionary<string, Dictionary<string, string >> map)
        {
            foreach(string key in map.Keys)
            {
                HolderData hd = new HolderData(key, map[key]);
                maps.Add(hd);
            }
        }
    }

}
