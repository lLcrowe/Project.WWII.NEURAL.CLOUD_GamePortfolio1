using lLCroweTool.DataBase;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace lLCroweTool.UnitBatch
{
    public class UnitBatchCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler ,IPointerDownHandler,IPointerUpHandler
    {
        //UI
        //유닛카드
        public UnitObjectInfo targetUnitInfo;

        //이미지 지정할수 있게 처리해주기
        private Image targetImage;//카드이미지
        public Image classImage;//클래스이미지
        public Image charecterImage;//캐릭터이미지
        public TextMeshProUGUI unitNameText;//유닛네임


        private void Awake()
        {
            targetImage = GetComponent<Image>();
            Image[] imageArray = GetComponentsInChildren<Image>();

            for (int i = 0; i < imageArray.Length; i++)
            {
                if (targetImage == imageArray[i])
                {
                    continue;
                }

                imageArray[i].raycastTarget = false;
            }
        }

        /// <summary>
        /// 유닛배치카드 초기화
        /// </summary>
        /// <param name="unitObjectInfo">유닛정보</param>
        public void InitUnitBatchCardUI(UnitObjectInfo unitObjectInfo)
        {
            targetUnitInfo = unitObjectInfo;
            if (targetUnitInfo == null)
            {
                return;
            }

            classImage.sprite = targetUnitInfo.classIcon;
            charecterImage.sprite = targetUnitInfo.icon;
            unitNameText.text = targetUnitInfo.labelNameOrTitle;
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            UnitBatchUIManager.Instance.SetUnitBatchUI(UnitBatchUIManager.UnitBatchStateType.SelectUnitUI, transform, transform.parent, targetUnitInfo);
            targetImage.raycastTarget = false;//감지안되게해서 밑에 카드 부분을 알수 있게
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //마우스포인터를 때면 다시 감지되게 처리
            targetImage.raycastTarget = true;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //마우스포인터를 올렸을때 해당되는 오브젝트를 위로올리기위한 구역
            UnitBatchUIManager.Instance.EnterTheOnCard(transform);
        }


        public void OnPointerExit(PointerEventData eventData)
        {
            //리셋
            UnitBatchUIManager.Instance.EnterTheOnCard(null);
        }

    }
}



