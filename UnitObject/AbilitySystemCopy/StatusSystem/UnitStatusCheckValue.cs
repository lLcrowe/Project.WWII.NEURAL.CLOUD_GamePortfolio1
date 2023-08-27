using lLCroweTool.Ability.Util;

namespace lLCroweTool.Ability
{
    /// <summary>
    /// 유닛스탯 값을 비교하기 위한 클래스
    /// </summary>
    [System.Serializable]
    public class UnitStatusCheckValue
    {
        public UnitStatusType unitStatusType;
        public CompareOperatorType compareOperatorType;
        public float checkValue;
        public float GetFloatValue() => checkValue;
        public int GetIntValue() => (int)(checkValue = (checkValue + 0.5f) / 1);


        public override string ToString()
        {
            return $"{unitStatusType},{AbilityUtil.GetCompareOperatorTypeToString(compareOperatorType)}, 값: {checkValue}";
        }
    }
}
