using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sheeps;
using BaseObject;
using UnityEditor;
public class SheepMgr 
{
    static Dictionary<int, int> sheep_store = new Dictionary<int, int>();
    static string[] sheep_id = {"1001", "1002", "1003", "1004", "1005", "1006", "1007", "1008", "1009", "1010"};
    static public void init_store()
    {
        foreach(string id in sheep_id)
        {
            sheep_store[int.Parse(id)] = int.Parse(ExcMgr.Instance.get_data("character", id, "库存"));
        }
    }
    static public int[] get_random_percent()
    {
        int[] shop_sheep_id = new int[3];

        int total_num = 0;
        foreach(int id in sheep_store.Keys)
            total_num += sheep_store[id];
        for(int i=0;i<3&&i<total_num;i++)
        {
            int random = Random.Range(0,total_num);
            int index = 1001;
            while(index < 1010 && random - sheep_store[index] > 0)
            {
                index ++;
                if(sheep_store[index]!=0)
                    random -= sheep_store[index];
            }
            shop_sheep_id[i] = index;
        }
        return shop_sheep_id;
    }
    static public void dele_id(int id)
    {
        sheep_store[id]--;
    }
}
