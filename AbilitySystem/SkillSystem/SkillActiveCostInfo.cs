using lLCroweTool.Ability.Util;

namespace lLCroweTool.Ability
{
    /// <summary>
    /// 스킬을 발동할때 체크할 사항
    /// </summary>
    public class SkillActiveCostInfo
    {
        //작동될때 필요한 니드
        public UnitNeedBible unitNeedBible = new();

        //차감되는 데이터//스탯만 해당//니드랑 동일하게할지는 사용자마음
        public UnitStatusBible decreaseUnitStatusBible = new UnitStatusBible();

        /// <summary>
        /// 니드코스트를 체크하는 함수
        /// </summary>
        /// <param name="unitObjectStatusBible">유닛오브젝트의 스탯사전</param>
        /// <param name="unitObjectStateBible">유닛오브젝트의 상태사전</param>
        /// <returns>니드코스트에 충족된 여부</returns>
        public bool CheckCost(UnitStatusBible unitObjectStatusBible, UnitStateBible unitObjectStateBible)
        {
            //니드량 체크
            return unitNeedBible.CheckNeedCost(unitObjectStatusBible, unitObjectStateBible);
        }

        /// <summary>
        /// 특정 코스틈만큼 차감하는 함수
        /// </summary>
        /// <param name="unitObjectStatusBible">유닛오브젝트의 스탯사전</param>
        public void CalCost(UnitStatusBible unitObjectStatusBible)
        {
            //차감시작//스탯만 차감됨.//상태는 노터치//상태를 변경하고 싶으면 능력작동부분
            foreach (var item in decreaseUnitStatusBible)
            {
                UnitStatusValue decreaseStatData = item.Value;

                //앞에서 미리 다 체크했으니 무조건 존재
                if (!unitObjectStatusBible.TryGetValue(decreaseStatData.GetUnitStatusType(), out UnitStatusValue targetUnitStatus))
                {
                    continue;
                }

                //차감시작
                switch (AbilityUtil.GetUnitStatusValueCATType(targetUnitStatus.unitStatusType))
                {
                    case UnitStatusValueCATType.Int:
                        targetUnitStatus.value -= decreaseStatData.GetIntValue();
                        break;
                    case UnitStatusValueCATType.Float:
                        targetUnitStatus.value -= decreaseStatData.GetFloatValue();
                        break;
                }
            }
        }

    }

}
