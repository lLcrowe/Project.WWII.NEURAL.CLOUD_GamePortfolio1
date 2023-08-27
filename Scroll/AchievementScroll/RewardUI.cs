using lLCroweTool.Achievement;
using lLCroweTool.DataBase;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace lLCroweTool
{
    public class RewardUI : MonoBehaviour
    {
        //보상관련된 UI
        public Image iconImageObject;
        public TextMeshProUGUI amountTextObject;
        public Image backGroundImageObject;
        public string curTargetId;

        public void InitRewardUI(string itemName, int amount)
        {   
            ItemInfo itemData = DataBaseManager.Instance.RequestItemData(itemName);
            iconImageObject.sprite = itemData.icon;
            //backGroundImageObject.sprite = DataBaseManager.Instance.RequestSprite(itemData.gradeType.ToString());
            backGroundImageObject.sprite = itemData.backGroundImage;
            amountTextObject.text = amount.ToString();

            curTargetId = itemName;
        }
    }
}