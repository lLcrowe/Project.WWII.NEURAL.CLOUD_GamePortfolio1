using lLCroweTool.Achievement;
using System.Collections.Generic;
using UnityEngine;

namespace lLCroweTool.DataBase
{
    [CreateAssetMenu(fileName = "New DataBaseInfo", menuName = "lLcroweTool/New DataBaseInfo")]    
    public class DataBaseInfo : ScriptableObject
    {
        //A0/Resources 폴더안에 배치할것
        //CSV텍스트파일//변경될수 있으니 그대로 둠
        public TextAsset recordActionInfoTextSheet;
        public TextAsset itemInfoTextSheet;

        public TextAsset achievementInfoTextSheet;
        public TextAsset achievementConditionInfoTextSheet;        
        public TextAsset achievementRewardTextSheet;

        public TextAsset searchMapInfoTextSheet;
        public TextAsset searchMapRewardTextSheet;

        public TextAsset MVPTextSheet;
        public TextAsset storeTextSheet;
        public TextAsset storePullSheet;
        public TextAsset unitObjectTextSheet;


        //데이터베이스//리스토로 가지고 동작되면 다른 매니저에서 딕셔너리로 가짐//인포와 연동
        public List<ItemInfo> itemInfoList = new List<ItemInfo>();
        public List<RecordActionInfo> recordActionInfoList = new List<RecordActionInfo>();

        public List<AchievementInfo> achievementInfoList = new List<AchievementInfo>();
        public List<AchievementConditionInfo> achievementConditionInfoList = new List<AchievementConditionInfo>();        
        public List<RewardInfo> achievementRewardInfoList = new List<RewardInfo>();

        public List<SearchMapInfo> searchMapInfoList = new List<SearchMapInfo>();
        public List<RewardInfo> searchMapRewardInfoList = new List<RewardInfo>();


        public List<MVPInfo> mvpInfoList = new List<MVPInfo>();
        public List<StoreInfo> storeInfoList = new List<StoreInfo>();
        public List<StorePullInfo> storePullInfoList = new List<StorePullInfo>();
        public List<UnitObjectInfo> unitObjectInfoList = new List<UnitObjectInfo>();
    }
}

