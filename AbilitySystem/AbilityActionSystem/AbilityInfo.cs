
using lLCroweTool.BuffSystem;

namespace lLCroweTool.Ability
{
    /// <summary>
    /// 어빌리티 정보는 여러종류들이 모여진 콜렉터이다 
    /// 어빌리티 행위데이터, 스탯,
    /// </summary>
    public class AbilityInfo : IconLabelBase
    {
        public AbilityActionType abilityActionType;//액티브 패시브 여부

        //[Header("스킬이 정상작동된후 가만히 있어야할 시간")]
        //스킬을 사용후 가만히 서있어야할 시간//무력화버프를 거는게
        //public float standingTime = 0;

        //능력 행위데이터
        //스테이트형태//메쉬망? 일단 지금은 지원하지 말자
        //public AbilityActionInfo[] abilityActionInfoArray = new AbilityActionInfo[0];

        //간단히 간단히
        //적용할 것들을 여기서 적고 
        //CollectAbilityModule에서 적용시켜주기


        public BuffInfo[] buffInfo = new BuffInfo[0];
        public AbilityActionInfo abilityActionInfo;

        public override LabelBaseDataType GetLabelDataType()
        {
            return LabelBaseDataType.Nothing;
        }
    }

}