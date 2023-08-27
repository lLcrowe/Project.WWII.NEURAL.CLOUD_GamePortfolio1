using lLCroweTool.Cinemachine;
using UnityEditor;
using UnityEngine;

namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(CustomCinemachineManager))]
    public class CustomCinemachineManagerInspectorEditor : Editor
    {

        private CustomCinemachineManager targetCustomCinemachineManager;

        private void OnEnable()
        {
            targetCustomCinemachineManager = (CustomCinemachineManager)target;
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("시네머신 생성"))
            {
                GameObject go = new GameObject();
                var temp = go.AddComponent<CustomCinemachine>();
                temp.transform.SetParent(targetCustomCinemachineManager.transform);
                Selection.activeGameObject = go;
            }
        }
    }
}