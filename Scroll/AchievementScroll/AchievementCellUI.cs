using lLCroweTool.DataBase;
using lLCroweTool.UI.Bar;
using lLCroweTool.UI.InfinityScroll;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace lLCroweTool.Achievement
{
    public class AchievementCellUI : CellUI_Base
    {
        public Image iconImageObject;
        public TextMeshProUGUI titleTextObject;
        public TextMeshProUGUI descriptionTextObject;

        public List<UIBar_Base> progressBarList = new List<UIBar_Base>();
        public Transform targetProgressBarPos;

        public List<RewardUI> rewardUIList = new List<RewardUI>();
        public Transform targetRewardUIPos;

        private AchievementData achievementData;

        protected override void Awake()
        {
            base.Awake();
            button.onClick.AddListener(() => ActionButtonEvent(index));
        }

        protected override void ActionButtonEvent(int index)
        {
            //보상얻기
            AchievementManager.Instance.GiveAchievementDataReward(achievementData);
        }

        public override void SetData<T>(T cellData)
        {
            //UI리셋
            UIReset();

            achievementData = cellData as AchievementData;

            //기본데이터 집어넣기
            iconImageObject.sprite = achievementData.achievementInfo.icon;
            titleTextObject.text = achievementData.achievementInfo.labelNameOrTitle;
            descriptionTextObject.text = achievementData.achievementInfo.description;

            AchievementConditionInfo achievementConditionData = AchievementManager.Instance.RequestAchievementConditionData(achievementData.achievementInfo.labelID);

            //업적 프로그래스처리
            for (int i = 0; i < achievementConditionData.checkRecordIDArray.Length; i++)
            {
                string recordID = achievementConditionData.checkRecordIDArray[i];
                if (string.IsNullOrEmpty(recordID))
                {
                    continue;
                }

                int recordValue = AchievementManager.Instance.RequestRecordActionDataCount(recordID);
                UIBar_Base bar = ObjectPoolManager.Instance.RequestDynamicComponentObject(achievementData.achievementInfo.uiBarPrefab);
                bar.transform.InitTrObjPrefab(targetProgressBarPos.position, quaternion.identity, targetProgressBarPos);

                RectTransform rect = bar.transform as RectTransform;
                rect.FocusRectAnchorPos(Vector2.down, 10, i);
                bar.gameObject.SetActive(true);

                bar.InitUIBar(0, achievementConditionData.checkValueArray[i], recordValue);
                progressBarList.Add(bar);
            }

            RewardInfo rewardData = DataBaseManager.Instance.RequestAchievementRewardData(achievementData.achievementInfo.labelID);

            //보상UI처리
            for (int i = 0; i < rewardData.itemNameArray.Length; i++)
            {
                if (rewardData.amountArray[i] == 0)
                {
                    continue;
                }
                RewardUI rewardUI = ObjectPoolManager.Instance.RequestDynamicComponentObject(achievementData.achievementInfo.rewardUIPrefab);
                rewardUI.transform.InitTrObjPrefab(targetRewardUIPos.position, Quaternion.identity, targetRewardUIPos);
                rewardUI.gameObject.SetActive(true);
                rewardUI.InitRewardUI(rewardData.itemNameArray[i], rewardData.amountArray[i]);
                rewardUIList.Add(rewardUI);
            }
        }

        public void UIReset()
        {
            for (int i = 0; i < progressBarList.Count; i++)
            {
                progressBarList[i].gameObject.SetActive(false);
            }
            progressBarList.Clear();

            for (int i = 0; i < rewardUIList.Count; i++)
            {
                rewardUIList[i].gameObject.SetActive(false);
            }
            rewardUIList.Clear();
        }
    }
}