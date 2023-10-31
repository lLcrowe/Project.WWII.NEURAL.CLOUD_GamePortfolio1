using System.Collections.Generic;
using UnityEngine;

namespace lLCroweTool.UI.UIThema
{
    /// <summary>
    /// UI테마에 대한 아이콘프리셋정보
    /// </summary>
    [CreateAssetMenu(fileName = "New IconListInfo", menuName = "lLcroweTool/New IconListInfo")]
    public class IconPresetInfo : LabelBase
    {
        //기존거와 다른게 있으니 좀 문제가 있긴한데//이렇게 해야 쉽게처리가능ㅎ
        public List<IconPresetData> iconDataList = new();//여러아이콘들

        public override LabelBaseDataType GetLabelDataType()
        {
            return LabelBaseDataType.Nothing;
        }
    }
    /// <summary>
    /// 아이콘데이터
    /// </summary>
    [System.Serializable]
    public class IconPresetData
    {
        public string iconName;
        public Sprite iconSprite;


        public override string ToString()
        {
            return $"{iconName}, {iconSprite}";
        }
    }
}
