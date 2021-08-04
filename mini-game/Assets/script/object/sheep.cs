using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace sheeps
{
    [Serializable]
    public class sheep
    {
        public string name;
        public int hp;
        public int max_hp;
        public int attack;
        public int mana;
        public int move_range;
        public int attack_range;
        public int cordon;
        public string camp;
        public string unit;
        public int id;
        public bool isUsed = false;
        public bool isSkilled = false;
        public string skill;
        public string charc_level;
        public int star;
        public string img;
        public float hp_index;
        public float attack_index;
        public string class_id;
        public GameObject this_sheep;
        public string skill_id;
        public bool is_user = false;
        public int sell;


        public sheep(bool is_monster = false)
        {
            if (!is_monster)
                id = GlobalFuncMgr.new_sheep_id();
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

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
        public void set_pos(float x, float y)
        {
            if (this_sheep)
            {
                Vector3 pos = new Vector3(x, y, -1);
                this_sheep.transform.position = pos;
            }
        }
        public void load_data(string sheep_id, int new_star = 1)
        {
            star = new_star;
            camp = ExcMgr.Instance.get_data("character", sheep_id, "阵营") == "0" ? "our" : "enemy";
            name = ExcMgr.Instance.get_data("character", sheep_id, "角色名字");
            int.TryParse(ExcMgr.Instance.get_data("character", sheep_id, "hp"), out hp);
            int.TryParse(ExcMgr.Instance.get_data("character", sheep_id, "攻击"), out attack);
            int.TryParse(ExcMgr.Instance.get_data("character", sheep_id, "攻击范围"), out attack_range);
            int.TryParse(ExcMgr.Instance.get_data("character", sheep_id, "移动范围"), out move_range);
            float.TryParse(ExcMgr.Instance.get_data("character", sheep_id, "升星血量提升系数"), out hp_index);
            float.TryParse(ExcMgr.Instance.get_data("character", sheep_id, "升星攻击提升"), out attack_index);
            int.TryParse(ExcMgr.Instance.get_data("character", sheep_id, "警戒范围"), out cordon);
            int.TryParse(ExcMgr.Instance.get_data("character", sheep_id, "售价"), out sell);
            if (star == 2)
            {
                hp = (int)(hp * hp_index);
                attack = (int)(attack * attack_index);
            }
            string new_skill_id = ExcMgr.Instance.get_data("character", sheep_id, "技能id");
            if (new_skill_id != null && new_skill_id != "")
            {
                skill_id = new_skill_id;
                skill = ExcMgr.Instance.get_data("skill", skill_id, "技能名字");
            }
            else
                skill = "attack";
            class_id = sheep_id;

            max_hp = hp;

        }
        public int get_id()
        {
            return id;
        }
        public void set_is_user(bool val)
        {
            is_user = val;
        }
    }

}
