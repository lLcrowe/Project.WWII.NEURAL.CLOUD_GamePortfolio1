using System.Collections;
using UnityEngine;

namespace lLCroweTool.Achievement
{
    [CreateAssetMenu(fileName = "New RecordActionInfo", menuName = "lLcroweTool/New RecordActionInfo")]
    public class RecordActionInfo : IconLabelBase
    {
        //public string recordID;     //아이디
        //public string title;        //제목
        //public string description;  //설명        
        public override LabelBaseDataType GetLabelDataType()
        {
            return LabelBaseDataType.Nothing;
        }
    }
}