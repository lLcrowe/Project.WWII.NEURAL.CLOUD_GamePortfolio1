using lLCroweTool.DataBase;
using lLCroweTool.UI.InfinityScroll;
using TMPro;

namespace lLCroweTool.SearchMap
{
    public class SearchMapCellButton : CellUI_Base
    {
        private SearchMapCellData searchCellData;
        public TextMeshProUGUI mapNameText;

        protected override void Awake()
        {
            base.Awake();
            button.onClick.AddListener(()=>ActionButtonEvent(index));
        }

        protected override void ActionButtonEvent(int index)
        {
            base.ActionButtonEvent(index);
            searchCellData.seachMapAction.Invoke();
        }

        public override void SetData<T>(T cellData)
        {
            SearchMapCellData data = cellData as SearchMapCellData;
            searchCellData = data;
            mapNameText.text = searchCellData.searchMapInfo.labelNameOrTitle;
        }
    }
}