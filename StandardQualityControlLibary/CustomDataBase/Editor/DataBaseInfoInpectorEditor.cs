using lLCroweTool.DataBase;
using UnityEditor;
using UnityEngine;

namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(DataBaseInfo))]
    public class DataBaseInfoInpectorEditor : Editor
    {
        private DataBaseInfo targetDataBaseInfo;

        private void OnEnable()
        {
            targetDataBaseInfo = (DataBaseInfo)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("게임데이터베이스윈도우열기"))
            {
                DataBaseInfoWindowEditor.targetDataBaseInfo = targetDataBaseInfo;
                DataBaseInfoWindowEditor.ShowWindow();
            }
        }
    }
}