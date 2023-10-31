using lLCroweTool.LevelSystem;

using System.Collections;
using UnityEditor;
using UnityEngine;

namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(LevelInfo))]
    public class LevelInfoInspectorEditor : CustomDataOpenWindowInspecterEditor<LevelInfo>
    {
        public override void ShowEditorWindow(LevelInfo targetData)
        {
            LevelInfoEditor.isLoadData = true;
            LevelInfoEditor.targetData = targetData;
            LevelInfoEditor.ShowWindow();
        }
    }
}

