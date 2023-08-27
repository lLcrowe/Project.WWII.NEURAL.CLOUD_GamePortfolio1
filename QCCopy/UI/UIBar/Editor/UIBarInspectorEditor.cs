using lLCroweTool.UI.Bar;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(UIBar_Base))]
    public class UIBarInspectorEditor : CustomDataInspecterEditor<UIBar_Base>
    {
        private UIBar_Base targetUIBar;

        protected override void InitAddFunc()
        {
            targetUIBar = (UIBar_Base)target;
            
        }

        protected override bool CheckAutoGenerate(ref string content)
        {
            bool isCheck = false;

            content += "-=UIBar 필요사항=-\n";

            if (targetUIBar.fill == null)
            {
                content += "UIBar의 채우기이미지오브젝트가 없습니다.\n";
                isCheck = true;
            }

            //이건 필수로 필요하지는 않음
            //if (targetUIBar.border == null)
            //{
            //    content += "UIBar의 테두리이미지오브젝트가 없습니다.\n";
            //    isCheck = true;
            //}

            return isCheck;
        }

        protected override void AutoGenerateSection()
        {
            targetUIBar.gameObject.name = "UIBar";
            //스트레치로 붙이기

            AutoGenerateUIBarBase(targetUIBar);
        }

        public static void AutoGenerateUIBarBase(UIBar_Base uIBar_Base)
        {
            GameObject gameObject = null;
            if (uIBar_Base.fill == null)
            {
                //트리슬롯상태 이미지오브젝트
                gameObject = new GameObject();
                gameObject.transform.SetParent(uIBar_Base.transform);
                gameObject.transform.position = uIBar_Base.transform.position;
                gameObject.name = "fillImageObject";
                uIBar_Base.fill = gameObject.AddComponent<Image>();
                uIBar_Base.fill.type = Image.Type.Filled;

                RectTransform rect = (RectTransform)uIBar_Base.fill.transform;
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.sizeDelta = Vector2.zero;
            }

            if (uIBar_Base.border == null)
            {
                //트리슬롯상태 이미지오브젝트
                gameObject = new GameObject();
                gameObject.transform.SetParent(uIBar_Base.transform);
                gameObject.transform.position = uIBar_Base.transform.position;
                gameObject.name = "borderImageObject";
                uIBar_Base.border = gameObject.AddComponent<Image>();

                RectTransform rect = (RectTransform)uIBar_Base.border.transform;
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.sizeDelta = Vector2.zero;
            }
        }

        protected override void DisplaySection()
        {
            //여기서 필수아닌것들만 표시하기

            if (GUILayout.Button("사이즈 리셋"))
            {
                if (!targetUIBar.TryGetComponent(out RectTransform rect))
                {
                    rect = targetUIBar.gameObject.AddComponent<RectTransform>();
                }
                rect.sizeDelta = new Vector2 (200, 50);
            }

            //뒷배경제작 안내문
            if (targetUIBar.TryGetComponent(out Image backGroundImage))
            {
                //삭제버튼
                if (GUILayout.Button("뒷배경 삭제"))
                {
                    DestroyImmediate(backGroundImage);
                }
            }
            else
            {
                //제작버튼
                if (GUILayout.Button("뒷배경 생성"))
                {
                    targetUIBar.gameObject.AddComponent<Image>();
                }
            }

            //필모드 변경
            if (GUILayout.Button("필이미지 설정하기 위해 선택하기"))
            {
                Selection.activeObject = targetUIBar.fill;
            }
        }
    }
}