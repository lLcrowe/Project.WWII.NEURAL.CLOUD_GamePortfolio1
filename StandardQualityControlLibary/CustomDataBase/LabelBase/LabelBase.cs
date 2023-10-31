using UnityEngine;

namespace lLCroweTool
{
    /// <summary>
    /// 모든 고정형 데이터_기초
    /// </summary>
    // [CreateAssetMenu(fileName = "New ClassName", menuName = "lLcroweTool/New ClassName")]
    public abstract class LabelBase : ScriptableObject
    {
        public string labelID;//ID

        /// <summary>
        /// 라벨 기초데이터 타입을 가져오는 함수
        /// </summary>
        /// <returns>라벨 기초데이터 타입</returns>
        public abstract LabelBaseDataType GetLabelDataType();
    }

    /// <summary>
    /// 형변환을 위한 라벨 기초데이터 타입
    /// </summary>
    public enum LabelBaseDataType
    {
        Nothing,//아무것도해당되지않은//데이터변환을 사용하지않은 라벨데이터
        LevelData,
        UnitData,
        TileMapData,
        TileMapBatchObjectData,
        SkillData,
        SearchMapData,
        RewardData,
        ItemData,
        AchievementData,
        SetElementData,
        CollectAbilityData,
        AbilityBaseData,
        BuffData,
        AnimData,
    }

    /// <summary>
    /// 필요데이터 그룹
    /// </summary>
    public class NeedDataGroup
    {
        public LabelBase[] needDataArray = new LabelBase[0];//특정데이터
        public int[] needAmountArray = new int[0];//수량
    }

    /// <summary>
    /// 필요데이터 스트럭쳐
    /// </summary>
    [System.Serializable]
    public struct NeedDataStruct
    {
        public LabelBase needTargetData;//특정데이터
        public int needAmount;//수량

        public NeedDataStruct(LabelBase labelBase, int amount)
        {
            needTargetData = labelBase;
            needAmount = amount;
        }

        /// <summary>
        /// 더많거나 동일하게 가지고 있는지 체크하는 함수
        /// </summary>
        /// <param name="needData">필요데이터</param>
        /// <returns>더많거나 동일한 여부</returns>
        public bool CheckMoreThanEqual(NeedDataStruct needData)
        {
            return CheckMoreThanEqual(needData.needTargetData, needAmount);
        }

        /// <summary>
        /// 더많거나 동일하게 가지고 있는지 체크하는 함수
        /// </summary>
        /// <param name="targetData">데이터</param>
        /// <param name="amount">수량</param>
        /// <returns>더많거나 동일한 여부</returns>
        public bool CheckMoreThanEqual(LabelBase targetData, int amount)
        {
            //다른데이터면
            if (needTargetData != targetData)
            {
                return false;
            }

            //수량체크
            return needAmount <= amount;
        }
    }

}