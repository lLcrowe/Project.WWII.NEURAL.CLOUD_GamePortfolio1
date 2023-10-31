using lLCroweTool.Cinemachine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(ObjectBatchInfo))]
    public class ObjectBatchInfoInspectorEditor : Editor
    {
        private ObjectBatchInfo targetObjectBatchInfo;

        //수정관련
        private static bool isModify = false;
        private static ObjectBatchInfo modifyObjectBatchInfo;

        private static bool isCopy;
        private static bool isPosCopy;
        private static Vector3 copyPos;
        private static bool isAngleCopy;
        private static Quaternion copyAngle;

        private void OnEnable()
        {
            targetObjectBatchInfo = (ObjectBatchInfo)target;
            ResetModify();
        }


        public override void OnInspectorGUI()
        {   
            ObjectBatchInfoOnInspectorGUI(targetObjectBatchInfo);
            SceneView.RepaintAll();
        }

        private void OnSceneGUI()
        {
            //부모 전체처리하느 곳에서 호출
            CustomCinemachineInspectorEditor.CustomCinemachineOnSceneGUI(targetObjectBatchInfo.targetCinemachine);
            ObjectBatchInfoOnSceneGUI(targetObjectBatchInfo, isModify);
        }

        public static void ObjectBatchInfoOnInspectorGUI(ObjectBatchInfo cameraBatchInfo)
        {
            //갱신
            UpdateCheckBatchInfo(cameraBatchInfo);
            CustomCinemachine targetCinemachine = cameraBatchInfo.targetCinemachine;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("시네머신 선택"))
            {
                Selection.activeObject = targetCinemachine.gameObject;
            }
            if (GUILayout.Button("★끄기전에 꼭 눌려서 저장하기"))
            {
                EditorSceneManager.SaveScene(targetCinemachine.gameObject.scene);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            string modifyStateContent = isModify ? "Modify-On" : "Modify-Off";
            if (GUILayout.Button(modifyStateContent))
            {
                //수정할시 잠금상태 발동
                Modify(cameraBatchInfo);
            }


            if (GUILayout.Button("SelectPlay"))
            {
                targetCinemachine.timer = cameraBatchInfo.startTime;
                targetCinemachine.ActionCamera();
            }
            if (GUILayout.Button("Delete"))
            {
                targetCinemachine.cameraBatchList.Remove(cameraBatchInfo);
                DestroyImmediate(cameraBatchInfo.gameObject);
                return;
            }
            EditorGUILayout.EndHorizontal();


            if (isModify)
            {
                if (!Application.isPlaying)
                {
                    if (EditorSceneManager.GetActiveScene().isDirty == false)
                    {
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    }
                }

                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("------ModifyOn------");
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("시작포인트 포커싱"))
                {
                    Selection.activeTransform = cameraBatchInfo.startTr;
                    SceneView.lastActiveSceneView.FrameSelected();
                }

                if (GUILayout.Button("배치포인트 포커싱"))
                {
                    Selection.activeTransform = cameraBatchInfo.transform;
                    SceneView.lastActiveSceneView.FrameSelected();
                }

                if (GUILayout.Button("끝포인트 포커싱"))
                {
                    Selection.activeTransform = cameraBatchInfo.endTr;
                    SceneView.lastActiveSceneView.FrameSelected();
                }
                EditorGUILayout.EndHorizontal();


                if (cameraBatchInfo.movePosType == MovePosType.Bezier)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("시작핸들포인트 포커싱"))
                    {
                        Selection.activeTransform = cameraBatchInfo.startHandleTr;
                        SceneView.lastActiveSceneView.FrameSelected();
                    }

                    if (GUILayout.Button("끝핸들포인트 포커싱"))
                    {
                        Selection.activeTransform = cameraBatchInfo.endHandleTr;
                        SceneView.lastActiveSceneView.FrameSelected();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                
                //선택한 오브젝트의 트랜스폼을 보여주기
                if (Selection.activeTransform != null)
                {
                    var targetTr = Selection.activeTransform;
                    EditorGUILayout.LabelField($"{targetTr.name}");
                    targetTr.position = EditorGUILayout.Vector3Field("위치", targetTr.position);
                    targetTr.eulerAngles = EditorGUILayout.Vector3Field("회전", targetTr.eulerAngles);

                                        
                    EditorGUILayout.BeginHorizontal();
                    string title = isPosCopy ? "Paste" : "Copy";
                    if (GUILayout.Button($"{title} Pos"))
                    {
                        if (isPosCopy)
                        {
                            targetTr.position = copyPos;
                        }
                        else
                        {
                            copyPos = targetTr.position;
                            isPosCopy = true;
                        }
                    }
                    title = isAngleCopy ? "Paste" : "Copy";
                    if (GUILayout.Button($"{title} Angle"))
                    {
                        if (isAngleCopy)
                        {
                            targetTr.rotation = copyAngle;
                        }
                        else
                        {
                            copyAngle = targetTr.rotation;
                            isAngleCopy = true;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                                      

                    EditorGUILayout.BeginHorizontal();                    
                    if (isPosCopy||isAngleCopy)
                    {
                        if (GUILayout.Button("ResetCopy"))
                        {
                            ResetCopy();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("위치 스왑"))
                {
                    Vector3 tempPos = cameraBatchInfo.startTr.position;
                    cameraBatchInfo.startTr.position = cameraBatchInfo.endTr.position;
                    cameraBatchInfo.endTr.position = tempPos;

                    //핸들도 스왑
                    if (cameraBatchInfo.movePosType == MovePosType.Bezier)
                    {
                        tempPos = cameraBatchInfo.startHandleTr.position;
                        cameraBatchInfo.startHandleTr.position = cameraBatchInfo.endHandleTr.position;
                        cameraBatchInfo.endHandleTr.position = tempPos;
                    }
                }
                if (GUILayout.Button("앵글 스왑"))
                {
                    Quaternion tempAngle = cameraBatchInfo.startTr.rotation;
                    cameraBatchInfo.startTr.rotation = cameraBatchInfo.endTr.rotation;
                    cameraBatchInfo.endTr.rotation = tempAngle;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField("--------------------");
                EditorGUILayout.Space(10);
            }


            cameraBatchInfo.contentName = EditorGUILayout.TextField("오브젝트배치관련 설명", cameraBatchInfo.contentName);

            

            EditorGUILayout.HelpBox("부모가 될 타겟입니다. 지정될시 로컬, 안됫을시 월드포지션으로 계산됩니다.", MessageType.Info);
            lLcroweUtilEditor.ObjectFieldAndNullButton("따라다닐 오브젝트", ref cameraBatchInfo.followObject, true);
            //후에 있을 기능
            if (cameraBatchInfo.followObject != null)
            {
                cameraBatchInfo.isFixedRotateForFollowObject = EditorGUILayout.Toggle("따라다닐오브젝트에 회전값을 고정시키겠습니까?", cameraBatchInfo.isFixedRotateForFollowObject);
            }



            EditorGUILayout.HelpBox("봐라볼 대상입니다. 지정될시 세팅한 회전값대신 봐라볼대상으로 회전되며.\n 회전축은 월드포지션기준이지만 따라갈대상이 존재할시 따라갈대상의 회전축을 사용합니다.", MessageType.Info);
            lLcroweUtilEditor.ObjectFieldAndNullButton("봐라볼 오브젝트", ref cameraBatchInfo.lookAtObject, true);
            cameraBatchInfo.lookAtOffset = EditorGUILayout.Vector3Field("봐라볼 오프셋", cameraBatchInfo.lookAtOffset);

            EditorGUILayout.LabelField("------------------------");
            if (cameraBatchInfo.controlTarget == null)
            {
                EditorGUILayout.HelpBox("컨트롤할 대상이 없습니다.(필수조건)", MessageType.Error);
            }
            lLcroweUtilEditor.ObjectFieldAndNullButton("컨트롤할 타겟", ref cameraBatchInfo.controlTarget, true);


            EditorGUILayout.LabelField("------------------------");
            cameraBatchInfo.curve = EditorGUILayout.CurveField("커브", cameraBatchInfo.curve);
            EditorGUILayout.BeginHorizontal();
            cameraBatchInfo.startTime = EditorGUILayout.FloatField("StartTime", cameraBatchInfo.startTime);
            cameraBatchInfo.endTime = EditorGUILayout.FloatField("EndTime", cameraBatchInfo.endTime);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            cameraBatchInfo.movePosType = (MovePosType)EditorGUILayout.EnumPopup("이동타입", cameraBatchInfo.movePosType);
        }

        public static void ObjectBatchInfoOnSceneGUI(ObjectBatchInfo cameraBatchInfo, bool isModify)
        {

            if (cameraBatchInfo == null)
            {
                return;
            }

            //자기자신위치를 체크
            lLcroweUtilEditor.DrawSphere(cameraBatchInfo.transform.position);


            Vector3 startPos = cameraBatchInfo.startTr.position;
            Vector3 endPos = cameraBatchInfo.endTr.position;
            Quaternion startAngle = cameraBatchInfo.startTr.rotation;
            Quaternion endAngle = cameraBatchInfo.endTr.rotation;

            //봐라볼대상이 존재하면//덮어쓰기
            if (cameraBatchInfo.lookAtObject != null)
            {
                //쫒아갈대상이 있으면 지정
                Vector3 upAxis = Vector3.up;
                if (cameraBatchInfo.followObject != null)
                {
                    upAxis = cameraBatchInfo.followObject.up;
                }

                Vector3 lookPos = cameraBatchInfo.lookAtObject.position;
                Vector3 resultLookPos = lookPos + cameraBatchInfo.lookAtOffset;
                Vector3 dir = resultLookPos - startPos;
                startAngle = Quaternion.LookRotation(dir, upAxis);

                dir = resultLookPos - endPos;
                endAngle = Quaternion.LookRotation(dir, upAxis);

                //쫒아갈대상위치와 회전축, 오프셋그려주기
                //x,y,z축을 그려주기
                Handles.color = Color.blue;
                Handles.DrawWireCube(lookPos, Vector3.one * 0.5f);

                Handles.color = Color.yellow;
                Handles.DrawLine(resultLookPos, resultLookPos + (upAxis * 0.5f));

                Handles.color = Color.green;
                Handles.DrawWireDisc(resultLookPos, Vector3.up, 0.3f);
                Handles.DrawWireDisc(resultLookPos, Vector3.right, 0.3f);
                Handles.DrawWireDisc(resultLookPos, Vector3.forward, 0.3f);
            }

            AnimationCurve curve = cameraBatchInfo.curve;
            Vector3 startSize = Vector3.one * 0.8f;
            Vector3 endSize = Vector3.one * 0.5f;
            //시작위치
            Handles.color = Color.white;
            Handles.DrawWireCube(startPos, startSize);
            lLcroweUtilEditor.DrawConn(startPos, startAngle);
            lLcroweUtilEditor.DrawLine(startPos, startAngle, 5f, Vector3.forward);
            lLcroweUtilEditor.DrawLine(startPos, startAngle, 1f, Vector3.up);


            //끝나는위치
            Handles.color = Color.red;
            Handles.DrawWireCube(endPos, endSize);
            lLcroweUtilEditor.DrawConn(endPos, endAngle);
            lLcroweUtilEditor.DrawLine(endPos, endAngle, 5f, Vector3.forward);
            lLcroweUtilEditor.DrawLine(endPos, endAngle, 1f, Vector3.up);

            //중간경로 그리기
            switch (cameraBatchInfo.movePosType)
            {
                case MovePosType.Linear:
                    Handles.color = Color.green;
                    Handles.DrawLine(startPos, endPos);


                    //중간카메라 그리기
                    Vector3 middlePos = Vector3.Lerp(startPos, endPos, curve.Evaluate(0.25f));
                    Quaternion middleAngle = Quaternion.Lerp(startAngle, endAngle, curve.Evaluate(0.25f));
                    lLcroweUtilEditor.DrawConn(middlePos, middleAngle);
                    lLcroweUtilEditor.DrawLine(middlePos, middleAngle, 5f, Vector3.forward);
                    lLcroweUtilEditor.DrawLine(middlePos, middleAngle, 1f, Vector3.up);

                    middlePos = Vector3.Lerp(startPos, endPos, curve.Evaluate(0.5f));
                    middleAngle = Quaternion.Lerp(startAngle, endAngle, curve.Evaluate(0.5f));
                    lLcroweUtilEditor.DrawConn(middlePos, middleAngle);
                    lLcroweUtilEditor.DrawLine(middlePos, middleAngle, 5f, Vector3.forward);
                    lLcroweUtilEditor.DrawLine(middlePos, middleAngle, 1f, Vector3.up);

                    middlePos = Vector3.Lerp(startPos, endPos, curve.Evaluate(0.75f));
                    middleAngle = Quaternion.Lerp(startAngle, endAngle, curve.Evaluate(0.75f));
                    lLcroweUtilEditor.DrawConn(middlePos, middleAngle);
                    lLcroweUtilEditor.DrawLine(middlePos, middleAngle, 5f, Vector3.forward);
                    lLcroweUtilEditor.DrawLine(middlePos, middleAngle, 1f, Vector3.up);

                    break;
                case MovePosType.Bezier:
                    Vector3 firPos = Vector3.zero;
                    Vector3 startHandlePos = cameraBatchInfo.startHandleTr.position;
                    Vector3 endHandlePos = cameraBatchInfo.endHandleTr.position;

                    firPos.x = lLcroweUtil.FourPointBezier(startPos.x, startHandlePos.x, endHandlePos.x, endPos.x, 0);
                    firPos.y = lLcroweUtil.FourPointBezier(startPos.y, startHandlePos.y, endHandlePos.y, endPos.y, 0);
                    firPos.z = lLcroweUtil.FourPointBezier(startPos.z, startHandlePos.z, endHandlePos.z, endPos.z, 0);

                    //곡선보여주는 함수
                    for (int j = 0; j <= 20; j++)
                    {
                        Vector3 newPos = Vector3.zero;
                        float norValue = lLcroweUtil.MinMaxNormalize(0, 20, j);
                        float curveValue = curve.Evaluate(norValue);
                        newPos.x = lLcroweUtil.FourPointBezier(startPos.x, startHandlePos.x, endHandlePos.x, endPos.x, curveValue);
                        newPos.y = lLcroweUtil.FourPointBezier(startPos.y, startHandlePos.y, endHandlePos.y, endPos.y, curveValue);
                        newPos.z = lLcroweUtil.FourPointBezier(startPos.z, startHandlePos.z, endHandlePos.z, endPos.z, curveValue);

                        //경로끼리선연결
                        Handles.color = Color.green;
                        Handles.DrawLine(firPos, newPos);
                        lLcroweUtilEditor.DrawSphere(newPos);

                        //중간카메라그리기
                        if (j % 5 == 0)
                        {
                            Quaternion tempMiddleAngle = Quaternion.Lerp(startAngle, endAngle, curveValue);
                            lLcroweUtilEditor.DrawConn(newPos, tempMiddleAngle);
                            lLcroweUtilEditor.DrawLine(newPos, tempMiddleAngle, 5f, Vector3.forward);
                            lLcroweUtilEditor.DrawLine(newPos, tempMiddleAngle, 1f, Vector3.up);
                        }

                        firPos = newPos;
                    }
                    break;
            }


            //컨트롤오브젝트를 보여주는 기능
            if (cameraBatchInfo.controlTarget != null)
            {
                //Handles.color = Color.green;
                lLcroweUtilEditor.DrawLine(cameraBatchInfo.controlTarget.position, cameraBatchInfo.controlTarget.rotation, 5f, Vector3.forward);
                lLcroweUtilEditor.DrawLine(cameraBatchInfo.controlTarget.position, cameraBatchInfo.controlTarget.rotation, 1f, Vector3.up);
            }





            //이거 안하고 보여주기면 그렇게 보여주는게 맞아보이는데

            //부모 위치를 옮겨버리자
            //그러면 오히려 편해지겠다

            //팔로우니까

            //존재할시 시작과 끝위치를 변경해야됨
            if (cameraBatchInfo.followObject == null)
            {
                //배치오브젝트의 부모가 시네머신이 아닐시 옮김
                if (cameraBatchInfo.transform.parent != cameraBatchInfo.targetCinemachine.transform)
                {
                    cameraBatchInfo.transform.SetParent(cameraBatchInfo.targetCinemachine.transform);
                    cameraBatchInfo.transform.localPosition = Vector3.zero;
                }
            }
            else
            {
                //배치오브젝트의 부모가 팔로우오브젝트가 아닐시 옮김
                if (cameraBatchInfo.transform.parent != cameraBatchInfo.followObject.transform)
                {
                    cameraBatchInfo.transform.SetParent(cameraBatchInfo.followObject.transform);
                    cameraBatchInfo.transform.localPosition = Vector3.zero;
                }
            }

            //위치수정부분
            if (isModify)
            {
                Handles.Label(cameraBatchInfo.startTr.position + Vector3.one + (Vector3.up * 0.2f), $"{cameraBatchInfo.contentName} 시작포인트{cameraBatchInfo.startTr.position}");
                Handles.Label(cameraBatchInfo.endTr.position + Vector3.one + (Vector3.up * 0.2f), $"{cameraBatchInfo.contentName} 끝포인트{cameraBatchInfo.endTr.position}");
                lLcroweUtilEditor.TransformHandle(cameraBatchInfo.startTr);
                lLcroweUtilEditor.TransformHandle(cameraBatchInfo.endTr);

                //핸들
                if (modifyObjectBatchInfo.movePosType == MovePosType.Bezier)
                {
                    Handles.Label(modifyObjectBatchInfo.startHandleTr.position + Vector3.right, $"수정타겟 \n{cameraBatchInfo.contentName} 시작핸들포인트");
                    Handles.Label(modifyObjectBatchInfo.endHandleTr.position + Vector3.right, $"수정타겟 \n{cameraBatchInfo.contentName} 끝핸들포인트");
                    lLcroweUtilEditor.TransformHandle(cameraBatchInfo.startHandleTr);
                    lLcroweUtilEditor.TransformHandle(cameraBatchInfo.endHandleTr);
                }
            }
        }

        private static string startTrName = "StartTr";
        private static string endTrName = "EndTr";
        private static string startHandleTrName = "StartHandleTr";
        private static string endHandleTrName = "EndHandleTr";

        //배치정보 생성
        public static void CreateObjectBatchInfo(ObjectBatchInfo objectBatchInfo)
        {
            //비어있는 트랜스폼 다 배치하기
            //배치오브젝트부모체크
            CreateTr(ref objectBatchInfo.startTr, objectBatchInfo, startTrName);
            CreateTr(ref objectBatchInfo.endTr, objectBatchInfo, endTrName);
            objectBatchInfo.endTr.position += Vector3.right;
        }

        //배치정보 삭제
        public static void DestroyObjectBatchInfo(ObjectBatchInfo objectBatchInfo)
        {
            DestroyTr(ref objectBatchInfo.startTr);
            DestroyTr(ref objectBatchInfo.endTr);
            DestroyTr(ref objectBatchInfo.startHandleTr);
            DestroyTr(ref objectBatchInfo.endHandleTr);
        }

        //배치정보의 라인처리 체크
        public static void UpdateCheckBatchInfo(ObjectBatchInfo objectBatchInfo)
        {
            //이름변경
            string targetName = objectBatchInfo.contentName;
            ChangeTrName(objectBatchInfo.transform, $"Cinemachine_{objectBatchInfo.targetCinemachine.cinemachineID}_{targetName}");
            ChangeTrName(objectBatchInfo.startTr, $"{startTrName}_{targetName}");
            ChangeTrName(objectBatchInfo.endTr, $"{endTrName}_{targetName}");
            if (objectBatchInfo.movePosType == MovePosType.Linear)
            {
                //핸들러삭제
                DestroyTr(ref objectBatchInfo.startHandleTr);
                DestroyTr(ref objectBatchInfo.endHandleTr);
            }
            else
            {
                //핸들러추가
                CreateTr(ref objectBatchInfo.startHandleTr, objectBatchInfo, startHandleTrName);
                CreateTr(ref objectBatchInfo.endHandleTr, objectBatchInfo, endHandleTrName);
                ChangeTrName(objectBatchInfo.startHandleTr, $"{startHandleTrName}_{targetName}");
                ChangeTrName(objectBatchInfo.endHandleTr, $"{endHandleTrName}_{targetName}");
            }
        }


        //없으면 생성
        private static void CreateTr(ref Transform targetTr, ObjectBatchInfo objectBatchInfo, string gameObjectName)
        {
            if (targetTr == null)
            {
                targetTr = new GameObject().transform;
                targetTr.name = gameObjectName;
                targetTr.SetParent(objectBatchInfo.transform);
            }
        }

        //있으면 지우기
        private static void DestroyTr(ref Transform targetTr)
        {
            if (targetTr)
            {
                GameObject.DestroyImmediate(targetTr.gameObject);
            }
        }

        private static void ChangeTrName(Transform targetTr, string contetName)
        {
            if (targetTr.name != contetName)
            {
                targetTr.name = contetName;
            }
        }


        private static void ResetCopy()
        {   
            isPosCopy = false;
            isAngleCopy = false;
            copyPos = Vector3.zero;
            copyAngle = Quaternion.identity;
        }


        /// <summary>
        /// 수정버튼 기능
        /// </summary>
        /// <param name="targetObjectBatchInfo">오브젝트 배치 정보</param>
        /// <param name="index">인덱스</param>
        private static void Modify(ObjectBatchInfo targetObjectBatchInfo)
        {
            if (isModify)
            {
                //수정중일때
                if (modifyObjectBatchInfo == targetObjectBatchInfo)
                {
                    //같은 타겟이면
                    //리셋
                    ResetModify();
                    Selection.activeGameObject = targetObjectBatchInfo.gameObject;
                    ActiveEditorTracker.sharedTracker.isLocked = false;
                    return;
                }
                //다르면세팅
                modifyObjectBatchInfo = targetObjectBatchInfo;
            }
            else
            {
                //수정중이 아닐떄//잠그기
                isModify = true;
                modifyObjectBatchInfo = targetObjectBatchInfo;
                ActiveEditorTracker.sharedTracker.isLocked = true;
            }
        }

        /// <summary>
        /// 수정 리셋
        /// </summary>
        private static void ResetModify()
        {
            isModify = false;
            modifyObjectBatchInfo = null;
        }
    }
}