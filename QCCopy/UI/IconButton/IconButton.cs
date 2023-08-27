using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace lLCroweTool.UI.IconImage
{
    public class IconButton : MonoBehaviour
    {
        //이미지 처리 할수 있게 버튼템플릭제작
        //해당형식이 많이 보여서 따로 템플릿제작

        
        public Button button;
        public Image buttonImage;

        //자식으로 붙어있음
        //이름처리대기
        public Image iconImage;
        public Image iconBackGroundImage;
        public TextMeshProUGUI text;


        //button,image
        //  text
        //--Type1
        //  iconBackGoundImage
        //      icon
        //---------OR--------
        //--Type2
        //  icon

        //이름을 좀 단순화시키는것도 괜찮을수도 
        //xxBackGoundImage보다 xx 쓰고 안에다 Sprite를 쓰던지하는게 괜찮아보임
        //예시 
        //xxBackGoundImage => BackGoundImage//현
        //xx => xxSprite //바꿀거


        public enum IconButtonElementType
        {
            Icon,
            IconBackGround,
            Button,
        }

        private void Awake()
        {   
            button = GetComponent<Button>();
            buttonImage = GetComponent<Image>();
            var tempArray = GetComponentsInChildren<Image>();
            for (int i = 0; i < tempArray.Length; i++)
            {
                Debug.Log(tempArray[i]);
            }
        }

        public void InitIconImageObject(UnityAction action)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(action);
        }

        public void SetColor(Color color, IconButtonElementType iconButtonElementType)
        {
            switch (iconButtonElementType)
            {
                case IconButtonElementType.Icon:
                    if (iconImage == null)
                    {
                        return;
                    }
                    iconImage.color = color;
                    break;
                case IconButtonElementType.IconBackGround:
                    if (iconBackGroundImage == null)
                    {
                        return;
                    }
                    iconBackGroundImage.color = color;
                    break;
                case IconButtonElementType.Button:
                    if (buttonImage == null)
                    {
                        return;
                    }
                    buttonImage.color = color;
                    break;
            }
        }

        public void SetImage(Sprite sprite, IconButtonElementType iconButtonElementType)
        {
            switch (iconButtonElementType)
            {
                case IconButtonElementType.Icon:
                    if (iconImage == null)
                    {
                        return;
                    }
                    iconImage.sprite = sprite;
                    break;
                case IconButtonElementType.IconBackGround:
                    if (iconBackGroundImage == null)
                    {
                        return;
                    }
                    iconBackGroundImage.sprite = sprite;
                    break;
                case IconButtonElementType.Button:
                    if (buttonImage == null)
                    {
                        return;
                    }
                    buttonImage.sprite = sprite;
                    break;
            }
        }        

        public void SetColorText(Color color)
        {
            if (text == null)
            {
                return;
            }
            text.color = color;
        }

        public void SetText(string content)
        {
            text.text = content;
        }
    }
}