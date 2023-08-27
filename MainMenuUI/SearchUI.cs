using lLCroweTool.DataBase;
using lLCroweTool.UI.Confirm;
using lLCroweTool.UI.InfinityScroll.Scroll;
using lLCroweTool.UnitBatch;
using System.Collections.Generic;

namespace lLCroweTool.UI.MainMenu
{
    public class SearchUI : NotMainmenu
    {
        //탐색을 위한 UI

        //스크롤로 작전지역을 살핀다

        //20230417
        //하나로 합침ㅁ
        //배치부분과 탐색 맵 중간부분 보급필요한거 구현중에 멈춤
        //인게임부분 제작하기

        //탐색맵과 스크롤을 합쳐버려서 세션에 넘겨야겟음

        //그래야지 관리도 되고 
        //다른곳도 그래야될 느낌이 몇구간있을듯함

        public CustomInfinityScroll scroll;

        private void Start()
        {
            //임시데이터처리//외부에서 처리하는것            
            DataBaseManager dataBaseManager = DataBaseManager.Instance;

            var list = dataBaseManager.searchMapDataBaseBible;
            List<SearchMapCellData> valueList = new List<SearchMapCellData>(list.Values);

            for (int i = 0; i < valueList.Count; i++)
            {
                SearchMapCellData searchMapData = valueList[i];
                searchMapData.seachMapAction = () =>
                {
                    //선택되었을때
                    //보급이 충분하면//배치씬으로 넘어가기                    
                    if (dataBaseManager.playerData.supply < searchMapData.searchMapInfo.needSupply)
                    {
                        GlobalConfirmWindow.Instance.SetConfirmWindow("보급이 부족합니다", () => { }, "확인");
                        return;
                    }

                    //카드가 없을시 체크
                    if (dataBaseManager.playerData.unitDataInfoList.Count == 0)
                    {
                        GlobalConfirmWindow.Instance.SetConfirmWindow("카드가.. 없어요!!", () => { }, "확인");
                        return;
                    }

                    //배치 보여주기
                    //this.SetActive(false);                                        
                    OffUIView();
                    UnitBatchUIManager.Instance.ShowBatchUI( searchMapData.searchMapInfo, ()=> {
                        //뒤로가기할때의 액션
                        //this.SetActive(true);
                        ShowUIView();
                    });
                };
            }
            scroll.Init(valueList);
        }
    }
}