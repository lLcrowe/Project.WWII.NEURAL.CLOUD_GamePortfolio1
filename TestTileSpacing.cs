using lLCroweTool;
using lLCroweTool.TileMap.HexTileMap;
using UnityEngine;
using static lLCroweTool.lLcroweUtil.HexTileMatrix;
using static lLCroweTool.TileMap.HexTileMap.Custom3DHexTileMap;

public class TestTileSpacing : MonoBehaviour
{
    public int widthAmount = 7;
    public int heightAmount = 5;

    public float widthSpacingValue = 1;
    public float heightSpacingValue = 1;

    public float tileSize = 1f;
    public float tileDistance = 2f;//타일끼리의 간격

    public HexTileType hexTileType;
    public CreateTileAxisType createTileAxisType;

    public Material material;
    public HexTileObject hexTilePrefab;
    public HexTileObjectBible hexTileObjectBible = new HexTileObjectBible();

    public Transform target;

    [ButtonMethod]
    public void Init()
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
                        hexTile.hexTileData.InitHexTileData(tilePos,hexTile,null);
                        hexTile.CreateMesh();

                        hexTileObjectBible.Add(tilePos, hexTile);
                    }
                }
                break;
        }
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

    private void OnDrawGizmos()
    {
        if (target == null)
        {
            return;
        }

        //월드투셀
        //로컬위치값

        var pos = ConvertWorldPosForTilePos(target.position);
        Debug.Log(pos);
    }

    
    /// <summary>
    /// 월드위치를 타일위치로 변환하는 함수
    /// </summary>
    /// <param name="worldPos">월드위치</param>
    /// <returns>타일위치</returns>
    private Vector3Int ConvertWorldPosForTilePos(Vector3 worldPos)
    {
        Vector3 targetPos = transform.InverseTransformPoint(worldPos);
        targetPos = ConventAxis(targetPos);//축변경

        ////각축의 간격
        float xSpacing = tileDistance + widthSpacingValue;
        float xHalfSpacing = xSpacing * 0.5f;
        float ySpacing = tileDistance + heightSpacingValue;
        float yHalfSpacing = ySpacing * 0.5f;

        ////본래의 위치//간격을 나누어 본래위치를 구함
        //float xPos = targetPos.x;
        //float yPos = targetPos.y;

        //float xTileMousePos = targetPos.x / xSpacing;
        //float yTileMousePos = targetPos.y / ySpacing;

        ////타일위치//이위치로 찾아가야됨
        //int xTilePos = Mathf.RoundToInt(xTileMousePos);
        //int yTilePos = Mathf.RoundToInt(yTileMousePos);

        //float xTilePosF = xTilePos;
        //float yTilePosF = yTilePos;

        ////오프셋들
        //Vector2 posOffSet = Vector2.zero;        
        //Vector3Int tileOffset = Vector3Int.zero;//타일위치 오프셋처리


        //1. 홀수에서 문제가 발생 => MathF.RoundToInt 문제
        //특정위치가 홀수일시 처리
        //switch (hexTileType)
        //{
        //    case HexTileType.FlatTop:
        //        //float addYPos = tilePos.x % 2 == 1 ? (tileDistance + heightSpacingValue) * 0.5f : 0;
        //        //addPos = new Vector2(0, addYPos);
        //        if (xTilePos % 2 == 1)
        //        {
        //            posOffSet = new Vector2(0, ySpacing * 0.5f);
        //            //offset = new Vector3Int(0, -Mathf.RoundToInt(spacing));
        //        }
        //        break;
        //    case HexTileType.PointyTop:

        //        //홀수일시 처리


        //        if (yTilePos % 2 == 1)
        //        {
        //            //음수양수처리
        //            int mulValue = (int)Mathf.Sign(xPos);
        //            mulValue = mulValue >= 0 ? 1 : mulValue;

        //            //여기대기
        //            //xTilePos = Mathf.RoundToInt(xPos + xHalfSpacing);

        //            ////강제오프셋 +-1//0이 아니게 변경
        //            ////xTilePosF = 1 * mulValue;

        //            //posOffSet.x = RoundToNear(xTileMousePos, xTilePosF, xHalfSpacing);

        //            //xTilePosF이걸로 작업해야됨//이걸로 작업할려면 xTileMousePos 이걸로 상호작용해야되고
        //            //차라리 0 만 예외처리하고
        //            //xTilePos = Mathf.RoundToInt(xTileMousePos - xHalfSpacing);
        //            //tilePosXF += tilePosX * xSpacing;

        //            Debug.Log($"{xPos}\n{(xTilePos * xSpacing) + xHalfSpacing}");

        //            //posOffSet = new Vector2(tilePosXF, 0) * mulValue;
        //            //offset = new Vector3Int(-Mathf.RoundToInt(spacing), 0);
        //            //타일포스를 더해주는걸 잊지말기
        //        }
        //        break;
        //}


        //순회해서 처리
        Vector3Int targetTilePos = Vector3Int.zero;
        foreach (var item in hexTileObjectBible)
        {
            var hexTileObject = item.Value;
            var hexTileData = hexTileObject.GetHexTileData;
            Vector3 hexTileWorldPos = ConventAxis(hexTileObject.transform.localPosition);//축변경            

            if (!CheckRoundToNear(targetPos.y, hexTileWorldPos.y, yHalfSpacing))
            {
                continue;
            }


            if (!CheckRoundToNear(targetPos.x, hexTileWorldPos.x, xHalfSpacing))
            {

                continue;
                
            }

            targetTilePos = hexTileData.GetTilePos();
            //Debug.Log(targetTilePos);
            break;
        }


        //tilePosX = RoundToNear(xPos / xSpacing,, xHalfSpacing);
        //tilePosY = RoundToNear(yPos / ySpacing,, yHalfSpacing);







        //float positionValue = (xSpacing * 0.5f) + xSpacing / 2;
        //positionValue += (tilePosX - 1) * xSpacing;

        //간격 2
        //홀수일시는 절반간격을 추가 == 5 * 0.5f;
        //홀수

        //홀수//1 3 5 => 변경될 기준선 2 4 
        //float resultX = RoundToNear(xPos, (tilePosX * xSpacing) +  addPos.x, xSpacing * 0.5f);

        //짝수//0 2 4 => 변경될 기준선 1 3//작동완료
        //float resultX = RoundToNear(xPos, tilePosX, xHalfSpacing);
        //float resultY = RoundToNear(yPos,tilePosY + posOffSet.y, yHalfSpacing);
        //Debug.Log($"{resultX},{resultY}");


        //Debug.Log($"{xPos}=>{tilePosX}\n {posOffSet.x},{RoundToNear(posOffSet.x, posOffSet.x, xHalfSpacing)}");
        //Debug.Log($"{xPos}\n{tilePosX}\n{posOffSet}\n{resultX}");
        //Debug.Log($"{xPos},{yPos}\n{tilePosX},{tilePosY}\n{resultX},{resultY}");


        //Debug.Log($"{xPos},{yPos}+{addPos.x},{addPos.y}\n=>{addPos.x + xPos},{addPos.y + addPos.y}");
        //Debug.Log($"{xPos},{yPos}\n{addPos.x},{addPos.y}=>{addPos.x + xPos},{addPos.y + addPos.y}\n{x1IntPos},{y1IntPos}");

        //if (cashVec != newIntPos)
        //{
        //    cashVec = newIntPos;
        //    Debug.Log($"{newPos}\n {newIntPos}");
        //} 

        return targetTilePos;
    }

    /// <summary>
    /// 일정값을 특정값으로 반올림시켜주는 함수.
    /// RoundToInt는 int로 만되니 대신만드는것
    /// </summary>
    /// <param name="value">현재 값</param>
    /// <param name="checkValue">기준이 될 위치값</param>
    /// <param name="range">범위</param>
    /// <returns>라운드될 값안에 들어왔는지 여부</returns>
    private bool CheckRoundToNear(float value, float checkValue, float range)
    {
        float lowerBound = checkValue - range;
        float upperBound = checkValue + range;

        //라운드 범위체크
        //0 ~ 0.4, 0.5~0.9
        //0<=0.5, 0.5 < (1 == 0.9999..)
        return lowerBound <= value && value < upperBound;
    }

    /// <summary>
    /// 일정값을 특정값으로 반올림시켜주는 함수.
    /// RoundToInt는 int로 만되니 대신만드는것
    /// </summary>
    /// <param name="value">현재 값</param>
    /// <param name="checkValue">기준이 될 위치값</param>
    /// <param name="range">범위</param>
    /// <returns>라운드된 최종값</returns>
    private float RoundToNear(float value, float checkValue, float range)
    {
        //범위체크
        if (CheckRoundToNear(value, checkValue, range))
        {
            //안이면 체크할밸류로 넘김
            return checkValue;
        }
        else
        {
            //밖이면 그대로 넘김
            return value;
        }
    }

    private Vector3 ConventAxis(Vector3 worldPos)
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
}
