using lLCroweTool.Achievement;
using lLCroweTool.Dictionary;
using lLCroweTool.UI.UIThema;
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

        //UI테마에 맞게 CSV처리대기
        public TextAsset iconPresetTextSheet;
        public TextAsset uiThemaTextSheet;


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


        public List<IconPresetInfo> iconPresetInfoList = new List<IconPresetInfo>();
        public List<UIThemaInfo> uIThemaInfoList = new List<UIThemaInfo>();


        //Resources 관련//이거 따로빼서 가지고 있는게 맞아보임


        [System.Serializable]
        public class ResourcesBible : CustomDictionary<string, Object> { }
        public ResourcesBible resourcesSpriteBible = new();
        public ResourcesBible resourcesObjectBible = new();

        public List<string> allResourcesPathList = new();

    }

    /// <summary>
    /// Resources폴더내에서 특정 클래스타입 찾을 종류들
    /// </summary>
    public enum ResourcesSearchClassType
    {
        Sprite,
        GameObject,
    }

    //리소스관리 관련

    //나중에 처리
    public class CustomResources
    {
        public ResourcesSearchClassType resourcesSearchClassType;
        public string name;
        public int nameHash;//+그룹핑추가
        public Object targetObject;
    }
}

