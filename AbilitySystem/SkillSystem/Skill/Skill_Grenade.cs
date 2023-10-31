using lLCroweTool.DestroyManger;
using System.Collections.Generic;
using UnityEngine;

namespace lLCroweTool.SkillSystem
{
    public class Skill_Grenade : Skill_Base
    {
        //특정한위치에 던짐
        //사용하는 스킬카테고리 : ActiveSkill_Other_Pos

        //던질 위치
        private Vector2 targetPos;

        //수류탄물체
        //public Grenade fragGrenadePrefab;

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
            //현재스킬포인터의 위치를 참조할까?
            //아니면 현재 스킬을 트랜스폼과 사거리를 참조하여 작동시키게하는것이 좋은가

            //Grenade targetObject = ObjectPoolManager.Instance.RequestGrenade(fragGrenadePrefab);
            //targetObject.InitGrenade(GetTargetSkillSlot().GetTargetWorldUnitObject());
            //targetObject.ThrowGrenade(transform.position, targetPos);
            //targetObject.ActionGrenade();
            //수류탄 투척기능
            //해당 위치로 던지는 함수
            //targetPos

        }

        public override void ResetSkill()
        {

        }

        public override void SetPosition(Vector2 _pos)
        {
            targetPos = _pos;
        }
    }
}