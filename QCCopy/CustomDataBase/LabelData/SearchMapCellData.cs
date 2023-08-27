using lLCroweTool.UI.InfinityScroll;

namespace lLCroweTool.DataBase
{
    public class SearchMapCellData : CellData
    {
        //셀관련데이터처리
        public delegate void SeachMapAction();
        public SeachMapAction seachMapAction;//데이터베이스에서 행위를 집어넣음

        public SearchMapInfo searchMapInfo;
        public int count;//시도횟수?
        public int victoryCount;//승리
        public int defeatCount;//패배
    }
}