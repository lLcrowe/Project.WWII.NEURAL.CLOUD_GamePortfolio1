using lLCroweTool.UI.Bar;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(UIBar_Slide))]
    public class UIBar_SlideInspectorEditor : UIBarInspectorEditor
    {
        private UIBar_Slide targetUIBar_Slide;

        protected override void InitAddFunc()
        {
            base.InitAddFunc();
            targetUIBar_Slide = (UIBar_Slide)target;
        }

        protected override bool CheckAutoGenerate(ref string content)
        {
            bool isCheck = base.CheckAutoGenerate(ref content);

            content += "-=UIBar_Slide 필요사항=-\n";

            if (targetUIBar_Slide.slider == null)
            {
                content += "슬라이더가 없습니다.";
                isCheck = true;
            }

            return isCheck;
        }

        protected override void AutoGenerateSection()
        {
            base.AutoGenerateSection();
            targetUIBar_Slide.gameObject.name = "UIBar_Slide";

            AutoGenerateUIBarSlide(targetUIBar_Slide);
        }

        private static void AutoGenerateUIBarSlide(UIBar_Slide uIBar_Slide)
        {
            if (uIBar_Slide.slider == null)
            {
                uIBar_Slide.slider = uIBar_Slide.gameObject.AddComponent<Slider>();
                Slider slider = uIBar_Slide.slider;

                slider.interactable = false;
                slider.transition = Selectable.Transition.None;
                //uIBar_Slide.slider.navigation.mode = Navigation.Mode.None;
                Navigation temp = new Navigation();
                temp.mode = Navigation.Mode.None;
                slider.navigation = temp;
                slider.fillRect = (RectTransform)uIBar_Slide.fill.transform;
            }
        }

        protected override void DisplaySection()
        {
            base.DisplaySection();

            if (targetUIBar_Slide.iconHandle == null)
            {
                if (GUILayout.Button("아이콘 핸들 생성버튼"))
                {
                    GameObject gameObject = new GameObject();
                    RectTransform parentRect =  gameObject.AddComponent<RectTransform>();
                    parentRect.SetParent(targetUIBar_Slide.transform);
                    parentRect.position = Vector3.zero;
                    gameObject.name = "iconParent";

                    gameObject = new GameObject();
                    RectTransform rect = gameObject.AddComponent<RectTransform>();
                    rect.SetParent(parentRect);
                    rect.position = Vector3.zero;
                    gameObject.name = "iconImage";

                    targetUIBar_Slide.slider.handleRect = parentRect;
                    targetUIBar_Slide.iconHandle =  rect.gameObject.AddComponent<Image>();
                }
            }

        }
    }
}