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
        public virtual void redraw(GameObject window = null)
        {
            if(!window)
                window = this.gameObject;

            window.SetActive(true);
        }
        public virtual void destroy(GameObject window = null)
        {
            if(!window)
                window = this.gameObject;

        }
        public virtual void close(GameObject window = null)
        {
            if(!window)
                window = this.gameObject;

            window.SetActive(false);
        }
    }

}
