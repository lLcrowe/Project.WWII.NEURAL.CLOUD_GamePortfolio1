using System.Collections.Generic;
using UnityEngine;
using lLCroweTool.Singleton;
using TMPro;
using UnityEngine.Events;
using Doozy.Engine.UI;
using lLCroweTool.Session;
using lLCroweTool.Achievement;
using lLCroweTool.DataBase;
using UnityEngine.Assertions.Must;
using lLCroweTool.UI.Confirm;

namespace lLCroweTool.UnitBatch
{
    public class UnitBatchUIManager : MonoBehaviourSingleton<UnitBatchUIManager>
    {
        //유닛배치를 위한 매니저
        //유닛배치UI와 유닛배치 기능을 가짐
        //유닛카드들이 배치될 위치들을 지정
        //유닛카드들을 체크

        //구조
        //UnitBatchUIManager 
        //  MasterCavnas
        //      CardBatchWindow
        //          CardBatchPos
        //          mapNameText
        //          intoTheBattleButton
        //          backButton
        //  UnitBatchObject
        //  UnitBatchObject
        //  UnitBatchObject
        //  UnitBatchObject
        //  UnitBatchObject
        //  targetBatchCamera


        [Header("기본세팅")]
        public Transform targetBatchCameraPos;
        public Transform cardBatchPos;        

        public UnitBatchStateType unitBatchStateType;
        public LayerMask targetLayer;

        public float distance = 100;//카드끼리의 거리
        public float moveSpeed = 5f;//선택한 오브젝트가 움직이는 속도

        //카드선택, 변경관련
        public Vector2 offsetForUI;//선택한 카드 오프셋
        public Vector2 offsetForEnterUI = Vector2.up;//마우스포인터로 들어온 카드오프셋

        //유닛이 카메라로 부터 얼마나 떨어져야되는지
        public float characterDistance = 5f;

        //테스트후 private하기
        
        //배치관련
        public Transform selectObject;//현재 선택된 오브젝트
        public UnitObjectInfo selectUnitObjectInfo;//현재 선택된 유닛스탯데이터
        public Transform selectOriginTr;//돌아갈원래위치

        //카드이동
        public int selectIndexCash;//카드를 선택할시 인덱스캐싱용
        public Transform enterTr;//마우스포인터가 들어간위치

        //유닛이동
        public UnitBatchObject enterBatchTr;
        public UnitBatchObject newUnitExistBatchObject;
        
        //가야될 맵 정보
        public SearchMapInfo targetSearchMapInfo;
        public List<Transform> cardUITrList = new List<Transform>();//카드배치위치들
        public List<UnitBatchObject> batchTrList = new List<UnitBatchObject>();//배치위치들//전장 진입할때 유닛들을 보관하는구역

        //UI관련        
        [Header("UI세팅")]
        public TextMeshProUGUI mapNameText;
        public UIButton intoTheBattleButton;//전장으로 가는 버튼        

        public UIButton backButton;//돌아가는 버튼

        //전투 스테이션이면 유닛오브젝트들을 활성화 비활성화시킴
        //아닐시 그냥 카드옮기는 용도

        //드래그관련
        //UI밖여부

        //카드데이터들 세팅
        //카드에서 프리팹을 가져와야됨
        //비리 배치해놓고 활성화 비활성화로 체크
        public enum UnitBatchStateType
        {
            None,
            SelectUnitObject,
            SelectUnitUI,
        }

        protected override void Awake()
        {
            base.Awake();
            InitBatchUI();
            intoTheBattleButton.Button.onClick.AddListener(EnterTheBattle);
            intoTheBattleButton.SetLabelText("출격하기");
            backButton.SetLabelText("뒤로가기");

            this.SetActive(false);
        }

        private void Update()
        {
            BatchCardUI();
            BatchUnit();

            if (selectObject == null)
            {
                return;
            }
            switch (unitBatchStateType)
            {
                case UnitBatchStateType.None:
                    return;
                case UnitBatchStateType.SelectUnitObject:
                    //유닛오브젝트를 선택했을시
                    SelectUnitObjectFunc();
                    break;
                case UnitBatchStateType.SelectUnitUI:
                    //유닛카드를 선택했을시
                    SelectUnitUIFunc();
                    break;
            }
        }

        /// <summary>
        /// 카드배치를 위한 초기화
        /// </summary>
        public void InitBatchUI()
        {
            //첫번째 자기자손들을 가져와서 리스트에 집어넣기
            Transform[] trArray = cardBatchPos.GetComponentsInChildren<Transform>();
            cardUITrList.Clear();
            for (int i = 0; i < trArray.Length; i++)
            {
                if (trArray[i] == cardBatchPos)
                {
                    continue;
                }

                if (trArray[i].parent != cardBatchPos)
                {
                    continue;
                }
                cardUITrList.Add(trArray[i]);
            }
        }

        /// <summary>
        /// 유닛배치UI 리셋
        /// </summary>
        private void ResetUnitBatchUI()
        {
            unitBatchStateType = UnitBatchStateType.None;
            selectObject = null;
            selectOriginTr = null;
            selectUnitObjectInfo = null;
            enterBatchTr = null;
            newUnitExistBatchObject = null;
        }


        /// <summary>
        /// 카드들을 특정위치에 배치하는 함수
        /// </summary>
        private void BatchCardUI()
        {
            int totalCardAmount = cardUITrList.Count;
            Vector2 middlePos = cardBatchPos.position;
            float deltaTime = Time.deltaTime;

            //Debug.Log($"{Time.deltaTime}");

            //float startPos = (totalCardAmount * 0.5f) * -distance;
            //float addDistance = distance / totalCardAmount;

            //카드을 전체길이의 절반, -마이너스값
            float startPos = (distance * totalCardAmount) * -0.5f;
            //float startAngle = angle * totalCardAmount * 0.5f;
            //Debug                .Log(startAngle);
            //z축 -왼쪽

            for (int i = 0; i < cardUITrList.Count; i++)
            {
                int index = i;
                Transform tempTr = cardUITrList[i];
                if (selectObject == tempTr)
                {
                    //선택한 오브젝트는 넘어감
                    startPos += distance;//어떤게 좋을까
                    continue;
                }
                
                Vector2 addPos = Vector2.zero;
                if (enterTr == tempTr)
                {
                    //현재 포인터가 되있으면 추가오프셋
                    addPos = offsetForEnterUI;
                }

                tempTr.SetSiblingIndex(index);//카드정렬때문에 필요함
                tempTr.position = Vector2.Lerp(tempTr.position, new Vector2(middlePos.x + startPos, middlePos.y) + addPos, deltaTime * moveSpeed);
                //tempTr.localEulerAngles = Vector3.Lerp(tempTr.localEulerAngles, new Vector3(0, 0, startAngle), deltaTime * moveSpeed);
                startPos += distance;
                //startAngle += angle;
            }
        }

        /// <summary>
        /// 배치오브젝트에 배치된 유닛들을 특정위치에 놓는 함수
        /// </summary>
        private void BatchUnit()
        {
            //배치처리
            float deltaTime = Time.deltaTime;

            for (int i = 0; i < batchTrList.Count; i++)
            {
                int index = i;

                if (batchTrList[i].GetUnitObject() == null)
                {
                    //비어있으면 넘어감
                    continue;
                }

                Transform unitTr = batchTrList[i].GetUnitObject().transform;

                if (selectObject == unitTr)
                {
                    //선택한 오브젝트는 넘어감
                    continue;
                }

                //해당 배치 위치로 옮기기
                unitTr.position = Vector3.Lerp(unitTr.position, batchTrList[index].transform.position, deltaTime * moveSpeed);
                unitTr.rotation = Quaternion.Lerp(unitTr.rotation, batchTrList[index].transform.rotation, deltaTime * moveSpeed);
            }
        }

        /// <summary>
        /// 유닛배치UI매니저의 작동방식을 변경하는 함수
        /// </summary>
        /// <param name="state">상태</param>
        /// <param name="targetMoveObject">움직일 타겟오브젝트</param>
        /// <param name="targetOriginTr">원래 있던 위치</param>
        public void SetUnitBatchUI(UnitBatchStateType state, Transform targetMoveObject, Transform targetOriginTr, UnitObjectInfo unitStatusData)
        {
            if (targetMoveObject == null)
            {
                unitBatchStateType = UnitBatchStateType.None;
                return;
            }

            unitBatchStateType = state;
            selectObject = targetMoveObject;
            selectOriginTr = targetOriginTr;
            selectUnitObjectInfo = unitStatusData;

            //추가처리
            switch (state)
            {
                case UnitBatchStateType.SelectUnitObject:
                    if (selectOriginTr.TryGetComponent(out UnitBatchObject unitBatchObject))
                    {
                        enterBatchTr = unitBatchObject;
                    }
                    break;
                case UnitBatchStateType.SelectUnitUI:
                    selectIndexCash = selectObject.GetSiblingIndex();//기존카드위치 캐싱
                    selectObject.SetParent(selectOriginTr.parent);
                    selectObject.SetAsLastSibling();
                    break;
            }
        }


        /// <summary>
        /// 카드리스트에 등록
        /// </summary>
        /// <param name="target">타겟</param>
        private void AddTrCard(Transform target)
        {
            target.gameObject.SetActive(true);
            cardUITrList.Add(target);
        }

        /// <summary>
        /// 카드리스트 등록해체
        /// </summary>
        /// <param name="target">타겟</param>
        private void RemoveTrCard(Transform target)
        {
            target.gameObject.SetActive(false);
            cardUITrList.Remove(target);
        }

        /// <summary>
        /// 카드트랜스폼 변경
        /// </summary>
        /// <param name="enterCard">포인터에 들어온 카드</param>
        private void ChangeTrCard(Transform enterCard)
        {
            if (enterTr == null)
            {
                //만약없고
                //패널내부의 제일 좌측에 있는 카드 위치랑 우측위치를 체크해서 넘어가면
                //해당자리를 교채
                float leftX = cardUITrList[0].localPosition.x;
                float rightX = cardUITrList[cardUITrList.Count - 1].localPosition.x;
                
                if (leftX > selectObject.localPosition.x)
                {
                    //좌측x- 비교
                    //작을시//맨앞에 집어넣기                    
                    cardUITrList.Remove(selectObject);
                    cardUITrList.Insert(0, selectObject);

                    //Debug.Log($"작아");

                }
                else if (rightX < selectObject.localPosition.x)
                {
                    //우측x+ 비교
                    //클시//맨뒤에 집어넣기                    
                    cardUITrList.Remove(selectObject);
                    cardUITrList.Add(selectObject);
                    //Debug.Log($"커");
                }
                //Debug.Log($"{leftX} {rightX} {selectObject.localPosition.x}");
                return;
            }

            //스왑
            int enterIndex = enterCard.GetSiblingIndex();//지금들어온대상
            int addSelectIndex = enterIndex >= selectIndexCash ? 1 : 0;//위치에 따른변경법 변경
            //Debug.Log($"{enterIndex} {addSelectIndex}");

            //리스트로 바꿔야됨//이동이 리스트로 처리해서
            cardUITrList.Swap(enterIndex + addSelectIndex, selectIndexCash);
        }


       
        /// <summary>
        /// 마우스포인터가 UI진입시 호출하는 함수
        /// </summary>
        /// <param name="targetTr">트랜스폼</param>
        public void EnterTheOnCard(Transform targetTr)
        {
            //현재 트랜스폼이 무엇을 지칭하느냐에 따라 작동이 다름
            enterTr = targetTr;
        }

        /// <summary>
        /// 유닛오브젝트끼리의 스왑
        /// </summary>
        private void SwapUnitBatchObject(UnitBatchObject unitBatchObjectA, UnitBatchObject unitBatchObjectB)
        {
            var unitObject = unitBatchObjectA.GetUnitObject();
            unitBatchObjectA.SetUnitObject(unitBatchObjectB.GetUnitObject());
            unitBatchObjectB.SetUnitObject(unitObject);
        }

        /// <summary>
        /// 유닛오브젝트를 선택했을시 기능
        /// </summary>
        private void SelectUnitObjectFunc()
        {
            float deltaTime = Time.deltaTime;
            MousePointer.Instance.mouseScreenDistance = characterDistance;
            Vector3 pos = MousePointer.Instance.mouseWorldPosition;
            Ray ray = MousePointer.Instance.mouseRay;

            if (Input.GetKey(KeyCode.Mouse0))
            {
                //캐릭터를 끌고다님//캐릭터가 쭉 움직임
                selectObject.transform.position = Vector3.Lerp(selectObject.transform.position, pos, deltaTime * moveSpeed);

                //중간에 배치오브젝트가 있으면//배치오브젝트안에 캐릭터가 있으면 해당캐릭터를 선택한 배치오브젝트의 위치로 이동 이동시킴                

                //배치오브젝트있고 캐릭터가 있으면 잠시 변경
                if (!Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, targetLayer))
                {
                    //배치오브젝트가 없으면
                    if (newUnitExistBatchObject != null)
                    {
                        //있을시 원복처리//트리거용
                        SwapUnitBatchObject(newUnitExistBatchObject, enterBatchTr);
                        newUnitExistBatchObject = null;
                    }
                    return;
                }

                //배치오브젝트가 있는지
                if (!hitInfo.collider.TryGetComponent(out UnitBatchObject searchBatchObject))
                {
                    //배치오브젝트가 없으면
                    if (newUnitExistBatchObject != null)
                    {
                        //있을시 원복처리//트리거용
                        SwapUnitBatchObject(newUnitExistBatchObject, enterBatchTr);
                        newUnitExistBatchObject = null;
                    }
                    return;
                }

                //있으면 러프회전
                selectObject.transform.rotation = Quaternion.Lerp(selectObject.transform.rotation,searchBatchObject.transform.rotation,deltaTime * moveSpeed);

                //같은배치오브젝트이면 패스
                if (searchBatchObject == enterBatchTr)
                {
                    return;
                }

                //배치오브젝트에 유닛이 있는지
                if (searchBatchObject.GetUnitObject() == null)
                {
                    return;
                }

                //마지막이거남음
                //유닛이 있으면 해당유닛은 잠시 들어온 위치로 가져온다
                if (newUnitExistBatchObject == null)
                {
                    //없을시 초기처리//트리거용
                    //없을시 배정해주고 위치를 변경해줌
                    newUnitExistBatchObject = searchBatchObject;

                    SwapUnitBatchObject(newUnitExistBatchObject, enterBatchTr);
                }
            }

                       


            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                //스왑기능처리
                //패널밖이면 

                if (CheckOnCanvas.onUIPanel)
                {
                    //안
                    //유닛오브젝트 비활성화
                    //배치오브젝트 비우기 처리후
                    selectObject.gameObject.SetActive(false);
                    enterBatchTr.SetUnitObject(null);

                    //카드로 변경
                    UnitBatchCardUI unitBatchCardUI = ObjectPoolManager.Instance.RequestDynamicComponentObject(selectUnitObjectInfo.unitCardUI);
                    unitBatchCardUI.InitUnitBatchCardUI(selectUnitObjectInfo);
                    unitBatchCardUI.transform.InitTrObjPrefab(cardBatchPos.position, Quaternion.identity, cardBatchPos);
                    AddTrCard(unitBatchCardUI.transform);
                }
                else
                {
                    //밖
                    //새로배치된구역이 없으면 바꿔버리기
                    if (newUnitExistBatchObject == null)
                    {
                        //자리바꾸기
                        //배치오브젝트있으면 변경
                        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, targetLayer))
                        {
                            //배치오브젝트가 있는지
                            if (hitInfo.collider.TryGetComponent(out UnitBatchObject searchBatchObject))
                            {
                                //같으면 넘어감
                                if (searchBatchObject != enterBatchTr)
                                {
                                    //다르면 위치변경
                                    var unitObject = searchBatchObject.GetUnitObject();
                                    searchBatchObject.SetUnitObject(enterBatchTr.GetUnitObject());
                                    enterBatchTr.SetUnitObject(unitObject);
                                }
                            }
                        }
                    }
                }
                ResetUnitBatchUI();
            }
        }

        /// <summary>
        /// 유닛카드를 선택했을시 기능
        /// </summary>
        private void SelectUnitUIFunc()
        {   
            float deltaTime = Time.deltaTime;
            if (Input.GetKey(KeyCode.Mouse0))
            {
                //카드 움직임처리
                Vector2 mousePos = (Vector2)MousePointer.Instance.mouseScreenPosition + offsetForUI;
                selectObject.position = Vector2.Lerp(selectObject.position, mousePos, deltaTime * moveSpeed);
            }


            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                //스왑기능처리
                //패널밖이면 오브젝트 활성화배치
                //패널안이면 넘어가기
                if (CheckOnCanvas.onUIPanel)
                {
                    //안
                    //원래위치로 돌아가고 넘어감
                    selectObject.SetParent(selectOriginTr);
                    ChangeTrCard(enterTr);
                }
                else
                {
                    //밖
                    //해당방향에 유닛배치오브젝트가 존재하면 그때 활성화시켜서 배치
                    Ray ray = MousePointer.Instance.mouseRay;
                    if (!Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, targetLayer))
                    {
                        selectObject.SetParent(selectOriginTr);
                        selectObject = null;
                        return;
                    }

                    //유닛배치오브젝트가 있는지
                    if (!hitInfo.collider.TryGetComponent(out UnitBatchObject unitBatchObject))
                    {
                        selectObject.SetParent(selectOriginTr);
                        selectObject = null;
                        return;
                    }

                    //유닛이 있는지
                    if (unitBatchObject.GetUnitObject() != null)
                    {
                        //안에 있으면
                        //유닛을 비활성화 처리후
                        UnitObject_Base disableUnitObject = unitBatchObject.GetUnitObject();
                        disableUnitObject.SetActive(false);

                        //카드로 변경
                        UnitBatchCardUI unitBatchCardUI = ObjectPoolManager.Instance.RequestDynamicComponentObject(disableUnitObject.unitObjectInfo.unitCardUI);
                        unitBatchCardUI.InitUnitBatchCardUI(disableUnitObject.unitObjectInfo);
                        unitBatchCardUI.transform.InitTrObjPrefab(cardBatchPos.position, Quaternion.identity, cardBatchPos);
                        AddTrCard(unitBatchCardUI.transform);
                        //print($"복귀한 카드 {}");
                    }
                    

                    //Debug.Log("배치대상있음");
                    //선택한 카드배치하기
                    RemoveTrCard(selectObject);

                    //소환
                    //배틀용인지 체크
                    var checkPrefab = selectUnitObjectInfo.unitPrefab as BattleUnitObject;
                    if (checkPrefab == null)
                    {
                        Debug.Log($"배틀유닛오브잭트가 아닌 프리팹이 할당되었습니다. {selectUnitObjectInfo.unitPrefab.GetType()}");
                        return;
                    }

                    var unitObject = ObjectPoolManager.Instance.RequestDynamicComponentObject(checkPrefab);
                    unitObject.transform.InitTrObjPrefab(unitBatchObject.transform.position, unitBatchObject.transform.rotation, transform);
                    unitBatchObject.SetUnitObject(unitObject);//배치오브젝트에 세팅
                }
                ResetUnitBatchUI();
            }
        }

        /// <summary>
        /// 전장으로 진입하는 함수
        /// </summary>
        private void EnterTheBattle()
        {
            //배치병력을 모아서 출동
            List<BattleUnitObject> unitObjectList = new List<BattleUnitObject>();
            for (int i = 0; i < batchTrList.Count; i++)
            {
                var temp = batchTrList[i].GetUnitObject();
                if (temp == null)
                {
                    continue;
                }
                unitObjectList.Add(temp);
            }

            //중간처리작업체크
            int count = unitObjectList.Count;
            string content = unitObjectList.Count == 0 ? "카드를 배치하지 않았습니다!" : "카드를 다배치하지 않으셧습니다. 진행하시겠습니까?";
            var confirmWindow = GlobalConfirmWindow.Instance;
          
            if (count < 5)
            {
                if (count == 0)
                {
                    confirmWindow.SetConfirmWindow(content, null, "확인");
                    
                }
                else
                {
                    confirmWindow.SetConfirmWindow(content, () =>
                    {                        
                        ConfirmEnterTheBattle(unitObjectList);
                    }, null, "예. 출격하겠습니다.", "아니요");
                }
                confirmWindow.ShowConfirmWindowUIView();
                return;
            }

            ConfirmEnterTheBattle(unitObjectList);
        }

        private void ConfirmEnterTheBattle(List<BattleUnitObject> unitObjectList)
        {
            //전장가는 업적 체크
            AchievementManager.Instance.UpdateRecordData("EnterTheBattle", 1);
            AchievementManager.Instance.UpdateRecordData("UseSupply", targetSearchMapInfo.needSupply);


            //로딩처리
            //보내버리기
            SessionManager.Instance.LoadingTheTargetScene(unitObjectList, targetSearchMapInfo.stageName, () => BattleManager.Instance.InitBattleManager(unitObjectList, targetSearchMapInfo));
            HideBatchUI();
        }


        /// <summary>
        /// 배치UI를 보여주는 함수 유닛데이터들을 받아서 카드로 뿌려주기
        /// </summary>
        /// <param name="unitDataInfoList">배치해줄 유닛데이터들</param>
        /// <param name="searchMapInfo">타겟팅할 맵정보</param>
        /// <param name="backAction">뒤로가기 버튼에 들어갈 액션</param>
        public void ShowBatchUI(SearchMapInfo searchMapInfo, UnityAction backAction)
        {
            //가진 캐릭터들을 위주로 체크
            //그럼 플레이어의 데이터가 필요함 => 채워넣음
            this.SetActive(true);
            Transform cameraTr = Camera.main.transform;
            Vector3 originPos = cameraTr.position;
            Quaternion originQuaternion = cameraTr.rotation;

            cameraTr.SetPositionAndRotation(targetBatchCameraPos.position, targetBatchCameraPos.rotation);

            targetSearchMapInfo = searchMapInfo;
            mapNameText.text = targetSearchMapInfo.labelNameOrTitle;

            var unitDataInfoList = DataBaseManager.Instance.playerData.unitDataInfoList;
            //카드를 소환
            for (int i = 0; i < unitDataInfoList.Count; i++)
            {   
                var unitData = unitDataInfoList[i];
                var card = ObjectPoolManager.Instance.RequestDynamicComponentObject(unitData.unitCardUI);
                card.InitUnitBatchCardUI(unitData);
                card.transform.InitTrObjPrefab(cardBatchPos.position, Quaternion.identity, cardBatchPos);
            }

            //배치체크
            InitBatchUI();

            //꺼지는거 집어넣음
            backAction += () =>
            {
                cameraTr.SetPositionAndRotation(originPos, originQuaternion);
                HideBatchUI();
            };
            backButton.Button.onClick.RemoveAllListeners();
            backButton.Button.onClick.AddListener(backAction);
        }

        private void HideBatchUI()
        {
            intoTheBattleButton.StopAllCoroutines();
            backButton.StopAllCoroutines();


            this.SetActive(false);

            //배치오브젝트들과 카드오브젝트들 비활성화
            for (int i = 0; i < cardUITrList.Count; i++)
            {
                cardUITrList[i].SetActive(false);
            }
            cardUITrList.Clear();

            //배치오브젝트는 클리어하면 안됨
            for (int i = 0; i < batchTrList.Count; i++)
            {
                UnitObject_Base unitObject = batchTrList[i].GetUnitObject();
                if (unitObject == null)
                {
                    continue;
                }
                unitObject.SetActive(false);
                batchTrList[i].SetUnitObject(null);
            }
        }
    }
}