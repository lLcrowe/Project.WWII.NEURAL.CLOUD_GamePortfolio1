using lLCroweTool.DataBase;
using lLCroweTool.TileMap;
using lLCroweTool.TileMap.HexTileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace lLCroweTool.QC.EditorOnly
{

    //타일맵에 유닛들을 배치하기 위한 에디터
    //따로 오브젝트안하고 유닛으로 통일시킴

    public class TileMapBatchWindowEditor : EditorWindow
    {
        public enum TileMapBatchMode
        {   
            Modify,
            Create,
            Destroy,
        }

        [MenuItem("lLcroweTool/TileMapBatchWindowEditor _9")]
        public static void ShowWindow()
        {
            EditorWindow editorWindow = GetWindow(typeof(TileMapBatchWindowEditor));
            editorWindow.titleContent.text = "타일맵 배치 관리자";
            editorWindow.minSize = new Vector2(325,  625);
            editorWindow.maxSize = new Vector2(325, 625);
            
        }
                
        List<UnitObjectInfo> batchObjectInfoList = new List<UnitObjectInfo>();
        List<Custom3DHexTileMap> custom3DHexTileMapList = new List<Custom3DHexTileMap>();
        //string path = "Assets/A0/Resources/UnitDataFolder";
        string path = $"Assets/A0/Resources/GameDataBaseFolder/UnitObjectInfo";

        Texture[] previewTexture = new Texture[0];

        private void OnEnable()
        {
            //특정경로에 있는 배치오브젝트들의 정보들을 가져와서 캐싱하기
            //유닛경로에서 찾아와서 배치할수 있게 제작
            Object[] dataArray = lLcroweUtilEditor.GetScriptableObjectFile<UnitObjectInfo>(path, "*.asset");
            batchObjectInfoList.Clear();

            for (int i = 0; i < dataArray.Length; i++)
            {
                UnitObjectInfo UnitDataInfo = dataArray[i] as UnitObjectInfo;
                if (UnitDataInfo == null)
                {
                    continue;
                }
                batchObjectInfoList.Add(UnitDataInfo);
            }

            //코루틴으로 프리퓨 이미지가져오기
            EditorCoroutine editorCoroutine = new EditorCoroutine(GetAssetPreviewTexture(batchObjectInfoList));

            //현재하이어라키에 있는 타일맵들을 가져와서 처리
            Custom3DHexTileMap[] custom3DHexTileMapArray = Transform.FindObjectsOfType<Custom3DHexTileMap>();

            custom3DHexTileMapList.Clear();
            custom3DHexTileMapList.AddRange(custom3DHexTileMapArray);

            //씬뷰의 업데이트 집어넣기
            //UnityEditor.Undo.undoRedoPerformed += ;
            SceneView.duringSceneGui += UpdateSceneGUI;

            Selection.activeObject = null;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= UpdateSceneGUI;
            ResetTargetBatchObject();
        }
        
        
        private IEnumerator GetAssetPreviewTexture(List<UnitObjectInfo> valueList)
        {
            previewTexture = new Texture[valueList.Count];
            for (int i = 0; i < valueList.Count; i++)
            {   
                UnitObjectInfo batchObjectInfo = valueList[i];
                if (batchObjectInfo.unitPrefab == null)
                {
                    continue;
                }
                int instance = batchObjectInfo.unitPrefab.gameObject.GetInstanceID();
                UnitObject_Base tileMapBatchObect = batchObjectInfo.unitPrefab;
                do
                {
                    //프리뷰미리보기                    
                    if (!AssetPreview.IsLoadingAssetPreviews())
                    //if (!AssetPreview.IsLoadingAssetPreview(instance))
                    {   
                        var texture = AssetPreview.GetAssetPreview(tileMapBatchObect.gameObject);//게임오브젝트로해야지 해당 원하는 프리뷰가 나옴
                        if (texture != null)
                        {
                            previewTexture[i] = texture;
                            //previewTexture[i] = AssetPreview.GetMiniThumbnail(tileMapBatchObect.gameObject);
                            break;
                        }
                    }

                    yield return null;
                } while (true);
            }
        }

        TileMapBatchMode tileMapBatchMode;
        Custom3DHexTileMap targetCustom3DHexTileMap;
        LayerMask targetCheckLayer;

        Vector2 tileMapScrollPos;
        Vector2 batchDataScollPos;
        
        float batchAngle = 0;

        UnitObject_Base targetTileMapBatchObect;//배치용도의 오브젝트
        Vector3 movePos;//움직일 위치
        HexTileObject selectHexTileObject;//수정할려고 선택된 헥스타일

        /// <summary>
        /// 타겟 배치오브젝트 리셋함수
        /// </summary>
        private void ResetTargetBatchObject()
        {
            if (targetTileMapBatchObect != null)
            {
                DestroyImmediate(targetTileMapBatchObect.gameObject);
                targetTileMapBatchObect = null;
            }
        }
        
        private void OnGUI()
        {
            //Debug.Log("OnGuI");
            //모든타일맵들을 다 검색해와서 가져오는것도?
            //타일맵 탐색영역//토글//스크롤
            GUILayout.Label("찾은 타일맵 : " + custom3DHexTileMapList.Count.ToString() + "개");
            tileMapScrollPos = GUILayout.BeginScrollView(tileMapScrollPos, false, true, GUILayout.Height(100));

            for (int i = 0; i < custom3DHexTileMapList.Count; i++)
            {
                var tileMap = custom3DHexTileMapList[i];
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label($"-={tileMap.name}=-");

                if (targetCustom3DHexTileMap == tileMap)
                {
                    if (GUILayout.Button("맵데이터추출하기"))
                    {
                        //데이터를 추출해서 맵 데이터형태로 변경
                        ExportMapData();
                    }
                }

                if (GUILayout.Button("타겟팅하기", GUILayout.Width(100)))
                {
                    targetCustom3DHexTileMap = tileMap;
                    targetCheckLayer = tileMap.gameObject.layer;
                    Selection.activeObject = null;
                }
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space(10);

            //타겟팅된 타일맵 보여주는 영역
            string title = targetCustom3DHexTileMap == null ? "타겟팅된 타일맵이 없습니다" : $"{targetCheckLayer.ToString()}/{LayerMask.LayerToName(targetCustom3DHexTileMap.gameObject.layer)} \n동일여부:{targetCheckLayer == targetCustom3DHexTileMap.gameObject.layer} \n{targetCustom3DHexTileMap.name} ({LayerMask.LayerToName(targetCustom3DHexTileMap.gameObject.layer)})이 타겟팅되있습니다.";
            EditorGUILayout.HelpBox(title, MessageType.Info);

            if (targetCustom3DHexTileMap == null)
            {
                return;
            }
            EditorGUILayout.Space(10);

            //모드설정
            EditorGUILayout.HelpBox("Mode :" + tileMapBatchMode.ToString(), MessageType.Info);
            //버튼추가
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("파괴모드"))
            {
                tileMapBatchMode = TileMapBatchMode.Destroy;
                ResetTargetBatchObject();
                Selection.activeObject = null;
            }
            if (GUILayout.Button("수정모드"))
            {
                tileMapBatchMode = TileMapBatchMode.Modify;
                ResetTargetBatchObject();
                Selection.activeObject = null;
            }
            EditorGUILayout.EndHorizontal();

            //수정이나 생성할때 회전관련한 기능 만들기
            //어덯게 만들까 여기서는 회전만하고 (인게임에서 R누르면 회전하듯이)
            //배치는 다른곳에서
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("회전R(R)"))
            {
                RotateAngle(true);
            }
            if (GUILayout.Button("회전L(T)"))
            {
                RotateAngle(false);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            //타겟팅이 완료됫으면 이제 배치에디터가 제대로 보이게
            //배치될 대상들이보이게
            //선택할시 배치상태로 변경됨
            //하나복제하고 끌고다님
            //체크해보니 컴포넌트하나가 더필요//태그용도로 쓰일대상
            //배치용도의 컴포넌트

            GUILayout.Label("찾은 타일배치 오브젝트수 : " + batchObjectInfoList.Count.ToString());
            batchDataScollPos = GUILayout.BeginScrollView(batchDataScollPos, false, true, GUILayout.Height(300));

            for (int i = 0; i < batchObjectInfoList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < 2; j++)
                {   
                    var batchObjectInfo = batchObjectInfoList[i];

                    if (batchObjectInfo.unitPrefab == null)
                    {
                        continue;
                    }

                    EditorGUILayout.BeginVertical();
                    GUILayout.Label($"{batchObjectInfo.name}");

                    //AssetPreview를 비동기로 가져온것
                    Texture texture = previewTexture[i];
                    int iconSize = 150;

                    if (GUILayout.Button(texture, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
                    {
                        //배치시작
                        //생성해서 따라가게 만들어주기
                        //카드배치체크하기
                        ResetTargetBatchObject();
                        tileMapBatchMode = TileMapBatchMode.Create;
                        targetTileMapBatchObect = (UnitObject_Base)PrefabUtility.InstantiatePrefab(batchObjectInfo.unitPrefab);
                        targetTileMapBatchObect.unitObjectInfo = batchObjectInfo;
                        Vector3 eular = targetTileMapBatchObect.transform.rotation.eulerAngles;
                        batchAngle = targetCustom3DHexTileMap.createTileAxisType == lLcroweUtil.HexTileMatrix.CreateTileAxisType.XY ? eular.z : eular.y;
                    }
                    EditorGUILayout.EndVertical();

                    //인덱스체크
                    if (i >= batchObjectInfoList.Count - 1)
                    {
                        break;
                    }

                    if (j != 1)
                    {
                        i++;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }

        string exportPath = "TileMapInfoData";


        /// <summary>
        /// 맵추출하기
        /// </summary>
        private void ExportMapData()
        {
            TileMapBatchInfo tileMapInfo = new TileMapBatchInfo();
            tileMapInfo.labelNameOrTitle = targetCustom3DHexTileMap.name;
            var tileMapInfoBible = tileMapInfo.tileMapInfoBible;
            var hexTileObjectBible = targetCustom3DHexTileMap.hexTileObjectBible;

            foreach (var item in hexTileObjectBible)
            {
                //있는 요소들만 집어넣기
                if (item.Value.BatchUnitObject == null)
                {
                    continue;
                }

                //타일위치//이름
                tileMapInfoBible.Add(item.Key,item.Value.BatchUnitObject.unitObjectInfo.labelNameOrTitle);
            }

            tileMapInfoBible.SyncDictionaryToList();
            lLcroweUtilEditor.CreateDataObject(ref tileMapInfo, tileMapInfo.labelNameOrTitle, exportPath, null);
        }

        private static bool IsLeftMouseDown =>
            Event.current.type == EventType.MouseDown && Event.current.button == 0;

        private static bool IsRightMouseDown =>
            Event.current.type == EventType.MouseDown && Event.current.button == 1;

        private static bool IsEscapeKeyDown =>
            Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Escape;

        private static bool IsRKeyDown =>
            Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.R;

        private static bool IsTKeyDown =>
            Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.T;

        private void UpdateSceneGUI(SceneView sceneView)
        {
            //Debug.Log("Scene");
            //여기는 이벤트가 무조건 존재하는듯
            //씬뷰터치안되게
            lLcroweUtilEditor.DontSelectObjectOnSceneView();
           
            if (IsEscapeKeyDown)
            {
                Debug.Log("Escape");
                ResetTargetBatchObject();
                //Close();
            }

            if (IsRKeyDown)
            {
                RotateAngle(true);
            }

            if (IsTKeyDown)
            {
                RotateAngle(false);
            }

            Vector3 mousePos = Event.current.mousePosition;
            Ray ray = lLcroweUtilEditor.GetUnityEditorGUIToWorldRay(mousePos);

            //레이어가 존재하는건 작동이안됨
            RaycastHit[] raycastHitArray = Physics.RaycastAll(ray, Mathf.Infinity);

            for (int i = 0; i < raycastHitArray.Length; i++)
            {
                RaycastHit hitInfo = raycastHitArray[i];

                Transform target = hitInfo.transform;
                if (target == null)
                {
                    continue;
                }

                //레이어체크
                if (target.gameObject.layer != targetCheckLayer)
                {
                    continue;
                }

                //타일오브젝트 존재체크
                if (!target.TryGetComponent(out HexTileObject newHexTileObject))
                {
                    continue;
                }

                //타일맵에 해당 타일오브젝트가 있는지 체크
                if (!targetCustom3DHexTileMap.hexTileObjectBible.ContainsValue(newHexTileObject))
                {
                    continue;
                }

                //존재하면 배치할지 체크
                movePos = newHexTileObject.transform.position;

                //7개여야지 맞는데//그리기
                DrawHexTile(newHexTileObject, Color.green);
                DrawBatchDirection(movePos, GetBatchAngleRotation());

                switch (tileMapBatchMode)
                {
                    case TileMapBatchMode.Modify:

                        if (selectHexTileObject != null)
                        {
                            //선택한거 그려주기
                            DrawHexTile(selectHexTileObject, Color.red);
                        }

                        //수정모드
                        if (IsLeftMouseDown)
                        {
                            //클릭해서 선택하기//들어올리기
                            //선택된 오브젝트가 있으면 원복

                            //순서체크하기
                            //타입이 두개임// 1.선택하기전, 2.선택후
                            //1.선택하기
                            //2.이동
                            //3.헥스타일 선
                            if (selectHexTileObject == null)
                            {
                                //선택하기전
                                if (newHexTileObject.BatchUnitObject == null)
                                {
                                    //해당타일이 비어있으면//넘기기
                                    return;
                                }

                                //선택하기
                                targetTileMapBatchObect = newHexTileObject.BatchUnitObject;
                                selectHexTileObject = newHexTileObject;

                                //배치되있는 유닛의 회전값을 가져와서 세팅하기
                                var targetRot = targetTileMapBatchObect.transform.localRotation.eulerAngles;//배치된 오브젝트의 각도
                                batchAngle = targetCustom3DHexTileMap.createTileAxisType == lLcroweUtil.HexTileMatrix.CreateTileAxisType.XY ? targetRot.z : targetRot.y;
                            }
                            else
                            {
                                //선택후
                                //비어있든말든 교체이다
                                //교체
                                Undo.RecordObjects(new Object[] { newHexTileObject, selectHexTileObject }, "교체");
                                var tempObject = newHexTileObject.BatchUnitObject;
                                newHexTileObject.BatchUnitObject = selectHexTileObject.BatchUnitObject;
                                selectHexTileObject.BatchUnitObject = tempObject;                                


                                //교체된거 배치//자기자신거
                                if (newHexTileObject.BatchUnitObject != null)
                                {                                    
                                    Transform tempTr = newHexTileObject.BatchUnitObject.transform;
                                    tempTr.InitTrObjPrefab(newHexTileObject.transform.position, newHexTileObject.transform);
                                    newHexTileObject.BatchUnitObject.curHexTileObject = newHexTileObject;
                                    newHexTileObject.BatchUnitObject.unitUI?.uIBarFollowObject.OnValidate();
                                }

                                if (selectHexTileObject.BatchUnitObject != null)
                                {
                                    Transform tempTr = selectHexTileObject.BatchUnitObject.transform;
                                    tempTr.InitTrObjPrefab(selectHexTileObject.transform.position, selectHexTileObject.transform);
                                    selectHexTileObject.BatchUnitObject.curHexTileObject = selectHexTileObject;
                                    selectHexTileObject.BatchUnitObject.unitUI?.uIBarFollowObject.OnValidate();

                                    //이거 언도부분 잘안되네 수정모드 언도는 나중에하기
                                    //Undo.RecordObject(tempTr, "트랜스폼");
                                    //Undo.SetTransformParent(tempTr, selectHexTileObject.transform, "부모변경");
                                    //Undo.RecordObject(tempTr, "트랜스폼");
                                    //tempTr.position = selectHexTileObject.transform.position;
                                    //Undo.RecordObject(tempTr, "트랜스폼");
                                }
                                targetTileMapBatchObect = null;
                                selectHexTileObject = null;
                            }
                        }

                        if (IsRightMouseDown)
                        {
                            //선택된 오브젝트가 있으면 원복
                            if (targetTileMapBatchObect != null)
                            {
                                targetTileMapBatchObect.transform.parent = selectHexTileObject.transform;
                                targetTileMapBatchObect.transform.position = selectHexTileObject.transform.position;
                                selectHexTileObject.BatchUnitObject = targetTileMapBatchObect;

                                targetTileMapBatchObect = null;
                                selectHexTileObject = null;
                            }
                        }
                        break;
                    case TileMapBatchMode.Create:
                        //생성모드
                        if (IsLeftMouseDown)
                        {
                            //배치//생성
                            //Debug.Log("MouseLe");
                            if (newHexTileObject.BatchUnitObject == null)
                            {
                                //AssetDatabase에 있는걸로 호출해야지 널로 반화안함
                                //var targetBatch = (UnitObject_Base)PrefabUtility.InstantiatePrefab(targetTileMapBatchObect, newHexTileObject.transform);
                                var targetBatch = (UnitObject_Base)PrefabUtility.InstantiatePrefab(targetTileMapBatchObect.unitObjectInfo.unitPrefab, newHexTileObject.transform);
                                targetBatch.transform.parent = newHexTileObject.transform;
                                targetBatch.transform.position = newHexTileObject.transform.position;

                                Quaternion rotation = GetBatchAngleRotation();
                                targetBatch.transform.rotation = rotation;

                                targetBatch.unitUI?.uIBarFollowObject.OnValidate();//미리 다지정
                                Undo.RegisterCreatedObjectUndo(targetBatch.gameObject, "생성");
                                newHexTileObject.BatchUnitObject = targetBatch;
                                targetBatch.curHexTileObject = newHexTileObject;
                            }
                        }

                        //if (IsRightMouseDown)
                        //{
                        //    //선택한 대상 파괴//취소
                        //    //Debug.Log("취소");
                        //    ResetTargetBatchObject();
                        //}
                        break;
                    case TileMapBatchMode.Destroy:
                        //파괴모드
                        if (IsLeftMouseDown)
                        {
                            //비어있지않으면//클릭해서 파괴하기
                            if (newHexTileObject.BatchUnitObject != null)
                            {
                                Undo.DestroyObjectImmediate(newHexTileObject.BatchUnitObject.gameObject);
                                //DestroyImmediate(newHexTileObject.tileMapBatchObect.gameObject);
                            }
                        }
                        break;
                }
                break;
            }
            SceneView.RepaintAll();
        }

        float addRotate = 45f;

        private void RotateAngle(bool isRight)
        {
            batchAngle += isRight ? addRotate : -addRotate;
        }

        private Quaternion GetBatchAngleRotation()
        {
            Vector3 angleAxis = targetCustom3DHexTileMap.createTileAxisType == lLcroweUtil.HexTileMatrix.CreateTileAxisType.XY ? Vector3.forward : Vector3.up;
            return Quaternion.AngleAxis(batchAngle, angleAxis) * GetCustomHexTileMapRotation();
        }

        private Quaternion GetCustomHexTileMapRotation()
        {
            return targetCustom3DHexTileMap.transform.rotation;//이거 되로하는게 맞음
        }

        private void DrawHexTile(HexTileObject newHexTileObject, Color targetColor)
        {
            Vector3 originPos = newHexTileObject.transform.position;
            Vector3[] points = new Vector3[newHexTileObject.mesh.vertices.Length + 1];

            //타일맵 회전대응
            //회전처리//원점 처리
            Quaternion rot = GetCustomHexTileMapRotation();
            //이미 버텍스설정할떄 방향이 설정됫는데 필요한가?
            //Vector3 dir = targetCustom3DHexTileMap.createTileAxisType == lLcroweUtil.HexTileMatrix.CreateTileAxisType.XY ? Vector3.forward : Vector3.up;

            for (int j = 0; j < points.Length; j++)
            {
                if (j == points.Length - 1)
                {
                    points[j] = points[0];
                }
                else
                {
                    //이미 지정된
                    points[j] = (rot * newHexTileObject.mesh.vertices[j]) + originPos;
                }
            }
            Handles.color = targetColor;
            Handles.DrawAAPolyLine(points);
            Handles.color = Color.white;
        }

        private void DrawBatchDirection(Vector3 pos, Quaternion angle)
        {
            lLcroweUtilEditor.DrawConn(pos, angle);
            lLcroweUtilEditor.DrawLine(pos, angle, 3f, Vector3.forward);
        }

        private void Update()
        {
            //현재 배치할 타겟팅된 배치유닛
            if (targetTileMapBatchObect != null)
            {
                //이동
                float time = 0.5f;
                targetTileMapBatchObect.transform.position = Vector3.Lerp(targetTileMapBatchObect.transform.position, movePos, time);

                //회전
                Quaternion rotation = GetBatchAngleRotation();
                targetTileMapBatchObect.transform.rotation = Quaternion.Lerp(targetTileMapBatchObect.transform.rotation, rotation, time);
            }
        }
    }
}