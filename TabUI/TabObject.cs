using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace lLCroweTool.UI.Tab
{   
    public class TabObject : MonoBehaviour, IPointerEnterHandler
    {
        //탭오브젝트 

        //선택버튼
        public Button selectButton;


        public void Init()
        {
            if (selectButton != null)
            {
                selectButton.onClick.AddListener(() =>
                {
                    transform.SetAsLastSibling();



                });
            }
        }




        //맨위로올리기
        public void OnPointerEnter(PointerEventData eventData)
        {
            
        }


    }

}