using UnityEngine;

namespace lLCroweTool.Ability
{
    public class SkillInfo : IconLabelBase
    {
        [Header("스킬을 작동시키기위한 조건(조건1)")]
        public SkillActiveCostInfo SkillActiveCostData = new();


        [Header("스킬을 작동시키기위한 조건(조건2)")]
        //시작시 필요한 데이터
        [Header("쿨타임")]
        public float skillCoolTime;//스킬 재사용시간

        [Header("스택관련")]//이거 내부나 외부 둘다필요한거인데//근데 내부는 레코드로 처리하는게 있지않나
        public bool isUseStack = false;
        public int stackMaxNum = 2;
        public float stackingTimer = 1.5f; //스택시스템을 이용할시 쿨타임보다는 커야함


        [Header("작동시킬 능력들")]//패시브는 패시브//액티브는 액티브
        public AbilityActionType abilityActionType;
        public AbilityInfo abilityInfo;


        /// <summary>
        ///  코스트를 체크하고 코스트만큼 차감하는 함수
        /// </summary>
        /// <param name="unitObjectStatusBible">유닛오브젝트의 스탯사전</param>
        /// <param name="unitObjectStateBible">유닛오브젝트의 상태사전</param>
        /// <param name="abilityData">어빌리티정보에 해당되는 어빌리티데이터</param>
        /// <param name="isUseNotice">경고문자 사용여부</param>
        /// <returns>작동될수 있고 계산했는지 여부</returns>
        public bool CheckCost(UnitStatusBible unitObjectStatusBible, UnitStateBible unitObjectStateBible, SkillData abilityData, bool isUseNotice)
        {
            //니드량 체크
            if (!abilityData.CheckAbilityReady(this, isUseNotice))
            {
                return false;
            }

            //작동코스트체크
            if (!SkillActiveCostData.CheckCost(unitObjectStatusBible, unitObjectStateBible))
            {
                return false;
            }

            return true;
        }




        //이거통채로 작동부분에 넘기기
        public void ActionAbility(UnitStatusBible unitObjectStatusBible, AbilityActionData abilityData)
        {
            SkillActiveCostData.CalCost(unitObjectStatusBible);
            //abilityData.CalStack();//이건필요없업임//왜냐하면 함수 통채로 넘기는거라//스택줄이는거임//작동부분하고 합쳐질거
            //abilityInfo.ActionAbility();
        }

        public override LabelBaseDataType GetLabelDataType()
        {
            return LabelBaseDataType.SkillData;
        }
    }

}
