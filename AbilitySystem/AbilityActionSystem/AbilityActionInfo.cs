using lLCroweTool.UI.Bar;
using UnityEngine;

namespace lLCroweTool.Ability
{
    //능력작동 행위데이터
    //기존의 스킬베이스구역을 가져옴
    public abstract class AbilityActionInfo : IconLabelBase
    {
        public AbilityActionType abilityActionType;//액티브 패시브 여부
                                                   //액티브때도 패시브를 호출할수 있으니까 가능하지않을까
        public PassiveAbilityInterectType passiveAbilityInterectType;//패시브일때 필요//상호작용

        [Header("캐스팅관련")]
        public bool isMoveCancel;//움직일시 캔슬되는 여부//=> 다른명령을 할시 캔슬//어볼트관련

        public bool showCastingBar;
        public UIBar_Base barPrefab;
        public float castingTime = 0.2f;

        [Header("능력작동관련")]
        public float actionDurationTime;//능력이 작동될때의 지속시간//start,end사이의 시간//애니메이션시간하고 동일하게?//이건 타입에 따라 다름


        public float skillRange;//스킬 사거리//사거리가 짧으면 자동적으로 자기자신한테 쓰는 용도로 바꾸기
        public float skillAreaSize = 0.2f;//스킬범위 크기//실질적인 스킬크기와 다르니 스킬포인터에서 다른방식으로 변경해보자

        //여기는 여러가지로 분할시키기
        [Header("능력 타겟팅관련")]
        public AbilityTargetType targetType;
        [SerializeField] private int targetAmount = 1;//타겟량//위치타겟, 유닛타겟,




        //이건 각각의 액션에서 처리하는건데 많이 쓰는것ㄴ 묶어버리는것도 괜찮을거 같다.


        [Header("추가적기능관련")]//애니메이션과 연동//여기는 나주에 생각하기
        public AnimationClip abilityClip;//패시브면 처리방식이 좀 바뀌어야할듯//블랜딩,

        //이팩트
        //사운드            
        public AudioClip audioClip;

        //위치를 이름으로?




        //버프로 콤보줄려면
        //클래스를 오브젝트폴링해줘야함
        //기존오브젝트폴링에서 제네릭고치고
        //사용할 특정클래스 제작




        //캐스트할시 필요
        //중단데이터
        //비교가 필요함
        public bool isUseAbort;
        public UnitNeedBible abortBible = new UnitNeedBible();


        //주기적으로 중단데이터들을 확인하는 함수
        public void CheckUpdateAbort(UnitStatusBible unitStatusBible, UnitStateBible unitStateBible)
        {
            if (!isUseAbort)
            {
                return;
            }
        }


        //스킬베이스참조
        /// <summary>
        /// 초기화 액션을 작동시키기전에 초기화해줘야될것
        /// </summary>
        public abstract void ActionInit();

        /// <summary>
        /// 
        /// </summary>
        public abstract void ActionStart();
        public abstract void ActionEnd();
        //public abstract void ActionReset();//필요할까
    }

}
