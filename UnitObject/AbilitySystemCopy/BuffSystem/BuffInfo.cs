using lLCroweTool.Ability;
using lLCroweTool.Dictionary;
using UnityEngine;

namespace lLCroweTool.BuffSystem
{
    //20230531//능력시스템을 재설계후 재구축.

    /// <summary>
    /// 유닛의 특정스탯에 버프에 대한 세팅을 줄려고 제작된 클래스
    /// </summary>
    [System.Serializable]
    public class BuffUnitStatusValue
    {
        //버프가 적용될때 작동될 타입
        public BuffStatusValueApplyType buffStatusValueApplyType;
        public UnitStatusValue unitStatusValue;

        public override string ToString()
        {
            return $" (ApplyType:{buffStatusValueApplyType}) {unitStatusValue}";
        }
    }

    /// <summary>
    /// 버프 스탯값이 적용될때 작동할 타입
    /// </summary>
    public enum BuffStatusValueApplyType
    {
        /// <summary>
        /// 더하기//int
        /// </summary>
        Add,
        /// <summary>
        /// 곱하기//float
        /// </summary>
        Multiple,
        /// <summary>
        /// 퍼센트//int
        /// </summary>
        Percent,    
    }

    [System.Serializable]
    public class BuffUnitStateValue
    {
        public UnitStateType unitStateType;
        public bool value;
    }


    [System.Serializable]
    public class BuffUnitStatusBible : CustomDictionary<UnitStatusType, BuffUnitStatusValue> { }
    
    [System.Serializable]
    public class BuffUnitStateBible : CustomDictionary<UnitStateType, BuffUnitStateValue> { }

    /// <summary>
    /// 유닛어빌리티에 달릴 버프사전//유닛에 버프가 존재하는지 체크하는 용도 
    /// </summary>
    [System.Serializable]
    public class BuffBible : CustomDictionary<BuffInfo, BuffData> {}


    [CreateAssetMenu(fileName = "New BuffInfo", menuName = "lLcroweTool/New BuffInfo")]
    public class BuffInfo : IconLabelBase
    {
        //메뉴얼
        //※기록용버프(콤보, 스택 등등)
        //기록용버프를 만들려면 스택하고 시간버프만 건드려야함.
        //격투게임에서 콤보라는걸 구현하고 싶으면
        //시간버프와 스택을 쌓게 제작


        [Space]
        [Header("버프 카테고리")]        
        public BuffKategorie buffKategorie;

        //여기서 하는것보다는 다른곳에서 상호작용할때 하는게 맞아보임
        //[Space]
        //[Header("랜덤으로 적용될지 말지 설정")]
        ////랜덤 적용관련
        //public bool isUseRandomApply = false;
        //[Range(0, 101)] public int randomPercent = 51;

        [Space]
        [Header("버프를 받을수 있는 상태인지 체크해주는 설정")]
        //버프를 받을수 있는 상태인지 여부
        public bool isUseApplyNeedCondition;
        public UnitNeedBible applyNeedBible = new UnitNeedBible();

        [Space]
        [Header("시간버프 설정")]
        [Tooltip("시간버프를 체크할시 일정시간동안 버프가 있고 시간이 지나면 없어지는 버프")]
        public bool isUseTimeBuff;//시간버프 여부
        public float buffDurationTime;//버프의 지속시간

        [Space]
        [Header("시간버프 일시 특정 조건에 자동적으로 사라지는 설정")]
        //버프중단구역
        public bool isUseTimeBuffDisableBuffCondition;
        public UnitNeedBible disableBuffConditionBible = new UnitNeedBible();

        [Space]
        [Header("중복버프가능한지")]
        [Tooltip("해당 버프가 중복적으로 적용될수 있는 버프인지")]
        public bool isStackBuff;//버프스택여부                                
        [Min(1)] public int maxStack;//버프가 중첩될때의 최대 스택//기본적으로 곱하기로 처리//능력일경우 배열로 처리//커브로 하면 잣될듯

        [Space]
        [Header("적용할 스탯버프설정")]//기본적인스탯들을 올려주는 버프
        //유닛의 스탯설정들
        [Header("적용할 스탯관련")]
        public BuffUnitStatusBible buffStatusBible = new BuffUnitStatusBible();

        [Header("적용할 상태관련")]
        public BuffUnitStateBible buffStateBible = new BuffUnitStateBible();

        //[Header("능력행위데이터(나중에 패시브능력, 논타겟팅만 가능)")]

        //이팩트설정
        //데이터상으로만 존재
        //아직 제작하는곳에 집어넣지않음 
        //나중에 작업해줘야함
        //[Space]
        //[Header("이팩트설정")]
        //public EffectComponentType targetEffectComponent;
        //public EffectObjectScript targetEffectObjectData;




        public override LabelBaseDataType GetLabelDataType()
        {
            return LabelBaseDataType.BuffData;
        }
    }

    //버프 카테고리
    public enum BuffKategorie
    {
        AddStat,//능력치 상승
        AddEffect,//추가능력 버프 (이팩트추가..? 능력추가 : 주변유닛불타기 ,주변유닛슬로우걸기)
        Complex//복합
    }
}