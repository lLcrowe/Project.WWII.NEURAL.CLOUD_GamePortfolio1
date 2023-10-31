using lLCroweTool.UI.UIThema;
using lLCroweTool.UI.UIThema.UIThemaButton;
using UnityEditor;
using UnityEngine;

namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(ThemaUIButton), true)]
    public class ThemaUIButtonInspectorEditor : CustomDataInspecterEditor<ThemaUIButton>
    {
        private static UIThemaInfo targetUIThemaInfo;//타겟테마
        //구조
        //buttonThemaUI
        //  iconThemaUI
        //  textThemaUI

        protected override void AutoGenerateSection()
        {
            //테마들 생성 초기화
            GenerateUIThemaButton(targetObject);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="themaUIButton"></param>
        public static void GenerateUIThemaButton(ThemaUIButton themaUIButton)
        {
            //각 테마들 추가
            if (themaUIButton.buttonThemaUI == null)
            {
                lLcroweUtilEditor.NullAddComonent(themaUIButton.transform, ref themaUIButton.buttonThemaUI);
                themaUIButton.buttonThemaUI.themaUIType = ThemaUIType.Button;
            }

            if (themaUIButton.isUseIcon)
            {
                if (themaUIButton.iconThemaUI == null)
                {
                    lLcroweUtilEditor.CheckEmptyComponentForNewGameObject(themaUIButton.transform, "IconThemaUI", ref themaUIButton.iconThemaUI, themaUIButton.transform);
                    themaUIButton.iconThemaUI.themaUIType = ThemaUIType.Icon;
                }
            }
            else
            {
                lLcroweUtilEditor.NotNullDestroy(ref themaUIButton.iconThemaUI, true);
            }



            if (themaUIButton.isUseText)
            {
                if (themaUIButton.textThemaUI == null)
                {
                    lLcroweUtilEditor.CheckEmptyComponentForNewGameObject(themaUIButton.transform, "textThemaUI", ref themaUIButton.textThemaUI, themaUIButton.transform);
                    themaUIButton.textThemaUI.themaUIType = ThemaUIType.Text;
                }
            }
            else
            {
                if (themaUIButton.textThemaUI)
                {
                    lLcroweUtilEditor.NotNullDestroy(ref themaUIButton.textThemaUI, true);
                }
            }

            ThemaUIInspectorEditor.GenerateUIThema(themaUIButton.buttonThemaUI);

            //안쪽패널때문에 +1 추가된 상태
            if (themaUIButton.isUseIcon)
            {
                ThemaUIInspectorEditor.GenerateUIThema(themaUIButton.iconThemaUI);
                themaUIButton.iconThemaUI.SetSibling(1);
            }

            if (themaUIButton.isUseText)
            {
                ThemaUIInspectorEditor.GenerateUIThema(themaUIButton.textThemaUI);
                themaUIButton.textThemaUI.SetSibling(2);
                //텍스트는 버튼과 하나된 경우가 많으니
                themaUIButton.textThemaUI.panelImage.rectTransform.SetAnchorPreset(lLcroweUtil.RectAnchorPreset.StretchBoth);
            }
        }

        protected override bool CheckAutoGenerate(ref string content)
        {
            bool check = false;
            if (targetObject.buttonThemaUI == null)
            {
                content += "버튼테마UI가 없습니다.";
                check = true;
            }

            if (targetObject.isUseIcon)
            {
                if (targetObject.iconThemaUI == null)
                {
                    content += "아이콘테마UI가 없습니다.";
                    check = true;
                }
            }
            else
            {
                if (targetObject.iconThemaUI)
                {
                    content += "아이콘테마UI가 있습니다.";
                    check = true;
                }
            }

            if (targetObject.isUseText)
            {
                if (targetObject.textThemaUI == null)
                {
                    content += "텍스트테마UI가 없습니다.";
                    check = true;
                }
            }
            else
            {
                if (targetObject.textThemaUI)
                {
                    content += "텍스트테마UI가 있습니다.";
                    check = true;
                }
            }


            
            return check;
        }

        protected override void DisplaySection()
        {
            lLcroweUtilEditor.ObjectFieldAndNullButton("타겟이 될 UI테마", ref targetUIThemaInfo, false);
            if (targetUIThemaInfo == null)
            {
                EditorGUILayout.HelpBox("UI테마가 비어있습니다.", MessageType.Warning);
            }
            else
            {
                if (GUILayout.Button("UI테마 적용하기"))
                {
                    targetObject.buttonThemaUI.InitThemaUI(targetUIThemaInfo);

                    if (targetObject.isUseIcon)
                    {
                        targetObject.iconThemaUI.InitThemaUI(targetUIThemaInfo);
                    }
                    if (targetObject.isUseText)
                    {
                        targetObject.textThemaUI.InitThemaUI(targetUIThemaInfo);
                    }
                }
            }
        }

        protected override void InitAddFunc()
        {
            
        }
    }
}