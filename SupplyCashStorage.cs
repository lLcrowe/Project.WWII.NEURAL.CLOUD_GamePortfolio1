using lLCroweTool.Achievement;
using lLCroweTool.Effect.VFX;
using lLCroweTool.TimerSystem;
using TMPro;
using UnityEngine;
using System;
using static lLCroweTool.QC.Security.Security;

namespace lLCroweTool
{   
    public class SupplyCashStorage : MonoBehaviour
    {
        //시간에 따른 자원을 습득하고 보관하는 클래스

        //보관중인 자원
        public int cashMoney;
        public int cashSupply;

        //최대치
        public int maxMoney;
        public int maxSupply;

        //시간(초) 지남에 따른 상승률
        public int addSecondTimeToMoney = 10;
        public int addSecondTimeToSupply = 10;

        public TextMeshProUGUI moneyTextObject;
        public TextMeshProUGUI supplyTextObject;

        //기록소아아디
        [RecordIDTagAttribute] public string moneyCollectID;
        
        //비쥬얼이팩트
        public ResourceSpreadVFX[] resourceSpreadVFXArray = new ResourceSpreadVFX[0];

        //자원 UI의 위치이동용
        public PlayerSupplyDataUI playerDataUI;
        public Transform originParent;

        //시간처리
        private TimerModule_Element updateLimtTimerModule;//1초        

        //프로그램이 시작했을때 시간
        private static DateTime startDataTime;
        private static float startDataTimeF;

        

        //처음시작했을때 시간값을 체크
        //시스템적의 시간을 값을 체크

        //클라부분은 클라에서 처리해주기
        //인터넷에서 처리하는게 아님

        //활용되는시간값
        //서버에서 판단하니 클라쪽에서 판단할 이유는 없다
        //왜이렇게 했는지 말만 들음


        private string key = "Harvest";
        private float limitButtonTime;

        [ButtonMethod]
        public void ResetDataBtn()
        {
            PlayerPrefs.SetString(key, "0");
        }



        private void Awake()
        {
            //시간제한
            updateLimtTimerModule = new TimerModule_Element(1, true);            
            startDataTime = DateTime.Now;
            startDataTimeF = (float)GetSecond(startDataTime.Ticks);           
        }

        private bool CheckCurTime()
        {
            //시간검증
            float time = Time.time;//프로그램이 켜진시간을 가져옴
            time = startDataTimeF + time;//게임이 시작했을때의 초를 가져옴 + 켜진시간
            //총지난시간이 산출됨
                        
            DateTime curDateTime = DateTime.Now;//현재시간
            //DateTime prevDateTime = GetPrevDateTime();//전시간을 가져와서

            //TimeSpan curSpan = curDateTime - prevDateTime;            
            //float spanSecond = (float)GetSecond(curSpan.Ticks);

            float spanSecond = (float)GetSecond(curDateTime.Ticks);

            //차이값을 체크//0이 나오는게 정상일것
            float result = time - spanSecond;

            //거의동일하면 상관없음


            //이상하면 로그
            if (result > 1 || result < -1)
            {
                print("현재시간이 이상합니다");
            }

            return false;
        }
     
        private void OnEnable()
        {
            //켜지면 시간을 가져와서 계산

            //시간세팅//현재, 과거
            DateTime curDateTime = GetCurDateTime();
            DateTime prevDateTime = GetPrevDateTime();
            updateLimtTimerModule.ResetTime();

            //시간에 따른 수량초기화
            TimeSpan span = curDateTime - prevDateTime;
            double second = GetSecond(span.Ticks);

            cashMoney += (int)(addSecondTimeToMoney * second);
            cashSupply += (int)(addSecondTimeToSupply * second);
            LimitCheckCashSupply();

            UpdateText();            
        }

        private void OnDisable()
        {
            //시간저장
            SetPrevDataTime(GetCurDateTime());
        }

        /// <summary>
        /// 이전 데이터타임을 가져오는 함수
        /// </summary>
        /// <returns>데이터타임</returns>
        public DateTime GetPrevDateTime()
        {   
            string temp = PlayerPrefs.GetString(key);
            temp = DESEncryption.EncryptString(temp, key, ConvertEncryptType.Decrypt);
            double value = 0;
            double.TryParse(temp, out value);
            long tempTick = (long)(value * TimeSpan.TicksPerSecond);
            return new DateTime(tempTick);
        }

        /// <summary>
        /// 이전 데이터시간 세팅함수
        /// </summary>
        /// <param name="dataTime">데이터타임</param>
        public void SetPrevDataTime(DateTime dataTime)
        {
            var seconds = GetSecond(dataTime.Ticks);
            string temp = DESEncryption.EncryptString(seconds.ToString(), key, ConvertEncryptType.Encrypt);
            PlayerPrefs.SetString(key, temp);
        }

        public DateTime GetCurDateTime()
        {
            DateTime dateTime = DateTime.Now;
            //시간 검증            
            if (CheckCurTime())
            {
                dateTime = GetPrevDateTime();
            }

            return dateTime;
        }

        /// <summary>
        /// DateTime과 TimeSpan의 Ticks을 초단위로 변경가져오는 함수
        /// </summary>
        /// <param name="ticks">틱</param>
        /// <returns>초(시간)</returns>
        private double GetSecond(double ticks)
        {
            return ticks / TimeSpan.TicksPerSecond;
        }

        private void Update()
        {
            if (!updateLimtTimerModule.CheckTimer())
            {
                return;
            }

            //추가해버리기
            cashMoney += addSecondTimeToMoney;
            cashSupply += addSecondTimeToSupply;

            LimitCheckCashSupply();
            UpdateText();            
        }

        private void LimitCheckCashSupply()
        {
            cashMoney = Mathf.Clamp(cashMoney, 0, maxMoney);
            cashSupply = Mathf.Clamp(cashSupply, 0, maxSupply);
        }

        private void UpdateText()
        {
            moneyTextObject.text = $"{cashMoney}/{maxMoney}";
            supplyTextObject.text = $"{cashSupply}/{maxSupply}";
        }


        public void HarvestSupply()
        {
            if (cashMoney == 0 || cashSupply == 0)
            {
                return;
            }

            //버튼클릭제한
            if (Time.time > limitButtonTime + 1 == false)
            {
                return;
            }

            //보급주기
            DataBaseManager instacne = DataBaseManager.Instance;
            instacne.playerData.money += cashMoney;
            instacne.playerData.supply += cashSupply;

            //업적처리
            AchievementManager.Instance.UpdateRecordData(moneyCollectID, cashMoney);

            //초기화
            cashMoney = 0;
            cashSupply = 0;
            limitButtonTime = Time.time;
            updateLimtTimerModule.ResetTime();

            //시간저장
            DateTime curDateTime = GetCurDateTime();
            SetPrevDataTime(curDateTime);

            //자원UI
            UpdateText();
            FindObjectOfType<PlayerSupplyDataUI>(true)?.UpdateUI();

            //이팩트처리
            for (int i = 0; i < resourceSpreadVFXArray.Length; i++)
            {
                resourceSpreadVFXArray[i].Action();
            }
        }
    }
}
