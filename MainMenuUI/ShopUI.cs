using lLCroweTool.Achievement;
using lLCroweTool.Cinemachine;
using lLCroweTool.DataBase;
using lLCroweTool.TimerSystem;
using lLCroweTool.UI.Confirm;
using lLCroweTool.UI.PullCard;
using UnityEngine;
using UnityEngine.UI;

namespace lLCroweTool.UI.MainMenu
{
    public class ShopUI : NotMainmenu
    {
        //상점을 보여주는 UI
        public GameObject tabRootObject;



        //뽑기 시스템
        //뽑기운..
        //옛날북마크한거 가져오기



        //뽑기관련

        //일단만들자
        public Button pullButton;


        public int needPullMoney = 1000;//필요한 뽑기머니
               
        
        private PlayerSupplyDataUI playerSupplyDataUI;
        [RecordIDTagAttribute] public string useMoneyAchievement;

        public CustomCinemachine pullCinemachine;
        public Button skipButton;


        //카드보여주는거 관련처리
        //UI구조 잘못만든거 같은데 일단 그냥 넘기자
        public PullCardBoardUI pullCardBoardUI;


        protected override void Awake()
        {
            base.Awake();            

            //등록
            pullButton.onClick.AddListener(() =>
            {
                if (!CheckPullForResource(DataBaseManager.Instance.playerData.money))
                {
                    //돈모아오세용 UI
                    GlobalConfirmWindow.Instance.SetConfirmWindow("돈이 부족합니다", null, "돈을 더모으세요!");
                    return;
                }

                //뽑으시겠습니까?//만들까? 시간없는데




                //돈이 충분하면 뽑기 ㄱㄱ
                DataBaseManager.Instance.playerData.money -= needPullMoney;
                playerSupplyDataUI.UpdateUI();

                //작동
                pullCinemachine.ActionCamera();                
            });

            skipButton.onClick.AddListener(() =>
            {
                pullCinemachine.Skip();
            });

            pullCinemachine.startEvent.AddListener(() =>
            {
                this.SetActive(false);
                skipButton.SetActive(true);
            });


            pullCinemachine.endEvent.AddListener(() =>
            {
                this.SetActive(true);
                pullCinemachine.SetActive(false);
                skipButton.SetActive(false);
                PullUnitCard();
                
            });

            pullCardBoardUI.SetActive(false);
        }

        public void SetPlayerSupplyDataUI(PlayerSupplyDataUI value)
        {
            playerSupplyDataUI = value;
        }

        public bool CheckPullForResource(int target)
        {
            return needPullMoney <= target;
        }

        public void PullUnitCard()
        {
            //시네머신작동
            //어떤형태가 좋을까

            //1. 공수부대 떨구듯이. 하늘에서 보급이!

            //2. 차마시거나 하늘구경중인데 땅이 울려서 주변 두리번하다가 저멀리 먼지가 오는거보고 오오오옷 이러는거

            //3. 도구로 상자 까기?


            

            //카드보여주기
            var targetUnit = DataBaseManager.Instance.GetRandomPullUnitInfo();
            //데이터에 카드 꽂아넣기
            print($"{targetUnit}가 나왔습니다.");
            DataBaseManager.Instance.playerData.unitDataInfoList.Add(targetUnit);


            pullCardBoardUI.Show(targetUnit, () =>
            {
                //업적갱신
                AchievementManager.Instance.UpdateRecordData(useMoneyAchievement, needPullMoney);
                AchievementManager.Instance.UpdateRecordData("DrawUnitCard", 1);

                switch (targetUnit.unitClassType)
                {
                    case UnitClassType.RifleMan:
                    case UnitClassType.Officer:
                    case UnitClassType.MachineGunMan:
                        AchievementManager.Instance.UpdateRecordData("DrawInfantryman", 1);
                        break;
                    case UnitClassType.Tank:
                        AchievementManager.Instance.UpdateRecordData("DrawTankUnit", 1);
                        break;
                }
            });
        }




        public override void ShowUIView()
        {
            base.ShowUIView();
            tabRootObject.SetActive(true);
        }


    }
}