using UnityEngine;
using UnityEngine.Tilemaps;

namespace lLCroweTool.AstarPath.Test
{
    [System.Serializable]
    public class TestNode: IAstarNode
    {
        public Vector3Int targetTilePos;
        public bool isWall;
        [SerializeField] public Vector3Int[] nearAreaArray;//근처영역 위치
        [SerializeField] public bool[] nearCheckAreaArray;//근처영역이 있는 여부//기본은 다 false
        public Tilemap targetTilemap;


        public bool CheckObstacleOrExistObject()
        {
            return isWall;
        }

        public bool[] GetNearExistSidePosArray()
        {
            return nearCheckAreaArray;
        }

        public Vector3Int[] GetNearSidePosArray()
        {
            return nearAreaArray;
        }

        public Vector3Int GetNodePos()
        {
            return targetTilePos;
        }

        public Vector3 GetNodeWorldPos()
        {   
            return targetTilemap.CellToWorld(targetTilePos);
        }

    }
}