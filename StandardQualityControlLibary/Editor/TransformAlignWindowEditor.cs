using lLCroweTool.QC.EditorOnly;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace lLCroweTool.QC.EditorOnly
{
    public class TransformAlignWindowEditor : EditorWindow
    {
        //AssetGridSetter의 정렬기능을 가져오는 함수

        private Transform trOffset;//첫번쨰 자손
        private List<GameObject> gameObjectList = new List<GameObject>();
        private GameObject[] gameObjectArray = new GameObject[0];
        public bool isModifyMode = false;

        public bool isWorldOffsetMode = true;
        public Vector3 posOffset = Vector3.right;//추가 처리        
        public Vector3 rotateOffSet = Vector3.zero;
        public Vector3 scaleOffset = Vector3.zero;        

        [MenuItem("lLcroweTool/TransformAlignWindowEditor _0")]
        public static void ShowWindow()
        {
            EditorWindow editorWindow = GetWindow(typeof(TransformAlignWindowEditor));
            editorWindow.titleContent.text = "트랜스폼 정렬 관리자";
            editorWindow.minSize = new Vector2(280, 410);
            editorWindow.maxSize = new Vector2(280, 410);
        }
        
        private void Update()
        {
            if (!isModifyMode)
            {
                return;
            }

            Vector3 originPos = trOffset.position;
            Quaternion originRotation = trOffset.rotation;
            Vector3 originScale = trOffset.localScale;

            for (int i = 0; i < gameObjectList.Count; i++)
            {
                Transform tr = gameObjectList[i].transform;
                if (tr == null)
                {
                    continue;
                }
              
                Vector3 addPos = posOffset * i;
                Quaternion addQuaternion = Quaternion.Euler(rotateOffSet * i);
                Vector3 addScale = scaleOffset * i;

                if (isWorldOffsetMode)
                {
                    tr.SetPositionAndRotation(originPos + addPos, originRotation * addQuaternion);
                }
                else
                {
                    tr.SetLocalPositionAndRotation(originPos + addPos, originRotation * addQuaternion);
                }
                tr.localScale = originScale + addScale;
            }
        }
        
        private void OnGUI()
        {   
            if (gameObjectList.Count != 0)
            {
                string tempContent = $"{gameObjectList[0].name} <= *기준점\n";
                for (int i = 1; i < gameObjectList.Count; i++)
                {
                    //최대4개까지만 보여줌
                    if (i == 5)
                    {
                        tempContent += "...";
                        break;
                    }
                    tempContent += gameObjectList[i].name + "\n";
                }
                tempContent = tempContent.Substring(0, tempContent.Length - 1);

                
                EditorGUILayout.HelpBox($"{tempContent}", MessageType.Info);               
                lLcroweUtilEditor.TransformDataShow(trOffset, "루트");
                EditorGUILayout.Space();
            }

            string buttonContent = isModifyMode ? "-=수정모드=-" : "-=기본모드=-";
            EditorGUILayout.HelpBox($"{buttonContent}", MessageType.Info);
            if (GUILayout.Button($"모드변경"))
            {
                isModifyMode = !isModifyMode;
            }

            isWorldOffsetMode = EditorGUILayout.Toggle("월드오프셋 기준 여부", isWorldOffsetMode);
            posOffset = EditorGUILayout.Vector3Field("위치오프셋", posOffset);
            rotateOffSet = EditorGUILayout.Vector3Field("회전오프셋", rotateOffSet);
            scaleOffset = EditorGUILayout.Vector3Field("스케일오프셋", scaleOffset);
            SceneView.RepaintAll();
        }

     

        //private void OnHierarchyChange()
        //{
        //    //계층구조에서 변환이 있어야지 호출
        //    Debug.Log("OnHierarchyChange");
        //}

        private void OnSelectionChange()
        {
            //하이어라키와 프로젝트 창에서 선택되면 작동됨
            //Debug.Log("OnSelectionChange");

            //선택된것들 다가져옴
            gameObjectArray = Selection.gameObjects;//이거 클릭한순서되로 주지않음//리스트추가시켜서 안변하게 처리

            if (gameObjectArray.Length == 0)
            {
                //리셋
                gameObjectList.Clear();
                trOffset = null;
            }
            else
            {
                //다른거 선택
                if (gameObjectArray.Length == 1 && gameObjectList.Count > 0)
                {
                    if (gameObjectArray[0] != gameObjectList[0])
                    {
                        //리셋
                        gameObjectList.Clear();
                        trOffset = null;
                    }
                }

                //빼기체크
                if (gameObjectArray.Length < gameObjectList.Count)
                {
                    //없는걸 빼기
                    List<GameObject> tempList = new List<GameObject>(gameObjectList);                   

                    for (int i = 0; i < gameObjectArray.Length; i++)
                    {
                        GameObject target = gameObjectArray[i];
                        if (tempList.Contains(target))
                        {
                            tempList.Remove(target);
                        }
                    }

                    for (int i = 0; i < tempList.Count; i++)
                    {
                        GameObject target = tempList[i];
                        gameObjectList.Remove(target);
                    }
                }

                //추가선택//체크사항 <===작업중이던거
                for (int i = 0; i < gameObjectArray.Length; i++)
                {
                    GameObject target = gameObjectArray[i];
                    if (gameObjectList.Contains(target))
                    {
                        continue;
                    }
                    gameObjectList.Add(target);
                }
                trOffset = gameObjectList[0].transform;
                Debug.Log(trOffset.name);
            }

            isModifyMode = false;

            //for (int i = 0; i < gameObjectArray.Length; i++)
            //{
            //    Debug.Log(gameObjectArray[i].name);
            //}
            this.Repaint();
        }


    }
}
