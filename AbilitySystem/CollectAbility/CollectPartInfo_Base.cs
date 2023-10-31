using UnityEngine;

namespace lLCroweTool.Ability.Collect
{
    /// <summary>
    /// 콜렉트요소 체크용 클래스//상속처리해서 아이템콜렉트 능력콜렉트등으로 추가 가능하게 하는 클래스
    /// </summary>
    public abstract class CollectPartInfo_Base : IconLabelBase
    {
        [Header("타겟팅되는 세트능력정보")]
        [SerializeField] private CollectAbilityInfo targetCollectAbilityInfo;

        /// <summary>
        /// 타겟팅된 세트능력정보
        /// </summary>
        public CollectAbilityInfo TargetCollectAbilityInfo
        {
            get => targetCollectAbilityInfo;
            set
            {
                if (Application.isEditor)
                {
                    targetCollectAbilityInfo = value;
                }
            }
        }
    }
}
