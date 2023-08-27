using Assets;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.TextCore.Text;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;
using lLCroweTool.TimerSystem;
using lLCroweTool;
using static lLCroweTool.lLcroweUtil.HexTileMatrix;

public class HexTest : MonoBehaviour
{
    //움직임과 맵배치


    //카메라클릭위치를 체크해야됨//애매한데 어찌했누

    Tilemap tilemap;


    [System.Serializable]
    public class HexTileInfoTest
    {
        public HexTest parentHexTileMap;

        public Vector3Int hexTilePos;//헥스타일로컬위치
        public GameObject batchObject;//배치된 오브젝트 체크//장애물이든//유닛이든//
        //나중에 다른컴포넌트로 교체해야 될듯

        //주변 헥스타일 체크
        //기준점은 우측상단 타일부터 좌측상단타일까지 총 6개를 받는다

        public bool[] isExistNearSideHexTileArray;
        public Vector3Int[] nearSideHexTileArray;//이거 헥스타일정보로 했다가 에러생겨서 위치값으로 변경함

        /// <summary>
        /// 헥스타일 초기화
        /// </summary>
        /// <param name="hexTest">헥스타일맵 부모</param>
        /// <param name="pos">헥스타일 현재위치 세팅</param>
        public HexTileInfoTest(HexTest hexTest, Vector3Int pos)
        {
            parentHexTileMap = hexTest;
            hexTilePos = pos;


        }

        /// <summary>
        /// 배치오브젝트를 세팅
        /// </summary>
        /// <param name="targetObject">타겟이 될 오브젝트</param>
        public void SetBatchObject(GameObject targetObject)
        {
            batchObject = targetObject;
        }

        //찾을것들//우측상단부터 좌측상단까지
        private static Vector3Int[] checkPosArray =
        {
            new Vector3Int(1, 0, 1),
            new Vector3Int(1, 0, 0),
            new Vector3Int(1, 0, -1),
            new Vector3Int(0, 0, -1),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 0, 1),
        };

        public void FindNearSideHexTile()
        {
            //우측상단부터 시계방향(오른방향)으로 돌아가서 
            //좌측상단까지 체크


            isExistNearSideHexTileArray = new bool[6];
            nearSideHexTileArray = new Vector3Int[6];

            for (int i = 0; i < checkPosArray.Length; i++)
            {
                int index = i;
                //해당위치에 타일맵이 있는지
                if (parentHexTileMap.hexTileInfoBible.TryGetValue(checkPosArray[index] + hexTilePos, out HexTileInfoTest hexTileInfo))
                {
                    isExistNearSideHexTileArray[index] = true;
                    nearSideHexTileArray[index] = hexTileInfo.hexTilePos;
                }
                else
                {
                    isExistNearSideHexTileArray[index] = false;
                    nearSideHexTileArray[index] = Vector3Int.zero;
                }
            }

        }

    }

    public float speed;
    public GameObject targetObject;
    public Vector3Int currentPos;
    public Vector3Int targetPos;


    //딕 사용하자//느리다
    public List<HexTileInfoTest> hexInfoList = new List<HexTileInfoTest>();

    public Dictionary<Vector3Int, HexTileInfoTest> hexTileInfoBible = new Dictionary<Vector3Int, HexTileInfoTest>();


    //육각타일맵체크
    public HexTileType hexType;
    public float size;

    public TimerModule_Element timer;    

    public int height = 5;
    public int width = 6;
    public GameObject prefab;

    public float heightInterval = 2f;
    public float widthInterval = 1.75f;

    public Vector3 p0;
    public Vector3 p1;
    public Vector3 p2;
    public Vector3 p3;

    private void Awake()
    {
        for (int i = 0; i < height; i++)
        //for (int i = 0; i < 2; i++)
        {
            int indexZ = i;
            //int indexXAddValue = 0;//짝수구역에 있는 배열 위치값을 처리

            //높이가 짝수일때
            int size = 0;
            if (indexZ % 2 != 0)
            {
                size--;
                //indexXAddValue += 1;
            }

            for (int j = 0; j < width + size; j++)
            //for (int j = 0; j < 7 + size; j++)
            {
                int indexX = j;


                //위치설정하기
                //widthInterval

                Vector3Int curPos = new Vector3Int(indexX, 0, indexZ);

                HexTileInfoTest hexTileInfo = new HexTileInfoTest(this, curPos);

                hexTileInfoBible.Add(new Vector3Int(indexX, 0, indexZ), hexTileInfo);
                hexInfoList.Add(hexTileInfo);
            }
        }

        for (int i = 0; i < hexInfoList.Count; i++)
        {
            hexInfoList[i].FindNearSideHexTile();
        }
    }

    private void Update()
    {
        //if (timer.CheckTimer())
        //{
        //    Debug.Log("Tick");
        //}

        //길찾기를 해보자
        //Camera.main.ScreenToWorldPoint();

        Vector3 originPos = transform.position;
        Vector3 lastPoint = Vector3.zero + originPos;
        if (hexTileInfoBible.ContainsKey(targetPos))
        {
            HexTileInfoTest hexTileInfoTest = hexTileInfoBible[targetPos];
            Vector3Int hexTilePos = hexTileInfoTest.hexTilePos;

            //z가 홀수 일시
            float startXPos = 0;

            if (hexTilePos.z % 2 != 0)
            {
                startXPos += size;
            }

            lastPoint = originPos + new Vector3((hexTilePos.x * heightInterval * size) + startXPos, 0, hexTilePos.z * widthInterval * size);
        }
        //이동
        targetObject.transform.position = Vector3.Lerp(targetObject.transform.position, lastPoint, speed * Time.deltaTime);

        if (Input.GetKey(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 mousePoint = ray.origin;
            Debug.Log(mousePoint);
            mousePoint = new Vector3(mousePoint.x, 0, mousePoint.z);

            //z가 홀수 일시
            //float startXPos = 0;

            //if (targetPos.z % 2 != 0)
            //{
            //    startXPos += outerSize;
            //}

            //mousePoint = originPos + new Vector3((mousePos.x * heightInterval * outerSize) + startXPos, 0, mousePos.z * widthInterval * outerSize);
            //mousePos = Vector3Int.CeilToInt(mousePoint);
            //targetPos = new Vector3Int(mousePos.x, 0, mousePos.z);

            //원형값으로는 체크가 금방됨




            //붙어있으면 나누어라
            //정육각형, 직각육각형이라도 체크를 정확히 해라




            for (int i = 0; i < hexInfoList.Count; i++)
            {
                int index = i;
                Vector3Int hexTilePos = hexInfoList[index].hexTilePos;
                //z가 홀수 일시
                float startXPos = 0;

                if (hexTilePos.z % 2 != 0)
                {
                    startXPos += size;
                }
                lastPoint = originPos + new Vector3((hexTilePos.x * heightInterval * size) + startXPos, 0, hexTilePos.z * widthInterval * size);


                //Debug.Log($"마우스{mousePoint} : 헥스타일{checkPos}");

                if (Vector3.Distance(mousePoint, lastPoint) < size)
                {
                    Debug.Log($"{hexInfoList[index].hexTilePos} : Find");
                    targetPos = hexInfoList[index].hexTilePos;
                    break;
                }

            }



            //targetObject.transform.position = mousePos;
        }




        //if (Check(targetPos, currentPos))
        //{
        //    Debug.Log($"도착할수 있습니다.");
        //}
        //else
        //{
        //    Debug.Log($"도착할수 없습니다.");
        //}




    }

    private static Queue<HexTileInfoTest> hexTileInfoQueue = new Queue<HexTileInfoTest>(100);

    private bool Check(Vector3Int currentPos, Vector3Int targetPos)
    {
        //현재 위치가 존재하는지 체크
        if (!hexTileInfoBible.TryGetValue(currentPos, out HexTileInfoTest hexTileInfo))
        {
            return false;
        }

        //타겟 위치가 존재하는지 체크
        if (!hexTileInfoBible.ContainsKey(targetPos))
        {
            return false;
        }

        bool isFind = false;
        hexTileInfoQueue.Clear();
        hexTileInfoQueue.Enqueue(hexTileInfo);

        //미리집어넣기

        do
        {
            //작동
            //해당타일 주변 친구타일들을 체크
            hexTileInfo = hexTileInfoQueue.Dequeue();
            for (int i = 0; i < 6; i++)
            {
                int index = i;
                //타일이 존재하는지
                if (!hexTileInfo.isExistNearSideHexTileArray[index])
                {
                    continue;
                }

                //타겟위치인지 체크
                Vector3Int tempPos = hexTileInfo.nearSideHexTileArray[index];
                if (tempPos == targetPos)
                {
                    //타겟위치면 집어넣기
                    isFind = true;
                    break;
                }
                else
                {
                    //타겟위치가 아니면 집어넣기
                    hexTileInfoQueue.Enqueue(hexTileInfoBible[tempPos]);
                }
            }
        } while (hexTileInfoQueue.Count <= 0);

        return isFind;
    }


    private void OnDrawGizmos()
    {
        //베지어곡선
        for (int i = 0; i < 10; i++)
        {
            int index = i;
            Gizmos.DrawWireSphere(new Vector3(lLcroweUtil.ThreePointBezier(p0.x, p1.x, p2.x, index), 0, lLcroweUtil.ThreePointBezier(p0.z, p1.z, p2.z, index)), 0.2f);
        }

        //헥스처리

        //if (Application.isPlaying)
        //{
        //    if (Check(currentPos, targetPos)) 
        //    {
        //        Gizmos.color = Color.blue;
        //    }
        //    else
        //    {
        //        Gizmos.color = Color.red;
        //    }

        //    Gizmos.DrawLine(currentPos, new Vector3(targetPos.x * widthInterval, 0, targetPos.z * heightInterval));

        //}

        Gizmos.color = Color.white;

        Vector3 originPos = transform.position;

        switch (hexType)
        {
            case HexTileType.PointyTop:
                //outer가 상하로 되있는 모양
                //좌우먼저 진행하고 상하를 진행해야됨


                for (int i = 0; i < height; i++)
                //for (int i = 0; i < 2; i++)
                {
                    int indexZ = i;
                    float startXPos = 0;

                    //높이가 짝수일때
                    int size = 0;
                    if (indexZ % 2 != 0)
                    {
                        size--;
                        startXPos += this.size;
                    }

                    for (int j = 0; j < width + size; j++)
                    //for (int j = 0; j < 7 + size; j++)
                    {
                        int indexX = j;


                        //위치설정하기
                        //widthInterval
                        Handles.Label(originPos + new Vector3((indexX * heightInterval * this.size) + startXPos, 0, indexZ * widthInterval * this.size), $"배열:{indexX}, {indexZ}");


                        DrawHex(originPos + new Vector3((indexX * heightInterval * this.size) + startXPos, 0, indexZ * widthInterval * this.size));
                    }
                }


                break;
            case HexTileType.FlatTop:


                //outer가 좌우로 되있는 모양
                //상하먼저작업하고 옆으로 진행해야됨

                for (int i = 0; i < width; i++)
                {
                    int indexX = i;
                    float startZPos = 0;

                    //width가 홀수부분이면 절반높이에서 +1 크기
                    int size = 0;
                    if (indexX % 2 != 0)
                    {
                        size++;
                        startZPos -= this.size;
                    }

                    for (int j = 0; j < height + size; j++)
                    {
                        int indexZ = j;

                        DrawHex(originPos + new Vector3(indexX * widthInterval * this.size, 0f, (indexZ * heightInterval * this.size) + startZPos));
                    }
                }
                break;
        }
    }

    private void DrawHex(Vector3 originPos)
    {
        Vector3[] tempArray = null;
        
        tempArray = lLcroweUtil.HexTileMatrix.GetHexTilePoint(originPos, size, hexType, CreateTileAxisType.XZ);

        Gizmos.DrawLine(tempArray[0], tempArray[1]);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(tempArray[1], tempArray[2]);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(tempArray[2], tempArray[3]);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(tempArray[3], tempArray[4]);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(tempArray[4], tempArray[5]);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(tempArray[5], tempArray[0]);
        Gizmos.color = Color.blue;
    }
}
