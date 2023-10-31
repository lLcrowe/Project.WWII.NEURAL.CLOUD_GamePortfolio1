using UnityEngine;
using System.Collections.Generic;
using lLCroweTool.TimerSystem;
using lLCroweTool.LevelSystem;

namespace lLCroweTool.SkillSystem
{
    [System.Serializable]
    public class SkillSlot
    {
        //스킬의 쿨타임
        //스킬의 활성화여부
        //스킬의 언락여부
        //스킬에 필요한 니드값(아직필요없는것)
        //스킬은 프리팹으로 관리할예정
        //스킬시스템에 집어넣어서 씬에서 관리하고
        //쓸사람이 있으면 해당 오브젝트를 복사해서 가져옴

        //동일한 스킬은 같은 오브젝트가 가지지를 못함
        //액티브 => 타겟팅 위치,오브젝트액티브, 버프 ==>쿨타임버튼으로 구현
        //패시브 => 오브젝트액티브, 버프    ==> 주기표현 쿨타임버튼, 작동됫는지 체크 

        //스킬//각각의 스킬은 프리팹화상태
        //자기자신(스킬의 주최자)
        private BattleUnitObject targetWorldObject;
        [Space]

        public LevelModule_Element skillLvModule;//레벨모듈 사용

        //스킬데이터
        [SerializeField] private SkillObjectScript skillData;
        [SerializeField] private int stackCurNum = 0;//현재스택량
        public CoolTimerModule_Element skillCooltimer = new CoolTimerModule_Element();//버튼으로 표시할시 이모듈을 사용해야함//UI로 작업할려면 이 쿨타이머를 참조해야함
        public CoolTimerModule_Element skillStackTimer = new CoolTimerModule_Element();//스택쿨타임모듈 //스택에 대한 쿨타임만 관리함 
        public CoolTimerModule_Element skillCastTimer = new CoolTimerModule_Element();//스킬캐스트타임모듈 //스킬에 대한 실질적인 작동

        //[Header("타겟이된 스킬")]
        //[Tooltip("Skill_Base 클래스를 참조함")]
        private Skill_Base targetSkillPrefab;

        [Header("타겟이 될 오브젝트or위치")]//다시 생각해볼것//단일로 하자
        public List<UnitObject_Base> targetWorldObjectList = new List<UnitObject_Base>(6);//프라이비트 예정//스킬과 연동한 특정한 위치나 오브젝트
        public List<Vector2> targetPosList = new List<Vector2>(6);

        /// <summary>
        ///스킬슬롯업데이트
        /// </summary>
        public void UpdateSkillSlot()
        {
            CoolTimerModule_Element.UpdateCoolTimer(skillCooltimer);
            CoolTimerModule_Element.UpdateCoolTimer(skillStackTimer);
            CoolTimerModule_Element.UpdateCoolTimer(skillCastTimer);
        }

        /// <summary>
        /// 스킬슬롯 초기화
        /// </summary>
        /// <param name="skillData">스킬데이터</param>
        /// <param name="_targetWorldObject">스킬주최자</param>
        public void InitSkillSlot(SkillObjectScript skillData, BattleUnitObject _targetWorldObject)
        {
            targetWorldObject = _targetWorldObject;
            this.skillData = skillData;

            //스킬데이터가 있는가?
            if (!ReferenceEquals(skillData, null))
            {
                //있으면
                //스킬슬롯을 초기세팅
                SkillManager.Instance.InitSkillSlot(this);
            }
            else
            {
                SkillManager.Instance.ResetSkillSlot(this);
            }
        }

        /// <summary>
        /// 스킬슬롯 스왑해주는 함수
        /// </summary>
        /// <param name="skillSlotA">변경할스킬슬롯A</param>
        /// <param name="skillSlotB">변경할스킬슬롯B</param>
        public static void SwapSkillSlot(SkillSlot skillSlotA, SkillSlot skillSlotB)
        {
            //바꾸고
            SkillSlot tempSkillSlot = skillSlotA;
            skillSlotA = skillSlotB;
            skillSlotB = tempSkillSlot;

            //초기화
            skillSlotA.InitSkillSlot(skillSlotA.GetSkillData(), skillSlotA.GetTargetWorldUnitObject());
            skillSlotB.InitSkillSlot(skillSlotB.GetSkillData(), skillSlotB.GetTargetWorldUnitObject());
        }

        public BattleUnitObject GetTargetWorldUnitObject()
        {
            return targetWorldObject;
        }

        public void SetStackCurNum(int _stackCurNum)
        {
            stackCurNum = _stackCurNum;
        }

        public int GetStackCurNum()
        {
            return stackCurNum;
        }

        public void SetSkillPrefab(Skill_Base _skill_Base)
        {
            targetSkillPrefab = _skill_Base;
        }

        public Skill_Base GetSkillPrefab()
        {
            return targetSkillPrefab;
        }

        public SkillObjectScript GetSkillData()
        {
            return skillData;
        }
    }
}
