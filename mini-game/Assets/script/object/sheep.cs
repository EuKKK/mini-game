using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sheeps
{
    public class sheep 
    {
        public int hp;
        public int attack;
        public int mana;
        public int move_range;
        public int attack_range;
        public string camp;
        public string unit;
        public int id;
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
        }
    }

}
