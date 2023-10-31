using System.Collections;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace lLCroweTool.QC.EditorOnly
{
    //public abstract class CustomDataOpenWindowInspecterEditor<T1, T2> : Editor where T1 : ScriptableObject where T2 : CustomDataWindowEditor<ScriptableObject>
    //[CustomEditor(typeof(T1))]
    public abstract class CustomDataOpenWindowInspecterEditor<T1> : Editor where T1 : ScriptableObject
    {
        //인스팩터에디터에서 윈도우에디터를 열기위한 기능을 가짐
        //스크립터블데이터를 체크 
        private T1 data;
        private string contentName;

        private void OnEnable()
        {
            data = (T1)target;
            contentName = typeof(T1).Name;
        }

        public sealed override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button($"{contentName}윈도우열기"))
            {
                //T2.targetData = data;
                ShowEditorWindow(data);
            }
        }

        /// <summary>
        /// 에디터윈도우를 보여주는 함수(예시존재)
        /// </summary>
        /// <param name="targetData">타겟팅된 데이터</param>
        public abstract void ShowEditorWindow(T1 targetData);

        //예시
        //public override void ShowEditorWindow(LevelInfo targetData)
        //{
        //    LevelInfoEditor.isReset = false;          //CustomDataWindowEditor를 상속받은 윈도우에디터
        //    LevelInfoEditor.targetData = targetData;  //CustomDataWindowEditor.targetData
        //    LevelInfoEditor.ShowWindow();             //CustomDataWindowEditor를 상속받은 클래스의 ShowWindow
        //}

    }
}

