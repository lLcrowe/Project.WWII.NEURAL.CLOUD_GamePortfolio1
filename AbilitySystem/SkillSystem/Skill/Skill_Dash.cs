using System.Collections;
using UnityEngine;

namespace lLCroweTool.SkillSystem
{
    public class Skill_Dash : Skill_Base
    {
        //행동 : => 유닛이 특정방향으로 대쉬(순간적인 힘으로 움직임)를 함
        //사용하는 스킬카테고리 
        //ActiveSkill_Other_Pos,//액티브스킬 다른 위치에 사용
        
        private Rigidbody2D rb2d;
        public float dashPower = 15f;
        private Vector2 targetPos;

        public override void ActionSkill()
        {
            rb2d.velocity = Vector2.zero;
            rb2d.velocity += targetPos.normalized * dashPower;
        }

        public override void ActionSkillCast()
        {
            //아무행동 안함
        }

        public override void CancelSkillCast()
        {
            //아무행동 안함
        }

        public override void InitSkill()
        {
            targetSkillSlot.GetTargetWorldUnitObject().TryGetComponent(out rb2d);
        }

        public override void ResetSkill()
        {
            targetPos = Vector2.zero;
        }

        public override void SetPosition(Vector2 _pos)
        {
            targetPos = _pos;
        }
    }
}