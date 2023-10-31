using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Events;
using TMPro;
using UnityEditor;
using System;

namespace Assets.A1.CustomScript.QCScript.MultipleWindow
{
    public class MultipleLayout : EditorWindow
    {
       

        private VisualElement bodyElement1;
        private VisualElement bodyElement2;
        private VisualElement bodyElement3;
        private VisualElement bodyElement4;


        [MenuItem("lLcroweTool/TestMultipleWindow")]
        public static void ShowWindow()
        {
            // 생성되어있는 윈도우를 가져온다. 없으면 새로 생성한다. 싱글턴 구조인듯하다.


            //Type[] types = { typeof(MultipleLayout), typeof(MultipleLayoutChild)};
            //EditorWindow editorWindow = GetWindow<EditorWindow>(types);
            //editorWindow.titleContent.text = "멀티플 윈도우에디터";

            MultipleLayout editorWindow = (MultipleLayout)GetWindow(typeof(MultipleLayout));
            editorWindow.titleContent.text = "멀티플 윈도우에디터";

            //EditorWindow hierarchyWindow = GetWindow<MultipleLayout>("Hierarchy");
            //EditorWindow eventsWindow = GetWindow<MultipleLayoutChild>("Events", typeof(MultipleLayoutChild));
        }

        private void OnEnable()
        {
            bodyElement1 = new VisualElement();
            TextElement textField = new TextElement();
            textField.text = "1번";
            bodyElement1.Add(textField);
            bodyElement1.style.backgroundColor = Color.red;

            bodyElement2 = new VisualElement();
            textField = new TextElement();
            textField.text = "2번";
            bodyElement2.Add(textField);
            bodyElement2.style.backgroundColor = Color.yellow;

            bodyElement3 = new VisualElement();
            textField = new TextElement();
            textField.text = "3번";
            bodyElement3.Add(textField);
            bodyElement3.style.backgroundColor = Color.green;

            bodyElement4 = new VisualElement();
            textField = new TextElement();
            textField.text = "4번";
            bodyElement4.Add(textField);
            bodyElement4.style.backgroundColor = Color.blue;


            //루트
            rootVisualElement.Add(bodyElement1);
            rootVisualElement.Add(bodyElement2);
            rootVisualElement.Add(bodyElement3);
            rootVisualElement.Add(bodyElement4);
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(bodyElement1);
            rootVisualElement.Remove(bodyElement2);
            rootVisualElement.Remove(bodyElement3);
            rootVisualElement.Remove(bodyElement4);
        }

        private void OnSelectionChange()
        {

        }

        protected void RegisterCallbacksOnTarget(VisualElement target)
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
        }

        protected void UnregisterCallbacksFromTarget(VisualElement target)
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        }

        protected void OnMouseDown(MouseDownEvent e)
        {
            DebugLog("click");
        }

       
        protected void OnMouseMove(MouseMoveEvent e)
        {
           
        }
     
        protected void OnMouseUp(MouseUpEvent e)
        {
           
        }


        private void DebugLog(string content)
        {
            Debug.Log(content);
        }


    }
}