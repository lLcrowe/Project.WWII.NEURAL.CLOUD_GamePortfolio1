using lLCroweTool.Ability.Util;
using UnityEngine;

namespace lLCroweTool.Ability
{

    /// <summary>
    /// 유닛에서 필요한 니드를 참고하기 위한 묶음 클래스
    /// </summary>
    [System.Serializable]
    public class UnitNeedBible
    {
        [SerializeField] private UnitStatusCheckBible needUnitStatusCheckBible = new();
        [SerializeField] private UnitStateCheckBible needUnitStateCheckBible = new();

        public UnitStatusCheckBible NeedUnitStatusCheckBible { get => needUnitStatusCheckBible; }
        public UnitStateCheckBible NeedUnitStateCheckBible { get => needUnitStateCheckBible; }

        /// <summary>
        /// 코스트를 체크하고 코스트만큼 차감하는 함수
        /// </summary>
        /// <param name="unitObjectStatusBible">유닛오브젝트의 스탯사전</param>
        /// <param name="unitObjectStateBible">유닛오브젝트의 상태사전</param>
        /// <returns>작동될수 있는지 여부</returns>
        public bool CheckNeedCost(UnitStatusBible unitObjectStatusBible, UnitStateBible unitObjectStateBible)
        {
            //스탯조건 체크
            foreach (var item in needUnitStatusCheckBible)
            {
                UnitStatusCheckValue needStat = item.Value;

                if (!unitObjectStatusBible.TryGetValue(needStat.unitStatusType, out UnitStatusValue targetUnitStatus))
                {
                    //존재하지않으면 작동안시킴
                    return false;
                }


                //스탯코스트체크//성공하지않았을경우 취소
                switch (AbilityUtil.GetUnitStatusValueCATType(needStat.unitStatusType))
                {
                    case UnitStatusValueCATType.Int:
                        int targetIntValue = targetUnitStatus.GetIntValue();
                        int checkIntValue = needStat.GetIntValue();

                        if (!AbilityUtil.CheckIntCompare(needStat.compareOperatorType, targetIntValue, checkIntValue))
                        {
                            return false;
                        }
                        break;
                    case UnitStatusValueCATType.Float:
                        float targetFloatValue = targetUnitStatus.GetFloatValue();
                        float checkFloatValue = needStat.GetFloatValue();

                        if (!AbilityUtil.CheckFloatCompare(needStat.compareOperatorType, targetFloatValue, checkFloatValue))
                        {
                            return false;
                        }
                        break;
                }
            }


            //상태조건 체크
            foreach (var item in needUnitStateCheckBible)
            {
                UnitStateCheckValue needState = item.Value;

                if (!unitObjectStateBible.TryGetValue(needState.unitStateType, out bool unitStateValue))
                {
                    //존재하지않으면 작동안시킴
                    return false;
                }

                bool targetBoolValue = unitStateValue;
                bool checkBoolValue = needState.checkValue;

                //조건에 맞지 않으면 취소
                if (!AbilityUtil.CheckBoolCompare(needState.compareOperatorType, targetBoolValue, checkBoolValue))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
