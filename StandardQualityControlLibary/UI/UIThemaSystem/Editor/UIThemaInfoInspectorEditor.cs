using lLCroweTool.UI.UIThema;
using UnityEditor;
using UnityEngine;

namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(UIThemaInfo))]
    public class UIThemaInfoInspectorEditor : Editor
    {
        private UIThemaInfo uIThemaInfo;

        private void OnEnable()
        {
            uIThemaInfo = target as UIThemaInfo;
        }

        public override void OnInspectorGUI()
        {
            ImportButton();

            ThemaUIInspectorEditor.UIThemaPresetShow("기본형",uIThemaInfo.panelUIThemaPreset);
            ThemaUIInspectorEditor.UIThemaPresetShow("아이콘", uIThemaInfo.iconUIThemaPreset);

            ThemaUIInspectorEditor.UIThemaPresetShow("버튼", uIThemaInfo.buttonUIThemaPreset);
            ThemaUIInspectorEditor.ButtonColorPresetShow(uIThemaInfo.buttonColorPreset);
            ThemaUIInspectorEditor.ButtonSpriteSwapPresetShow(uIThemaInfo.buttonSpriteSwapPreset);

            ThemaUIInspectorEditor.UIThemaPresetShow("텍스트", uIThemaInfo.textUIThemaPreset);
            ThemaUIInspectorEditor.FontPresetShow(uIThemaInfo.textFontPreset);

            ImportButton();
        }

        private void ImportButton()
        {
            lLcroweUtilEditor.EditorButton("임포트", () =>
            {
                var manager = FindObjectOfType<ThemaUIManager>();
                Debug.Log(manager);
                if (manager == null)
                {
                    //존재하면 그걸로 처리
                    return;
                }
                manager.InitAllThemaUI(uIThemaInfo);
            });
        }

    }
}
