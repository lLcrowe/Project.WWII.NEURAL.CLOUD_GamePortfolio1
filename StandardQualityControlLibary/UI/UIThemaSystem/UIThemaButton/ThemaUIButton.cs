using UnityEngine;
using UnityEngine.Events;

namespace lLCroweTool.UI.UIThema.UIThemaButton
{
    public class ThemaUIButton : MonoBehaviour
    {
        //이미지 처리 할수 있게 버튼템플릭제작
        //해당형식이 많이 보여서 따로 템플릿제작

        //UIThema랑 통합됨//20230831
        public ThemaUI buttonThemaUI;//버튼형        

        public bool isUseIcon;
        public ThemaUI iconThemaUI;//아이콘

        public bool isUseText;
        public ThemaUI textThemaUI;//텍스트

        //구조
        //buttonThemaUI        
        //  iconThemaUI
        //  textThemaUI//텍스트를 후자로 놓는 이유는 랜더링되서 보여줘야되기떄문


        //이름을 좀 단순화시키는것도 괜찮을수도 
        //xxBackGoundImage보다 xx 쓰고 안에다 Sprite를 쓰던지하는게 괜찮아보임
        //예시 
        //xxBackGoundImage => BackGoundImage//현
        //xx => xxSprite //바꿀거

        public void InitIconImageObject(UnityAction action)
        {
            buttonThemaUI.button.onClick.RemoveAllListeners();
            buttonThemaUI.button.onClick.AddListener(action);
        }

        public void SetIconColor(Color color)
        {
            if (iconThemaUI == null)
            {
                return;
            }
            iconThemaUI.iconImageObject.color = color;
        }

        public void SetIconImage(Sprite sprite)
        {
            if (iconThemaUI == null)
            {
                return;
            }
            iconThemaUI.iconImageObject.sprite = sprite;
        }        

        public void SetColorText(Color color)
        {
            if (textThemaUI == null)
            {
                return;
            }
            textThemaUI.textObject.color = color;
        }

        public void SetText(string content)
        {
            if (textThemaUI == null)
            {
                return;
            }
            textThemaUI.textObject.text = content;
        }
    }
}