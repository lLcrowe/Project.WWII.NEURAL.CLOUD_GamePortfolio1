using lLCroweTool.UI.InfinityScroll.Scroll;
using lLCroweTool.UI.InfinityScroll;
using lLCroweTool.NoticeDisplay.Extend;
using System.Collections.Generic;

namespace lLCroweTool.NoticeDisplay.Scroll
{

    [System.Serializable]
    public class NoticeCellData : CellData
    {
        public string content;
    }

    public class NoticeDisplayUIScroll : NoticeDisplayUIExtend_Base
    {
        public bool isShowNoticeUI;
        public CustomInfinityScroll scroll;

        private void Start()
        {
            List<NoticeCellData> noticeCellDataList=null;
            scroll?.Init(noticeCellDataList);
        }

        public override bool SendContentMessage(string content)
        {
            if (scroll == null)
            {
                return true; 
            }
            NoticeCellData noticeCellData = new NoticeCellData();
            noticeCellData.content = content;

            scroll.AddData(noticeCellData);
            return isShowNoticeUI;
        }
    }
}