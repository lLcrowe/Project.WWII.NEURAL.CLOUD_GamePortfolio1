using UnityEngine;
using UnityEngine.EventSystems;

namespace lLCroweTool.UI.DragMoveUIModule
{
    public class DragMoveWindowUI : MonoBehaviour, IDragHandler, IPointerDownHandler
    {
        //https://docs.unity3d.com/2019.1/Documentation/ScriptReference/EventSystems.IPointerClickHandler.html
        //참고하기

        //현컴포넌트는 창을 핸들러해주는 기능을 가짐
        //창을 핸들러했을때 움직여줄 객체를 지정해주면 이동됨

        [Header("창을 핸들러했을때 움직이는 타겟")]
        public RectTransform dragMoveUITarget;//무조건 필요//레이캐스트On

        private Canvas parnetCanvas;
        private void Awake()
        {
            parnetCanvas = GetComponentInParent<Canvas>();

            if (dragMoveUITarget == null)
            {
                dragMoveUITarget = gameObject.GetComponent<RectTransform>();
            }
        }

        /// <summary>
        /// 순서 전면으로 이동
        /// </summary>
        /// <param name="tr">트랜스폼</param>
        private static void SortUIFirst(Transform tr)
        {
            tr.SetAsLastSibling();
        }
      
        public void OnDrag(PointerEventData eventData)
        {
            dragMoveUITarget.anchoredPosition = dragMoveUITarget.anchoredPosition += eventData.delta / parnetCanvas.scaleFactor;
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            //필요업을듯 체크하기
            //솔팅교체
            SortUIFirst(dragMoveUITarget);
            OnDrag(eventData);
        }
    }
}
