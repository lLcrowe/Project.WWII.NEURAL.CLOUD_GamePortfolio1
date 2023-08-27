using lLCroweTool.UI.Bar;
using UnityEngine;

namespace lLCroweTool.DataBase
{
    [CreateAssetMenu(fileName = "New AchievementInfo", menuName = "lLcroweTool/New AchievementInfo")]    
    public class AchievementInfo : IconLabelBase
    {
        //업적행위 태그타입들//레코드ID임 별거없음//기록소랑 연동됨
        public enum EAchievementActionType
        {
            MoneyCollect,
            KillEnemy,
            EnterTheBattle,
            UseSupply,
            UseMoney,
            StartGame,
            DestroyTank,
            KillIInfantryman,            
            DrawUnitCard,
            DrawTankUnit,
            DrawInfantryman,
            Victory,
            Defeat
        }

        public EAchievementActionType achievementActionType;//레코드ID//정렬용

        //설정
        public bool isAutoUnlock; //자동으로 해금되는요소//주기적으로 체크하는 부분?
        
        //프리팹들
        public UIBar_Base uiBarPrefab;
        public RewardUI rewardUIPrefab;

        public override LabelBaseDataType GetLabelDataType()
        {
            return LabelBaseDataType.AchievementData;
        }
    }
}