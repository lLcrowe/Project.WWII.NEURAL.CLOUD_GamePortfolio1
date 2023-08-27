using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace lLCroweTool.AstarPath.Test
{
    public class TestGrid : MonoBehaviour
    {
        public Tilemap tilemap;
        public bool isUseTextGizmo = false;
        
        public int amount = 5;

        public int wallAmount = 10;
        public List<TestNode> nodeList = new();


        public AStarPath aStarPath;
        public IAstarNodeBible astarNodeBible = new IAstarNodeBible();



        public Transform startPos;
        public Transform endPos;

        public List<Vector3Int> pathList = new();

        private void Awake()
        {
            Grid grid = GetComponentInParent<Grid>();


            for (int i = 0; i < amount; i++)
            {
                for (int j = 0; j < amount; j++)
                {
                    var temp = new TestNode();
                    nodeList.Add(temp);
                    temp.targetTilemap = tilemap;
                    temp.targetTilePos = new Vector3Int(j, i);

                    switch (grid.cellLayout)
                    {
                        case GridLayout.CellLayout.Rectangle:
                            //사각타일
                            temp.nearAreaArray = lLcroweUtil.GetSideTilePos(temp.targetTilePos);
                            temp.nearCheckAreaArray = lLcroweUtil.GetSideTileIsHas(temp.targetTilePos, tilemap);
                            break;
                        case GridLayout.CellLayout.Hexagon:
                            //육각타일//다이아그아가 먹힘
                            temp.nearAreaArray = lLcroweUtil.GetSideHexTilePos(temp.targetTilePos);
                            temp.nearCheckAreaArray = lLcroweUtil.GetSideHexTileIsHas(temp.nearAreaArray, tilemap);
                            break;
                    }

                    astarNodeBible.Add(temp.targetTilePos, temp);
                }
            }


            aStarPath = new AStarPath(astarNodeBible);

        }

        [ButtonMethod]
        public void GenerateMap()
        {
            int cashWallAmount = 0;

            for (int i = 0; i < nodeList.Count; i++)
            {
                TestNode testNode = nodeList[i];

                if (cashWallAmount > wallAmount)
                {

                    testNode.isWall = false;
                    lLcroweUtil.SetTile(testNode.targetTilePos, Color.white, tilemap);
                    
                }
                else
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        testNode.isWall = true;
                        lLcroweUtil.SetTile(testNode.targetTilePos, Color.red, tilemap);
                        cashWallAmount++;

                    }
                    else
                    {
                        testNode.isWall = false;
                        lLcroweUtil.SetTile(testNode.targetTilePos, Color.white, tilemap);

                    }

                }
            }

        }


        // Update is called once per frame
        void Update()
        {

            Vector3Int start = lLcroweUtil.GetWorldToCell(startPos.position, tilemap);
            Vector3Int end = lLcroweUtil.GetWorldToCell(endPos.position, tilemap);


            aStarPath.Search(start,end, ref pathList);




        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                return;
            }


            for (int i = 0; i < pathList.Count; i++)
            {
                Gizmos.DrawWireSphere(tilemap.CellToWorld(pathList[i]), 0.3f);
            }

            for (int i = 0; i < nodeList.Count; i++)
            {
                var data = nodeList[i];

                if (data.isWall)
                {
                    Handles.Label(tilemap.CellToWorld(data.GetNodePos()), "W");
                }

            }

         


            var list = aStarPath.GetNodeDataList();
            for (int i = 0; i < list.Count; i++)
            {
                var data = list[i];
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(tilemap.CellToWorld(data.NodePos), 0.1f);

                if (isUseTextGizmo == false)
                {
                    continue;
                }
                Handles.Label(tilemap.CellToWorld(data.NodePos), data.ToString());
            }
        }
    }
}