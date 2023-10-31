using lLCroweTool.Achievement;
using lLCroweTool.DataBase;
using lLCroweTool.Dictionary;
using lLCroweTool.Singleton;
using System.Collections.Generic;
using UnityEngine;
using lLCroweTool.LogSystem;
using lLCroweTool.UI.UIThema;

namespace lLCroweTool
{
    public class DataBaseManager : MonoBehaviourSingleton<DataBaseManager>
    {
        //온갖게임에 들어갈 데이터를 다 집어넣는 구역
        //오브젝트매니저와는 다른 종류의 기능
        //여기는 데이터만
        //오브젝트폴은 소환물만
        //행위에 대한 데이터는 다 집어넣음
        //특정아이템들을 연동하기 위한 구역

        //여기뭔가 이상함
        //업적구역과 어덯게 나눌지 체크해야됨
        //가변데이터는 스크립터블로 만들어지지 말아야됨
        //여기자체를 게임데이터매니저를 하는게

        //여기는 말단(고정, 스크립터블)데이터만 넣는 구역

        //모든 불변데이터는 스크립터블로 처리하기
        //그럼 이게 필요한가?


        //여기를 스크립터블 오브젝트로 도배
        public DataBaseInfo dataBaseInfo;


        //아이템정보들을 넣음
        [System.Serializable]
        public class ItemDataBaseBible : CustomDictionary<string, ItemInfo> { }
        public ItemDataBaseBible itemDataBaseBible = new ItemDataBaseBible();

        //이미지들을 넣을 
        [System.Serializable]
        public class ImageDataBaseBible : CustomDictionary<string, Sprite> { }
        public ImageDataBaseBible imageDataBaseBible = new ImageDataBaseBible();

     

        //전장맵데이터
        [System.Serializable]
        public class SearchMapInfoBaseBible : CustomDictionary<string, SearchMapCellData> {}   
        public SearchMapInfoBaseBible searchMapDataBaseBible = new SearchMapInfoBaseBible();

        //전장맵보상데이터들을 넣음
        [System.Serializable]
        public class RewardDataBaseBible : CustomDictionary<string, RewardInfo> { }
        public RewardDataBaseBible searchMapRewardDataBaseBible = new RewardDataBaseBible();

        //뽑기관련
        public List<UnitObjectInfo> unitObjectInfoList = new List<UnitObjectInfo>();

        //플레이어데이터
        public PlayerData playerData = new PlayerData();

        /// <summary>
        /// 플레이어데이터//이거 들고 있어야됨
        /// </summary>
        [System.Serializable]
        public class PlayerData
        {
            public string nickName;

            public int money;
            public int supply;

            //유닛도 처리해야되는데
            public List<UnitObjectInfo> unitDataInfoList = new List<UnitObjectInfo>();

            //public PlayerData()
            //{
            //    ISaveTarget.JoinSaveTarget(this);
            //}
        }

        protected override void Awake()
        {
            base.Awake();
            if (CheckOverLapSingleTon())
            {
                return;
            }

            LogManager.Register("DataImport", "DataBaseManager", false, false);

            //데이터베이스에서 가져와서 다 박아버리기
            if (dataBaseInfo == null)
            {
                LogManager.Log("DataImport","데이터베이스 정보가 비어있습니다.");
                return;
            }

            //데이터베이스
            itemDataBaseBible.AddBibleForInfoList(dataBaseInfo.itemInfoList);
            searchMapRewardDataBaseBible.AddBibleForInfoList(dataBaseInfo.searchMapRewardInfoList);
            searchMapDataBaseBible.AddBibleForInfoData<SearchMapInfoBaseBible, SearchMapInfo, SearchMapCellData>(dataBaseInfo.searchMapInfoList, ImportSearchMapCellData);

            //업적 임포트//위치를 업적매니저에서 작동시키기로 바꿈. 나중에 통합될 예정
            //AchievementManager.Instance.InitAchievementManager(dataBaseInfo);

            //UI테마 임포트//위치를 테마매니저에서 작동시키기로 바꿈. 나중에 통합될 예정
            //ThemaUIManager.Instance.InitThemaUIManager(dataBaseInfo);

            //뽑기 임포트//수동처리
            foreach (var item in dataBaseInfo.storePullInfoList)
            {
                //뽑기가능한 유닛만
                if (!item.pullEnable)
                {
                    continue;
                }   
                
                //아이디가 같으면 처리
                foreach (var tempItem in dataBaseInfo.unitObjectInfoList)
                {
                    
                    if (tempItem.labelID != item.unitObjectID)
                    {
                        continue;
                    }
                    unitObjectInfoList.Add(tempItem);
                }
            }

            //플레이어데이터를 임포트

            //가져오기//잘깨짐 대기
            //playerData.nickName = PlayerPrefs.GetString("PlayerData1");
            //playerData.money = PlayerPrefs.GetInt("PlayerData2");
            //playerData.supply = PlayerPrefs.GetInt("PlayerData3");



        }

        private SearchMapCellData ImportSearchMapCellData(SearchMapCellData searchMapData, SearchMapInfo searchMapInfo)
        {
            searchMapData.searchMapInfo = searchMapInfo;
            searchMapData.count = 0;
            searchMapData.defeatCount = 0;
            searchMapData.victoryCount = 0;
            return searchMapData;
        }

        public ItemInfo RequestItemData(string id)
        {
            itemDataBaseBible.TryGetValue(id, out ItemInfo itemData);
            return itemData;
        }
        
        public Sprite RequestSprite(string id)
        {
            imageDataBaseBible.TryGetValue(id, out Sprite sprite);
            return sprite;
        }

        public RewardInfo RequestAchievementRewardData(string id)
        {
            AchievementManager.Instance.achievementRewardDataBaseBible.TryGetValue(id, out RewardInfo rewardData);            
            return rewardData;
        }

        public RewardInfo RequestSearchMapRewardData(string id)
        {
            searchMapRewardDataBaseBible.TryGetValue(id, out RewardInfo rewardData);
            return rewardData;
        }

        //보상주는 함수
        public void GiveReward(RewardInfo rewardData)
        {
            //이름과 데이터 비교
            for (int i = 0; i < rewardData.itemNameArray.Length; i++)
            {
                string itemName = rewardData.itemNameArray[i];

                if (!itemDataBaseBible.ContainsKey(itemName))
                {
                    continue;
                }

                int value = rewardData.amountArray[i];
                switch (itemName)
                {
                    case "Money":
                        playerData.money += value;
                        AchievementManager.Instance.UpdateRecordData("MoneyCollect", value);
                        break;
                    case "Supply":
                        playerData.supply += value;
                        //AchievementManager.Instance.UpdateRecordData(recordID_Supply, value);
                        break;
                    case "Shell":

                        break;
                }
            }
        }

        private void OnApplicationQuit()
        {
            //쓰기
            PlayerPrefs.SetString("PlayerData1", playerData.nickName);
            PlayerPrefs.SetInt("PlayerData2", playerData.money);
            PlayerPrefs.SetInt("PlayerData3", playerData.supply);
        }

        public UnitObjectInfo GetRandomPullUnitInfo()
        {
            int index = Random.Range(0, unitObjectInfoList.Count);
            return unitObjectInfoList[index];
        }
    }
}