using lLCroweTool.DataBase;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace lLCroweTool.UI.PullCard
{
    public class PullCardUI : MonoBehaviour
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
        public void InitPullCardUI(UnitObjectInfo unitObjectInfo)
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
    }
}