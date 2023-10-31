using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace lLCroweTool.UI.ShaderCanvas
{
    public class ShaderUICanvas : MonoBehaviour
    {
        //임시
        public Canvas canvas;

        private void Awake()
        {
            //URP쉐이더그래프를 사용할때 


            canvas = GetComponent<Canvas>();
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                var camera = canvas.worldCamera;
                canvas.planeDistance = camera.nearClipPlane;
            }
            
        }


    }
}
