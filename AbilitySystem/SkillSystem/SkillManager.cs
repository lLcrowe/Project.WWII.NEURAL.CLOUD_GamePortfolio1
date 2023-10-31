using lLCroweTool.DestroyManger;
//using lLCroweTool.NoticeSystem;
using lLCroweTool.TimerSystem;
using UnityEngine;

namespace lLCroweTool.SkillSystem
{
    public class SkillManager : MonoBehaviour
    {
        //스킬시스템(매니저)
        //여러스킬에 대한 백과사전같은 역할
        //스킬리스트를 만들어서 제어함
        //커맨드시스템이 현시스템을 참조하며
        //스킬시스템에 등록된 스킬에 있는 커맨드를 참조함

        //추가필요사항
        //스킬노드
        //사용여부-> 스킬언락여부
        //스킬데이터로 처리->문제있음 프리팹으로 처리예정
        //스킬이 만들어지면 그걸로 처리할예정
        //20200326


        
        private static SkillManager instance;
        public static SkillManager Instance
        {
            get
            {
                if (ReferenceEquals(instance, null))
                {
                    instance = FindObjectOfType<SkillManager>();
                    if (ReferenceEquals(instance, null))
                    {
                        GameObject tmp = new GameObject();
                        instance = tmp.AddComponent<SkillManager>();
                        tmp.name = "-=SkillManager=-";
                    }
                }
                return instance;
            }
        }

        //캐싱구역
        private Skill_Base targetSkillObject;
        private KeyCode inputSkillKeyCode;
        
        //월드오브젝트 추가
        //유닛모듈추가
        //Skill에 있는 몇가지 매니저로 전환
        private bool isDone = false;

        private void Awake()
        {
            instance = this;
        }

        /// <summary>
        /// 스킬슬롯의 스킬데이터안의 스킬베이스를 초기화해주는 함수
        /// </summary>
        /// <param name="_skillSlot"></param>
        public void InitSkillSlot(SkillSlot _skillSlot)
        {
            //먼저 해당되는 스킬슬롯을 초기화해준다//다른데에서 함

            //해당 스킬슬롯에 스킬데이터의 스킬프리팹을 할당해준다.
            targetSkillObject = Instantiate(_skillSlot.GetSkillData().targetSkillPrefab, _skillSlot.GetTargetWorldUnitObject().transform);
            _skillSlot.SetSkillPrefab(targetSkillObject);

            //스킬프리팹을 세팅해준다.
            targetSkillObject.SetTargetSkillSlot(_skillSlot);
            targetSkillObject.InitSkill();

            //해당 스킬슬롯에 스킬데이터의 세팅값을 세팅해준다.
            SetSkillSlotCoolTimer(_skillSlot);
            targetSkillObject = null;

            ////트랜스폼 조절
            //Transform targetWorldObjectTranform = _skillSlot.GetTargetWorldObject().transform;
            //targetSkillObject.transform.parent = targetWorldObjectTranform.parent;
            //targetSkillObject.transform.SetPositionAndRotation(targetWorldObjectTranform.position, targetWorldObjectTranform.rotation);
        }

        /// <summary>
        /// 스킬슬롯의 스킬데이터안의 스킬베이스를 작동해주는 함수
        /// </summary>
        /// <param name="_skillSlot">타겟이 될 스킬슬롯</param>
        /// <param name="isUsePlayer">플레이어가 사용하는가 여부</param>
        /// <param name="usingSkillSlotNum">사용할 스킬슬롯 번호</param>
        public void ActionSkill(SkillSlot _skillSlot, bool isUsePlayer)
        {
            //작동로직 체크
            //사용자가 스킬을 사용한다.
            //버튼을 누른다.
            //패시브인가 액티브인가 체크
            //스택인가 체크
            //Yes 스택상태를 확인. 스택이 있다 캐스팅 쿨타임으로 넘어감
            //No 캐스팅쿨타임으로 곧장 작동시킴
            //캐스팅인가 체크=>할필요가있나? 딜레이로체크
            //캐스팅시간이 다되면 스킬이벤트 발동
            //스킬이 작동된다.

            //스킬을사용하고 있으면 작동안하게 변경
            if (!_skillSlot.GetTargetWorldUnitObject().skillToolBar.GetIsUsingSkill()) 
            {
                switch (_skillSlot.GetSkillData().skillCATType)
                {
                    //액티브 스킬
                    case SkillCATType.ActiveSkill_Myself:
                        //스택확인
                        if (CheckSkillStack(_skillSlot, isUsePlayer))
                        {
                            //AI사용여부 체크
                            if (isUsePlayer)
                            {
                                //플레이어사용
                                //스킬 액티브 이벤트가 들어간 캐스트 쿨타이머를 작동
                                _skillSlot.skillCastTimer.StartSkill();//캐스팅타이머에 ActiveSkill이 들어가있음
                                _skillSlot.GetTargetWorldUnitObject().skillToolBar.SetIsUsingSkill(true, _skillSlot);
                            }
                            else
                            {
                                //AI사용
                                Debug.Log("AI가 액티브스킬사용.");
                            }
                        }
                        break;
                    case SkillCATType.ActiveSkill_Other_Object:
                    case SkillCATType.ActiveSkill_Other_Pos:
                        //스택확인
                        if (CheckSkillStack(_skillSlot, isUsePlayer))
                        {
                            //AI사용여부 체크
                            if (isUsePlayer)
                            {
                                //플레이어사용
                                //스킬 포인터로 이동
                                //SkillPointer.Instance.InitSkillPointer(_skillSlot, inputSkillKeyCode);
                            }
                            else
                            {
                                //AI사용
                                Debug.Log("AI가 좌표지정스킬사용.");
                            }
                        }
                        break;
                    //패시브 스킬
                    case SkillCATType.PassiveSkill:
                        _skillSlot.GetSkillPrefab().ActionSkill();

                        //case SkillCATType.PassiveSkill_ObjectActive_Other_Object:
                        //case SkillCATType.PassiveSkill_ObjectActive_Other_Position:
                        //case SkillCATType.PassiveSkill_Spawn:
                        //패시브 스킬 분류
                        //switch (skillData.passiveSkillType)
                        //{
                        //    case PassiveSkillType.Periodic://영구
                        //        SkillManager.Instance.SkillActionLibary(this);
                        //        break;
                        //    case PassiveSkillType.Permanent://주기                            
                        //        skillCastTimer.StartSkill();
                        //        skillCastTimer.isRepeat = true;
                        //        break;
                        //    case PassiveSkillType.Responsive://반응                            
                        //        //월드 오브젝트 히트박스에 집어넣기
                        //        targetWorldObject.objectHitbox.hitEvent.AddListener(delegate { Responsive(); });
                        //        break;
                        //}
                        break;
                }
            }
            else
            {
                //스킬을 사용중이면?
                if (isUsePlayer)
                {
                    //스킬을 사용중이니 스킬을 사용못함
                }
                else
                {
                    
                    Debug.Log("AI가 스킬을 사용못합니다.");
                }
            }
        }

        /// <summary>
        /// 스킬을 캔슬할때 사용하는 함수
        /// </summary>
        /// <param name="_skillSlot">타겟이될 스킬슬롯</param>
        public void CancelSkill(SkillSlot _skillSlot)
        {
            _skillSlot.skillCastTimer.CancelCoolTime();
            _skillSlot.GetSkillPrefab().CancelSkillCast();
        }

        /// <summary>
        /// 스킬슬롯의 스킬데이터안의 스킬베이스를 리셋해주는 함수
        /// </summary>
        /// <param name="_skillSlot">스킬슬롯</param>
        public void ResetSkillSlot(SkillSlot _skillSlot)
        {            
            targetSkillObject = _skillSlot.GetSkillPrefab();
            if (targetSkillObject != null)
            {
                targetSkillObject.ResetSkill();
                DestroyManager.Instance.AddDestoryGameObject(targetSkillObject.gameObject);
                targetSkillObject = null;
            }

            _skillSlot.SetSkillPrefab(null);
            ResetSkillSlotCoolTimer(_skillSlot);
        }

        /// <summary>
        /// 스킬슬롯의 쿨타이머모듈을 리셋시켜주는 함수
        /// </summary>
        /// <param name="_targetSkillSlot">리셋 시킬 스킬 슬롯</param>
        private static void ResetSkillSlotCoolTimer(SkillSlot _targetSkillSlot)
        {
            Debug.Log("스킬슬롯 쿨타임리셋작동");
            //쿨타이머들 초기화
            //변경시키면 기능을 다 정지시키기
            //원래 돌았가는것도 멈추어버리기
            _targetSkillSlot.SetStackCurNum(0);
            _targetSkillSlot.skillCooltimer.isRepeat = false;
            _targetSkillSlot.skillStackTimer.isRepeat = false;
            _targetSkillSlot.skillCastTimer.isRepeat = false;
            _targetSkillSlot.skillCooltimer.SetActionEvent(delegate { });
            _targetSkillSlot.skillStackTimer.SetActionEvent(delegate { });
            _targetSkillSlot.skillCastTimer.SetActionEvent(delegate { });
            _targetSkillSlot.skillCooltimer.SetReadyToCoolEvent(delegate { });
            _targetSkillSlot.skillStackTimer.SetReadyToCoolEvent(delegate { });
            _targetSkillSlot.skillCastTimer.SetReadyToCoolEvent(delegate { });
            _targetSkillSlot.skillCooltimer.SetTimeValue(1f);
            CoolTimerModule_Element.UpdateCoolTimer(_targetSkillSlot.skillCooltimer);
            _targetSkillSlot.skillStackTimer.SetTimeValue(1f);
            CoolTimerModule_Element.UpdateCoolTimer(_targetSkillSlot.skillStackTimer);
            _targetSkillSlot.skillCastTimer.SetTimeValue(1f);
            CoolTimerModule_Element.UpdateCoolTimer(_targetSkillSlot.skillCastTimer);
        }

        //스킬데이터에 있는 데이터와 이벤트를 집어넣기 위한 함수
        //스킬이 데이터에 따른 이벤트초기화용
        //다시 재정립해서 생각해보자


        /// <summary>
        /// 스킬슬롯의 쿨타이머를 세팅하기위한 함수//데이터가 들어가있어야됨
        /// </summary>
        /// <param name="_targetSkillSlot">타겟이 될 스킬슬롯</param>
        private void SetSkillSlotCoolTimer(SkillSlot _targetSkillSlot)
        {
            SkillObjectScript skillData = _targetSkillSlot.GetSkillData();

            Debug.Log("스킬슬롯 쿨타임초기화작동");
            switch (skillData.skillCATType)
            {
                case SkillCATType.ActiveSkill_Myself:
                case SkillCATType.ActiveSkill_Other_Object:
                case SkillCATType.ActiveSkill_Other_Pos:
                  
                    //액티브 스킬데이터 세팅
                    //쿨타임 세팅
                    _targetSkillSlot.skillCooltimer.SetCoolTime(skillData.skillCooltime);

                    //스택 세팅
                    if (skillData.isUseStack)
                    {
                        _targetSkillSlot.skillStackTimer.SetCoolTime(skillData.stackingTimer);
                        //_targetSkillSlot.skillStackTimer.SetReadyToCoolEvent(delegate { CalSkillStackCount(_targetSkillSlot, 1); });
                        _targetSkillSlot.skillStackTimer.SetReadyToCoolEvent(delegate { AddSkillStackCount(_targetSkillSlot, 1); });
                        _targetSkillSlot.skillStackTimer.StartSkill();
                    }
                    //else
                    //{
                    //    skillStackTimer.SetCoolTime(0);
                    //    skillStackTimer.SetReadyToCoolEvent(delegate { });
                    //}

                    //캐스팅 세팅
                    //_targetSkillSlot.skillCastTimer.SetActionEvent(delegate { _targetSkillSlot.GetTargetWorldObject().skillToolBar.SetisUsingSkill(true); _targetSkillSlot.GetSkillPrefab().ActionSkillCast(); });
                    _targetSkillSlot.skillCastTimer.SetActionEvent(delegate {_targetSkillSlot.GetSkillPrefab().ActionSkillCast(); });
                    _targetSkillSlot.skillCastTimer.SetCoolTime(skillData.castingTime);
                    _targetSkillSlot.skillCastTimer.SetReadyToCoolEvent(delegate { ActiveSkill(_targetSkillSlot); });//스킬작동                               
                    //Debug.Log("액티브스킬세팅완료");
                    break;
                case SkillCATType.PassiveSkill:
                    //패시브데이터세팅
                    //아무행동안함
                    //Debug.Log("패시브스킬세팅완료");
                    break;
            }
        }

        /// <summary>
        /// 실질적으로 스킬이 작동되는 구역
        /// 캐스터에서 돌아감
        /// </summary>
        /// <param name="_skillSlot"></param>
        private void ActiveSkill(SkillSlot _skillSlot)
        {
            //Debug.Log("스킬작동");
            //스킬작동
            _skillSlot.GetSkillPrefab().ActionSkill();
            //스택계산
            _skillSlot.skillCooltimer.StartSkill();//스킬쿨타임작동
            SubSkillStackCount(_skillSlot, 1);//스택쿨타임작동
            //CalSkillStackCount(_skillSlot, -1);

            //위치정리
            _skillSlot.targetWorldObjectList.Clear();
            _skillSlot.targetPosList.Clear();
            //스킬사용리셋
            _skillSlot.GetTargetWorldUnitObject().skillToolBar.SetIsUsingSkill(false, null);
        }

        /// <summary>
        /// 스킬슬롯의 스택 확인 함수.
        /// </summary>
        /// <param name="_skillSlot">스킬슬롯</param>
        /// <param name="isUseNotice">알림에 띄울것인가 여부</param>
        /// <returns>스택량이 맞는가 여부</returns>
        private bool CheckSkillStack(SkillSlot _skillSlot, bool isUseNotice)
        {
            isDone = false;
            SkillObjectScript skillData = _skillSlot.GetSkillData();
        
            //스택 사용여부
            if (skillData.isUseStack)
            {
                //스택 사용시
                //현재 스택수
                //스킬가능한지

                if (1 > _skillSlot.GetStackCurNum())
                {
                    if (isUseNotice)
                    {
                        //경고문자                    
                        //NoticeUIManager.Instance.ShowNotice("스택이 부족합니다.", NoticeUIManager.NoticeType.Info);
                        _skillSlot.skillCooltimer.SetTimeValue(1);//<=UI떄문에 해논것
                    }
                }
                else if(!_skillSlot.skillCooltimer.GetEnableSkill())
                {
                    if (isUseNotice)
                    {
                        //경고문자
                        //NoticeUIManager.Instance.ShowNotice("스킬이 쿨타임입니다.", NoticeUIManager.NoticeType.Info);
                    }
                }
                else
                {
                    isDone = true;
                }
            }
            else
            {
                //쿨타임확인
                if (!_skillSlot.skillCooltimer.GetEnableSkill())
                {
                    if (isUseNotice)
                    {
                        //경고문자
                        //Debug.Log("스킬이 쿨타임입니다.");
                        //NoticeUIManager.Instance.ShowNotice("스킬이 쿨타임입니다.", NoticeUIManager.NoticeType.Info);
                    }
                }
                else
                {
                    isDone = true;
                }
            }

            return isDone;
        }

        /// <summary>
        /// 스킬슬롯의 스택 계산 함수(더하기 빼기 여기서작동)//이제 사용안함///안하는 이유는 다른곳에서 Add Remove 함수를 따로 만들어서 관리를 하는데 여기에서만 그렇게 작동시켜야 될이유를 모르겠음.
        /// </summary>
        /// <param name="_skillSlot">타겟될 스킬슬롯</param>
        /// <param name="AddAmount">얼만큼 추가할건지</param>
        //private void CalSkillStackCount(SkillSlot _skillSlot, int _addAmount)
        //{
        //    //스킬이 스택을 사용하는가?
        //    if (_skillSlot.skillData.isUseStack)
        //    {
        //        //스택계산
        //        int temp = _skillSlot.GetStackCurNum() + _addAmount;
        //        if (temp > _skillSlot.skillData.stackMaxNum)
        //        {

        //        }
        //        _skillSlot.SetStackCurNum(temp);


        //        //리미트 체크//이하이면 스택쿨타임을 작동시켜 스택을 쌓게함
        //        if (_skillSlot.GetStackCurNum() < _skillSlot.skillData.stackMaxNum)
        //        {
        //            //스택쿨타임작동
        //            _skillSlot.skillStackTimer.StartSkill();
        //        }
        //    }
        //}

        /// <summary>
        /// 스킬슬롯의 스택향상 함수
        /// </summary>
        /// <param name="_skillSlot">타겟이 될 스킬슬롯</param>
        /// <param name="_addAmount">추가할 수량</param>
        private void AddSkillStackCount(SkillSlot _skillSlot, int _addAmount)
        {
            //스킬이 스택을 사용하는가?
            SkillObjectScript skillData = _skillSlot.GetSkillData();

            if (skillData.isUseStack)
            {
                //스택계산
                int temp = _skillSlot.GetStackCurNum() + _addAmount;
                if (temp > skillData.stackMaxNum)
                {
                    temp = skillData.stackMaxNum;
                }
                _skillSlot.SetStackCurNum(temp);

                //리미트 체크//이하이면 스택쿨타임을 작동시켜 스택을 쌓게함
                if (_skillSlot.GetStackCurNum() < skillData.stackMaxNum)
                {
                    //스택쿨타임작동
                    _skillSlot.skillStackTimer.StartSkill();
                }
            }
        }

        /// <summary>
        /// 스킬슬롯의 스택감소 함수
        /// </summary>
        /// <param name="_skillSlot">타겟이 될 스킬슬롯</param>
        /// <param name="_subAmount">감소할 수량</param>
        private void SubSkillStackCount(SkillSlot _skillSlot, int _subAmount)
        {
            SkillObjectScript skillData = _skillSlot.GetSkillData();

            //스킬이 스택을 사용하는가?
            if (skillData.isUseStack)
            {
                //스택계산
                int temp = _skillSlot.GetStackCurNum() - _subAmount;
                if (temp < 0)
                {
                    temp = 0;
                }
                _skillSlot.SetStackCurNum(temp);

                //리미트 체크//이하이면 스택쿨타임을 작동시켜 스택을 쌓게함
                if (_skillSlot.GetStackCurNum() < skillData.stackMaxNum)
                {
                    //스택쿨타임작동
                    _skillSlot.skillStackTimer.StartSkill();
                }
            }
        }

        public void SetUseKeyCode(KeyCode _keyCode)
        {
            inputSkillKeyCode = _keyCode;
        }
    }

    //스킬 카테고리 설정
    //좀더 체크해볼것
    //모든사항 집어넣어보고 주석으로 정리
    //스킬데이터 수정완료
    //현재카테고리는 세팅할때와 작동할때 사용
    //20201213
    //스킬카테고리 개편
    public enum SkillCATType
    {
        ////액티브 5개
        //ActiveSkill_Buff_Myself,
        //ActiveSkill_Buff_Other_Object,//오브젝트 부모로
        ////ActiveSkill_Buff_Other_Position,
        //ActiveSkill_ObjectActive_Myself,
        //ActiveSkill_ObjectActive_Other_Object,//오브젝트 부모로
        //ActiveSkill_ObjectActive_Other_Position,//위치로 
        ////ActiveSkill_Spawn_Myself,
        ////ActiveSkill_Spawn_Other_Object,
        ////ActiveSkill_Spawn_Other_Position,
        ////패시브관련 4개
        //PassiveSkill_Buff_Myself,
        //PassiveSkill_ObjectActive_Myself,
        ////영구 주기에는 집어넣기는 좀 애매
        ////PassiveSkill_ObjectActive_Other_Object,
        ////PassiveSkill_ObjectActive_Other_Position,
        ////PassiveSkill_Spawn,
        ////현재 9개

        //20201213 개편
        ActiveSkill_Myself,//액티브스킬 자기자신에게 사용
        ActiveSkill_Other_Pos,//액티브스킬 다른 위치에 사용
        ActiveSkill_Other_Object,//액티브스킬 다른 오브젝트에 사용
        PassiveSkill,//패시브스킬
    }
}

