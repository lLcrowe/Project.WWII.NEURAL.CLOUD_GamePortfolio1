

namespace lLCroweTool.DataBase
{
    public class SearchMapInfo : IconLabelBase
    {
        public int seed;//맵의 시드번호
        //public string battleStageName;//전장이름
        //CSV에 넣어야됨//어차피 이름으로 하면 되지않나//다름 달라야됨

        //CSV 갱신해야될 목록
        public int needSupply;

        public string stageName;

        public override LabelBaseDataType GetLabelDataType()
        {
            return LabelBaseDataType.SearchMapData;
        }
    }
}