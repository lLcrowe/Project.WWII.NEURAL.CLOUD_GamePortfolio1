using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
#pragma warning disable 0618
namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(Transform), true)]
    [CanEditMultipleObjects]
    public class MemoryCheckerToTransformInspectorEditor : Editor
    {
        public static bool isMemoryLookOn = false;

        //초기클릭시 세팅해주는곳
        private Editor _defaultEditor;
        private Transform targetObject;

        //동적으로 세팅
        private Component[] componentArray;
        private Transform[] targetObjectChildArray;


        //private System.Diagnostics.PerformanceCounter ramCounter;
        private void OnEnable()
        {
            //ramCounter = new System.Diagnostics.PerformanceCounter("Memory", "Available MBytes");
            targetObject = (Transform)target;
            _defaultEditor = CreateEditor(targets, Type.GetType("UnityEditor.TransformInspector, UnityEditor"));

            List<Transform> tempTrList = new List<Transform>();
            for (int i = 0; i < targets.Length; i++)
            {
                tempTrList.Add((Transform)targets[i]);
            }

            //순서되로 나옴
            //targetObjectChildArray = targetObject.GetComponentsInChildren<Transform>();

            List<Transform> tempTr = new List<Transform>();
            for (int i = 0; i < tempTrList.Count; i++)
            {
                targetObjectChildArray = tempTrList[i].GetComponentsInChildren<Transform>();

                tempTr.AddRange(targetObjectChildArray);
            }

            targetObjectChildArray = tempTr.ToArray();
            //targetObjectChildArray = targetObject.GetComponentsInChildren<Transform>();
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
            bool tempOn = isMemoryLookOn;
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
                isMemoryLookOn = !tempOn;
            }


            string temp;

            if (tempOn)
            {
                long totalMemorySize = 0;
                for (int i = 0; i < targetObjectChildArray.Length; i++)
                {
                    //EditorGUILayout.LabelField(targetObjectChildArray[i].name);
                    Transform tr = targetObjectChildArray[i];

                    //유니티 개체에서 사용되는 네이티브메모리 양
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
                    temp = tr.name + " 오브젝트 총 메모리 사용량 " + GetSize(totalSize);

                    totalMemorySize += totalSize;
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(temp);
                    EditorGUILayout.Space();
                }

                temp = "할당된 그래픽 메모리량" + GetSize(Profiler.GetAllocatedMemoryForGraphicsDriver()) + "\n";
                temp += "모노 힙메모리량 크기" + GetSize(Profiler.GetMonoHeapSizeLong()) + "\n";
                temp += "모노가 사용하는 메모리량" + GetSize(Profiler.GetMonoUsedSizeLong()) + "\n";
                temp += "총 할당된 메모리량" + GetSize(Profiler.GetTotalAllocatedMemoryLong());

                EditorGUILayout.HelpBox(temp, MessageType.Info);

                //EditorGUILayout.LabelField("오브젝트 내의 총 메모리 사용량" + GetSize(totalMemorySize));                
                //EditorGUILayout.LabelField("할당된 그래픽 메모리량" + GetSize(Profiler.GetAllocatedMemoryForGraphicsDriver()));
                //EditorGUILayout.LabelField("모노 힙메모리량 크기" + GetSize(Profiler.GetMonoHeapSizeLong()));
                //EditorGUILayout.LabelField("모노가 사용하는 메모리량" + GetSize(Profiler.GetMonoUsedSizeLong()));
                //EditorGUILayout.LabelField("총 할당된 메모리량" + GetSize(Profiler.GetTotalAllocatedMemoryLong()));

                //신규
                //EditorGUILayout.HelpBox(getAvailableRAM(), MessageType.Info);
                //EditorGUILayout.HelpBox(GetSize(GC.GetTotalMemory(true)), MessageType.Info);






                if (GUILayout.Button("메모리" + btnContent))
                {
                    isMemoryLookOn = !tempOn;
                }
            }
        }

        //public string getAvailableRAM()
        //{
        //    return ramCounter.NextValue() + "Mb";
        //}


        /// <summary>
        /// 용량을 계산해주는 함수. 드라이브 용량을 넘겨주면 KB, MB, GB, TB로 바꿔준다.
        /// </summary>
        /// <param name="pDrvSize">계산할 용량</param>
        /// <returns>계산된 용량</returns>
        public static string GetSize(long pDrvSize)
        {
            return GetSize(pDrvSize, 3);
        }

        /// <summary>
        /// 용량을 계산해주는 함수. 드라이브 용량을 넘겨주면 KB, MB, GB, TB로 바꿔준다.
        /// </summary>
        /// <param name="pDrvSize">계산할 용량</param>
        /// <param name="pi">계산된 용량에서 표시할 소수점 자리수</param>
        /// <returns>계산된 용량</returns>
        public static string GetSize(long pDrvSize, int pi)
        {
            int mok = 0;
            double drvSize = pDrvSize;
            string Space = "Byte";
            string returnStr = "";

            while (drvSize > 1024.0)
            {
                drvSize /= 1024.0;
                mok++;
            }

            if (mok == 1)
                Space = "KB";
            else if (mok == 2)
                Space = "MB";
            else if (mok == 3)
                Space = "GB";
            else if (mok == 4)
                Space = "TB";

            if (mok != 0)
                if (pi == 1)
                    returnStr = string.Format("{0:F1}{1}", drvSize, Space);
                else if (pi == 2)
                    returnStr = string.Format("{0:F2}{1}", drvSize, Space);
                else if (pi == 3)
                    returnStr = string.Format("{0:F3}{1}", drvSize, Space);
                else
                    returnStr = string.Format("{0}{1}", Convert.ToInt32(drvSize), Space);
            else
                returnStr = string.Format("{0}{1}", Convert.ToInt32(drvSize), Space);

            return returnStr;
        }
    }
}