using lLCroweTool.SkillSystem;
using System.Collections.Generic;
using lLCroweTool.TimerSystem;
using UnityEngine;
using lLCroweTool.Ability;
using lLCroweTool.ScoreSystem;
using lLCroweTool.TileMap.HexTileMap;
using lLCroweTool.GamePlayRuleSystem;
using lLCroweTool.Achievement;
using lLCroweTool.Effect;

namespace lLCroweTool
{
    public abstract class BattleUnitObject : UnitObject_Base
    {
        //배틀하는 종류들만 상속받아서 처리        

        //히트박스인가 거기 로직 체크하기
        public CoolTimerModule_Element attackCoolTimer;
        public SkillToolBar skillToolBar;

        [Header("스킬관련")]
        //스킬쿨타임을 위한 구역
        public float skillChargeMaxValue = 100;//스킬충전최대치
        public float skillChargeCurValue;//현재값

        [Header("탐지관련")]
        public BattleUnitObject targetUnitObject;//타겟팅된 적군

        public TeamType unitTeamType;
        public TeamRuleData teamRuleData = new TeamRuleData();

        public float searchDistance = 5;
        public List<BattleUnitObject> detectUnitObjectList = new List<BattleUnitObject>(20);


        [Header("길찾기관련")]
        public TimerModule_Element searchPathTimerModule;

        public Vector3Int startTilePos;
        public Vector3Int endTilePos;
        public List<Vector3Int> pathList = new List<Vector3Int>(50);//경로리스트//길찾기로는 벡터로 돌리는게 맞아보이는데
        public int pathCount = 0;//경로카운트


        [Header("이팩트")]
        public EffectGroup moveEffectGroup;        
        public AudioSource audioSource;

        [Header("유닛의 게임진행여부")]
        //여기부분을 전장타일에 배치되있으면 AI가 작동될수 있게 제작할수 있게 변경하자
        //이건나중에 처리하기
        public bool isGameStart = false;

        public bool IsGameStart { get => isGameStart; set => isGameStart = value; }

        // public UnitStatusData UnitStatusData { get => unitStatusData; }
        //public UnitStateData UnitStateData => unitStateData;

        private BattleManager battleManager;
        private HexBasementTileMap hexTileMap;

        protected override void Awake()
        {
            base.Awake();

            if (TryGetComponent(out unitAbilityModule))
            {
                //공격처리//공격쿨타이머추가
                unitAbilityModule.AddStatusChangeEvent(UnitStatusType.AttackSpeed, SetAttackTimer);
                if (unitAbilityModule.GetUnitStatusValue(UnitStatusType.AttackSpeed, out var attackSpeed))
                {
                    SetAttackTimer(attackSpeed);
                    attackCoolTimer.SetActionEvent(() => { AttackAction(targetUnitObject); });
                }

                //죽음처리
                unitAbilityModule.AddStatusChangeEvent(UnitStatusType.HealthPoint, DeadEvent);

                //시야처리
                if (unitAbilityModule.GetUnitStatusValue(UnitStatusType.AttackDistance, out var searchDistanceValue))
                {
                    searchDistance = searchDistanceValue;
                }
            }



            ////능력들체크
            //if (unitAbility.unitObject_Base == null)
            //{
            //    unitAbility.InitUnitAbility(this);
            //}

            ////콜렉트능력체크
            //if (unitAbility != null)
            //{
            //    collectAbilityData?.InitCollectAbilityData(unitAbility);
            //}


            audioSource = GetComponent<AudioSource>();
            searchPathTimerModule.SetTimer(0.1f);
            gizmosColor = Random.ColorHSV();

            moveEffectGroup.Stop();
        }        

        private void SetAttackTimer(float value)
        {
            attackCoolTimer.SetCoolTime(value);
        }

        private void DeadEvent(float value)
        {
            if (1 > value)
            {
                this.SetActive(false);//나중에 애니메이션으로 바꾸고 애니메이션이 끝나면 집어넣어서 처리하기
                curHexTileObject.BatchUnitObject = null;
                //BattleManager.Instance.UnBatchUnit(this);
                BattleManager.Instance.gamePlayRuleManager.RemoveTeamUnit(this);
                BattleManager.Instance.lastLiveEnemy = this;
                unitUI.SetActive(false);





                //업적갱신
                if (unitTeamType == TeamType.Enemy)
                {
                    //적군사망
                    AchievementManager.Instance.UpdateRecordData(unitObjectInfo.recordID_KillEnemy, 1);

                    //적군종류
                    switch (unitObjectInfo.unitClassType)
                    {
                        case DataBase.UnitClassType.RifleMan:
                        case DataBase.UnitClassType.Officer:
                        case DataBase.UnitClassType.MachineGunMan:
                            AchievementManager.Instance.UpdateRecordData(unitObjectInfo.recordID_KillIInfantryman, 1);
                            break;
                        case DataBase.UnitClassType.Tank:
                            AchievementManager.Instance.UpdateRecordData(unitObjectInfo.recordID_DestroyTank, 1);
                            break;
                        case DataBase.UnitClassType.Structure:
                            break;
                    }
                }
            }
        }


        public override bool GetGameTeamRule(out TeamRuleData teamRuleData)
        {
            teamRuleData = this.teamRuleData;
            return true;
        }

        public override bool GetTeamRolePriorityBible(out TeamRolePriorityBible teamRolePriorityBible)
        {
            teamRolePriorityBible = null;   
            return false;
        }

        public override TeamType GetTeamType()
        {
            return unitTeamType;
        }


        protected override void Update()
        {
            //배틀유닛오브젝트//메인루프
            if (!isGameStart)
            {
                return;
            }

            attackCoolTimer.UpdateCoolTimer();
            unitUI.UpdateUnitUIBar();

            //20230620
            //이거 나중에 가이드라인으로 적어두기
            //AI는 대부분 공통적이다.
            //1. 명령을 기다린다.
            //명령은 외부적인 행위이다.
            //명령은 외내부에의 특정 객체가 주는 행위이다.

            //2. 탐색, 감지등 대상 확인을 우선.
            //여기서 탐색이란 근처를 확인하는 행위이다
            //무엇을 확인하느냐에 따라 다르다. 아군? 적군? 스탯? 특정 오브젝트?등을
            //탐색, 감지 후 행위로 넘어간다.
            //각 감지행위를 상태값으로 가지고 있어야 함


            //3. 행위
            //행위는 행동, 액션, 움직임이다.
            //외부행위를 먼저 작업하고, 내부행위를 진행한다.
            //내부행위란 자체적인 행동방식을 뜻함.




            //먼저 적군감지 후 이동이 먼저
            //공격할대상이 없으면 다음대상을 찾음
            //일정시간마다 길찾기 시작
            bool isIdleState = true;
            Vector3 unitPos = tr.position;
            if (!hexTileMap.GetHexAreaInfo(lLcroweUtil.GetWorldToCell(unitPos, hexTileMap.GetTileMap()), out var startHexTileData))
            {
                return;
            }
            startTilePos = startHexTileData.GetTilePos();


            //이동//뉴럴클라우드 체크했을시 이동하고 타일이 변경되는게 아니라//타일이 변경되고 이동함//그래서 이동하는자리에 선점을 못함
            //목적 //길찾기 경로를 체크하여 해당지점으로 이동
            //순서체크
            //1. 경로는 일정하게 재확인하여 가져와야됨//일단은 한번만 확인//그래야지 안겹침
            //2. 경로가 있는지 체크
            //3. 이동할경로에다 먼저 데이터들을 대입한다.
            //4. 이동한다.
            //5. 위치에 도달하면 2번으로 간다
            //반복
            //6. 도달하면 종료

            //중첩되는 이동구역처리



            //2. 경로가 있는지 체크
            if (pathList.Count != 0)
            {
                //존재하면//다음구역가져오기
                //변환이 필요//다음위치로 갈수 있는지 체크
                if (!hexTileMap.GetHexAreaInfo(pathList[pathCount], out var nextHexTileData))
                {
                    //없으면//작동안함            
                    return;
                }

                //배치구역갱신하기
                var nextHexTileObject = nextHexTileData.GetHexTileObject;

                //다르면 데이터 갱신하기
                if (curHexTileObject != nextHexTileObject)
                {
                    //다른타일로 갱신되었을때 해당타일에 오브젝트가 존재하는지체크
                    if (nextHexTileObject.hexTileData.IsExistObject)
                    {
                        //존재하면 경로 다시 체크
                        FindPath(battleManager);
                        return;
                    }

                    //기존데이터 변경
                    curHexTileObject.BatchUnitObject = null;
                    nextHexTileObject.BatchUnitObject = this;
                    curHexTileObject = nextHexTileObject;
                }



                //또한 갱신된 타일의 위치에 있는 내용물체크//중첩되는거 처리
                if (curHexTileObject.BatchUnitObject != this)
                {
                    //Debug.Log("위치중첩", gameObject);
                    //중첩되면 길찾기 다시하기//근처길로 반환

                    FindPath(battleManager);
                    return;
                }


                //월드거리체크//다음위치로 갔는지 여부
                Vector3 trPos = tr.position;
                Vector3 nextMoveWorldPos = nextHexTileObject.transform.position;
                if (!lLcroweUtil.CheckDistance(nextMoveWorldPos, trPos, 0.1f))
                {
                    //이동중엔 공격 안함
                    //isIdleStat = false;
                    Move(nextMoveWorldPos);
                    moveEffectGroup.Play(audioSource);
                    return;
                }

                //다 도착했으면
                moveEffectGroup.Stop();

                //공격탐지
                if (DetectEnemyUnitObject(this, hexTileMap))
                {
                    //찾은 첫번째요소를 가져온후//공격
                    targetUnitObject = detectUnitObjectList[0];
                    Attack(targetUnitObject.tr.position);
                    return;
                }

                //경로최대치체크
                if (pathCount < pathList.Count - 1)
                {
                    isIdleState = false;
                    ++pathCount;
                }
            }

            //1. 경로체크
            if (searchPathTimerModule.CheckTimer())
            {
                FindPath(battleManager);
            }





            //아무행동을 안했으면 아이돌상태
            if (isIdleState)
            {
                Idle();
            }
        }


        public void FindPath(BattleManager battleManager)
        {
            //근처 적군위치를 가져오기//없으면 마지막타일그대로 주기           
            //battleManager.GetOtherUnitObject(unitTeamType, unitPos, ref endTilePos);

            //이속을 체크
            if (unitAbilityModule.GetUnitStatusValue(UnitStatusType.MoveSpeed, out var speed))
            {
                if (speed < 0.1f)
                {
                    //유닛이속이 느리면 이동을 안함
                    pathList.Clear();
                    pathCount = 0;
                    return;
                }
            }

            //다른 팀타겟을 요청
            if (!battleManager.gamePlayRuleManager.RequestOtherTeamTarget(this, out var returnTarget))
            {   
                return;
            }
            
            var battleUnit = returnTarget as BattleUnitObject;
            if (battleUnit == null)
            {
                return;
            }
            endTilePos = battleUnit.GetBatchTilePos();

            //매니저로부터 살아있는 적군이 존재하는지 체크
            //존재하면 해당위치로 근처(근처타일)로 가기
            if (battleManager.AStarHex.Search(startTilePos, endTilePos, ref pathList, false))
            {
                //신규위치면
                //자동갱신
                pathCount = 0;
            }
        }

        /// <summary>
        /// 경로 초기화
        /// </summary>
        public void ResetPath()
        {
            pathList.Clear();
        }


        /// <summary>
        /// 전투시작시 진행되는 이벤트 함수 발동
        /// </summary>
        public void StartBattleEvent()
        {   
            battleManager = BattleManager.Instance;
            hexTileMap = battleManager.hexBasementTileMap;
            

        }


        /// <summary>
        /// 전투가 끝났을시 진행되는 이벤트 함수
        /// </summary>
        public void EndBattleEvent()
        {
            //회복처리
            if (!unitAbilityModule.GetUnitStatusValue(UnitStatusType.CombatEndHill, out var value))
            {
                if (value > 0)
                {
                    if (unitAbilityModule.GetUnitStatusValue(UnitStatusType.HealthPoint,out var healthPoint))
                    {
                        healthPoint += value;
                        unitAbilityModule.SetUnitStatusValue(UnitStatusType.HealthPoint, healthPoint);
                        //게임오브젝트가 켜져있으면 UI를 띄어주기
                    }
                }
            }
            
            //쿨타임초기화
            attackCoolTimer.ResetTime();//이함수맞나 주석이 읍네
            attackCoolTimer.CancelCoolTime();//이건확실한데 체크하기

            //UI크기
            unitUI.SetActive(false);


            //길찾기 초기화
            isGameStart = false;
        }
            
        /// <summary>
        /// 공격할대상을 찾는 함수//매프레임마다 돌리는지는 않을거니 그러려니하자
        /// </summary>
        /// <param name="searchUnit">탐색하고 있는 유닛</param>
        /// <param name="isFindLive">살아잇는 유닛을 찾을 것인지 체크</param>
        /// <returns>찾았는지 여부</returns>
        private static bool DetectEnemyUnitObject(BattleUnitObject searchUnit, HexBasementTileMap hexBasementTileMap, bool isFindLive = true)
        {
            Vector3 origin = searchUnit.transform.position;
            float searchSize = searchUnit.searchDistance;
            var detectList = searchUnit.detectUnitObjectList;


            detectList.Clear();
            Custom3DHexTileMap custom3DHexTileMap = hexBasementTileMap.GetTileMap();
            Vector3Int tilePos = lLcroweUtil.GetWorldToCell(origin, custom3DHexTileMap);
            Vector3Int[] findTilePosArray =  lLcroweUtil.GetNearRangePos(tilePos, Mathf.RoundToInt(searchSize), custom3DHexTileMap);
            var teamRole = searchUnit.teamRuleData;

            //타일들에서 유닛오브젝트 체크
            for (int i = 0; i < findTilePosArray.Length; i++)
            {
                //타일이 존재하는지 체크
                if (!hexBasementTileMap.GetHexAreaInfo(findTilePosArray[i],out HexTileData hexTileData))
                {
                    continue;
                }


                //타일에 유닛이 존재하는지 체크
                BattleUnitObject detectBattleUnitObject = hexTileData.GetHexTileObject.BatchUnitObject as BattleUnitObject;
                if (detectBattleUnitObject == null)
                {
                    continue;
                }

                if (detectBattleUnitObject == searchUnit)
                {
                    continue;
                }

                if (detectBattleUnitObject.IsAlive() != isFindLive)
                {
                    //찾는 상태와 같지않으면 넘어가기
                    continue;
                }

                //적군 == 아군, 플레이어감지
                //아군 == 적군 감지
                //플레이어 == 적군감지
                //여기 고쳐야됨
                //저쪽인터페이스에서 작동되게하자
                //게임팀룰에 따른 상대방을 찾을수 있게


                if (teamRole.CheckIsOtherTeam(detectBattleUnitObject))
                {
                    detectList.Add(detectBattleUnitObject);
                }
            }

            //후에 거리 오름차순정렬 
            detectList.Sort(new DistanceSort(searchUnit.tr.position));

            //찾은게 없으면 false
            if (detectList.Count == 0)
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// 유닛이동및회전
        /// </summary>
        public virtual void Move(Vector3 target)
        {
            if (!unitAbilityModule.GetUnitStatusValue(UnitStatusType.MoveSpeed,out var speed))
            {
                return;
            }

            Vector3 dir = (target - tr.position).normalized;
            speed *= Time.deltaTime;
            Debug.DrawRay(tr.position, dir);

            Quaternion rot = Quaternion.LookRotation(dir);
            rot = Quaternion.Lerp(tr.rotation, rot, speed * 2);

            tr.SetPositionAndRotation(speed * dir + tr.position, rot);
        }

        /// <summary>
        /// 유닛일반공격
        /// </summary>
        public virtual void Attack(Vector3 target)
        {
            if (!unitAbilityModule.GetUnitStatusValue(UnitStatusType.MoveSpeed,out var rotateSpeed))
            {
                return;
            }
            Vector3 dir = (target - tr.position).normalized;
            rotateSpeed *= Time.deltaTime;
            Quaternion rot = Quaternion.LookRotation(dir);
            tr.rotation = Quaternion.Lerp(tr.rotation, rot, rotateSpeed * 2);

            //내적처리
            if (Vector3.Dot(tr.forward, dir) < 0.99f)
            {
                return;
            }
            attackCoolTimer.StartSkill();
        }

        /// <summary>
        /// 공격행위 함수//어택쿨타임에 달림
        /// </summary>
        /// <param name="targetUnitObject">맞출대상</param>
        public virtual void AttackAction(BattleUnitObject targetUnitObject)
        {
            //애니메이션등등 이팩트관련 및 공격처리
            if (TryGetComponent(out ScoreTarget attackUnitScore))
            {
                attackUnitScore.AddScore(ScoreType.AttackCount, 1);
            }


            var damageObject = ObjectPoolManager.Instance.RequestDynamicComponentObject(unitObjectInfo.damageObjectPrefab);
            damageObject.name += gameObject.name;

            var muzzlePos = GetMuzzleTransform();
            damageObject.InitTrObjPrefab(muzzlePos);
            damageObject.InitDamageObject(this, targetUnitObject);
            damageObject.SetActive(true);
        }

        /// <summary>
        /// 파이어이팩트 작동 함수
        /// </summary>
        /// <param name="targetMuzzlePos">타겟 노즐위치</param>
        public void ActionFireEffect(Transform targetMuzzlePos)
        {
            //이팩트 작동
            if (EffectManager.Instance.RequestFXObject(unitObjectInfo.fireEffectPrefab, targetMuzzlePos, true, out var effectObject))
            {
                Vector3 pos = targetMuzzlePos.position;
                effectObject.Action(pos, pos, 1f);
            }
        }


        /// <summary>
        /// 패시브스킬1//유닛이 자동
        /// </summary>
        public virtual void Skill1()
        {

        }

        /// <summary>
        /// 액티브스킬1//유닛이 자동
        /// </summary>
        public virtual void Skill2()
        {

        }

        /// <summary>
        /// 액티브스킬2//플레이어가 수동
        /// </summary>
        public virtual void Skill3()
        {

        }

        public abstract Transform GetMuzzleTransform();


        private Color gizmosColor;
        private void OnDrawGizmos()
        {
            //if (!Application.isPlaying)
            //{
            //    return;
            //}

            if (battleManager == null)
            {
                return;
            }
            if (hexTileMap == null)
            {
                return;
            }


            var temp = hexTileMap.hexAreaBible;
            Gizmos.color = gizmosColor;
            for (int i = 1; i < pathList.Count; i++)
            {
                if (temp.ContainsKey(pathList[i]) && temp.ContainsKey(pathList[i - 1]))
                {
                    Vector3 a = temp[pathList[i - 1]].GetWorldPos() + Vector3.up;
                    Vector3 b = temp[pathList[i]].GetWorldPos() + Vector3.up;

                    Gizmos.DrawLine(a, b);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, searchDistance);
        }
    }
}
