using lLCroweTool.Dictionary;
using UnityEngine;

namespace lLCroweTool.TileMap
{
    [CreateAssetMenu(fileName = "New TileMapBatchInfo", menuName = "lLcroweTool/New TileMapBatchInfo")]
    public class TileMapBatchInfo: IconLabelBase
    {
        //이름변경하기
        //TileMapInfo//TileMapBatchInfo
        //타일맵이 새로생성될때 오브젝트들 배치에 대한 내용들
        //헥스타일맵 데이터
        //에디터작업을 위한 기초작업

        /// <summary>
        /// 배치된 타일의 정보 데이터들//타일위치,
        /// </summary>
        [System.Serializable]
        public class TileMapInfoBible : CustomDictionary<Vector3Int, string> { }

        //생성된 타일위치들
        //각타일에 대한 정보들(?)        
        public TileMapInfoBible tileMapInfoBible = new TileMapInfoBible();

        public override LabelBaseDataType GetLabelDataType()
        {
            return LabelBaseDataType.TileMapData;
        }
    }
}
