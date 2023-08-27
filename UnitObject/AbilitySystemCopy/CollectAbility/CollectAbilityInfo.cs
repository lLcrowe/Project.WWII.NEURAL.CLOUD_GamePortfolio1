using lLCroweTool.Dictionary;

namespace lLCroweTool.Ability.Collect
{

    /// <summary>
    /// 세트아이템&세트능력을 모았을시 세트능력 정보
    /// </summary>
    public class CollectAbilityInfo : IconLabelBase
    {
        //어덯게 해야할까 
        //특정능력이 몇개 모엿으면 능력추가
        //특정능력과 아이템이 몇개 모였으면 능력추가 
        //이건 기획적인 부분에서 어덯게 해서 처리할지에 대한 처리인데 

        //제네릭화시키고 덮어쓰기

        /// <summary>
        /// 콜렉트요소 부속품들//세트아이템, 세트능력등의 각각 요소들
        /// </summary>
        public CollectPartInfo_Base[] collectPartPieceArray = new CollectPartInfo_Base[0];


        //몇개를 모앗을시 특정 능력이 발동되게
        //세트아이템이 어느정도모였을때 작동될 능력
        //얼마만큼 조각을 모았을시 효과가 발동되는가
        //세트아이템착용시 능력추가
        //최대수치는 CollectAbilityInfo.collectElementPieceArray의 크기//에디터에서처리
        //<수량,추가될 세트능력>
        /// <summary>
        /// 콜렉트요소의 수량에 따른 능력 정보//콜렉트를 일정이상 모았을시 작동될 능력정보//레벨중복은 허용안하니 딕셔너리로 처리
        /// </summary>
        [System.Serializable]
        public class CollectPartAmountAbilityBible : CustomDictionary<int, AbilityInfo[]> { }

        /// <summary>
        /// 콜렉트요소의 수량에 따른 능력 정보
        /// </summary>
        public CollectPartAmountAbilityBible collectPartAmountAbilityBible = new CollectPartAmountAbilityBible();

        /// <summary>
        /// 파츠수량에 따른 능력 가져오는 함수
        /// </summary>
        /// <param name="partAmount">파츠수량</param>
        /// <param name="abilityInfoArray"></param>
        /// <returns>파츠수량에 따른 능력이 존재하는지</returns>
        public bool GetPartAmountAbility(int partAmount, ref AbilityInfo[] abilityInfoArray)
        {
            return collectPartAmountAbilityBible.TryGetValue(partAmount, out abilityInfoArray);
        }

        public override LabelBaseDataType GetLabelDataType()
        {
            return LabelBaseDataType.CollectAbilityData;
        }
    }
}
