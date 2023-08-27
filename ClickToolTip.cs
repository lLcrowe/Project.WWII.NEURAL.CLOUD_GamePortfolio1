using System.Collections;
using UnityEngine;

namespace Assets.A0
{
    public class ClickToolTip : MonoBehaviour
    {

        //자료를 체크해야됨
        

        public enum ClickToolTipType
        {
            Minimum,//
            Window, //스트링만 있음
            ItemToolTip//


        }


        public void ShowToolTip(ClickToolTipType clickToolTipType)
        {
            switch (clickToolTipType)
            {
                case ClickToolTipType.Minimum:
                    break;
                case ClickToolTipType.Window:
                    break;
                case ClickToolTipType.ItemToolTip:
                    break;
            }
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}