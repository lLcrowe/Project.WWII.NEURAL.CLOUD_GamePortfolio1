using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace lLCroweTool.UI.UIThema
{
    /// <summary>
    /// UI테마에 대한 정보
    /// </summary>
    [CreateAssetMenu(fileName = "New UIThemaInfo", menuName = "lLcroweTool/New UIThemaInfo")]
    public class UIThemaInfo : LabelBase
    {
        //UI들의 테마를 정하기 위한 처리
        //여기는 이미지에 대한 내용만 담아둘예정
        //CSV처리시 네이밍을 일치화해서 처리예정4
        //량이 너무많은데
        //테마들만 따로 CSV 만들고
        //나머지는 단일 CSV내에 처리하자

        //통짜가 좋다//통짜로 짜자
        //어찌할까//임포트타입을 정해주자

        //-=설정
        //기본프리셋
        public UIThemaPreset panelUIThemaPreset = new();

        //아이콘프리셋//별개의오브젝트로 존재        
        public UIThemaPreset iconUIThemaPreset = new();
        //public IconPresetInfo iconPresetInfo;//대상아이콘셋//따로 처리됨

        //버튼프리셋
        public UIThemaPreset buttonUIThemaPreset = new();
        public ButtonColorPreset buttonColorPreset = new();
        public ButtonSpriteSwapPreset buttonSpriteSwapPreset = new();

        //텍스트프리셋//별개의오브젝트로 존재
        public UIThemaPreset textUIThemaPreset = new();
        public FontPreset textFontPreset = new();

        public override LabelBaseDataType GetLabelDataType()
        {
            return LabelBaseDataType.Nothing;
        }
    }

    /// <summary>
    /// UI테마를 적용시킬 이미지UI타입
    /// </summary>
    public enum ThemaUIType
    {
        Panel,
        Icon,
        Button,
        Text,
    }

    /// <summary>
    /// UI테마 프리셋
    /// </summary>
    [System.Serializable]
    public class UIThemaPreset
    {
        //테두리//뒷배경//처리방법을 띵킹 -1~0으로 처리//0이 디폴트
        //버튼 처리에 문제가 있으므로 방식 변경//20230904

        //패널
        public SpritePreset panelSpritePreset = new();

        //안쪽패널
        public BorderPreset innerPanelBorderPreset = new();
        public SpritePreset innerPanelSpritePreset = new();

        /// <summary>
        /// UI테마 프리셋에 맞게 초기화 하는 함수
        /// </summary>
        /// <param name="panelImage">패널이미지</param>
        /// <param name="innerPanelImage">안쪽패널이미지</param>
        public void InitImage(Image panelImage, Image innerPanelImage)
        {
            panelSpritePreset.InitImage(panelImage);

            innerPanelBorderPreset.InitImage(innerPanelImage);
            innerPanelSpritePreset.InitImage(innerPanelImage);
        }

        public void SetImageData(Image panelImage, Image innerPanelImage)
        {
            panelSpritePreset.SetImageData(panelImage);

            innerPanelBorderPreset.SetImageData(innerPanelImage);
            innerPanelSpritePreset.SetImageData(innerPanelImage);
        }
    }

    /// <summary>
    /// 스프라이트프리셋
    /// </summary>
    [System.Serializable]
    public class SpritePreset
    {
        //하나의 이미지는 하나의 컬러, 스프라이트, 메터리얼로 작동
        //메터리얼은 없으면 기존거 쓰고 있을때 메터리얼은 쉐이더처리가 따로 되있지만
        //메인텍스쳐를 받아와서 처리할수 있게 변경할것
        public Color color = Color.white;
        public Sprite sprite;
        public Material uiMaterial;//없으면 기본으로 작동되게
        public bool isUseRaycastTarget = true;

        /// <summary>
        /// 스프라이트 프리셋에 맞게 초기화하는 함수
        /// </summary>
        /// <param name="image">이미지</param>
        public void InitImage(Image image)
        {
            image.color = color;
            image.enabled = sprite == null ? false : true;
            image.sprite = sprite;
            image.material = uiMaterial == null ? Canvas.GetDefaultCanvasMaterial() : uiMaterial;//UIDefault 메터리얼
            image.raycastTarget = isUseRaycastTarget;
            //Graphic.defaultGraphicMaterial;
        }

        public void SetImageData(Image image)
        {
            color = image.color;
            sprite = image.sprite;
            uiMaterial = image.material;
            isUseRaycastTarget = image.raycastTarget;
        }
    }

    /// <summary>
    /// 버튼의 컬러 변경프리셋
    /// </summary>
    [System.Serializable]
    public class ButtonColorPreset
    {
        //버튼 컬러기본값으로        
        public Color highLightColor = lLcroweUtil.GetColor256(246, 246, 246, 256);
        public Color pressedColor = lLcroweUtil.GetColor256(201, 201, 201, 256);
        public Color selectedColor = lLcroweUtil.GetColor256(246, 246, 246, 256);
        public Color disabledColor = lLcroweUtil.GetColor256(201, 201, 201, 128);

        //public Color highLightColor = lLcroweUtil.GetHSVAColor(0, 0, 95, 100);
        //public Color pressedColor = lLcroweUtil.GetHSVAColor(0, 0, 78, 100);
        //public Color selectedColor = lLcroweUtil.GetHSVAColor(0, 0, 95, 100);
        //public Color disabledColor = lLcroweUtil.GetHSVAColor(0, 0, 78, 50);
        [Range(1, 5)]
        public float colorMultiplier = 1f;
        public float fadeDuration = 0.1f;

        /// <summary>
        /// 버튼 컬러프리셋에 맞게 초기화하는 함수
        /// </summary>
        /// <param name="button">버튼</param>
        public void InitButton(UnityEngine.UI.Button button)
        {
            ColorBlock colorBlock = new ColorBlock();
            colorBlock.normalColor = Color.white;
            colorBlock.highlightedColor = highLightColor;
            colorBlock.pressedColor = pressedColor;
            colorBlock.selectedColor = selectedColor;
            colorBlock.disabledColor = disabledColor;
            colorBlock.colorMultiplier = colorMultiplier;
            colorBlock.fadeDuration = fadeDuration;

            button.colors = colorBlock;
        }

        public void SetButtonData(UnityEngine.UI.Button button)
        {
            var colorBlock = button.colors;
            highLightColor = colorBlock.highlightedColor;
            pressedColor = colorBlock.pressedColor; 
            selectedColor = colorBlock.selectedColor;
            disabledColor = colorBlock.disabledColor;
            colorMultiplier = colorBlock.colorMultiplier;
            fadeDuration = colorBlock.fadeDuration;
        }
    }   


    /// <summary>
    /// 버튼의 스프라이트 변경프리셋
    /// </summary>
    [System.Serializable]
    public class ButtonSpriteSwapPreset
    {
        public Sprite highLightSprite;
        public Sprite pressedSprite;
        public Sprite selectedSprite;
        public Sprite disabledSprite;

        /// <summary>
        /// 버튼스프라이트 변경프리셋에 맞게 초기화하는 함수
        /// </summary>
        /// <param name="button">버튼</param>
        public void InitButton(UnityEngine.UI.Button button)
        {
            SpriteState spriteState = new SpriteState();
            spriteState.highlightedSprite = highLightSprite;
            spriteState.pressedSprite = pressedSprite;
            spriteState.selectedSprite = selectedSprite;
            spriteState.disabledSprite = disabledSprite;

            button.spriteState = spriteState;
        }

        public void SetButtonData(UnityEngine.UI.Button button)
        {
            SpriteState spriteState = button.spriteState;
            highLightSprite = spriteState.highlightedSprite;
            pressedSprite = spriteState.pressedSprite;
            selectedSprite = spriteState.selectedSprite;
            disabledSprite = spriteState.disabledSprite;
        }
    }

    /// <summary>
    /// 테두리 프리셋
    /// </summary>
    [System.Serializable]
    public class BorderPreset
    {
        //각방향 처리
        public float borderTopValue;
        public float borderBottomValue;
        public float borderLeftValue;
        public float borderRightValue;

        /// <summary>
        /// 테두리프리셋에 맞게 이미지를 초기화하는 함수
        /// </summary>
        /// <param name="image">이미지컴포넌트</param>
        public void InitImage(Image panel)
        {
            //if (!panel.TryGetComponent(out Canvas canvas))
            //{
            //     canvas = panel.GetAddComponent<Canvas>();
            //    //그래픽스가 감마가 아니고 리니어면
            //    canvas.vertexColorAlwaysGammaSpace = true;//해서 경고문 해제//나중에 자세히보기
            //}
            //canvas.overrideSorting = isUseBorderImage;
            var rect = panel.rectTransform;
            rect.SetAnchorPreset(lLcroweUtil.RectAnchorPreset.StretchBoth);

            //Left,Bottom
            rect.offsetMin = new Vector2(borderLeftValue, borderBottomValue);
            //Right,Top//최대 높이와 우측에서 빼줘야됨
            rect.offsetMax = -new Vector2(borderRightValue, borderTopValue);
        }

        public void SetImageData(Image panel)
        {
            var rect = panel.rectTransform;
            borderRightValue = rect.offsetMax.x;
            borderTopValue = rect.offsetMax.y;
            borderLeftValue = rect.offsetMin.x;
            borderBottomValue = rect.offsetMin.y;
        }



        public enum BorderDrawType
        {
            All,//상하좌우
            TopBottom,//상하
            LeftRight,//좌우
        }
    }


    /// <summary>
    /// 폰트프리셋
    /// </summary>
    [System.Serializable]
    public class FontPreset
    {
        public TMP_FontAsset fontAsset;
        public Color fontColor = Color.black;

        /// <summary>
        /// 폰트프리셋에 맞게 텍스트를 초기화하는 함수
        /// </summary>
        /// <param name="text">텍스트</param>
        public void InitText(TextMeshProUGUI text)
        {
            text.rectTransform.SetAnchorPreset(lLcroweUtil.RectAnchorPreset.StretchBoth);
            text.font = fontAsset == null ? TMP_Settings.defaultFontAsset : fontAsset;
            text.color = fontColor;
        }

        public void SetFontData(TextMeshProUGUI text)
        {
            fontAsset = text.font;
            fontColor = text.color;
        }
    }

    
}