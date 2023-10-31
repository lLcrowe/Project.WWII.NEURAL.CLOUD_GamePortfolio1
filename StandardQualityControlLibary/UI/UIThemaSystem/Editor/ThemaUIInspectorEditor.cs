using lLCroweTool.UI.UIThema;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(ThemaUI))]
    public class ThemaUIInspectorEditor : CustomDataInspecterEditor<ThemaUI>
    {
        private static UIThemaInfo targetUIThemaInfo;//타겟테마
        private static bool createUIThemaMode;

        private static UIThemaInfo tempInfo;

        public static void GenerateUIThema(ThemaUI themaUI)
        {
            switch (themaUI.themaUIType)
            {
                case ThemaUIType.Panel:
                    //필요없는것들
                    lLcroweUtilEditor.NotNullDestroy(ref themaUI.button, false);
                    lLcroweUtilEditor.NotNullDestroy(ref themaUI.iconImageObject, true);
                    lLcroweUtilEditor.NotNullDestroy(ref themaUI.textObject, true);

                    //필요한것들
                    CreatePanel(themaUI);
                    break;
                case ThemaUIType.Icon:
                    //필요없는것들                    
                    lLcroweUtilEditor.NotNullDestroy(ref themaUI.button, false);
                    lLcroweUtilEditor.NotNullDestroy(ref themaUI.textObject, true);

                    //필요한것들                    
                    CreatePanel(themaUI);

                    lLcroweUtilEditor.NullAddComonent(themaUI.transform, ref themaUI.iconImageObject, true, "iconImageObject");
                    themaUI.iconImageObject.rectTransform.SetAnchorPreset(lLcroweUtil.RectAnchorPreset.StretchBoth);
                    themaUI.iconImageObject.SetSibling(1);
                    break;
                case ThemaUIType.Button:
                    //필요없는것들
                    lLcroweUtilEditor.NotNullDestroy(ref themaUI.iconImageObject, true);
                    lLcroweUtilEditor.NotNullDestroy(ref themaUI.textObject, true);

                    //필요한것들
                    CreatePanel(themaUI);

                    lLcroweUtilEditor.NullAddComonent(themaUI.transform, ref themaUI.button);
                    break;
                case ThemaUIType.Text:
                    //필요없는것들
                    lLcroweUtilEditor.NotNullDestroy(ref themaUI.button, false);
                    lLcroweUtilEditor.NotNullDestroy(ref themaUI.iconImageObject, true);
                    

                    //필요한것들
                    CreatePanel(themaUI);

                    lLcroweUtilEditor.NullAddComonent(themaUI.transform, ref themaUI.textObject, true, "textObject");
                    themaUI.textObject.rectTransform.SetAnchorPreset(lLcroweUtil.RectAnchorPreset.StretchBoth);
                    lLcroweUtilEditor.InitTextMeshProUGUI(themaUI.textObject);
                    break;
            }
        }

        //패널 제작
        private static void CreatePanel(ThemaUI themaUI)
        {
            //패널처리 모든 오브젝트에서 사용함
            lLcroweUtilEditor.NullAddComonent(themaUI.transform, ref themaUI.panelImage);            
            InitImage(themaUI.panelImage);

            lLcroweUtilEditor.NullAddComonent(themaUI.transform, ref themaUI.innerPanelImageObject, true, "innerPanelImageObject");
            InitImage(themaUI.innerPanelImageObject);
            themaUI.innerPanelImageObject.rectTransform.SetAnchorPreset(lLcroweUtil.RectAnchorPreset.StretchBoth);
            themaUI.innerPanelImageObject.SetSibling(0);
            themaUI.innerPanelImageObject.enabled = false;//안쪽패널은 기본적으로 비활성화
        }

        private static void InitImage(Image image)
        {
            //패널은 타일이 맞는거 같고 아이콘은 Slice가 맞는거 같다//근데 별차이가 읍네. 타일로 통일
            image.type = Image.Type.Sliced;
            image.pixelsPerUnitMultiplier = 1;
        }

        protected override void AutoGenerateSection()
        {
            GenerateUIThema(targetObject);
        }


        private bool CheckPanelNotEmpty()
        {
            if (targetObject.panelImage == null)
            {
                return true;
            }
            if (targetObject.innerPanelImageObject == null)
            {
                return true;
            }
            return false;
        }
     

        protected override bool CheckAutoGenerate(ref string content)
        {
            bool check = false;            
            //자동화처리
            //구조에 맞게 변경
            switch (targetObject.themaUIType)
            {
                case ThemaUIType.Panel:
                    //필요한것들
                    if (CheckPanelNotEmpty())
                    {
                        content += "패널 초기화가 필요합니다\n";
                        check = true;
                    }


                    //필요없는것들
                    if (targetObject.button)
                    {
                        content += "버튼이 필요없습니다\n";
                        check = true;
                    }
                    if (targetObject.iconImageObject)
                    {
                        content += "아이콘이 필요없습니다\n";
                        check = true;
                    }
                    if (targetObject.textObject)
                    {
                        content += "텍스트가 필요없습니다\n";
                        check = true;
                    }
                    break;
                case ThemaUIType.Icon:
                    //필요한것들                    
                    check = CheckPanelNotEmpty();
                    if (targetObject.iconImageObject == null) 
                    {
                        content += "아이콘이 필요합니다\n ";
                        check = true;
                    }                    

                    //필요없는것들
                    if (targetObject.button)
                    {
                        content += "버튼이 필요없습니다\n";
                        check = true;
                    }
                    if (targetObject.textObject)
                    {
                        content += "텍스트가 필요없습니다\n";
                        check = true;
                    }
                    break;
                case ThemaUIType.Button:
                    //필요한것들
                    if (CheckPanelNotEmpty())
                    {
                        content += "패널 초기화가 필요합니다\n";
                        check = true;
                    }
                    if (targetObject.button == null)
                    {
                        content += "버튼이 필요합니다\n";
                        check = true;
                    }

                    //필요없는것들
                    if (targetObject.iconImageObject)
                    {
                        content += "아이콘이 필요없습니다\n";
                        check = true;
                    }
                    if (targetObject.textObject)
                    {
                        content += "텍스트가 필요없습니다\n";
                        check = true;
                    }
                    break;
                case ThemaUIType.Text:
                    //필요한것들
                    if (CheckPanelNotEmpty())
                    {
                        content += "패널 초기화가 필요합니다\n";
                        check = true;
                    }
                    if (targetObject.textObject == null)
                    {
                        content += "텍스트가 필요합니다\n";
                        check = true;
                    }

                    //필요없는것들                 
                    if (targetObject.iconImageObject)
                    {
                        content += "아이콘이 필요없습니다\n";
                        check = true;
                    }
                    if (targetObject.button)
                    {
                        content += "버튼이 필요없습니다\n";
                        check = true;
                    }   
                    break;
            }

            return check;
        }

        protected override void DisplaySection()
        {
            lLcroweUtilEditor.EnumPopup("UI타입", ref targetObject.themaUIType);
            if (targetObject.themaUIType == ThemaUIType.Text)
            {
                //텍스트필드를 보여주기//매번 안쪽꺼 클릭해서 바꾸는것보다 편해보임
                targetObject.textObject.text = EditorGUILayout.TextArea(targetObject.textObject.text);
            }

            string content = createUIThemaMode ? "ManualImport" : "InfoImport";
            createUIThemaMode = EditorGUILayout.BeginFoldoutHeaderGroup(createUIThemaMode, content);
            if (createUIThemaMode)
            {
                if (tempInfo == null)   
                {
                    tempInfo = new();
                }

                switch (targetObject.themaUIType)
                {
                    case ThemaUIType.Panel:
                        UIThemaPresetShow("기본형", tempInfo.panelUIThemaPreset);
                        break;
                    case ThemaUIType.Icon:
                        UIThemaPresetShow("아이콘", tempInfo.iconUIThemaPreset);
                        break;
                    case ThemaUIType.Button:
                        UIThemaPresetShow("버튼", tempInfo.buttonUIThemaPreset);
                        ButtonColorPresetShow(tempInfo.buttonColorPreset);
                        ButtonSpriteSwapPresetShow(tempInfo.buttonSpriteSwapPreset);
                        break;
                    case ThemaUIType.Text:
                        UIThemaPresetShow("텍스트", tempInfo.textUIThemaPreset);
                        FontPresetShow(tempInfo.textFontPreset);
                        break;
                }
                EditorGUILayout.Space(10);

                lLcroweUtilEditor.EditorButton("UI테마 적용하기", () =>
                {
                    targetObject.InitThemaUI(tempInfo);
                });
            }
            else
            {                
                lLcroweUtilEditor.ObjectFieldAndNullButton("타겟이 될 UI테마", ref targetUIThemaInfo, false);
                lLcroweUtilEditor.EditorButton("UI테마 적용하기", () =>
                {
                    if (targetUIThemaInfo == null)
                    {
                        return;
                    }
                    targetObject.InitThemaUI(targetUIThemaInfo);
                });

                //지금세팅되 있는 옵션을 가져오기
                lLcroweUtilEditor.EditorButton("UI테마 저장하기", () =>
                {
                    if (targetUIThemaInfo == null)
                    {
                        return;
                    }

                    //타입에 따라 현재 UI테마정보에 세팅
                    switch (targetObject.themaUIType)
                    {
                        case ThemaUIType.Panel:
                            OverridePanelData(targetUIThemaInfo, targetObject);
                            break;
                        case ThemaUIType.Icon:
                            OverrideIconData(targetUIThemaInfo, targetObject);
                            break;
                        case ThemaUIType.Button:
                            OverrideButtonData(targetUIThemaInfo, targetObject);
                            break;
                        case ThemaUIType.Text:
                            OverrideTextData(targetUIThemaInfo, targetObject);
                            break;
                    }
                    AssetDatabase.SaveAssets();//저장되지 않은 모든 자산 변경 사항을 디스크에 씁니다.
                    AssetDatabase.Refresh();//새로고침
                });
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void OverridePanelData(UIThemaInfo targetUIThemaInfo, ThemaUI themaUI)
        {
            targetUIThemaInfo.panelUIThemaPreset.SetImageData(themaUI.panelImage,themaUI.innerPanelImageObject);
        }

        private void OverrideButtonData(UIThemaInfo targetUIThemaInfo, ThemaUI themaUI)
        {
            targetUIThemaInfo.iconUIThemaPreset.SetImageData(themaUI.panelImage, themaUI.innerPanelImageObject);
        }

        private void OverrideIconData(UIThemaInfo targetUIThemaInfo, ThemaUI themaUI)
        {
            targetUIThemaInfo.buttonUIThemaPreset.SetImageData(themaUI.panelImage, themaUI.innerPanelImageObject);
            targetUIThemaInfo.buttonColorPreset.SetButtonData(themaUI.button);
            targetUIThemaInfo.buttonSpriteSwapPreset.SetButtonData(themaUI.button);
        }

        private void OverrideTextData(UIThemaInfo targetUIThemaInfo, ThemaUI themaUI)
        {
            targetUIThemaInfo.textUIThemaPreset.SetImageData(themaUI.panelImage, themaUI.innerPanelImageObject);
            targetUIThemaInfo.textFontPreset.SetFontData(themaUI.textObject);
        }

        protected override void InitAddFunc()
        {
            if (tempInfo == null)
            {
                tempInfo = new UIThemaInfo();
            }
        }
        public static void UIThemaPresetShow(string content, UIThemaPreset uIThemaPreset)
        {            
            SpritePresetShow($"{content}패널", uIThemaPreset.panelSpritePreset);

            BorderPresetShow(content, uIThemaPreset.innerPanelBorderPreset);
            SpritePresetShow($"{content}안쪽패널", uIThemaPreset.innerPanelSpritePreset);
        }
        public static void BorderPresetShow(string content, BorderPreset borderPreset)
        {
            //borderPreset.isUseBorderImage = EditorGUILayout.Toggle("테두리이미지 사용여부",borderPreset.isUseBorderImage);

            borderPreset.borderTopValue = EditorGUILayout.FloatField($"{content}위쪽 테두리", borderPreset.borderTopValue);
            borderPreset.borderBottomValue = EditorGUILayout.FloatField($"{content}아래쪽 테두리", borderPreset.borderBottomValue);
            borderPreset.borderLeftValue = EditorGUILayout.FloatField($"{content}왼쪽 테두리", borderPreset.borderLeftValue);
            borderPreset.borderRightValue = EditorGUILayout.FloatField($"{content}오른쪽 테두리", borderPreset.borderRightValue);
            EditorGUILayout.LabelField("---------");
        }
    
        public static void SpritePresetShow(string content, SpritePreset spritePreset)
        {
            spritePreset.color = EditorGUILayout.ColorField($"{content} 컬러", spritePreset.color);
            lLcroweUtilEditor.ObjectFieldAndNullButton($"{content} 스프라이트", ref spritePreset.sprite, false);
            lLcroweUtilEditor.ObjectFieldAndNullButton($"{content} 메터리얼", ref spritePreset.uiMaterial, false);
            spritePreset.isUseRaycastTarget = EditorGUILayout.Toggle($"{content} 레이캐스트사용여부", spritePreset.isUseRaycastTarget);
            EditorGUILayout.LabelField("---------");
        }
        public static void ButtonColorPresetShow(ButtonColorPreset buttonColorPreset)
        {
            buttonColorPreset.highLightColor = EditorGUILayout.ColorField("컨트롤이 강조될 때 컬러", buttonColorPreset.highLightColor);
            buttonColorPreset.pressedColor = EditorGUILayout.ColorField("컨트롤이 눌렀을 때 컬러", buttonColorPreset.pressedColor);
            buttonColorPreset.selectedColor = EditorGUILayout.ColorField("컨트롤이 선택될 때 컬러", buttonColorPreset.selectedColor);
            buttonColorPreset.disabledColor = EditorGUILayout.ColorField("컨트롤이 비활성화될 때 컬러", buttonColorPreset.disabledColor);

            buttonColorPreset.colorMultiplier = EditorGUILayout.FloatField("컬러 곱", buttonColorPreset.colorMultiplier);
            buttonColorPreset.fadeDuration = EditorGUILayout.FloatField("페이드 시간", buttonColorPreset.fadeDuration);
            EditorGUILayout.LabelField("---------");
        }
        public static void ButtonSpriteSwapPresetShow(ButtonSpriteSwapPreset buttonSpriteSwapPreset)
        {
            lLcroweUtilEditor.ObjectFieldAndNullButton("컨트롤이 강조될 때 사용할 스프라이트", ref buttonSpriteSwapPreset.highLightSprite, false);
            lLcroweUtilEditor.ObjectFieldAndNullButton("컨트롤이 눌렀을 때 사용할 스프라이트", ref buttonSpriteSwapPreset.pressedSprite, false);
            lLcroweUtilEditor.ObjectFieldAndNullButton("컨트롤이 선택될 때 사용할 스프라이트", ref buttonSpriteSwapPreset.selectedSprite, false);
            lLcroweUtilEditor.ObjectFieldAndNullButton("컨트롤이 비활성화될 때 사용할 스프라이트", ref buttonSpriteSwapPreset.disabledSprite, false);
            EditorGUILayout.LabelField("---------");
        }

        public static void FontPresetShow(FontPreset fontPreset)
        {
            lLcroweUtilEditor.ObjectFieldAndNullButton("사용할 폰트", ref fontPreset.fontAsset, false);
            fontPreset.fontColor = EditorGUILayout.ColorField("텍스트 컬러", fontPreset.fontColor);
            EditorGUILayout.LabelField("---------");
        }
    }
}