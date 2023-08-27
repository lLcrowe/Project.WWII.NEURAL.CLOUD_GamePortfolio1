using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace lLCroweTool.UI.SmoothScroll
{
    public class ScrollSmoothMove : MonoBehaviour, IScrollHandler
    {   
        //스크롤 마우스휠로 돌리면 스무스하게 안되는걸 스무스하게 만드는 기능을 가짐
        [Range(5f,10f)]
        public float smoothPower = 5f;
        public float scrollPower = 30f;//이정도가 좋음
        private float curPower;
        private ScrollRect scroll;

        private void Awake()
        {
            scroll = GetComponent<ScrollRect>();
        }
                
        void Update()
        {
            if (curPower < 0.001f && curPower > -0.001f)
            {
                return;
            }            

            //스무스처리
            curPower = Mathf.Lerp(curPower, 0, Time.deltaTime * smoothPower);
            scroll.scrollSensitivity = curPower;
            //최대 최소치 도착하면 힘값을 0으로 만드는거 체크

            SetContentAnchoredPosition(Vector2.one * curPower);
        }

        public void OnScroll(PointerEventData eventData)
        {
            //마우스의 휠돌리기를 받기
            Vector2 delta = eventData.scrollDelta;
            //print(delta);//위 1 아래 -1//Y만씀
            //curPower = delta.y * scroll.scrollSensitivity;
            curPower = delta.y * scrollPower;
        }

        /// <summary>
        /// 스크롤렉트의 앵커포지션위치를 이동시키는 함수
        /// </summary>
        protected void SetContentAnchoredPosition(Vector2 position)
        {
            if (!scroll.horizontal)
                position.x = scroll.content.anchoredPosition.x;
            if (!scroll.vertical)
                position.y = scroll.content.anchoredPosition.y;

            if (position == Vector2.zero)
            {
                return;          
            }
            scroll.content.anchoredPosition += position;
        }

    }
}