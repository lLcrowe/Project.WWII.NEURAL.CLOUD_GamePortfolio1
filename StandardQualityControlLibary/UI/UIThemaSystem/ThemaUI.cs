using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace lLCroweTool.UI.UIThema
{
    /// <summary>
    /// UI테마를 타겟팅해주는 ThemaUITarget
    /// </summary>
    public class ThemaUI : MonoBehaviour
    {
        //background => bg => panel

        //구조를 좀더 단순히 하자
        //themaUI
        //  panel1//=>image//양쪽스트래치
        //  panel2//=>image//양쪽스트래치


        //-=기본구조
        //-Panel
        //panelBorderImage//패널이 미지//ThemaUITarget
        //  panelImage//패널이미지//테두리밸류//Canvas

        //-Icon
        //panelBorderImage//ThemaUITarget//테두리이미지
        //  panelImage//패널이미지//테두리밸류//이미지//캔버스
        //      iconImageObject//아이콘이미지

        //-Button
        //panelBorderImage//ThemaUITarget//테두리이미지
        //button//버튼
        //  panelImage//패널이미지//테두리밸류//이미지//캔버스

        //-Text
        //text//텍스트
        
        //테마UI타입
        public ThemaUIType themaUIType;
        public bool isOverrideUIThema;

        //타입에 따른 필요한 오브젝트들
        public Image panelImage;//자기자신
        public Image innerPanelImageObject;//안쪽 패널

        //추가 컴포넌트
        public Button button;//버튼처리//버튼으로 쓸려면 이미지가 같이 있어야됨
        //상호작용 비활성화시에 적용되는 색깔을 가져와서 하단에 있는 모든 오브젝트들을 바꿔줘야할듯함
        //버튼의 비활성화 컬러가 비활성화시에 보이는 컬러
        //거기에 비활성화시 한번래핑시켜서 처리해야할듯
        public Image iconImageObject;//아이콘오브젝트
        public TextMeshProUGUI textObject;//텍스트오브젝트
        private Graphic[] childGraphicArray;//버튼이 비활성화될때를 대응하기 위한 처리

        private void Awake()
        {
            childGraphicArray = GetComponentsInChildren<Graphic>();
            //어덯게했나봣더니 버튼래핑해났구만
        }

        private void OnEnable()
        {   
            ThemaUIManager.Instance.InitTargetThemaUI(this);
        }

        /// <summary>
        /// 매니저에서 전체적으로 테마르 초기화하기 위한 함수
        /// </summary>
        /// <param name="themaInfo">UI테마 정보</param>
        public void InitThemaUI(UIThemaInfo themaInfo)
        {
            if (isOverrideUIThema || themaInfo == null)
            {
                return;
            }

            switch (themaUIType)
            {
                case ThemaUIType.Panel:
                    themaInfo.panelUIThemaPreset.InitImage(panelImage, innerPanelImageObject);
                    break;
                case ThemaUIType.Icon:
                    themaInfo.iconUIThemaPreset.InitImage(panelImage, innerPanelImageObject);                    
                    break;
                case ThemaUIType.Button:
                    themaInfo.buttonUIThemaPreset.InitImage(panelImage, innerPanelImageObject);
                    themaInfo.buttonColorPreset.InitButton(button);
                    themaInfo.buttonSpriteSwapPreset.InitButton(button);
                    break;
                case ThemaUIType.Text:
                    themaInfo.textUIThemaPreset.InitImage(panelImage, innerPanelImageObject);
                    themaInfo.textFontPreset.InitText(textObject);
                    break;
            }
        }

        /// <summary>
        /// 버튼활성화 비활성화 해주는 함수
        /// </summary>
        /// <param name="value">활성화 여부</param>
        public void SetInteractable(bool value)
        {
            //버튼형일시만 작동가능
            if (themaUIType != ThemaUIType.Button)
            {
                return;
            }
            button.interactable = value;

            for (int i = 0; i < childGraphicArray.Length; i++)
            {
                var temp = childGraphicArray[i];
                temp.color = value ? button.colors.normalColor : button.colors.disabledColor;
            }
        }


        //각 컴포넌트를 이어주는 함수들을 만들지 띵킹하기 아직 필요없어보임


    }
}

