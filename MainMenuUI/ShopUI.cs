using lLCroweTool.Achievement;
using lLCroweTool.DataBase;
using lLCroweTool.UI.Confirm;
using lLCroweTool.UI.Tab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace lLCroweTool.UI.MainMenu
{
    public class ShopUI : NotMainmenu
    {
        //상점을 보여주는 UI
        public TabRootObject tabRootObject;



        //뽑기 시스템
        //뽑기운..
        //옛날북마크한거 가져오기



        //뽑기관련

        //일단만들자
        public Button pullButton;


        public int needPullMoney = 1000;//필요한 뽑기머니
               
        
        private PlayerSupplyDataUI playerSupplyDataUI;
        [RecordIDTagAttribute] public string useMoneyAchievement;


        protected override void Awake()
        {
            base.Awake();

            pullButton.onClick.AddListener(() =>
            {
                if (!CheckPullForResource(DataBaseManager.Instance.playerData.money))
                {
                    //돈모아오세용 UI
                    GlobalConfirmWindow.Instance.SetConfirmWindow("돈이 부족합니다", null, "돈을 더모으세요!");
                    return;
                }

                //돈이 충분하면 뽑기 ㄱㄱ
                DataBaseManager.Instance.playerData.money -= needPullMoney;

                playerSupplyDataUI.UpdateUI();
                PullUnitCard();
            });
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
        }




        public override void ShowUIView()
        {
            base.ShowUIView();
            tabRootObject.SetActive(true);
        }


    }
}