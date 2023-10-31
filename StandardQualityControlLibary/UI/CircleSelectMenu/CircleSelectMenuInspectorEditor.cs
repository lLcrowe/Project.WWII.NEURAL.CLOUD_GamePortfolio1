using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.Progress;
using Doozy.Engine.Layouts;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#pragma warning disable 0618
namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(CircleSelectMenu), true)]
    public class CircleSelectMenuInspectorEditor : Editor
    {
        private CircleSelectMenu targetCircleSelectMenu;//데이터를 가져와서 세팅해주고 싶은 연구트리시스템


        private void OnEnable()
        {
            targetCircleSelectMenu = (CircleSelectMenu)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("=========================");
            EditorGUILayout.LabelField("-=원형선택메뉴 설정 인스팩터에디터=-");

            bool isCheck = false;

            if (targetCircleSelectMenu.radialLayout == null)
            {
                EditorGUILayout.HelpBox("라디안레이아웃을 세팅해주세요", MessageType.Warning);
                isCheck = true;
            }
            else
            {
                if (!targetCircleSelectMenu.radialLayout.TryGetComponent(out Progressor progressor))
                {
                    EditorGUILayout.HelpBox("프로그래서를 라디안레이아웃오브젝트에 세팅해주세요", MessageType.Warning);
                    isCheck = true;
                }
            }

            if (targetCircleSelectMenu.text == null)
            {
                EditorGUILayout.HelpBox("텍스트프로UI를 세팅해주세요", MessageType.Warning);
                isCheck = true;
            }

            EditorGUILayout.LabelField(targetCircleSelectMenu.uIButtonArray.Length + "개 버튼이 존재합니다.");

            if (isCheck)
            {
                if (GUILayout.Button("자동생성하기"))
                {
                    if (targetCircleSelectMenu.radialLayout == null)
                    {
                        GameObject gameObject = new GameObject();
                        gameObject.transform.parent = targetCircleSelectMenu.transform;
                        gameObject.transform.position = targetCircleSelectMenu.transform.position;
                        targetCircleSelectMenu.radialLayout = gameObject.AddComponent<RadialLayout>();
                        targetCircleSelectMenu.radialLayout.gameObject.AddComponent<Progressor>();
                        targetCircleSelectMenu.text = targetCircleSelectMenu.radialLayout.gameObject.AddComponent<TextMeshProUGUI>();
                    }
                    else
                    {
                        if (!targetCircleSelectMenu.radialLayout.TryGetComponent(out Progressor progressor))
                        {
                            targetCircleSelectMenu.radialLayout.gameObject.AddComponent<Progressor>();
                        }
                        if (targetCircleSelectMenu.text == null)
                        {
                            targetCircleSelectMenu.text = targetCircleSelectMenu.radialLayout.gameObject.AddComponent<TextMeshProUGUI>();
                        }
                    }
                }
            }
        }
    }
}
    #endif