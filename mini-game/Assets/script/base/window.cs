using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
////////////////////
定义所有UI窗口的基类

*/

namespace BaseObject
{
    public class window : MonoBehaviour
    {
        public void redraw(GameObject window = null)
        {
            if(!window)
                window = this.gameObject;


        }
        public void destroy(GameObject window = null)
        {
            if(!window)
                window = this.gameObject;

        }
    }

}
