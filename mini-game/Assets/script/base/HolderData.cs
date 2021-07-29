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
    public class HolderData
    {
        [Serializable]
        public struct OutMap
        {
            public string key;
            public List<InsideMap> imaps;
        }
        [Serializable]
        public struct InsideMap
        {
            public string key;
            public string val;
            public InsideMap(string k, string v)
            {
                key = k;
                val = v;
            }
        }
        public OutMap out_map;
        public HolderData(string key, Dictionary<string , string > map)
        {
            out_map = new OutMap();
            out_map.imaps = new List<InsideMap>();
            out_map.key = key;
            foreach(string id in map.Keys)
            {
                InsideMap in_side_map = new InsideMap(id, map[id]);
                out_map.imaps.Add(in_side_map);
            }
        }
    }

}
