using System.Collections;
using UnityEngine;
using lLCroweTool.Dictionary;
using lLCroweTool.Singleton;
using lLCroweTool.DataBase;

namespace lLCroweTool
{
    public class PlayerInventory : MonoBehaviourSingleton<PlayerInventory>
    {
        //고정형식임
        [System.Serializable]
        public class PlayerInventoryBible : CustomDictionary<string, ItemInfo> {}



        //플레이어 인벤토리바이블
        public PlayerInventoryBible playerInventorySlot = new PlayerInventoryBible();





        private void Start()
        {
            //기초자금
            //playerInventorySlot.Add("Money", 10000);
            //playerInventorySlot.Add("Supply", 1000);


        }


        public void Add()
        {
            ItemInfo itemData = new ItemInfo();
            
        }

        public void Remove()
        {

        }





    }
}