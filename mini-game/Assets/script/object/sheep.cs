using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace sheeps
{
    [Serializable]
    public class sheep
    {
        public int hp;
        public int attack;
        public int mana;
        public int move_range;
        public int attack_range;
        public int cordon;
        public string camp;
        public string unit;
        public string id;
        public bool isUsed = false;
        public bool isSkilled = false;
        public string skill;
        public string charc_level;
        public int star;
        public string img;
        public string hp_index;
        public string attack_index;
        public string class_id;
        public GameObject this_sheep;


        public sheep(string u_id)
        {
            id = u_id;
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void test()
        {
            hp = 10;
            attack = 1;
            mana = 10;
            move_range = 5;
            attack_range = 5;
            camp = "our";
            unit = "ground";
            skill = "push";
            cordon = 5;
            isUsed = false;
            isSkilled = false;
        }
        public string get_camp()
        {
            return camp;
        }
        public void destroy_self()
        {
            if (this_sheep)
            {
                UnityEngine.Object.Destroy(this_sheep);
            }
        }
        public void set_pos(int x, int y)
        {
            if (this_sheep)
            {
                Vector3 pos = new Vector3(x, y, -1);
                this_sheep.transform.position = pos;
            }
        }
        public void load_data(string sheep_id)
        {
            int.TryParse(ExcMgr.Instance.get_data("character", sheep_id, "星级"), out star);
            camp = ExcMgr.Instance.get_data("character", sheep_id, "阵营") == "0" ?"our":"enemy";
            int.TryParse(ExcMgr.Instance.get_data("character", sheep_id, "hp"), out hp);
            int.TryParse(ExcMgr.Instance.get_data("character", sheep_id, "攻击"), out attack);
            int.TryParse(ExcMgr.Instance.get_data("character", sheep_id, "攻击范围"), out attack_range);
            int.TryParse(ExcMgr.Instance.get_data("character", sheep_id, "移动范围"), out move_range);
            skill = "push";
            cordon = 5;
            class_id = sheep_id;
        }
        public string get_id()
        {
            return id;
        }
    }

}
