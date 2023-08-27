using lLCroweTool.TimerSystem;
using UnityEngine;

namespace lLCroweTool.Ability
{
    [System.Serializable]
    public class SkillData
    {
        [SerializeField] private int stackCurNum = 0;//현재스택량
        public CoolTimerModule_Element skillCooltimer = new CoolTimerModule_Element();//버튼으로 표시할시 이모듈을 사용해야함//UI로 작업할려면 이 쿨타이머를 참조해야함
        public CoolTimerModule_Element skillStackTimer = new CoolTimerModule_Element();//스택쿨타임모듈 //스택에 대한 쿨타임만 관리함 

        /// <summary>
        ///스킬슬롯업데이트
        /// </summary>
        public void UpdateAbilityData()
        {
            CoolTimerModule_Element.UpdateCoolTimer(skillCooltimer);
            CoolTimerModule_Element.UpdateCoolTimer(skillStackTimer);

        }

        public void SetStackCurNum(int _stackCurNum)
        {
            stackCurNum = _stackCurNum;
        }

        public int GetStackCurNum()
        {
            return stackCurNum;
        }

        /// <summary>
        /// 스킬슬롯의 쿨타이머를 세팅하기위한 함수//데이터가 들어가있어야됨
        /// </summary>
        /// <param name="_targetSkillSlot">타겟이 될 스킬슬롯</param>
        private void SetSkillSlotCoolTimer(SkillInfo abilityInfo)
        {
            Debug.Log("어빌리티데이터 쿨타임초기화작동");
            switch (abilityInfo.abilityActionType)
            {
                case AbilityActionType.Active:
                    //액티브 스킬데이터 세팅
                    //쿨타임 세팅

                    //이건 스킬데이터
                    skillCooltimer.SetCoolTime(abilityInfo.skillCoolTime);

                    //스택 세팅
                    if (abilityInfo.isUseStack)
                    {
                        skillStackTimer.SetCoolTime(abilityInfo.stackingTimer);
                        //_targetSkillSlot.skillStackTimer.SetReadyToCoolEvent(delegate { CalSkillStackCount(_targetSkillSlot, 1); });
                        skillStackTimer.SetReadyToCoolEvent(delegate { AddSkillStackCount(abilityInfo, 1); });
                        skillStackTimer.StartSkill();
                    }
                    //else
                    //{
                    //    skillStackTimer.SetCoolTime(0);
                    //    skillStackTimer.SetReadyToCoolEvent(delegate { });
                    //}

                    //캐스팅 세팅
                    //_targetSkillSlot.skillCastTimer.SetActionEvent(delegate { _targetSkillSlot.GetTargetWorldObject().skillToolBar.SetisUsingSkill(true); _targetSkillSlot.GetSkillPrefab().ActionSkillCast(); });
                    //skillCastTimer.SetActionEvent(delegate { _targetSkillSlot.GetSkillPrefab().ActionSkillCast(); });
                    //skillCastTimer.SetCoolTime(abilityInfo.castingTime);
                    //skillCastTimer.SetReadyToCoolEvent(delegate { ActiveSkill(_targetSkillSlot); });//스킬작동                               
                    //Debug.Log("액티브스킬세팅완료");
                    break;


                case AbilityActionType.Passive:
                    //패시브데이터세팅
                    //아무행동안함
                    //Debug.Log("패시브스킬세팅완료");
                    break;
            }
        }

        /// <summary>
        /// 스킬슬롯의 스택향상 함수
        /// </summary>
        /// <param name="_skillSlot">타겟이 될 스킬슬롯</param>
        /// <param name="_addAmount">추가할 수량</param>
        private void AddSkillStackCount(SkillInfo abilityInfo, int _addAmount)
        {
            //스킬이 스택을 사용하는가?

            if (abilityInfo.isUseStack)
            {
                //스택계산
                int temp = GetStackCurNum() + _addAmount;
                if (temp > abilityInfo.stackMaxNum)
                {
                    temp = abilityInfo.stackMaxNum;
                }
                SetStackCurNum(temp);

                //리미트 체크//이하이면 스택쿨타임을 작동시켜 스택을 쌓게함
                if (GetStackCurNum() < abilityInfo.stackMaxNum)
                {
                    //스택쿨타임작동
                    skillStackTimer.StartSkill();
                }
            }
        }

        /// <summary>
        /// 스킬슬롯의 스택감소 함수
        /// </summary>
        /// <param name="_skillSlot">타겟이 될 스킬슬롯</param>
        /// <param name="_subAmount">감소할 수량</param>
        private void SubSkillStackCount(SkillInfo abilityInfo, int _subAmount)
        {
            //스킬이 스택을 사용하는가?
            if (abilityInfo.isUseStack)
            {
                //스택계산
                int temp = GetStackCurNum() - _subAmount;
                if (temp < 0)
                {
                    temp = 0;
                }
                SetStackCurNum(temp);

                //리미트 체크//이하이면 스택쿨타임을 작동시켜 스택을 쌓게함
                if (GetStackCurNum() < abilityInfo.stackMaxNum)
                {
                    //스택쿨타임작동
                    skillStackTimer.StartSkill();
                }
            }
        }
        /// <summary>
        /// 능력이 준비됫는 체크하는 함수
        /// </summary>
        /// <param name="isUseNotice">UI알람사용여부</param>
        /// <returns>능력이 준비됫는지 여부</returns>
        public bool CheckAbilityReady(SkillInfo abilityInfo, bool isUseUINotice)
        {
            //스택 사용여부
            if (abilityInfo.isUseStack)
            {
                //현재 스택수 체크
                if (1 > GetStackCurNum())
                {
                    if (isUseUINotice)
                    {
                        //경고문자                    
                        //NoticeUIManager.Instance.ShowNotice("스택이 부족합니다.", NoticeUIManager.NoticeType.Info);
                        skillCooltimer.SetTimeValue(1);//=>UI떄문에 해논것
                    }
                    return false;
                }
                //쿨타임체크
                else if (!skillCooltimer.GetEnableSkill())
                {

                    if (isUseUINotice)
                    {
                        //경고문자
                        //NoticeUIManager.Instance.ShowNotice("스킬이 쿨타임입니다.", NoticeUIManager.NoticeType.Info);
                    }
                    return false;
                }
            }
            else
            {
                //쿨타임확인
                if (!skillCooltimer.GetEnableSkill())
                {
                    if (isUseUINotice)
                    {
                        //경고문자
                        //Debug.Log("스킬이 쿨타임입니다.");
                        //NoticeUIManager.Instance.ShowNotice("스킬이 쿨타임입니다.", NoticeUIManager.NoticeType.Info);
                    }
                    return false;
                }
            }

            return true;
        }
    }

}
