using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace lLCroweTool
{
    public class PlayerSupplyDataUI : MonoBehaviour
    {
        public Image moneyIconImage;
        public TextMeshProUGUI moneyText;
        
        public Image supplyIconImage;
        public TextMeshProUGUI supplyText;


        public void UpdateUI()
        {
            var playerData = DataBaseManager.Instance.playerData;
            moneyText.text = playerData.money.ToString();
            supplyText.text = playerData.supply.ToString();
        }
    }
}
