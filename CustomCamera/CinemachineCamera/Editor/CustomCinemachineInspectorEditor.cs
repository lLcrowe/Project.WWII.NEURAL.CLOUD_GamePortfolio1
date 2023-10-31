using lLCroweTool.Cinemachine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(CustomCinemachine))]
    public class CustomCinemachineInspectorEditor : Editor
    {
        private CustomCinemachine targetCinemachine;
        private Vector2 scrollPos;

        private void OnEnable()
        {
            targetCinemachine = (CustomCinemachine)target;
        }

        public override void OnInspectorGUI()
        {
            //스케일사이즈처리//1로 처리 안하고 있으면 값받는곳에서 작업할떄 밀리는 현상이 발생함
            //오브젝트배치에서는 이 작업을 안함 왜냐하면 그 오브젝트는 부모들을 계속 옮겨다니는데 이작업을 할시
            //위치값이 변동되는 문제가 발생함
            targetCinemachine.transform.localScale = Vector3.one;



            if (targetCinemachine.name != targetCinemachine.cinemachineID)
            {
                targetCinemachine.name = string.IsNullOrEmpty(targetCinemachine.cinemachineID) ? "-NullCinemachineID-" : $"-CinemachineID : {targetCinemachine.cinemachineID}-";
            }             
            EditorGUILayout.Space();            

            //전체재생
            EditorGUILayout.BeginHorizontal();
            string tempCotent = targetCinemachine.IsRun() ? "Pause" : "Play";
            if (GUILayout.Button(tempCotent) && Application.isPlaying)
            {
                targetCinemachine.ActionCamera();
            }

            if (GUILayout.Button("Stop"))
            {
                targetCinemachine.Stop();
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Add ObjectBatchInfo"))
            {
                if (!Application.isPlaying)
                {
                    if (EditorSceneManager.GetActiveScene().isDirty == false)
                    {
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    }
                }
                ObjectBatchInfo newBatchInfo = new GameObject().AddComponent<ObjectBatchInfo>();
                newBatchInfo.targetCinemachine = targetCinemachine;                
                newBatchInfo.InitTrObjPrefab(targetCinemachine.transform, targetCinemachine.transform);

                newBatchInfo.contentName = "EmtpyContent";
                newBatchInfo.curve = AnimationCurve.Linear(0, 0, 1, 1);
                newBatchInfo.startTime = 0;
                newBatchInfo.endTime = 3;

                ObjectBatchInfoInspectorEditor.CreateObjectBatchInfo(newBatchInfo);

                //전에게 있으면 그걸 기준으로 갱신
                if (targetCinemachine.cameraBatchList.Count != 0)
                {
                    //추가적인시간을 줌
                    ObjectBatchInfo prevBatchInfo = targetCinemachine.cameraBatchList[targetCinemachine.cameraBatchList.Count - 1];
                    newBatchInfo.startTime = prevBatchInfo.endTime;
                    newBatchInfo.endTime = newBatchInfo.startTime + 3;

                    newBatchInfo.startTr.position = prevBatchInfo.startTr.position;
                    newBatchInfo.endTr.position = prevBatchInfo.endTr.position;


                    //트랜스폼기준으로 변경해야됨
                    newBatchInfo.startTr.rotation = prevBatchInfo.startTr.rotation;
                    newBatchInfo.endTr.rotation = prevBatchInfo.endTr.rotation;

                    if (newBatchInfo.movePosType == MovePosType.Bezier)
                    {
                        newBatchInfo.startHandleTr.position = prevBatchInfo.startHandleTr.position;
                        newBatchInfo.endHandleTr.position = prevBatchInfo.endTr.position;
                    }
                }
                targetCinemachine.cameraBatchList.Add(newBatchInfo);
                return;
            }

            EditorGUILayout.Space(10);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.MinHeight(200));
            //선택하기
            for (int i = 0; i < targetCinemachine.cameraBatchList.Count; i++)
            {
                int index = i;

                //버그났을시 없애는것
                var batchObject = targetCinemachine.cameraBatchList[index];
                if (batchObject == null)
                {
                    EditorGUILayout.LabelField("Emtpy");
                    if (GUILayout.Button("Remove"))
                    {
                        targetCinemachine.cameraBatchList.RemoveAt(index);
                        break;
                    }
                    continue;
                }

                //버튼
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button($"Select -{batchObject.contentName}-"))
                {
                    Selection.activeGameObject = batchObject.gameObject;
                }

                if (GUILayout.Button($"X", GUILayout.Width(30)))
                {
                    targetCinemachine.cameraBatchList.Remove(batchObject);
                    DestroyImmediate(batchObject.gameObject);
                    if (!Application.isPlaying)
                    {
                        if (EditorSceneManager.GetActiveScene().isDirty == false)
                        {
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }
                    }
                    break;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                batchObject.startTime = EditorGUILayout.FloatField("시작", batchObject.startTime, GUILayout.MinWidth(30));
                batchObject.endTime = EditorGUILayout.FloatField("끝", batchObject.endTime, GUILayout.MinWidth(30));
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("★변경한거 있으면 눌려서 저장하기"))
            {
                EditorSceneManager.SaveScene(targetCinemachine.gameObject.scene);
            }
            base.OnInspectorGUI();
        }
      

        private void OnSceneGUI()
        {
            CustomCinemachineOnSceneGUI(targetCinemachine);
        }

        public static void CustomCinemachineOnSceneGUI(CustomCinemachine targetCinemachine)
        {
            for (int i = 0; i < targetCinemachine.cameraBatchList.Count; i++)
            {
                int index = i;
                ObjectBatchInfoInspectorEditor.ObjectBatchInfoOnSceneGUI(targetCinemachine.cameraBatchList[index], false);
            }
        }
    }
}
