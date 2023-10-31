using System.Collections;
using UnityEngine;

namespace lLCroweTool.DataBase
{
    [CreateAssetMenu(fileName = "New StorePullInfo", menuName = "lLcroweTool/New StorePullInfo")]
    public class StorePullInfo : LabelBase
    {
        public string unitObjectID;
        public bool pullEnable;


        public override LabelBaseDataType GetLabelDataType()
        {
            return LabelBaseDataType.Nothing;
        }
    }
}