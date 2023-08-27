using lLCroweTool.Ability.Collect;
using UnityEngine;

namespace lLCroweTool.DataBase
{
    [CreateAssetMenu(fileName = "New ItemInfo", menuName = "lLcroweTool/New ItemInfo")]
    public class ItemInfo : CollectPartInfo_Base
    {  
        public EItemType itemType;
        public EGradeType gradeType;

        public Sprite backGroundImage;

        public override LabelBaseDataType GetLabelDataType()
        {
            return LabelBaseDataType.ItemData;
        }
    }

    public enum EItemType
    {
        MaterialItem,//재료
        UsedItem,//사용 아이템
        Function
    }

    public enum EGradeType
    {
        Normal,
        Rare,
        Unique,
    }
}
