
using System.Collections;
using UnityEngine;

namespace lLCroweTool.SkillSystem
{
    public class Skill_Buff_MySelf : Skill_Base
    {
        //자기자신에게 사용
        //사용하는 스킬카테고리 : ActiveSkill_Myself
        [SerializeField] private bool isAddBuff = true;//버프를 추가 시킬건지 삭제할건지 체크
       // [SerializeField] private BuffInfo[] buffDataArray;//적용시킬버프들
        public override void InitSkill()
        {
            //아무행동안함
        }
        public override void ActionSkillCast()
        {
            
        }

        public override void CancelSkillCast()
        {
            
        }

        public override void ActionSkill()
        {
            //적용할 버프 추가
           
            //UnitStatusModule unitStatusModule = targetSkillSlot.GetTargetWorldUnitObject().unitStatus;
            ////적용시킬버프들을 적용
            //for (int i = 0; i < buffDataArray.Length; i++)
            //{
            //    if (isAddBuff)
            //    {
            //        BuffManager.Instance.AddBuff(unitStatusModule, buffDataArray[i], targetSkillSlot.GetTargetWorldUnitObject());
            //    }
            //    else
            //    {
            //        BuffManager.Instance.RemoveBuff(unitStatusModule, buffDataArray[i]);
            //    }
            //}
        }

        public override void ResetSkill()
        {
            //아무행동안함
        }

        public override void SetPosition(Vector2 _pos)
        {
            transform.position = _pos;
        }
    }
}