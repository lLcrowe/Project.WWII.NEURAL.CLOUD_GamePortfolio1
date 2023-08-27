using lLCroweTool.UI.IconImage;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(IconButton), true)]
    public class IconButtonInspectorEditor : Editor
    {
        private IconButton targetObject;

        //제작방식처리
        private enum MakeIconButtonType
        {
            IconBGType,//아이콘이 뒷배경이 있는 타입인지
            IconOnlyType,//뒷배경이 없는 아이콘 타입인지
        }

        private MakeIconButtonType makeIconButtonType;

        private void OnEnable()
        {
            targetObject = (IconButton)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            string content = "";
            bool state = false;
            //상태체크
            if (targetObject.button == null)
            {
                content = lLcroweUtil.GetCombineString(content, "버튼, ");
                state = true;
            }
            if (targetObject.buttonImage == null)
            {
                content = lLcroweUtil.GetCombineString(content, "버튼이미지, ");
                state = true;
            }
            if (targetObject.text == null)
            {
                content = lLcroweUtil.GetCombineString(content, "텍스트, ");
                state = true;
            }
            if (targetObject.iconImage == null)
            {
                content = lLcroweUtil.GetCombineString(content, "아이콘, ");
                state = true;
            }
            if (targetObject.iconBackGroundImage == null && makeIconButtonType == MakeIconButtonType.IconBGType)
            {
                content = lLcroweUtil.GetCombineString(content, "아이콘뒷배경, ");
                state = true;
            }


            if (state)
            {
                EditorGUILayout.HelpBox(content, MessageType.Warning);
            }


            makeIconButtonType = (MakeIconButtonType)EditorGUILayout.EnumPopup("제작타입", makeIconButtonType);
            var tr = targetObject.transform;
            if (GUILayout.Button("제작하기"))
            {
                lLcroweUtilEditor.ClearTransformForChild(tr);
                if (targetObject.button == null)
                {
                    //자기자신에서 처리
                    targetObject.button = tr.GetComponent<Button>();
                }
                if (targetObject.buttonImage == null)
                {
                    //자기자신에서 처리
                    targetObject.buttonImage = tr.GetComponent<Image>();
                }
                lLcroweUtilEditor.CheckEmptyComponentForNewGameObject(tr, "text", ref targetObject.text);
                targetObject.text.text = "Button";
                lLcroweUtilEditor.InitTextMeshProUGUI(targetObject.text);
                targetObject.text.rectTransform.SetAnchorPreset(lLcroweUtilEditor.RectAnchorPreset.StretchBoth);
                targetObject.text.rectTransform.sizeDelta = Vector2.zero;

                if (makeIconButtonType == MakeIconButtonType.IconBGType)
                {
                    lLcroweUtilEditor.CheckEmptyComponentForNewGameObject(tr, "iconBackGoundImage", ref targetObject.iconBackGroundImage);
                    lLcroweUtilEditor.CheckEmptyComponentForNewGameObject(tr, "icon", ref targetObject.iconImage, targetObject.iconBackGroundImage.transform);
                    targetObject.iconImage.rectTransform.SetAnchorPreset(lLcroweUtilEditor.RectAnchorPreset.StretchBoth);
                    targetObject.iconImage.rectTransform.sizeDelta = Vector2.zero;
                }
                else
                {
                    lLcroweUtilEditor.CheckEmptyComponentForNewGameObject(tr, "icon", ref targetObject.iconImage);
                }
            }
        }
    }
}