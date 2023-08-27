using UnityEngine;

namespace lLCroweTool
{
    /// <summary>
    /// 고정형데이터_아이콘라벨베이스
    /// </summary>
    // [CreateAssetMenu(fileName = "New ClassName", menuName = "lLcroweTool/New ClassName")]
    public abstract class IconLabelBase : LabelBase
    {
        public Sprite icon;//아이콘
        public string labelNameOrTitle = "";//이름//제목
        public string description = "";          //설명
    }
}