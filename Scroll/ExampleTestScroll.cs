using lLCroweTool.UI.InfinityScroll;
using lLCroweTool.UI.InfinityScroll.Scroll;
using System.Collections.Generic;
using UnityEngine;

namespace lLCroweTool.Test
{
    public class ExampleTestScroll : MonoBehaviour
    {
        //무한스크롤 사용법


        public int cellCapacity;

        private void Start()
        {
            if (TryGetComponent(out CustomInfinityScroll scroll))
            {
                //임시데이터처리//외부에서 처리하는것            
                List<CellData> tempCellDataList = new List<CellData>();
                for (int i = 0; i < cellCapacity; i++)
                {
                    CellData cellData = new CellData();
                    cellData.index = i;
                    tempCellDataList.Add(cellData);
                }
                scroll.Init(tempCellDataList);
            }
        }
    }
}