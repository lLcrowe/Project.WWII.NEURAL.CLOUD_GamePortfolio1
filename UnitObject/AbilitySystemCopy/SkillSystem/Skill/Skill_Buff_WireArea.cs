//using lLCroweTool.BuffSystem;
//using lLCroweTool.WorldObjectSystem;
using lLCroweTool.GamePlayRuleSystem;
using System.Collections.Generic;
using UnityEngine;

namespace lLCroweTool.SkillSystem
{
    public class Skill_Buff_WireArea : Skill_Base
    {
        //자기자신위치에서 광역으로 사용//버프가 시간초 제한이 있는걸로 쓰는걸 권장
        //사용하는 스킬카테고리는 ActiveSkill_Myself
        [SerializeField] private bool isAddBuff = true;//버프를 추가 시킬건지 삭제할건지 체크
       // [SerializeField] private BuffInfo[] buffDataArray;//적용시킬버프들
        [Space]
        [SerializeField] private bool isUseUnitTeamType;
        [SerializeField] private TeamType[] unitTeamTypes;//버프를 적용시킬 유닛의 팀
        
        [Space]
        [SerializeField] private ContactFilter2D targetLayerMask;//타겟이 될 레이어

        private List<Collider2D> collider2DList = new List<Collider2D>();

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
            UnitObject_Base worldObject = targetSkillSlot.GetTargetWorldUnitObject();

            if (Physics2D.OverlapCircle(worldObject.transform.position, skillAreaSize, targetLayerMask, collider2DList) == 0)
            {
                return;
            }

            for (int i = 0; i < collider2DList.Count; i++)
            {
                //월드오브젝트 존재 여부
                if (collider2DList[i].TryGetComponent(out worldObject))
                {
                    //3개의 항목이 맞는지?
                    //20201223
                    //나중에 매니저로 하나빼자. 각각 마다의 조건 합친것도 따라 제작
                    //두곳이상으로 사용처가 있을거같다(현재 히트매니저, 여기)
                    //이름은? 분류는 어디다?
                    //if (CheckUnitTeamType(worldObject) && CheckUnitType(worldObject) && CheckWorldObjectType(worldObject))
                    //{
                    //    //적용할 버프 추가
                    //    if (worldObject.isUseStat)
                    //    {
                    //        //적용시킬버프들을 적용
                    //        for (int j = 0; j < buffDataArray.Length; j++)
                    //        {
                    //            if (isAddBuff)
                    //            {
                    //                BuffManager.Instance.AddBuff(worldObject.unitStatus, buffDataArray[j], targetSkillSlot.GetTargetWorldUnitObject());
                    //            }
                    //            else
                    //            {
                    //                BuffManager.Instance.RemoveBuff(worldObject.unitStatus, buffDataArray[j]);
                    //            }
                    //        }
                    //    }
                    //}
                }
            }
        }

        public override void ResetSkill()
        {
            //아무행동안함
        }

        public override void SetPosition(Vector2 _pos)
        {
            transform.position = _pos;
        }

        private bool CheckUnitTeamType(UnitObject_Base _targetWorldObject)
        {
            bool isDone = false;

            if (isUseUnitTeamType)
            {
                for (int i = 0; i < unitTeamTypes.Length; i++)
                {
                    //존재하는 지 체크
                    //if (unitTeamTypes[i] == _targetWorldObject.unitTeamType)
                    //{
                    //    isDone = true;
                    //    break;
                    //}
                }
            }
            else
            {
                isDone = true;
            }

            return isDone;
        }
    }
}