using System.Collections;
using UnityEngine;
using Doozy.Engine.UI;

namespace lLCroweTool.ToolTipSystem
{
    public class ToolTipUITargetButton : ToolTipUITargetImage
    {
        //툴팁 Target이 되는 이미지버튼중에
        //버튼이 있으면 현 컴포넌트를 가져다슴//버튼을


        //총기& 탄알파트 UI
        //툴팁사용가능
        //총기파트 이미지 보여줌//Object를 보여주지않고 총기파트이미지를 보여줌
        //탄알파트 이미지 보여줌//Object를 보여주지않고 총기파트이미지를 보여줌
        //어떠한 파츠가 커스텀총기에 들어갔는지 확인해주는 UI

        private UIButton button;


        protected override void Awake()
        {
            base.Awake();
            button = GetComponent<UIButton>();
        }

        /// <summary>
        /// 툴팁타겟이 될 오브젝트의 버튼을 가져오는 함수
        /// </summary>
        /// <returns>버튼</returns>
        public UIButton GetUIButton()
        {
            return button;
        }

    }
}