using UnityEngine;

namespace lLCroweTool.DataBase
{
    [CreateAssetMenu(fileName = "New AchievementConditionInfo", menuName = "lLcroweTool/New AchievementConditionInfo")]
    /// <summary>
    /// 업적을 깨기위한 조건데이터
    /// </summary>    
    public class AchievementConditionInfo : LabelBase
    {   
        //CSV는 설명있음
        public string[] checkRecordIDArray = new string[0];//체크할 레코드 아이디
        public int[] checkValueArray = new int[0];//현값 이상에서 처리가능

        public override LabelBaseDataType GetLabelDataType()
        {
            return LabelBaseDataType.Nothing;
        }
    }
}