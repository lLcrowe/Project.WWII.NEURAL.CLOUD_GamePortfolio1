using System.Collections;
using UnityEngine;

namespace lLCroweTool.DataBase
{
    [CreateAssetMenu(fileName = "New StoreInfo", menuName = "lLcroweTool/New StoreInfo")]
    public class StoreInfo : IconLabelBase
    {




        public override LabelBaseDataType GetLabelDataType()
        {
            return LabelBaseDataType.Nothing;
        }
    }
}