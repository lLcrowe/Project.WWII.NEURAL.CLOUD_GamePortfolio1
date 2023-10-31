using lLCroweTool.Dictionary;
using lLCroweTool.GamePlayRuleSystem;
using System.Collections.Generic;

namespace lLCroweTool.Ability.Util
{
    public static class AbilityUtil
    {
        /// <summary>
        /// 비교한거 체크 함수
        /// </summary>
        /// <param name="targetValue">비교할대상</param>
        /// <param name="value">비교값</param>
        /// <returns>비교타입에 따른 성공여부</returns>
        public static bool CheckCompare(CompareOperatorType cot, int targetValue, int value)
        {
            switch (cot)
            {
                case CompareOperatorType.Greater:
                    return targetValue > value;
                case CompareOperatorType.Less:
                    return targetValue < value;
                case CompareOperatorType.Equal:
                    return targetValue == value;
                case CompareOperatorType.NotEqual:
                    return targetValue != value;
            }

            return false;
        }

        /// <summary>
        /// 비교한거 체크 함수
        /// </summary>
        /// <param name="targetValue">비교할대상</param>
        /// <param name="value">비교값</param>
        /// <returns>비교타입에 따른 성공여부</returns>
        public static bool CheckCompare(CompareOperatorType cot, float targetValue, float value)
        {
            switch (cot)
            {
                case CompareOperatorType.Greater:
                    return targetValue > value;
                case CompareOperatorType.Less:
                    return targetValue < value;
                case CompareOperatorType.Equal:
                    return targetValue == value;
                case CompareOperatorType.NotEqual:
                    return targetValue != value;
            }

            return false;
        }

        public static List<UnitStatusType> GetAllUnitStatusType()
        {
            //나중에 실행되면 미리캐싱하는것도 한가지 방법
            //정의된 이넘타입들을 모두가져오기
            var list = lLcroweUtil.GetEnumDefineData<UnitStatusType>();
            return list;
        }

        /// <summary>
        /// 비교오퍼레이터타입을 특정 string으로 알려주는 함수
        /// </summary>
        /// <param name="compareOperatorType">비교 오퍼레이터타입</param>
        /// <returns>customstring</returns>
        public static string GetCompareOperatorTypeToString(CompareOperatorType compareOperatorType)
        {
            switch (compareOperatorType)
            {
                case CompareOperatorType.Greater:
                    return "크다";
                case CompareOperatorType.GreaterOrEqual:
                    return "크거나 같다";
                case CompareOperatorType.Less:
                    return "작다";
                case CompareOperatorType.LessOrEqual:
                    return "작거나 같다";
                case CompareOperatorType.Equal:
                    return "같다";
                case CompareOperatorType.NotEqual:
                    return "같다";
                default:
                    return "잘못된";
            }
        }






        /// <summary>
        /// 비교한거 체크 함수
        /// </summary>
        /// <param name="targetValue">비교할대상</param>
        /// <param name="value">비교값</param>
        /// <returns>비교타입에 따른 성공여부</returns>
        public static bool CheckIntCompare(CompareOperatorType cot, int targetValue, int value)
        {
            switch (cot)
            {
                case CompareOperatorType.Greater:
                    return targetValue > value;
                case CompareOperatorType.GreaterOrEqual:
                    return targetValue >= value;
                case CompareOperatorType.Less:
                    return targetValue < value;
                case CompareOperatorType.LessOrEqual:
                    return targetValue <= value;
                case CompareOperatorType.Equal:
                    return targetValue == value;
                case CompareOperatorType.NotEqual:
                    return targetValue != value;
            }

            return false;
        }

        /// <summary>
        /// 주대상을 비교값과 비교하여 체크하는 함수
        /// </summary>
        /// <param name="targetValue">비교할 주대상</param>
        /// <param name="value">비교값</param>
        /// <returns>비교타입에 따른 성공여부</returns>
        public static bool CheckFloatCompare(CompareOperatorType cot, float targetValue, float value)
        {
            switch (cot)
            {
                case CompareOperatorType.Greater:
                    return targetValue > value;
                case CompareOperatorType.GreaterOrEqual:
                    return targetValue >= value;
                case CompareOperatorType.Less:
                    return targetValue < value;
                case CompareOperatorType.LessOrEqual:
                    return targetValue <= value;
                case CompareOperatorType.Equal:
                    return targetValue == value;
                case CompareOperatorType.NotEqual:
                    return targetValue != value;
            }

            return false;
        }

        /// <summary>
        /// 비교한거 체크 함수
        /// </summary>
        /// <param name="targetValue">비교할대상</param>
        /// <param name="value">비교값</param>
        /// <returns>비교타입에 따른 성공여부</returns>
        public static bool CheckBoolCompare(CompareOperatorType cot, bool targetValue, bool value)
        {
            switch (cot)
            {
                case CompareOperatorType.Greater:
                case CompareOperatorType.Less:
                case CompareOperatorType.GreaterOrEqual:
                case CompareOperatorType.LessOrEqual:
                    return false;

                case CompareOperatorType.Equal:
                    return targetValue == value;
                case CompareOperatorType.NotEqual:
                    return targetValue != value;
            }
            return false;
        }
        //이 함수는 맨 하단에 놓아서 같이볼수 있게 하기
        /// <summary>
        /// 어떤 스탯타입을 가졋는지에 따라 int,float형태를 돌려주는 함수
        /// </summary>
        /// <returns>IntOrFloat형</returns>
        public static UnitStatusValueCATType GetUnitStatusValueCATType(UnitStatusType unitStatusType)
        {
            //직접적으로 어떤 스탯을 쓰는지 코드로 지정해서 돌려주기
            //
            UnitStatusValueCATType unitStatusValueCAT = UnitStatusValueCATType.Int;

            switch (unitStatusType)
            {
                //INT
                case UnitStatusType.HealthPoint:
                case UnitStatusType.HealthRatePoint:
                case UnitStatusType.HealthMaxPoint:
                case UnitStatusType.Damage:
                case UnitStatusType.SkillDamage:
                case UnitStatusType.DamageArmor:
                case UnitStatusType.SkillArmor:
                case UnitStatusType.CriticalChance:
                case UnitStatusType.DamageArmorPenetration:
                case UnitStatusType.SkillArmorPenetration:
                case UnitStatusType.DodgeChanceRatio:
                case UnitStatusType.CombatEndHill:
                    break;

                //FLOAT
                case UnitStatusType.AttackSpeed:
                case UnitStatusType.AttackDistance:
                case UnitStatusType.MoveSpeed:
                case UnitStatusType.CriticalScaling:
                case UnitStatusType.SkillChargeSpeed:
                case UnitStatusType.EffectResistance:
                case UnitStatusType.IncreaseGiveDamageRatio:
                case UnitStatusType.DecreaseTakenDamageRatio:
                case UnitStatusType.IncreaseHillPowerRatio:
                    unitStatusValueCAT = UnitStatusValueCATType.Float;
                    break;
            }
            return unitStatusValueCAT;
        }

        public static class UnitStateTypeMatrix
        {
            
        }

        public static bool CheckUnitStateActionType(UnitStateType unitStateType)
        {
            //특정상태에 따른 해당 상태가 작동될 상태인지 체크하는 함수
            //피직스 매트릭스 같은 종류


            return false;
        }
    }
}

namespace lLCroweTool.Ability
{
    //작업일짜//2023.05.23 ~ 2023.06.08 (17일 소모)
    //20230608
    //스탯이 추가될때마다 AbilityUtil.GetUnitStatusValueCATType에서 IntFloat형을 지정해주기
    //이제 번거롭게 추가될때마다 여러클래스에 써주는걸 방지할수 있다.
    //능력시스템을 만들기 위해서는 3가지가 필요함. 버프, 스탯, 스킬 시스템들이 필요
    //스탯기능(완), 상태기능(완), 버프기능(완), 스킬(밑단구조만 제작), 능력(밑단구조만 제작)//시간이 없음    

    //후에 IntFloat형을 CSV에서 지정하여 자동처리예정

    /// <summary>
    /// 유닛스탯 타입을 정의하는 이넘
    /// </summary>
    public enum UnitStatusType
    {
        HealthMaxPoint,//최대체력
        HealthPoint,//체력
        HealthRatePoint,//체력재생율

        Damage,//대미지
        SkillDamage,//스킬대미지

        AttackSpeed,//공격스피드
        AttackDistance,//공격거리

        MoveSpeed,//움직이는 스피드

        DamageArmor,//아머
        SkillArmor,//스킬아머

        DamageType,//대미지타입  
        ProjectileSpeed,//투사체움직이는 속도//근접이면 0
        AttackRange,//공격범위

        CriticalChance,//크리티컬찬스
        CriticalScaling,//크리티컬 배율

        DamageArmorPenetration,//아머관통
        SkillArmorPenetration,//스킬아머관통
        DodgeChanceRatio,//회피율
        CombatEndHill,//전투후 힐
        SkillChargeSpeed,//스킬충전 스피드

        EffectResistance,//효과제어

        IncreaseGiveDamageRatio,//주는 피해량증폭
        DecreaseTakenDamageRatio,//받는 피해차감 비율
        IncreaseHillPowerRatio,//치료효과
    }

    /// <summary>
    /// 유닛스탯타입에 대한 값타입(int, float)을 정의하는 이넘
    /// </summary>
    public enum UnitStatusValueCATType { Int, Float }

    /// <summary>
    /// 유닛 상태타입을 정의하는 이넘//디폴트값을 false로 잡을수 있게 정의해줘야함
    /// </summary>
    public enum UnitStateType
    {
        Stun,           //스턴//무력화//기절
        Stealth,        //은신
        Facination,     //매료
        SuperArmor,     //슈퍼아머
        StopMoving,     //이동불가
        Silence,        //침묵
        UnDamage,       //무적
    }

    /// <summary>
    /// 이넘정의 카테고리들을 정의하는 이넘
    /// </summary>
    public enum EnumDefineCATType
    {
        DamageType,

    }

    //게임을 새롭게 만들때마다 달라지는 스탯상태때문에 바꿔야함

    //변화가 심한건 이걸로 하는게
    //이넘형식으로 제작?
    //1. 확장성은 끝내줄거 같은데
    //2. 가져올때 생각보다 많은 방식이 받추어짐=>필요할때 해당 스탯을 가져오거나 만드는 방식 => 기존문제점 해결
    //3. 클래스 형식으로 하는것보다는 딕셔너리로 제작하는게 제일좋아보임 => 그럴시 더많은연산
    //4. 필요한게 있는지 체크하고 작동시키기
    //5. 수동으로 추가


    //아니면 원래되로 하는 int 방식
    //1. 빠르다(어차피 둘다 값형인데 흠)
    //2. 해당 스탯을 직접적으로 추가할때 마다 스탯에 관련된기능을 다 추가해야하거나 빼야됨
    //3. 필요하든 말든 추가해야됨
    //4. 

    //순서 다시 체크했고 분할됫음

    [System.Serializable]
    public class UnitStatusBible : CustomDictionary<UnitStatusType, UnitStatusValue> { }
    [System.Serializable]
    public class UnitStateBible : CustomDictionary<UnitStateType, bool> { }
    [System.Serializable]
    public class EnumDefineCATBible : CustomDictionary<EnumDefineCATType, EnumDefineCATValue> { }

    [System.Serializable]
    public class UnitStatusCheckBible : CustomDictionary<UnitStatusType, UnitStatusCheckValue> { }
    [System.Serializable]
    public class UnitStateCheckBible : CustomDictionary<UnitStateType, UnitStateCheckValue> { }


    //필요할거 같긴한데 아직은 아닌듯함
    public class UnitFilter
    {
        public TeamType teamType;
        public int unitCount;
        public List<UnitObject_Base> unitObjectList = new List<UnitObject_Base>();
    }



    /// <summary>
    /// 능력이 상호작용할때의 상태//외부에서 상호작용하는//데이터분리할때는 이게 맞아보이는데
    /// </summary>
    public enum PassiveAbilityInterectType
    {
        Attack, //공격상호작용
        Hit,    //히트상호작용        
        Move,   //이동시

        Interect,//상호작용시//아이템이먹거나, e등등
        BattleStart,//전투시작시
        Dodge,//닷지

        //필요할지 체크
        //Skill,  //스킬사용시
        //SkillCast,//스킬캐스트시
        //SkillCastCancel,//스킬캐스트취소시
    }

    /// <summary>
    /// 능력작동방식에 대한 타입
    /// </summary>
    public enum AbilityActionType
    {
        Active,//사용자가 직접적으로 건드려서 작동시키는거고//패시브 포함가능함
        Passive,//장비착용//자동으로 작동//업데이트로 돌려야됨//따로빼기//애니메이션이 필요한가?
    }
    
    /// <summary>
    /// 능력이 작동될때 타겟팅되는 방식
    /// </summary>
    public enum AbilityTargetType
    {
        NonTarget,
        Target,
    }

    /// <summary>
    /// 트리거 사용시 비교연산자 타입
    /// </summary>
    public enum CompareOperatorType
    {
        /// <summary>
        /// 크다
        /// </summary>
        Greater,
        /// <summary>
        /// 크거나같다
        /// </summary>
        GreaterOrEqual,
        /// <summary>
        /// 작다
        /// </summary>
        Less,
        /// <summary>
        /// 작거나같다
        /// </summary>
        LessOrEqual,
        /// <summary>
        /// 같다
        /// </summary>
        Equal,

        /// <summary>
        /// 다르다
        /// </summary>
        NotEqual,
    }
}
