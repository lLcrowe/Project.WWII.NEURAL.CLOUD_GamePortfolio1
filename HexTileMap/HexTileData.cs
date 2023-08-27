using lLCroweTool.AstarPath;
using UnityEngine;

namespace lLCroweTool.TileMap.HexTileMap
{
    [System.Serializable]
    public class HexTileData : IAstarNode
    {
        //헥스타일정보 데이터

        //근처 헥스구역정보
        //헥스타일에서 이동가능한구역인지
        //가스가 존재하거나 불타는구역인지
        //체크하는 기능을 가짐

        public bool isGasArea;//가스구역//기능 => 번짐//전장에 주기적으로 번짐
                              //public bool isBurnArea;//불타는구역인가//기능 => 번짐

        //나중엔 두개를 합치는게 맞을거 같기도하다
        //장애물이라도 결국은 오브젝트 취급이다
        [SerializeField] private bool isExistObject;//오브젝트&장애물 존재여부
        [SerializeField] private bool isBatchTile;//유닛을 배치할수 있는 타일인지//처음배치할때사용하는것
        [SerializeField] private int heightArea = 0;//높이


        //해당되는 타일위치와 타일맵
        [SerializeField] private Vector3Int targetTilePos;//현산소정보타일좌표        
        [SerializeField] private HexTileObject targetHexTileObject;//해당타일위에 무엇이있는가//헥스디텍트타일로 변경예정//타일위에 무언가가 계속표시되고 사라질텐데 그거까지 중앙처리하기 엔좀

        //근처오브젝트
        [SerializeField] private Vector3Int[] nearAreaArray;//근처영역 위치
        [SerializeField] private bool[] nearCheckAreaArray;//근처영역이 있는 여부//기본은 다 false

        public bool IsExistObject { get => isExistObject; set => isExistObject = value; }
        public int HeightArea { get => heightArea; set => heightArea = value; }
        public Vector3Int[] NearAreaArray { get => nearAreaArray; }
        public bool[] NearCheckAreaArray { get => nearCheckAreaArray; }
        public bool IsBatchTile
        {
            get => isBatchTile;
            set => isBatchTile = value;
        }
        public HexTileObject GetHexTileObject { get => targetHexTileObject; }

        /// <summary>
        /// 헥스영역 정보
        /// </summary>
        /// <param name="tilePos">타일에 대한 타일맵의 위치값</param>
        /// <param name="basementTileMap">타일맵이 존재하는 대상</param>       

        public void InitHexTileData(Vector3Int tilePos, HexTileObject hexTileObject, Custom3DHexTileMap tileMap)
        {
            targetTilePos = tilePos;
            targetHexTileObject = hexTileObject;

            bool isOdd = tilePos.y % 2 == 1;

            nearAreaArray = lLcroweUtil.GetSideHexTilePos(tilePos, isOdd);
            nearCheckAreaArray = lLcroweUtil.GetSideHexTileIsHas(nearAreaArray, tileMap);
        }

        public void ResetHexTileArea()
        {
            isGasArea = false;
            isExistObject = false;
            heightArea = 0;
        }

        /// <summary>
        /// 빈공간에 세팅된 타일위치를 가져오는 함수
        /// </summary>
        /// <returns>타일위치</returns>
        public Vector3Int GetTilePos()
        {
            return targetTilePos;
        }

        /// <summary>
        /// 타일이 존재하는 월드위치를 가져오는 함수
        /// </summary>
        /// <returns>월드위치</returns>
        public Vector3 GetWorldPos()
        {
            return targetHexTileObject.transform.position;
        }

        public bool CheckObstacleOrExistObject()
        {
            return isExistObject;
        }

        public Vector3Int GetNodePos()
        {
            return GetTilePos();
        }

        public Vector3 GetNodeWorldPos()
        {
            return GetWorldPos();
        }

        public Vector3Int[] GetNearSidePosArray()
        {
            return nearAreaArray;
        }

        public bool[] GetNearExistSidePosArray()
        {
            return nearCheckAreaArray;
        }
    }
}
