using lLCroweTool.Ability.Util;

namespace lLCroweTool.Ability
{

    /// <summary>
    /// 유닛상태 값을 비교하기 위한 클래스
    /// </summary>
    [System.Serializable]
    public class UnitStateCheckValue
    {
        public UnitStateType unitStateType;//체크할 조건
        public CompareOperatorType compareOperatorType;//투타입만//그외타입은 다 false
        public bool checkValue;//체크할 값

        public override string ToString()
        {
            return $"체크할 조건:{unitStateType},{AbilityUtil.GetCompareOperatorTypeToString(compareOperatorType)}, 체크할 값: {checkValue}";
        }
    }
}
