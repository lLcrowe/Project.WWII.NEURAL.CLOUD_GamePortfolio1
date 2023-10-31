using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace lLCroweTool.QC.EditorOnly
{
    public class lLcroweCustomHotKey : Editor
    {
        //커스텀 단축키
        //메뉴아이템에 있는 편집툴을 가져와서 마지막 _ 옆에 원하는 단축키를 쓰면 됨
        //[MenuItem("GameObject/ActiveToggle _a")]
        [MenuItem("GameObject/ActiveToggle _`")]
        private static void SelectGameObjectActiveAndDeActive()
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                go.SetActive(!go.activeSelf);
            }   
        }

        [MenuItem("Tools/Toggle Inspector _1")]
        private static void ToggleLockInspector()
        {
            ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
            ActiveEditorTracker.sharedTracker.ForceRebuild();
        }
    }
}
