using System.Collections.Generic;
using UnityEngine;
using lLCroweTool.TimerSystem;


namespace lLCroweTool.SkillSystem
{
    /// <summary>
    /// 스킬들을 관리하고 사용할 스킬툴바
    /// </summary>
    public class SkillToolBar : UpdateTimerModule_Base
    {
        private BattleUnitObject targetWorldObject;
        [SerializeField] public SkillSlot[] skillSlotArray = new SkillSlot[0];//등록된 스킬슬롯들//사용할 실질적인스킬들//무조건있어야됨

        private bool isUsingSkill = false;//스킬을 사용중이면 공격못하게 변경용도
        private SkillSlot usingSkillSlot;//사용중인 스킬데이터


        //private int usingSkillSlotNum = -1;//사용중일때는 0 ~ X, 비사용시 -1

        /// <summary>
        /// 스킬툴바 안의 스킬슬롯들을 초기화해주는 작업(초기작업시)
        /// </summary>
        /// <param name="_targetWorldObject"></param>
        public void InitSkillToolBar(BattleUnitObject _targetWorldObject)
        {
            targetWorldObject = _targetWorldObject;
            for (int i = 0; i < skillSlotArray.Length; i++)
            {
                SkillSlot skillSlot = skillSlotArray[i];
                //스킬슬롯이 있는가?
                if (!ReferenceEquals(skillSlot, null))
                {
                    //스킬슬롯이 있다
                    //스킬데이터가 있는가?
                    if (ReferenceEquals(skillSlot.GetSkillData(), null))
                    {
                        //없으면
                        SkillManager.Instance.ResetSkillSlot(skillSlot);
                    }
                    else
                    {
                        //있으면
                        //스킬슬롯의 스킬주최자 설정
                        skillSlot.InitSkillSlot(skillSlot.GetSkillData(), targetWorldObject);
                        //스킬슬롯을 초기세팅
                        SkillManager.Instance.InitSkillSlot(skillSlot);
                    }
                }
            }
        }

        /// <summary>
        /// 스킬툴바 리셋
        /// </summary>
        public void ResetSkillTooBar()
        {
            targetWorldObject = null;
            for (int i = 0; i < skillSlotArray.Length; i++)
            {
                //스킬슬롯이 있는가?
                if (!ReferenceEquals(skillSlotArray[i], null))
                {
                    //스킬슬롯이 있다
                    //해당스킬슬롯 리셋
                    SkillManager.Instance.ResetSkillSlot(skillSlotArray[i]);
                }
            }
        }

        /// <summary>
        /// 스킬툴바의 쿹타임들 업데이트
        /// </summary>
        public override void UpdateTimerModuleFunc()
        {
            for (int i = 0; i < skillSlotArray.Length; i++)
            {
                int index = i;
                skillSlotArray[index].UpdateSkillSlot();
            }
        }

        /// <summary>
        /// 스킬추가
        /// </summary>
        /// <param name="skillData">스킬데이터</param>
        public void AddSkillData(SkillObjectScript skillData)
        {
            List<SkillSlot> skillSlotList = new List<SkillSlot>(skillSlotArray);

            //추가//초기화
            SkillSlot skillSlot = new SkillSlot();
            skillSlotList.Add(skillSlot);
            skillSlot.InitSkillSlot(skillData, targetWorldObject);

            skillSlotArray = skillSlotList.ToArray();
        }

        /// <summary>
        /// 해당 스킬슬롯에 새로운 스킬데이터를 집어넣고 초기세팅해주는 함수
        /// </summary>
        /// <param name="index">해당되는 인덱스</param>
        /// <param name="skillData">스킬데이터</param>
        public void AddSkillData(int index, SkillObjectScript skillData)
        {
            //한계선 체크
            if (index >= skillSlotArray.Length)
            {
                return;
            }

            //해당스킬슬롯을 먼저초기화함
            SkillManager.Instance.ResetSkillSlot(skillSlotArray[index]);

            //스킬데이터가 있는가?
            if (!ReferenceEquals(skillData, null))
            {
                //있으면
                //스킬슬롯의 스킬주최자 설정
                skillSlotArray[index].InitSkillSlot(skillData, targetWorldObject);
            }
        }

        /// <summary>
        /// 해당 스킬슬롯을 리셋 시켜주는 함수
        /// </summary>
        /// <param name="index"></param>
        public void RemoveSkillData(int index)
        {
            //한계선 체크
            if (index >= skillSlotArray.Length)
            {
                return;
            }

            //존재하는지 체크
            if (skillSlotArray[index] != null)
            {
                //존재하면 해당스킬슬롯 리셋                
                SkillManager.Instance.ResetSkillSlot(skillSlotArray[index]);
            }
        }

        /// <summary>
        /// 해당 스킬슬롯을 사용하는 함수
        /// 사용처 => 플레이어 컨트롤러, AI모듈
        /// </summary>
        /// <param name="index">작동시킬번호</param>
        /// <param name="isUsePlayer">플레이어가 사용하는가</param>
        public void ActionSkillSlot(int index, bool isUsePlayer)
        {
            //한계선 체크
            if (index >= skillSlotArray.Length)
            {
                return;
            }

            //해당위치의 스킬이 있는지 체크
            if (skillSlotArray[index] != null)
            {
                SkillManager.Instance.ActionSkill(skillSlotArray[index], isUsePlayer);
            }
        }

        public void TestUseSlot(int index)
        {
            //스킬툴바 초기초기화
            InitSkillToolBar(GetComponent<BattleUnitObject>());
            //한계선 체크
            if (index >= skillSlotArray.Length)
            {
                return;
            }

            //해당위치의 스킬이 있는지 체크
            if (skillSlotArray[index] != null)
            {
                SkillManager.Instance.ActionSkill(skillSlotArray[index], true);
            }
        }

        /// <summary>
        /// 사용중인 스킬을 취소하는 함수
        /// </summary>
        public void CancelUsingSkill()
        {
            if (!isUsingSkill)
            {
                return;
            }
            SkillManager.Instance.CancelSkill(usingSkillSlot);
            SetIsUsingSkill(false, null);
        }

        public bool IsMoveCancelToUsingSkillData()
        {
            return isUsingSkill ? usingSkillSlot.GetSkillData().isMoveCancel : isUsingSkill;
        }



        public bool GetIsUsingSkill()
        {
            return isUsingSkill;
        }

        /// <summary>
        /// 스킬사용으로인한 상태를 변경시키는 함수
        /// </summary>
        /// <param name="isUsing">스킬사용여부</param>
        /// <param name="skillSlot">스킬슬롯</param>
        public void SetIsUsingSkill(bool isUsing, SkillSlot skillSlot)
        {
            isUsingSkill = isUsing;
            usingSkillSlot = isUsingSkill ? skillSlot : null;
        }
    }
}
