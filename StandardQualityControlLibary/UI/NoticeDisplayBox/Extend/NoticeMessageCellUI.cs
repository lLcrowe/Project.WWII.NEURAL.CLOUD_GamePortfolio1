using lLCroweTool.UI.InfinityScroll;
using TMPro;


namespace lLCroweTool.NoticeDisplay.Scroll
{
    public class NoticeMessageCellUI : CellUI_Base
    {        
        public TextMeshProUGUI messageTextObject;

        public override void SetData<T>(T cellData)
        {
            NoticeCellData noticeMessageBox = cellData as NoticeCellData;
            messageTextObject.text = noticeMessageBox.content;
        }
    }
}