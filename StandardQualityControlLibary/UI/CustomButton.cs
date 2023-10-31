using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace lLCroweTool.UI.CustomUIButton
{
    public class CustomButton : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
    {
        //버튼에 UI이벤트들을 사용하기 위해 한번 래핑함.
        //이름처리도 집어넣어야됨

        //
        public Button button;

        public delegate void OnPointerHandler();
        public OnPointerHandler onPointerEnter;
        public OnPointerHandler onPointerExit;

        private void Awake()
        {
            button = gameObject.GetAddComponent<Button>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onPointerEnter?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onPointerExit?.Invoke();
        }
    }
}