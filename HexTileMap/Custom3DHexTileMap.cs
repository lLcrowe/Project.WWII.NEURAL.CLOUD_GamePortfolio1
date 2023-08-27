using lLCroweTool.Dictionary;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using static lLCroweTool.lLcroweUtil.HexTileMatrix;

namespace lLCroweTool.TileMap.HexTileMap
{
    public class Custom3DHexTileMap : MonoBehaviour
    {
        //유니티타일맵은 3D를 지원하지않음
        //그래서 따로 제작
        //유니티 타일맵처럼 배열을 뽑아야되니까 
        //넓이먼저 처리

        //플래툰처리 홀수여야할듯

        //제작될 타일맵 크기//현재 뉴럴클라우드 크기 그대로

        [Header("FlatTop인 경우 Width를 홀수크기\n PointyTop인 경우 Height를 홀수크기로 맞추기")]

        public int widthAmount = 7;
        public int heightAmount = 5;

        public float widthSpacingValue = 1;
        public float heightSpacingValue = 1;

        public float tileSize = 1f;
        public float tileDistance = 2f;//타일끼리의 간격

        public HexTileType hexTileType;
        public CreateTileAxisType createTileAxisType;

        public Material material;//=>적용할 메터리얼
        public List<Vector3Int> tileMapPosList = new List<Vector3Int>();

        [System.Serializable]
        public class HexTileObjectBible : CustomDictionary<Vector3Int, HexTileObject> { }
        public HexTileObjectBible hexTileObjectBible = new HexTileObjectBible();
        public HexTileObject hexTilePrefab;//랜더러와 배치 둘다 있는것

        public Transform testTarget;

        [ButtonMethod]
        public void CreateHexTileMap()
        {
            //지우기
            foreach (var item in hexTileObjectBible)
            {
                var hexTile = item.Value;
                DestroyImmediate(hexTile.gameObject);
            }
            hexTileObjectBible.Clear();

            //로컬 생성
            switch (hexTileType)
            {
                case HexTileType.FlatTop:
                    for (int x = 0; x < widthAmount; x++)
                    {
                        int decreaseValue = x % 2 == 1 ? 1 : 0;
                        for (int y = 0; y < heightAmount - decreaseValue; y++)
                        {
                            var hexTile = Instantiate(hexTilePrefab);
                            Vector3Int tilePos = new Vector3Int(x, y);//고정
                            hexTile.name = tilePos.ToString();
                            hexTile.InitTrObjPrefab(GetTileLocalPos(tilePos), Quaternion.identity, transform, false);
                            hexTile.InitHexTileRenderer(tileSize, hexTileType, createTileAxisType, material);
                            hexTile.hexTileData.InitHexTileData(tilePos, hexTile, null);
                            hexTile.CreateMesh();

                            hexTileObjectBible.Add(tilePos, hexTile);
                        }
                    }
                    break;
                case HexTileType.PointyTop:
                    for (int y = 0; y < heightAmount; y++)
                    {
                        int decreaseValue = y % 2 == 1 ? 1 : 0;
                        for (int x = 0; x < widthAmount - decreaseValue; x++)
                        {
                            var hexTile = Instantiate(hexTilePrefab);
                            Vector3Int tilePos = new Vector3Int(x, y);//고정
                            hexTile.name = tilePos.ToString();
                            hexTile.InitTrObjPrefab(GetTileLocalPos(tilePos), Quaternion.identity, transform, false);
                            hexTile.InitHexTileRenderer(tileSize, hexTileType, createTileAxisType, material);
                            hexTile.hexTileData.InitHexTileData(tilePos, hexTile, null);
                            hexTile.CreateMesh();

                            hexTileObjectBible.Add(tilePos, hexTile);
                        }
                    }
                    break;
            }
        }
        
        /// <summary>
        /// 월드포지션을 타일위치로 바꾸는 함수
        /// </summary>
        /// <param name="worldPos">월드포지션</param>
        /// <returns>타일위치</returns>
        public Vector3Int WorldToCell(Vector3 worldPos)
        {   
            return ConvertWorldPosForTilePos(worldPos);
        }


        /// <summary>
        /// 월드포지션을 타일맵의 타일위치에 맞게 변환해주는 함수
        /// </summary>
        /// <param name="worldPos">월드포지션</param>
        /// <returns>타일위치</returns>
        private Vector3Int ConvertWorldPosForTilePos(Vector3 worldPos)
        {
            Profiler.BeginSample("타일위치가져오기");
            Vector3 targetPos = transform.InverseTransformPoint(worldPos);
            targetPos = ConventAxisToTile(targetPos);//축변경

            ////각축의 간격
            float xSpacing = tileDistance + widthSpacingValue;
            float xHalfSpacing = xSpacing * 0.5f;
            float ySpacing = tileDistance + heightSpacingValue;
            float yHalfSpacing = ySpacing * 0.5f;

            //순회해서 처리
            Vector3Int targetTilePos = Vector3Int.zero;
            foreach (var item in hexTileObjectBible)
            {
                var hexTileObject = item.Value;
                var hexTileData = hexTileObject.GetHexTileData;
                Vector3 hexTileWorldPos = ConventAxisToTile(hexTileObject.transform.localPosition);//축변경            

                if (!lLcroweUtil.CheckRoundToNear(targetPos.y, hexTileWorldPos.y, yHalfSpacing))
                {
                    continue;
                }


                if (!lLcroweUtil.CheckRoundToNear(targetPos.x, hexTileWorldPos.x, xHalfSpacing))
                {

                    continue;

                }

                targetTilePos = hexTileData.GetTilePos();
                //Debug.Log(targetTilePos);
                break;
            }
            Profiler.EndSample();
            return targetTilePos;
        }

        /// <summary>
        /// 타일위치에 대한 월드포지션을 가져오는 함수
        /// </summary>
        /// <param name="tilePos">타일위치</param>
        /// <returns></returns>
        private Vector3 GetTileLocalPos(Vector3Int tilePos)
        {
            //각 타입에 따른 위치 변경//길이의 절반
            Vector2 addPos = Vector2.zero;
            switch (hexTileType)
            {
                case HexTileType.FlatTop:
                    float addYPos = tilePos.x % 2 == 1 ? (tileDistance + heightSpacingValue) * 0.5f : 0;
                    addPos = new Vector2(0, addYPos);
                    break;
                case HexTileType.PointyTop:
                    float addXPos = tilePos.y % 2 == 1 ? (tileDistance + widthSpacingValue) * 0.5f : 0;
                    addPos = new Vector2(addXPos, 0);
                    break;
            }

            //타일간의 거리 == 곱하기
            //좌표에 따른 spacing == 곱하기
            //spacing == 더하기
            //위치추가 == 더하기

            //최종계산
            //float xPos = (tilePos.x * tileDistance) + addPos.x + (widthSpacingValue * tilePos.x);
            //float yPos = (tilePos.y * tileDistance) + addPos.y + (heightSpacingValue * tilePos.y);
            //최적화
            float xPos = (tilePos.x * (tileDistance + widthSpacingValue)) + addPos.x;
            float yPos = (tilePos.y * (tileDistance + heightSpacingValue)) + addPos.y;

            //축변경
            Vector3 newPos = ConvertAxisToWorldPos(xPos, yPos);
            return newPos;
        }

        //이건급한거 아님
        //public Vector3 GetTileWorldPos(Vector3Int tilePos)
        //{   
        //    
        //    return GetTileLocalPos(tilePos) + transform.position;
        //}

        private void OnValidate()
        {            
            foreach (var item in hexTileObjectBible)
            {
                var hexTileTr = item.Value.transform;
                var tilePos = item.Key;
                hexTileTr.localPosition = GetTileLocalPos(tilePos);
                item.Value.InitHexTileRenderer(tileSize, hexTileType, createTileAxisType, material);
            }
        }

        public bool HasTile(Vector3Int vector3Int)
        {
            return hexTileObjectBible.ContainsKey(vector3Int);
        }

        public bool TryGetHexTile(Vector3Int vector3Int, out HexTileObject hexTileObject)
        {
            return hexTileObjectBible.TryGetValue(vector3Int, out hexTileObject);
        }

        private void OnDrawGizmos()
        {   
            if (testTarget == null)
            {
                return;
            }

            //Vector3Int newPos = ConvertWorldPosForTilePos(originPos);//이걸고치는것
            //타일맵의 위치를 찾는게 문제가 발생

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(testTarget.position, 0.5f);


            //스케일처리
            //음 생각해보자
            //
            //타일맵이 만들어지는 방향처리 방법//완//로컬로 처리
            //특정오브젝트 월드좌표값을 타일맵로컬로 변경하는것
            //InverseTransformPoint 사용



            //Vector3 newPos1 = transform.TransformVector(testTarget.position);
            //newPos1.y = 0;
            //Vector3 newPos2 = transform.InverseTransformVector(testTarget.position);
            //newPos2.y = 0;
            //Vector3 newPos3 = transform.TransformPoint(testTarget.position);
            //newPos3.y = 0;
            //Vector3 newPos4 = transform.InverseTransformPoint(testTarget.position);
            //newPos4.y = 0;
            //Vector3 newPos5 = transform.TransformDirection(testTarget.position);
            //newPos5.y = 0;
            //Vector3 newPos6 = transform.InverseTransformDirection(testTarget.position);
            //newPos6.y = 0;

            //print($"타일맵 월드위치 :{transform.position}\n 타겟오브젝트 월드위치 :{testTarget.position}\n" +
            //    $"위치1 :{newPos1}\n위치2 :{newPos2}\n위치3 :{newPos3}\n위치4 :{newPos4}\n위치5 :{newPos5}\n위치6 :{newPos6}\n"
            //    );

            

            //문제가 있음
            Vector3Int tilePos = ConvertWorldPosForTilePos(testTarget.position);
            //Vector3Int tilePos = GetHexCoordinates(targetPos);
            //Vector3Int tilePos = ConvertWorldPosForTilePos(targetPos);



            print($"위치 :{testTarget.position}\n타일 :{tilePos}");
            //outerSize * 0.866f;


            //디버그해서 문제가 어디에 있는지 체크할려는 구역
            
            Vector3 originPos = testTarget.position;
            Vector3 upRot = testTarget.forward;
            Vector3 rightRot = testTarget.right;
                        
            Vector3 upSpacing = upRot * (tileSize * 0.5f) * 0.866f;
            Vector3 rightSpacing = rightRot * ((tileSize * 0.5f) );

            Vector3 upSize = upRot * tileSize;
            Vector3 rightSize = rightRot * tileSize;

            //Ray upSpacingRay = new Ray(originPos, upSpacing);
            //Ray rightSpacingRay = new Ray(originPos, rightSpacing);

            //Ray upSizeRay = new Ray(originPos, upSize);
            //Ray rightSizeRay = new Ray(originPos, rightSize);




            Gizmos.color = Color.green;
            Gizmos.DrawLine(originPos, originPos + upSpacing);
            Gizmos.DrawLine(originPos + upSpacing, originPos + upSpacing + rightSpacing);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(originPos, originPos + rightSpacing);



        }

        // 좌표 상자 시스템에서 특정 헥스 타일의 월드 좌표를 반환하는 함수
        public Vector3 GetHexPosition(int q, int r)
        {
            float xOffset = q * tileSize * 1.5f; // x 방향 이동량
            float yOffset = r * tileSize * 2.0f * Mathf.Sqrt(3) / 4.0f; // y 방향 이동량
            return new Vector3(xOffset, 0f, yOffset);
        }

        /// <summary>
        /// 월드위치를 타일위치축에 맞게 변경하는 함수
        /// </summary>
        /// <param name="worldPos">월드위치</param>
        /// <returns>XY로 이루어진 축 벡터</returns>
        private Vector3 ConventAxisToTile(Vector3 worldPos)
        {
            switch (createTileAxisType)
            {
                case CreateTileAxisType.XY:
                    worldPos = new Vector3(worldPos.x, worldPos.y);
                    break;
                case CreateTileAxisType.XZ:
                    worldPos = new Vector3(worldPos.x, worldPos.z);
                    break;
            }
            return worldPos;
        }

        /// <summary>
        /// 월드위치에 맞게 벡터를 반환해줌
        /// </summary>
        /// <param name="xPos">x위치</param>
        /// <param name="yPos">y위치</param>
        /// <returns>월드위치</returns>
        private Vector3 ConvertAxisToWorldPos(float xPos, float yPos)
        {
            Vector3 newPos = Vector3.zero;
            switch (createTileAxisType)
            {
                case CreateTileAxisType.XY:
                    newPos = new Vector3(xPos, yPos);
                    break;
                case CreateTileAxisType.XZ:
                    newPos = new Vector3(xPos, 0, yPos);
                    break;
            }
            return newPos;
        }
             
    }
}