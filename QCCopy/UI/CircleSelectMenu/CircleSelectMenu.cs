using UnityEngine;
using Doozy.Engine.UI;
using Doozy.Engine.Progress;
using Doozy.Engine.Layouts;
using TMPro;

namespace lLCroweTool
{   
    [RequireComponent(typeof(UIView))]
    public class CircleSelectMenu : MonoBehaviour
    {
        //원형이미지를 사용해야됨

        [Header("중앙에 보여줄 텍스트")]
        public TextMeshProUGUI text;
        public RadialLayout radialLayout;
        [Space]
        [Header("각 위치에 존재할 버튼")]
        public UIButton[] uIButtonArray = new UIButton[0];//이벤트를 지정할 버튼들
        [Space]
        private UIView targetUIView;

        //구조
        //CircleSelectMenu//UIView        
        //  RadialLayoutObject//Progress//Text(텍스트위치는 바꿔도됨)
        //      UIButton//사용할버튼
        //      UIButton
        //      UIButton
        //      UIButton
        //      UIButton




        //지정된 아이템의 수많큼
        //메뉴에 띄워줘야됨

        //각도로 체크해주거나
        //숫자에 따른 각도를 체크해줌



        private void Awake()
        {
            targetUIView = GetComponent<UIView>();            
            Progressor progressor = radialLayout.GetComponent<Progressor>();
            progressor.OnValueChanged.AddListener(ProgressorSetValue);

            targetUIView.ShowProgressor = progressor;
            targetUIView.UpdateShowProgressorOnHide = true;
        }
        private void ProgressorSetValue(float value)
        {
            radialLayout.MinAngle = value;
        }

        //테스트용도
        //private void Update()
        //{
        //    for (int i = 0; i < uIButtonArray.Length; i++)
        //    {
        //        float angle = i * (2 * Mathf.PI / uIButtonArray.Length);
        //        Debug.Log(i + "번 :" + angle);
        //    }


            
        //}
    }
}