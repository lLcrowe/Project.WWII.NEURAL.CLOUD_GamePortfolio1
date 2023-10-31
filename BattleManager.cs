using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using lLCroweTool.Singleton;
using lLCroweTool.AstarPath;
using lLCroweTool.Camera3D;
using lLCroweTool.TileMap.HexTileMap;
using lLCroweTool.NodeMapSystem;
using lLCroweTool.TileMap;
using lLCroweTool.TimerSystem;
using lLCroweTool.Cinemachine;
using lLCroweTool.ScoreSystem.UI;
using lLCroweTool.ScoreSystem;
using lLCroweTool.NoticeDisplay;
using lLCroweTool.Session;
using lLCroweTool.GamePlayRuleSystem;
using lLCroweTool.DataBase;

namespace lLCroweTool
{
    public class BattleManager : MonoBehaviourSingleton<BattleManager>
    {
        //전투할때 사용할 매니저
        //전투할때 사용하는 요소들이 들어가 있음 

        [Header("타일관련")]
        public HexBasementTileMap hexBasementTileMap;
        public LayerMask targetLayer;

        [SerializeField] private AStarPath aStarHex;
        public AStarPath AStarHex { get => aStarHex; }

        [Header("배치관련")]
        public HexTileObject selectHexTile;
        public BattleUnitObject selectUnitObject;
        public HexTileObject prevNewHexTile;

        [Header("보여주기관련")]
        public HexTileObject prevFindHexTile;
        public Color prevFindHexTileColor;
        public Color selectColor; 
        public float offSetDistance = 3f;//바닥과의 거리    

        //유닛배치전에 임시로 배치하는 구역//뉴럴클라우드 뒤쪽 배치구역//배치인원수와 비례해야됨
        public HexTileObject[] batchHexTileObjectArray= new HexTileObject[0];

        [Header("카메라 설정")]
        public DirectorCamera directorCamera;


        [Header("유닛체크용")]
        //유닛체크용
        //public List<BattleUnitObject> playerUnitObjectTargetList = new List<BattleUnitObject>();
        //public List<BattleUnitObject> allyUnitObjectTargetList = new List<BattleUnitObject>();
        //public List<BattleUnitObject> enemyUnitObjectTargetList = new List<BattleUnitObject>();
        public BattleUnitObject lastLiveEnemy;

        public GamePlayRuleManager gamePlayRuleManager = new();
        private bool isOnUI;
        private float deltaTime;
        private Vector3 mousePos;
        private Ray ray;
        private Custom3DHexTileMap tilemap;


        //ui처리관련-------------------------------------------------------------
        [Header("게임UI작동버튼")]
        public Button inGameMenuButton;

        public Button battleStartButton;
        public Button gameSpeedChangeButton;
        public TextMeshProUGUI gameSpeedChangeText;


        [Header("승리처리UI")]        
        public Button goToMainButton;

        [Header("패배처리UI")]
        public Button defeatMainButton;
        public Image defeatUI;

        [Header("전투가 끝난후 점수처리관련")]
        public BattleUnitObject targetHighScoreUnit = null;


        //작동구역들 분리해서 스크립트 각자만들기
        public GameObject battleUI;
        public ScoreBoard scoreBoard;
        public MVPScoreBoard mVPScoreBoard;//이거완전 내용물이 최종결과창..

        //업적관련
        public SearchMapInfo targetSearchMapInfo;
        [RecordIDTagAttribute] public string recordID_Money;
        //[RecordIDTag] public string recordID_Supply;


        [Header("노드맵")]
        public CustomNodeMap customNodeMap;//이건 루프 완성되면 넘기기


        [Header("작동모드 설정")]
        public BattleManagerStateType battleManagerStateType;
        public CustomCinemachineCamera cinemachineCamera;
        public CustomCinemachineManager cinemachineManager;

        [Header("전장에서의 알람관련")]
        public NoticeDisplayUI noticeDisplay;

        //맵에 있는 유닛들
        public List<BattleUnitObject> inMapUnitList = new List<BattleUnitObject>();

        //배치된 플레이어 유닛리스트//이건 처음배치할떄 살아있고 그이후로는 없애지않음//점수확인할때 체크함
        //출격한 유닛들
        public List<BattleUnitObject> sallyPlayerUnitList = new List<BattleUnitObject>();

        public enum BattleManagerStateType
        {
            Batch,
            Battle,
            End,
        }

        private bool isNormalSpeed;

        protected override void Awake()
        {
            base.Awake();
            directorCamera.isCurrentScriptControl = false;


            battleStartButton.name = "BattleStart";
            battleStartButton.onClick.AddListener(BattleStart);
            inGameMenuButton.onClick.AddListener(ActiveMainMenuUI);
            goToMainButton.onClick.AddListener(GoToMainMenu);
            defeatMainButton.onClick.AddListener(GoToMainMenu);

            //이부분 바꿔야됨// 전투시작시 시간이 반영됨
            gameSpeedChangeButton.onClick.AddListener(()=> {
                isNormalSpeed = !isNormalSpeed; 
                BattleSpeedChange(isNormalSpeed); 
            });
            gameSpeedChangeText.text = "1X";

            //배틀끝 이벤트넣기
            if (cinemachineManager.RequestCustomCinemachine("Victory", out var victoryCinemachine))
            {
                victoryCinemachine.startEvent.AddListener(VictoryStartEvent);
                victoryCinemachine.endEvent.AddListener(VictoryEndEvent);
            }

            //점수판 이벤트넣기
            if (cinemachineManager.RequestCustomCinemachine("HighCost", out CustomCinemachine highCostCinemacine))
            {
                highCostCinemacine.endEvent.AddListener(() => ShowHighScoreUnit(targetHighScoreUnit));
            }

            //이벤트등록
            if (cinemachineManager.RequestCustomCinemachine("IntoBattle", out var IntoBattleCinemachine))
            {
                IntoBattleCinemachine.startEvent.AddListener(() => cinemachineCamera.SetPositionAndCash(IntoBattleCinemachine.transform));
                IntoBattleCinemachine.startEvent.AddListener(() => directorCamera.enabled = false);
                IntoBattleCinemachine.startEvent.AddListener(() => battleUI.SetActive(false));
                //IntoBattleCinemachine.startEvent.AddListener(() => customNodeMap.ShowUIView());//비쥬얼적으로 채워질때까지 사용안함


                IntoBattleCinemachine.endEvent.AddListener(() => cinemachineCamera.RecorveryCameraTransform());
                IntoBattleCinemachine.endEvent.AddListener(() => directorCamera.enabled = true);
                IntoBattleCinemachine.endEvent.AddListener(() => battleUI.SetActive(true));
                IntoBattleCinemachine.endEvent.AddListener(() => ShowAllUnitUI());
                
            }

            //버튼이벤트
            mVPScoreBoard.scoreShowButton.onClick.AddListener(() => scoreBoard.gameObject.SetActive(true));



            //게임룰 매니저이벤트처리
            gamePlayRuleManager.Init(
                    BatchControl,
                    BattleControl,
                    EndControl,
                    EndControl,
                    VictoryEvent,
                    DefeatEvent
                );


            //컴포넌트처리
            tilemap = hexBasementTileMap.GetTileMap();
        }

        private IEnumerator Start()
        {
            InitHexTileMap();
            return null;
        }

        private void InitHexTileMap()
        {
            var tempBible = hexBasementTileMap.astarNodeBible;
            aStarHex = new AStarPath(tempBible);
        }

        /// <summary>
        /// 배틀매너지초기화
        /// </summary>
        /// <param name="unitObjectList">아군유닛들</param>
        /// <param name="searchMapInfo">맵정보</param>
        public void InitBattleManager(List<BattleUnitObject> unitObjectList, SearchMapInfo searchMapInfo)
        {
            targetSearchMapInfo = searchMapInfo;
            //배치UI에서 전장으로 넘어올때처리하기 위한 구역

            //아군처리
            for (int i = 0; i < unitObjectList.Count; i++)
            {
                var tempUnit = unitObjectList[i];
                HexTileObject tempHexTile = batchHexTileObjectArray[i];
                tempHexTile.BatchUnitObject = tempUnit;
                tempUnit.curHexTileObject = tempHexTile;
                tempUnit.transform.InitTrObjPrefab(tempHexTile.transform.position, tempHexTile.transform.rotation, tempHexTile.transform);
                tempUnit.Idle();
            }

            CheckOnCanvas.onUIPanel = false;

            //스테이지마다 있는 시네마틱작동
            if (cinemachineManager.RequestCustomCinemachine("IntoBattle", out var customCinemachine))
            {
                customCinemachine.ActionCamera();
            }
        }

        public void InitTileMapInfo(TileMapBatchInfo tileMapInfo)
        { 
            //맵정보를 가져와서 배치시킨다음

            //적군처리
            var hexTileObjectBible = hexBasementTileMap.GetTileMap().hexTileObjectBible;

            foreach (var item in hexTileObjectBible)
            {
                //있는 요소들만 집어넣기
                if (item.Value.BatchUnitObject == null)
                {
                    continue;
                }

                //tileMapInfoBible.Add(item.Key, item.Value.tileMapBatchObect.tileMapBatchObjectInfo);
            }
        }


     
        private void Update()
        {   
            isOnUI = CheckOnCanvas.onUIPanel;
            deltaTime = Time.deltaTime;
            mousePos = MousePointer.Instance.mouseWorldPosition;
            MousePointer.Instance.mouseScreenDistance = 1000;
            ray = MousePointer.Instance.mouseRay; 

            gamePlayRuleManager.UpdateGamePlayRuleManager();
        }

        /// <summary>
        /// 배치상태에서의 업데이트 기능
        /// </summary>
        /// <param name="deltaTime">델타타임</param>
        /// <param name="mousePos">마우스위치</param>
        /// <param name="ray">마우스가 쏜레이</param>
        /// <param name="tilemap">타일맵</param>
        private void BatchControl()
        {
            //시네머신이 작동되고 있으면 처리해주기
            if (cinemachineManager.RequestCustomCinemachine("IntoBattle", out var customCinemachine))
            {
                if (customCinemachine.IsRun())
                {
                    if (Input.anyKey)
                    {  
                        //작동중일때 스킵처리
                        customCinemachine.Skip();
                    }

                    //작동되고 있으면 기존로직이 작동안되게처리
                    return;
                }
            }

            //배치로직이 뭔가 이상하다 안됨
            //다시 생각해보기
            //일단은 레이캐스트로 체크

            //클릭다운했을시 들고
            //클릭업 했을시 내려놓고

            //드래그할시 기능이 좀있음
            //1. 배치구역을 찾았으면 자리이동
            //2. 해당구역에 유닛이 존재하면 한번캐싱

            //20230426 작동은 잘되지만 버그가 있어보임
            //특정프레임에서 한번더 변경되는 경우가 있음
            //일단 넘어가기 

            bool checkRay = this.RayCast(out HexTileObject newHexTileObject, ray, targetLayer) & !isOnUI;
            //if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, targetLayer))
            //{
            //    print($"타일좌표:{lLcroweUtil.GetWorldToCell(hitInfo.point, tilemap)}");
            //}
            

            //디텍트체크//컬러나 인디케이터 작동
            CheckSelectIndicator(checkRay , newHexTileObject);

            //선택풀기
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                //신규로 발견한 타일체크
                if (checkRay)
                {
                    if (selectHexTile == null)
                    {
                        return;
                    }

                    //신규타일이 배치못하는 타일이면
                    if (!newHexTileObject.GetHexTileData.IsBatchTile)
                    {
                        //원복
                        ReturnSelectHexTileForUnitOriginPos();
                        return;
                    }

                    if (prevNewHexTile == null)
                    {
                        //비었으면 그대로 스왑하기
                        SwapHexTileInUnit(newHexTileObject, selectHexTile);
                    }
                    else
                    {
                        //존재하면 그대로 놓기//이미바뀌었으니까
                        selectUnitObject.InitTrObjPrefab(newHexTileObject.transform.position, selectUnitObject.transform.rotation);
                        prevNewHexTile = null;
                    }
                }
                else
                {
                    //존재하지 않으니//원복
                    ReturnSelectHexTileForUnitOriginPos();
                }
                selectHexTile = null;
                selectUnitObject = null;
                return;
            }

            //신규타일이 발견되야 다음기능들이 작동할수 있는 구역들
            if (!checkRay)
            {
                return;
            }

            //배치가능한 타일인지 체크
            if (!newHexTileObject.GetHexTileData.IsBatchTile)
            {
                return;
            }

            //선택
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                //존재해야지 기능이 작동될수 있음
                selectHexTile = newHexTileObject;
                selectUnitObject = newHexTileObject.BatchUnitObject as BattleUnitObject;
                if (selectUnitObject == null)
                {
                    //유닛이 없으면 비어버림
                    selectHexTile = null;
                }
                else
                {
                    //적군인지 체크
                    //유닛이 있는데 적군이면 툴팁처리
                    //아군이든 적군이든 툴팁으로 다보여주면 되지않나
                    if (selectUnitObject.GetTeamType() == TeamType.Enemy)
                    {
                        //툴팁처리


                        selectHexTile = null;//작동안되야 하니 비어버림
                    }
                }
            }

            //선택한 타일이 있어야됨
            if (selectHexTile == null)
            {
                return;
            }

            //드래그
            if (Input.GetKey(KeyCode.Mouse0))
            {
                //캐릭터를 끌고다님//드래그
                Vector3 offSet = Vector3.up * offSetDistance;
                selectUnitObject.transform.position = newHexTileObject.transform.position + offSet;

                //전에 있던 타일과 같으면 넘어가기
                if (prevNewHexTile == newHexTileObject)
                {
                    return;
                }

                //기존에 자리교체한게 있으면 원래상태로 돌리기
                else if (prevNewHexTile != null)
                {
                    SwapHexTileInUnit(selectHexTile, prevNewHexTile);
                    prevNewHexTile = null;
                }

                //다른데 유닛체크
                if (newHexTileObject.BatchUnitObject == null)
                {
                    ////유닛이 없으면 넘어가기
                    return;
                }

                //있으면자리교체 //한번만 작동되야됨
                prevNewHexTile = newHexTileObject;
                SwapHexTileInUnit(selectHexTile, prevNewHexTile);
            }
        }

        /// <summary>
        /// 선택한 타일을 표시관련 체크하는 함수
        /// </summary>
        /// <param name="checkRay">체크하는 레이 여부</param>
        /// <param name="newHexTileObject">신규헥스타일오브젝트</param>
        private void CheckSelectIndicator(bool checkRay, HexTileObject newHexTileObject)
        {
            if (checkRay)
            {
                if (!newHexTileObject.GetHexTileData.IsBatchTile)
                {
                    if (prevFindHexTile != null)
                    {
                        prevFindHexTile.TileColor = prevFindHexTileColor;
                        prevFindHexTile = null;
                    }
                    return;
                }

                if (prevFindHexTile == null)
                {
                    prevFindHexTile = newHexTileObject;
                    prevFindHexTileColor = prevFindHexTile.TileColor;
                    prevFindHexTile.TileColor = selectColor;
                }
                else if (prevFindHexTile != newHexTileObject)
                {
                    prevFindHexTile.TileColor = prevFindHexTileColor;
                    prevFindHexTile = newHexTileObject;
                    prevFindHexTileColor = prevFindHexTile.TileColor;
                    prevFindHexTile.TileColor = selectColor;
                }
            }
            else
            {
                if (prevFindHexTile != null)
                {
                    prevFindHexTile.TileColor = prevFindHexTileColor;
                    prevFindHexTile = null;
                }
            }
        }

        private void ReturnSelectHexTileForUnitOriginPos()
        {
            if (selectHexTile == null)
            {
                return;
            }
            
            selectUnitObject.InitTrObjPrefab(selectHexTile.transform.position, selectUnitObject.transform.rotation);
            selectHexTile.BatchUnitObject = selectUnitObject;
            selectHexTile = null;
            //Debug.Log("원복");
        }

        private void SwapHexTileInUnit(HexTileObject hexTileObjectA, HexTileObject hexTileObjectB)
        {
            //유닛교체
            UnitObject_Base targetUnit = hexTileObjectA.BatchUnitObject;
            hexTileObjectA.BatchUnitObject = hexTileObjectB.BatchUnitObject;
            hexTileObjectB.BatchUnitObject = targetUnit;


            //각각위치 전환//유닛에 있는 타일배치도 변경하기
            targetUnit = hexTileObjectA.BatchUnitObject;
            if (targetUnit != null)
            {
                targetUnit.InitTrObjPrefab(hexTileObjectA.transform.position, targetUnit.transform.rotation);
                targetUnit.curHexTileObject = hexTileObjectA;
            }

            targetUnit = hexTileObjectB.BatchUnitObject;
            if (targetUnit != null)
            {
                targetUnit.InitTrObjPrefab(hexTileObjectB.transform.position, targetUnit.transform.rotation);
                targetUnit.curHexTileObject = hexTileObjectB;
            }
        }
        
        public RaycastHit[] raycastHitArray = new RaycastHit[100];
        bool isNearRange = false;

        /// <summary>
        /// 전투상태에서의 업데이트 기능
        /// </summary>
        /// <param name="deltaTime">델타타임</param>
        /// <param name="mousePos">마우스위치</param>
        /// <param name="ray">마우스가 쏜레이</param>
        /// <param name="tilemap">타일맵</param>
        private void BattleControl()
        {
            //게임승리 패배체크
            //적군체크
            //아군체크




            //스킬작동값처리

            //스킬처리

            //결과창처리


            //맵이동
            if (Input.GetKey(KeyCode.Q))
            {
                directorCamera.YRotateCamera(false);
            }

            if (Input.GetKey(KeyCode.E))
            {
                directorCamera.YRotateCamera(true);
            }

            if (Input.GetKey(KeyCode.Z))
            {
                directorCamera.XRotateCamera(false);
            }

            if (Input.GetKey(KeyCode.X))
            {
                directorCamera.XRotateCamera(true);
            }



            //스킬사용예시
            //if (Input.GetKeyDown(KeyCode.Alpha3))
            //{
            //    isNearRange = !isNearRange;
            //}


                //if (Input.GetKeyDown(KeyCode.Mouse0))
                //{
                //    //선택
                //    if (this.RayCast(out HexTileObject newHexTileObject, ray, targetLayer))
                //    {
                //        astarHexTile = newHexTileObject;
                //        Vector3Int[] array = null;
                //        if (isNearRange)
                //        {
                //            array = lLcroweUtil.GetNearDistancePos(astarHexTile.hexTileData.GetTilePos(), hexDistance, tilemap);
                //        }
                //        else
                //        {
                //            array = lLcroweUtil.GetNearRangePos(astarHexTile.hexTileData.GetTilePos(), hexDistance, tilemap);
                //        }


                //        for (int i = 0; i < array.Length; i++)
                //        {
                //            if (tilemap.TryGetHexTile(array[i], out HexTileObject hexTile))
                //            {
                //                hexTile.TileColor = Color.magenta;
                //            }
                //        }
                //    }
                //}


        }

        public void VictoryEvent()
        {
            //전투끝
            //시네머신 처리
            if (cinemachineManager.RequestCustomCinemachine("Victory", out var customCinemachine))
            {
                //아니면 재생만
                //모든 UI끄기
                battleStartButton.SetActive(false);
                gameSpeedChangeButton.SetActive(false);
                inGameMenuButton.SetActive(false);

                //배틀유닛들 정지
                //전투끝으로 인한 아군유닛들 힐주기
                for (int i = 0; i < inMapUnitList.Count; i++)
                {
                    var battleObject = inMapUnitList[i];

                    battleObject.IsGameStart = false;
                    battleObject.EndBattleEvent();

                    //UnBatchUnit(battleObject);
                    gamePlayRuleManager.RemoveTeamUnit(battleObject);
                }
                inMapUnitList.Clear();

                directorCamera.enabled = false;

                //마지막 처리한 유닛을 지정후 재생
                var batchInfo = customCinemachine.cameraBatchList[0];
                batchInfo.SetFollowObject(lastLiveEnemy.transform);
                batchInfo.SetControlTarget(cinemachineCamera.transform);
                customCinemachine.ActionCamera();                
            }
        }

        //졋을시 이벤트
        public void DefeatEvent()
        {
            //결과창처리
            //UI처리
            //다시시작하기//점수판//=>나중에 시간없음
            //메뉴로 돌아가기

            defeatUI.SetActive(true);
        }
      




        /// <summary>
        /// 테스트상태에서의 업데이트 기능
        /// </summary>
        private void EndControl()
        {
            //전투끝
            //시네머신 처리
            if (Input.anyKey)
            {

                if (cinemachineManager.RequestCustomCinemachine("Victory", out var customCinemachine))
                {
                    if (customCinemachine.IsRun())
                    {
                        //작동중이면 스킵
                        customCinemachine.Skip();
                    }
                }


                //시네머신처리
                if (cinemachineManager.RequestCustomCinemachine("HighCost", out customCinemachine))
                {
                    if (customCinemachine.IsRun())
                    {
                        //작동중이면 스킵
                        customCinemachine.Skip();
                    }
                }
            }



            //전투재배치
            //노드맵열기
            //나중에 처리
            //if (Input.GetKeyDown(PlayerInPutKeySetting.Instance.NormalKeyBible[ECustomKeyCode.MouseLeftButton]))
            //{
            //    //메인화면가기
            //    //버튼으로 대체
            //}


        }



        private void ActiveMainMenuUI()
        {
            //현재는 메인메뉴로 돌아가버리게 제작
            GoToMainMenu();
        }

        private void BattleSpeedChange(bool isFastTime)
        {
            int cat = 0;
            string content = "1X";
            if (isFastTime)
            {
                cat = -3;
                content = "2X";
            }

            gameSpeedChangeText.text = content;

            if (battleManagerStateType == BattleManagerStateType.Battle)
            {
                TimerModuleManager.Instance.SetTimerScale(cat);
            }
        }
      

        public void BattleStart()
        {
            //적군 사그리가져오기
            //나중엔 타일맵배치정보에서 특정 정보만 체크
            //현재는 맵상에 있는걸 다 글거와서 차례차례체크
            //후에 타일맵상에서 배치된걸 가져오자
            //public TileMapBatchInfo tileMapBatchInfo;

           

            //아니다 아군도 놓을텐데 그냥 싸그리 하자

            //타일에 배치된 유닛들 전체를 등록해서 체크
            var bible = hexBasementTileMap.hexAreaBible;
            inMapUnitList.Clear();
            foreach (var item in bible)
            {
                var hexTileData = item.Value;
                hexTileData.GetHexTileObject.UpdateHexTileData();
                var batchObject = hexTileData.GetHexTileObject.BatchUnitObject as BattleUnitObject;

                //비었으면 넘어가기
                if (batchObject == null)
                {
                    continue;
                }
                                
                //BatchUnit(batchObject);
                inMapUnitList.Add(batchObject);
                gamePlayRuleManager.AddTeamUnit(batchObject);
            }

            //아군유닛이 한명 이상맵에 있어야됨
            if (gamePlayRuleManager.GetAmountToTeam(TeamType.Player) == 0)
            {
                //알림보내기
                noticeDisplay.AddNewMessage("아군유닛을 배치하세요");

                //등록된거 지우기
                gamePlayRuleManager.ClearAllTeamCheckData();
                inMapUnitList.Clear();
                return;
            }


            //게임시작하기
            foreach (var item in inMapUnitList)
            {
                //유닛이 맵에 자기를 할당시킴
                item.isGameStart = true;
                item.curHexTileObject.UpdateHexTileData();


                //플레이어 유닛일시 점수용체크용으로 여기다 집어넣기
                if (item.GetTeamType() == TeamType.Player)
                {
                    sallyPlayerUnitList.Add(item);
                }

                //유닛들 시작이벤트발동하기
                item.StartBattleEvent();
            }

            //유닛들
            foreach (var item in batchHexTileObjectArray)
            {
                //존재하는 타일들만 처리
                item?.BatchUnitObject?.SetActive(false);
            }

            //모드변경
            battleManagerStateType = BattleManagerStateType.Battle;
            gamePlayRuleManager.PlayGame();
            BattleSpeedChange(isNormalSpeed);
            //버튼을 비활성화시켜서 더등록 못하게 하기
            battleStartButton.SetActive(false);
        }

        public void VictoryStartEvent()
        {
            //시간느리게한후 원래되로
            TimerModuleManager.Instance.SetTimerScale(0.3f, 1);
        }
        
        /// <summary>
        /// 전투끝.시네머신에서 빅토리 끝날시 호출해주는 함수
        /// </summary>
        public void VictoryEndEvent() 
        {
            //다른종류로 바꿔서 작동안되게
            battleManagerStateType = BattleManagerStateType.End;

            //전투끝난후
            //

         


            //끝날시 점수판에서 제일 높은애 체크//딜로 정하자
            //플레이어유닛만 담어두는 객체를 추가
            int highDamageScore = 0;
            scoreBoard.ResetScoreBoard();
            mVPScoreBoard.ResetScoreBoard();
            for (int i = 0; i < sallyPlayerUnitList.Count; i++)
            {
                var unit = sallyPlayerUnitList[i];
                //체력업 및 보이게처리
                unit.unitAbilityModule.GetUnitStatusValue(Ability.UnitStatusType.HealthPoint, out float healthPoint);
                unit.unitAbilityModule.GetUnitStatusValue(Ability.UnitStatusType.CombatEndHill, out float combatHill);
                unit.unitAbilityModule.SetUnitStatusValue(Ability.UnitStatusType.HealthPoint, healthPoint + combatHill);
                unit.SetActive(true);

                //아군유닛들 승리모션처리


                //점수판에 각각의 플레이어유닛 점수들을 갱신시켜줌
                if (!unit.TryGetComponent(out ScoreTarget scoreTarget))
                {
                    continue;
                }

                //높은점수이면 새롭게 갱신해주기
                int score = scoreTarget.GetScore(ScoreType.GiveDamage);

                //높은점수이면 새롭게 갱신해주기
                if (highDamageScore < score)
                {
                    highDamageScore = score;
                    targetHighScoreUnit = unit;
                }

                //각보드에 등록
                scoreBoard.RegisterScoreTarget(unit, scoreTarget);
                mVPScoreBoard.RegisterScoreTarget(unit, scoreTarget);
            }

            //최종점수연산//처음은 대미지 정렬
            scoreBoard.SortScoreType(ScoreType.GiveDamage);
            var targetTr = mVPScoreBoard.ShowMVP().transform;

            //제일 높은애의 정면을 구한다음 역으로 만들고 거리를 두어 그 위치에 카메라를 두자
            

            //러프모드가 완성됫으니 적용시켜보자
            if (cinemachineManager.RequestCustomCinemachine("HighCost", out CustomCinemachine highCostCinemacine))
            {
                var batchInfo = highCostCinemacine.cameraBatchList[0];
                batchInfo.SetFollowObject(targetTr);
                batchInfo.SetControlTarget(cinemachineCamera.transform);
                batchInfo.SetLookAtObject(targetTr);
                highCostCinemacine.ActionCamera();
            }

            //보상주기
            var dataBaseManager = DataBaseManager.Instance;
            dataBaseManager.GiveReward(dataBaseManager.RequestSearchMapRewardData(targetSearchMapInfo.labelID));
        }

        private void ShowHighScoreUnit(BattleUnitObject targetHighScoreUnit)
        {
            //UI온
            //높은점수 유닛 UI띄워주기
            //피해통계량(점수통계량) 보여주는 버튼 On
            //BattleEnd에서 미리계산한걸 보여주기




            mVPScoreBoard.SetActive(true);
            //scoreBoard.SetActive(true);//버튼에 따로추가
            
            

            //노드맵으로 돌아가기. 다음 노드가 없을시 최종결과창을 보여주기
            //일단 노드맵은 빼고 최종결과창만 보여줄까

            //결과가 모두나왔으면

            //스위치문으로 처리
            //특정버튼을 눌려서//특정버튼에 노드맵작동 이벤트를 넣어주기
            //노드맵 작동
        }


        /// <summary>
        /// 맵안의 유닛의 UI를 보여주는 함수
        /// </summary>
        private void ShowAllUnitUI()
        {
            var bible = hexBasementTileMap.hexAreaBible;
            inMapUnitList.Clear();
            foreach (var item in bible)
            {
                var hexTileData = item.Value;
                hexTileData.GetHexTileObject.UpdateHexTileData();
                var batchObject = hexTileData.GetHexTileObject.BatchUnitObject as BattleUnitObject;

                //비었으면 넘어가기
                if (batchObject == null)
                {
                    continue;
                }

                batchObject.unitUI?.SetActive(true);
            }


            //플레이어 배치공간에 있는 유닛들 처리            
            foreach (var item in batchHexTileObjectArray)
            {
                var temp = item.BatchUnitObject;
                if (temp == null)
                {
                    continue;
                }
                temp.unitUI?.SetActive(true);
            }
        }


        /// <summary>
        /// 노드맵을 켜주는 함수
        /// </summary>
        public void ShowNodeMap()
        {
            customNodeMap.SetActive(true);
        }

        /// <summary>
        /// 노드맵을 꺼주는 함수
        /// </summary>
        public void HideNodeMap()
        {
            customNodeMap.SetActive(false);
        }

        /// <summary>
        /// 메인메뉴로 가기전에 처리해야할 목록들
        /// </summary>
        private void GoToMainMenu()
        {
            //다른씬에서 받아온 객체들
            SessionManager.Instance.LoadingMainMenu();
        }

      

        //이거 유닛한테 옮기기
        //요기서 그려줄필요없잖아
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            Vector3 origin = transform.position;
            //for (int i = 0; i < vector3IntArray.Length; i++)
            //{
            //    Vector3 tempPos = vector3IntArray[i];
            //    UnityEditor.Handles.Label(origin + tempPos, $"{tempPos}");
            //}

            if (hexBasementTileMap == null)
            {
                return;
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(origin + hexBasementTileMap.tileMapCenterPos, Vector3.one * 1.5f);

          

#if UNITY_EDITOR
            aStarHex.DebugGizmo();
#endif


            //Vector3 mousePos = MousePointer.Instance.mouseWorldPosition;
            //Ray ray = MousePointer.Instance.mouseRay;
            //Vector3Int pos = Vector3Int.zero;

            ////Gizmos.color = Color.yellow;
            ////Gizmos.DrawLine(mousePos, mousePos + Vector3.down * 100f);

            //Gizmos.color = Color.magenta;
            //Gizmos.DrawRay(ray.origin, ray.origin + ray.direction * 100f);
            //Gizmos.DrawRay(ray);

            //Gizmos.color = Color.blue;
            //Gizmos.DrawLine(mousePos + ray.origin, mousePos + ray.origin + ray.direction * 100f);

            //Gizmos.color = Color.red;
            //Gizmos.DrawLine(mousePos, mousePos + ray.direction * 100f);     

            if (!Application.isPlaying)
            {
                return;
            }
            Ray ray = MousePointer.Instance.mouseRay;
            Gizmos.DrawRay(ray.origin, ray.direction * 50f);
        }
    }
}