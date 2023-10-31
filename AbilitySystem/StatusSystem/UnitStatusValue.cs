using lLCroweTool.Ability.Util;

namespace lLCroweTool.Ability
{
    //스크립터블만들떄 사용해야됨
    /// <summary>
    /// 유닛스탯 값을 지정, 정의하는 클래스
    /// </summary>
    [System.Serializable]
    public struct UnitStatusValue
    {
        //나노세컨드만큼 차이나는데..구지 변경시킬까
        //그냥하자 
        public UnitStatusType unitStatusType;
        public float value;

        //세트할지 체크하기

        /// <summary>
        /// 값을 정해주는 함수. intfloat류에 맞게 자동분류하면서 정함
        /// </summary>
        /// <param name="newValue"></param>
        public void SetValue(float newValue)
        {
            var check = AbilityUtil.GetUnitStatusValueCATType(unitStatusType);
            value = newValue;
            switch (check)
            {
                case UnitStatusValueCATType.Int:
                    GetIntValue();
                    break;
                case UnitStatusValueCATType.Float:
                    //아무행동안함
                    break;
            }
        }

        public float GetFloatValue() => value;
        public int GetIntValue()
        {
            int result = (int)((value + 0.5f) / 1);
            value = result;
            return result;
        }
        public UnitStatusType GetUnitStatusType() => unitStatusType;

        public override string ToString()
        {
            return $"{unitStatusType} : {value}";
        }
    }
}
