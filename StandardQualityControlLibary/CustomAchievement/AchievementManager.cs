using System.Collections;
using UnityEngine;
using lLCroweTool.Dictionary;
using lLCroweTool.Singleton;
using lLCroweTool.UI.InfinityScroll;
using System.Collections.Generic;
using lLCroweTool.NoticeDisplay;
using lLCroweTool.DataBase;
using static lLCroweTool.DataBaseManager;

namespace lLCroweTool.Achievement
{
    public class AchievementManager : MonoBehaviourSingleton<AchievementManager>
    {
        //어떤걸 가져가는 데이터인지?
        //자동으로 해금되는 요소인지 & 클릭해서 해금하느건지


        //참고자료

        //-=커리어
        //인증키누적 소모 100 500 1000 2000
        //검색누적 10회실행 20 50 100
        //디그코인 누적 소모 20000 40000


        //-=인형
        //5성이상 인형을 10명보유
        //총 20명 인형의 친밀도가 레벨 15이상달성
        //레벨 10 이상 인형을 5명보유
        //레벨 20 이상 인형을 5명보유
        //재능돌파 2단계 이상인 인형 3명보유
        //인형스킬 레벨 누적 10회강화

        //-=탐색
        //제1섹터 일반스테이지 전부 클리어
        //제2섹터 일반스테이지 전부 클리어

        //-=건설
        //

        //일단 데이터기록과 업적은 같은구역에 묶어놓는게 맞는거 같다        
        [System.Serializable]
        public class RecordActionBible : CustomDictionary<string, RecordActionData> { }

        [System.Serializable]
        public class AchievementConditionDataBible : CustomDictionary<string, AchievementConditionInfo> { }

        [System.Serializable]
        public class AchievementBible : CustomDictionary<string, AchievementData> { }

        //행위기록 사전//사전에서 업데이트를 해줘야되기때문에 여기서 함//뭐 다른데서 할경우 여기도 호출해야되는게 있음
        [SerializeField]
        public RecordActionBible recordActionBible = new RecordActionBible();

        

        //업적 사전
        [SerializeField]
        public AchievementBible achievementBible = new AchievementBible();

        //업적조건 사전
        [SerializeField]
        public AchievementConditionDataBible achievementConditionDataBible = new AchievementConditionDataBible();

        //업적보상 사전
        [SerializeField]
        public RewardDataBaseBible achievementRewardDataBaseBible = new RewardDataBaseBible();


        //자동으로 체크할 업적데이터들
        public AchievementData[] autoCheckAchievementDataArray = new AchievementData[0];
        private bool isRunCoroutine = false;
        private int checkQueueCount = 0;

        //알림UI
        public NoticeDisplayUI noticeDisplay;


        //노티스 그거 만들어봐야겠다
        //위로올라가고 쌓으면서 내려가는거
        //이름을 뭘로짖지

        protected override void Awake()
        {
            base.Awake();
            InitAchievementManager(DataBaseManager.Instance.dataBaseInfo);
        }

        public IEnumerator Start()
        {
            //자동으로 갱신해줄 업적 데이터들
            List<AchievementData> tempList = new List<AchievementData>();
            foreach (var data in achievementBible)
            {
                if (data.Value.achievementInfo.isAutoUnlock)
                {
                    tempList.Add(data.Value);
                }
            }
            autoCheckAchievementDataArray = tempList.ToArray();


            //업적갱신
            UpdateRecordData("StartGame", 1);
            return null;
        }

        /// <summary>
        /// 업적매니저 초기화
        /// </summary>
        /// <param name="dataBaseInfo">데이터베이스 정보</param>
        public void InitAchievementManager(DataBaseInfo dataBaseInfo)
        {
            achievementBible.AddBibleForInfoData<AchievementBible, AchievementInfo, AchievementData>(dataBaseInfo.achievementInfoList, ImportAchievementData);
            achievementConditionDataBible.AddBibleForInfoList(dataBaseInfo.achievementConditionInfoList);
            achievementRewardDataBaseBible.AddBibleForInfoList(dataBaseInfo.achievementRewardInfoList);
            recordActionBible.AddBibleForInfoData<RecordActionBible, RecordActionInfo, RecordActionData>(dataBaseInfo.recordActionInfoList, ImportRecordActionData);           
        }

        /// <summary>
        /// 업적데이터 임포트 함수
        /// </summary>
        /// <param name="achievementData">업적데이터</param>
        /// <param name="achievementInfo">업적정보</param>
        /// <returns></returns>
        private AchievementData ImportAchievementData(AchievementData achievementData, AchievementInfo achievementInfo)
        {
            achievementData.achievementInfo = achievementInfo;
            achievementData.isUnlock = false;
            return achievementData;
        }

        /// <summary>
        /// 기록데이터 임포트 함수
        /// </summary>
        /// <param name="recordActionData">기록데이터</param>
        /// <param name="recordActionInfo">기록정보</param>
        /// <returns></returns>
        private RecordActionData ImportRecordActionData(RecordActionData recordActionData, RecordActionInfo recordActionInfo)
        {   
            recordActionData.recordActionInfo = recordActionInfo;
            recordActionData.count = 0;
            return recordActionData;
        }

        /// <summary>
        /// 레코드데이터를 갱신시켜주는 함수//기록하는 함수
        /// </summary>
        /// <param name="id">레코드아이디</param>
        /// <param name="addAmount">추가할 량</param>
        public void UpdateRecordData(string id, int addAmount)
        {
            RecordActionData recordActionData = RequestRecordActionData(id);

            if (recordActionData == null)
            {
                return;
            }

            recordActionData.count += addAmount;

            //업적자동갱신
            checkQueueCount++;
            if (isRunCoroutine)
            {
                return;
            }
            StartCoroutine(UpdateAutoAchievementData());
        }

        /// <summary>
        /// 자동갱신할 업적들을 갱신해주는 함수
        /// </summary>
        private IEnumerator UpdateAutoAchievementData()
        {
            isRunCoroutine = true;
            do
            {
                for (int i = 0; i < autoCheckAchievementDataArray.Length; i++)
                {
                    AchievementData achievementData = autoCheckAchievementDataArray[i];
                    AchievementConditionInfo achievementConditionData = RequestAchievementConditionData(achievementData.achievementInfo.labelID);

                    //업적조건 체크
                    if (!CheckAchievementCondition(achievementConditionData))
                    {
                        continue;
                    }
                    //잠금풀린지 체크
                    if (achievementData.isUnlock)
                    {
                        continue;
                    }
                    //자동해금인지체크
                    if (!achievementData.achievementInfo.isAutoUnlock)
                    {
                        continue;
                    }
                    yield return null;
                    GiveAchievementDataReward(achievementData);
                }
                checkQueueCount--;
            } while (checkQueueCount > 0);
            isRunCoroutine = false;
        }

        /// <summary>
        /// 업적데이터를 수동갱신시켜주는 함수
        /// </summary>
        /// <param name="id">업적아이디</param>
        public void UpdateAchievementData(string id)
        {            
            if (!achievementBible.TryGetValue(id, out AchievementData achievementData))
            {
                return;
            }
            
            AchievementConditionInfo achievementConditionData = RequestAchievementConditionData(achievementData.achievementInfo.labelID);

            //업적조건 체크
            if (!CheckAchievementCondition(achievementConditionData))
            {
                return;
            }
            //잠금풀린지 체크
            if (achievementData.isUnlock)
            {
                return;
            }
            //잠금이 안풀렷으면
            if (!achievementData.achievementInfo.isAutoUnlock)
            {
                return;
            }

            GiveAchievementDataReward(achievementData);
        }     

        /// <summary>
        /// 업적조건 체크 함수
        /// </summary>
        /// <param name="achievementConditionData">업적조건 데이터</param>
        /// <returns>조건이 다 완수되었는지 여부</returns>
        private bool CheckAchievementCondition(AchievementConditionInfo achievementConditionData)
        {
            if (achievementConditionData == null)
            {
                return false;
            }

            for (int i = 0; i < achievementConditionData.checkRecordIDArray.Length; i++)
            {
                int index = i;
                string recordID = achievementConditionData.checkRecordIDArray[index];
                RecordActionData recordActionData = RequestRecordActionData(recordID);

                //존재하는지 체크
                if (recordActionData == null)
                {
                    continue;
                }

                //카운트량 체크//작으면 처치
                if (achievementConditionData.checkValueArray[index] > recordActionData.count)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 업적데이터에 대한 보상을 주는 함수
        /// </summary>
        /// <param name="achievementData">업적데이터</param>
        public void GiveAchievementDataReward(AchievementData achievementData)
        {
            //언락후 
            achievementData.isUnlock = true;

            //보상주기
            string content = $"{achievementData.achievementInfo.labelNameOrTitle}의 보상이 제공되었습니다";
            print(content);

            //알림처리
            noticeDisplay.AddNewMessage(content);
        }

        /// <summary>
        /// 아이디에 따른 업적데이터를 호출하는 함수
        /// </summary>
        /// <param name="id">아이디</param>
        /// <returns>업적데이터</returns>
        public AchievementData RequestAchievementData(string id)
        {    
            achievementBible.TryGetValue(id, out AchievementData achievementData);
            return achievementData;
        }

        /// <summary>
        /// 아이디에 따른 업적조건데이터를 호출하는 함수
        /// </summary>
        /// <param name="id">아이디</param>
        /// <returns>업적조건데이터</returns>
        public AchievementConditionInfo RequestAchievementConditionData(string id)
        {   
            achievementConditionDataBible.TryGetValue(id, out AchievementConditionInfo achievementConditionData);
            return achievementConditionData;
        }

        /// <summary>
        /// 아이디에 따른 기록행위데이터를 가져오는 함수
        /// </summary>
        /// <param name="id">아이디</param>
        /// <returns>기록행위데이터</returns>
        public RecordActionData RequestRecordActionData(string id)
        {
            recordActionBible.TryGetValue(id, out RecordActionData recordActionData);
            return recordActionData;
        }

        /// <summary>
        /// 아이디에 따른 기록행위데이터의 기록카운트를 가져오는 함수
        /// </summary>
        /// <param name="id">아이디</param>
        /// <returns>카운트 갯수</returns>
        public int RequestRecordActionDataCount(string id)
        {
            int count = 0;
            if (recordActionBible.TryGetValue(id, out RecordActionData recordActionData))
            {
                count = recordActionData.count;
            }
            return count;
        }
    }

    /// <summary>
    /// 행위를 기록한데이터 (명사 + 동사)
    /// </summary>
    [System.Serializable]
    public class RecordActionData
    {
        public RecordActionInfo recordActionInfo;
        public int count = 0;           //카운트//가변데이터
    }

    /// <summary>
    /// 업적데이터 + 무한스크롤을 사용하기 위한 셀데이터
    /// </summary>
    [System.Serializable]
    public class AchievementData : CellData
    {
        public AchievementInfo achievementInfo;
        public bool isUnlock = false;//해금됫는지 여부//가변데이터
    }

}