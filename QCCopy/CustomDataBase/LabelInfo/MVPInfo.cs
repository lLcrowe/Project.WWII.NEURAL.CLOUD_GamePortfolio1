using lLCroweTool.ScoreSystem;
using UnityEngine;

namespace lLCroweTool.DataBase
{
    [CreateAssetMenu(fileName = "New MVPInfo", menuName = "lLcroweTool/New MVPInfo")]
    public class MVPInfo : LabelBase
    {
        public ScoreType scoreType;
        public Color colorMVPBackGroundColor_Hex;
        public float bunusScale;
        public string scoreMVPAlias;
        public string scoreMVPDescription;

        public override LabelBaseDataType GetLabelDataType()
        {
            return LabelBaseDataType.Nothing;
        }
    }
}