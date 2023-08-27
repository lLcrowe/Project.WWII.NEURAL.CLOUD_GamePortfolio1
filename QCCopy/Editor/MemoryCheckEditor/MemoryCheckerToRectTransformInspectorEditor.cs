using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
#pragma warning disable 0618
namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(RectTransform), false)]
    [CanEditMultipleObjects]
    public class MemoryCheckerToRectTransformInspectorEditor : Editor
    {
        //초기클릭시 세팅해주는곳
        private Editor _defaultEditor;
        private RectTransform targetObject;

        //동적으로 세팅
        private Component[] componentArray;
        private RectTransform[] targetObjectChildArray;


        private void OnEnable()
        {
            targetObject = (RectTransform)target;
            _defaultEditor = CreateEditor(targets, Type.GetType("UnityEditor.RectTransformEditor, UnityEditor"));

            List<RectTransform> tempTrList = new List<RectTransform>();
            for (int i = 0; i < targets.Length; i++)
            {
                tempTrList.Add((RectTransform)targets[i]);
            }

            //순서되로 나옴
            //targetObjectChildArray = targetObject.GetComponentsInChildren<RectTransform>();

            List<RectTransform> tempTr = new List<RectTransform>();
            for (int i = 0; i < tempTrList.Count; i++)
            {
                targetObjectChildArray = tempTrList[i].GetComponentsInChildren<RectTransform>();

                tempTr.AddRange(targetObjectChildArray);
            }

            targetObjectChildArray = tempTr.ToArray();
            //targetObjectChildArray = targetObject.GetComponentsInChildren<RectTransform>();
        }

        private void OnDisable()
        {
            //When OnDisable is called, the default editor we created should be destroyed to avoid memory leakage.
            //Also, make sure to call any required methods like OnDisable

            DestroyImmediate(_defaultEditor);
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            _defaultEditor.OnInspectorGUI();

            //serializedObject.Update();
            EditorGUILayout.Space();

            string btnContent;
            bool tempOn = MemoryCheckerToTransformInspectorEditor.isMemoryLookOn;
            if (tempOn)
            {
                btnContent = "닫기";
            }
            else
            {
                btnContent = "보기";
            }

            if (GUILayout.Button("메모리" + btnContent))
            {
                MemoryCheckerToTransformInspectorEditor.isMemoryLookOn = !tempOn;
            }


            string temp;


            if (tempOn)
            {
                long totalMemorySize = 0;
                for (int i = 0; i < targetObjectChildArray.Length; i++)
                {
                    //EditorGUILayout.LabelField(targetObjectChildArray[i].name);
                    RectTransform tr = targetObjectChildArray[i];

                    long memorysize = Profiler.GetRuntimeMemorySizeLong(tr.gameObject);
                    temp = tr.name + " =>" + memorysize + "Bytes";
                    EditorGUILayout.LabelField(temp);
                    componentArray = tr.GetComponents<Component>();

                    temp = "    ";
                    long totalSize = memorysize;
                    for (int j = 0; j < componentArray.Length; j++)
                    {
                        //null체크

                        if (ReferenceEquals(componentArray[j], null))
                        {
                            int tempindex = j;
                            temp += j + "번 컴포넌트는 비어있습니다 => None";
                            EditorGUILayout.LabelField(temp);
                        }
                        else
                        {
                            memorysize = Profiler.GetRuntimeMemorySizeLong(componentArray[j]);
                            totalSize += memorysize;
                            temp += componentArray[j].GetType() + " => " + memorysize + "Bytes";
                            EditorGUILayout.LabelField(temp);
                            temp = "    ";
                        }
                    }
                    temp = tr.name + " 오브젝트 총 메모리 사용량 " + MemoryCheckerToTransformInspectorEditor.GetSize(totalSize);

                    totalMemorySize += totalSize;
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(temp);
                    EditorGUILayout.Space();
                }

                temp = "할당된 그래픽 메모리량" + MemoryCheckerToTransformInspectorEditor.GetSize(Profiler.GetAllocatedMemoryForGraphicsDriver()) + "\n";
                temp += "모노 힙메모리량 크기" + MemoryCheckerToTransformInspectorEditor.GetSize(Profiler.GetMonoHeapSizeLong()) + "\n";

                temp += "모노가 사용하는 메모리량" + MemoryCheckerToTransformInspectorEditor.GetSize(Profiler.GetMonoUsedSizeLong()) + "\n";

                temp += "총 할당된 메모리량" + MemoryCheckerToTransformInspectorEditor.GetSize(Profiler.GetTotalAllocatedMemoryLong());


                EditorGUILayout.HelpBox(temp, MessageType.Info);


                //EditorGUILayout.LabelField("오브젝트 내의 총 메모리 사용량" + MemoryCheckerToTransformInspectorEditor.GetSize(totalMemorySize));
                //EditorGUILayout.LabelField("할당된 그래픽 메모리량" + MemoryCheckerToTransformInspectorEditor.GetSize(Profiler.GetAllocatedMemoryForGraphicsDriver()));
                //EditorGUILayout.LabelField("모노 힙메모리량 크기" + MemoryCheckerToTransformInspectorEditor.GetSize(Profiler.GetMonoHeapSizeLong()));
                //EditorGUILayout.LabelField("모노가 사용하는 메모리량" + MemoryCheckerToTransformInspectorEditor.GetSize(Profiler.GetMonoUsedSizeLong()));
                //EditorGUILayout.LabelField("총 할당된 메모리량" + MemoryCheckerToTransformInspectorEditor.GetSize(Profiler.GetTotalAllocatedMemoryLong()));


                if (GUILayout.Button("메모리" + btnContent))
                {
                    MemoryCheckerToTransformInspectorEditor.isMemoryLookOn = !tempOn;
                }
            }
        }
    }
}
