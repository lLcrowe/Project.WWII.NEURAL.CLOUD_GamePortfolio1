using lLCroweTool.NoticeDisplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace lLCroweTool.Achievement
{
    public class AchievementNoticeUI : NoticeUI
    {
        //여기는 좀 생각해보기
        //음.. TMP를 이용한 텍스트엔진을 만들어야될거 같은데



        public Image iconImage;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI descriptionText;
        public Image backGroundImage;//어떤 업적이냐에 따라 다른이미지를 위한것

        /// <summary>
        /// 업적 정보UI 초기화
        /// </summary>
        /// <param name="title">제목</param>
        /// <param name="description">해석</param>
        /// <param name="iconImage">아이콘</param>
        /// <param name="backGroundImage">뒷배경</param>
        public void InitAchievementNoticeUI(string title, string description, Sprite icon = null, Sprite backGround = null)
        {
            titleText.text = title;
            descriptionText.text = description;

            iconImage.sprite = icon;
            if (backGround == null)
            {
                return;
            }
            backGroundImage.sprite = backGround;
        }
    }
}