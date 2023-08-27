using System.Collections.Generic;
using UnityEngine;
using lLCroweTool.Dictionary;
using lLCroweTool.TimerSystem;
using lLCroweTool.AstarPath;
using DG.Tweening;
using System.Collections;
using Doozy.Engine.Extensions;

namespace lLCroweTool.TileMap.HexTileMap
{
    public class HexBasementTileMap : MonoBehaviour
    {
        //헥스베이스먼트타일맵
        //특정타일맵을 이용하여 새로운매커니즘을 작동시키게해주는 기능을 가짐


        //헥스타일맵(메커니즘관련)
        //타일에 무엇이있는지체크
        //BFS도 같이 짜기//길찾기
        //감염? 그것도 만들어보자
        //타일맵에 붙착


        public LayerMask obstacleLayer;
        public int batchLimitPos = 4;
        [SerializeField] protected Custom3DHexTileMap tilemap;

        [System.Serializable] public class HexAreaBible : CustomDictionary<Vector3Int, HexTileData> { }
        [Header("헥스영역 딕셔너리")]
        //위치값은 중복되지않음
        public HexAreaBible hexAreaBible = new HexAreaBible();
        public IAstarNodeBible astarNodeBible = new IAstarNodeBible();

        public List<HexTileObject> tileObjectList = new List<HexTileObject>();
        public Vector3 tileMapCenterPos;//중앙위치
        public TimerModule_Element timerModule;

        /// <summary>
        /// 계산된 헥스타일 체크용도
        /// </summary>
        private Dictionary<HexTileData, bool> calHexAeaInfoBible = new Dictionary<HexTileData, bool>();
        private Dictionary<Vector3Int, bool> calCashGasBible = new Dictionary<Vector3Int, bool>();//계산할 캐싱할가스

        public int oxygenTileCount;//타일갯수

        [Header("애니메이션관련")]
        public float waitFallTimer = 0.01f;
        public float fallSpeed = 0.5f;
        public float punchSpeed = 0.3f;

        //private TilemapCollider2D tilemapCol2D;//영역체크용//우주차량용애도 있는거 같은데 체크하기
        //이게우선순위가 더 높은거 같음
        public Vector3Int[] vector3IntArray;
        //[SerializeField] protected Tilemap tilemap;

        protected void Awake()
        {
            if (tilemap == null)
            {
                tilemap = GetComponent<Custom3DHexTileMap>();
            }
            
            timerModule.SetTimer(1f);
            InitHexBasementTileMap();
            //StartCoroutine(ActionFallHexTile());
        }

        public void InitHexBasementTileMap()
        {
            //생성된 타일들을 가져와서 처리
            int i = 0;
            vector3IntArray = new Vector3Int[tilemap.hexTileObjectBible.Count];
            tileObjectList.Clear();
            Vector3 total = Vector3.zero;
            foreach (var item in tilemap.hexTileObjectBible)
            {
                Vector3Int tilePos = item.Key;
                HexTileObject hexTileObject = item.Value;
                HexTileData hexTileData = hexTileObject.GetHexTileData;
                hexTileData.InitHexTileData(tilePos, hexTileObject, tilemap);
                if (tilePos.x < batchLimitPos)
                {
                    //여기 짝수일때 변하는거 체크
                    hexTileData.IsBatchTile = true;//배치위치들
                    float a = hexTileObject.TileColor.a;
                    hexTileObject.TileColor = new Color(0.5f, 0, 0, a);
                }


                hexAreaBible.Add(tilePos, hexTileData);
                astarNodeBible.Add(tilePos, hexTileData);
                tileObjectList.Add(hexTileObject);

                total += hexTileData.GetWorldPos();
                vector3IntArray[i] = tilePos;
                i++;
            }

            //중앙위치
            tileMapCenterPos = total / vector3IntArray.Length;
            oxygenTileCount = vector3IntArray.Length;
        }

        /// <summary>
        /// 업데이트처리해서 헥스타일들의 상태를 업데이트시켜주는 함수
        /// </summary>
        public void InitHexTileObjectList()
        {
            //업데이트처리해서 헥스타일들의 상태를 업데이트시켜서 
            //길찾기가 가능한지 체크함
            foreach (var item in tileObjectList)
            {
                item.UpdateHexTileData();
            }
        }

        [ButtonMethod]
        public void Test()
        {
            StartCoroutine(ActionFallHexTile());
        }


        private IEnumerator ActionFallHexTile()
        {
            for (int i = 0; i < tileObjectList.Count; i++)
            {
                tileObjectList[i].transform.localPosition += Vector3.up * 5;
            }

            var tempWait = new WaitForSeconds(waitFallTimer);
            for (int i = 0; i < tileObjectList.Count; i++)
            {   
                tileObjectList[i].transform.DOLocalMoveY(0, fallSpeed);
                yield return tempWait;
            }

            if (punchSpeed > 0.001f)
            {

                Vector3 punch = Vector3.one * 1.2f;
                for (int i = 0; i < tileObjectList.Count; i++)
                {
                    tileObjectList[i].transform.DOPunchScale(punch, punchSpeed);
                }
            }            
        }

        /// <summary>
        /// 헥스영역 초기화
        /// </summary>
        public void ResetHexArea()
        {
            var HexAreaInfoList = hexAreaBible.GetValueList();
            for (int i = 0; i < HexAreaInfoList.Count; i++)
            {
                HexTileData hexAreaInfo = HexAreaInfoList[i];
                hexAreaInfo.ResetHexTileArea();
            }
        }

        [ButtonMethod]
        public void GetTilePos()
        {
            if (tilemap == null)
            {
                tilemap = GetComponent<Custom3DHexTileMap>();
            }
            vector3IntArray = lLcroweUtil.GetAllTilePos(tilemap);//모든배치타일 가져오기
        }

        public void ClearTileColor()
        {
            for (int i = 0; i < tileObjectList.Count; i++)
            {
                tileObjectList[i].TileColor = new Color(0.45f,0.45f,0.45f);
            }
        }

        private void Update()
        {
            if (!timerModule.CheckTimer())
            {
                return;
            }

            //Profiler.BeginSample("-=OxygenUpdateLoop!=-");
            HexTileData voidRoom = null;

            for (int i = 0; i < vector3IntArray.Length; i++)
            {
                if (hexAreaBible.ContainsKey(vector3IntArray[i]))
                {
                    //Profiler.BeginSample("-=OxygenUpdate!=-");
                    voidRoom = hexAreaBible[vector3IntArray[i]];
                    UpdateHexAreaInfo(ref voidRoom, this);
                    //Profiler.EndSample();
                }

            }
            //Profiler.EndSample();

            calHexAeaInfoBible.Clear();

            //모아둔값들을 가스변경값들을 변경시킴
            foreach (var item in calCashGasBible)
            {
                Vector3Int pos = item.Key;
                hexAreaBible[pos].isGasArea = true;
                lLcroweUtil.SetTile(pos, Color.red, tilemap);
            }
            calCashGasBible.Clear();
        }

        /// <summary>
        /// 가스 업데이트
        /// </summary>
        public static void UpdateHexAreaInfo(ref HexTileData hexAreaInfo, HexBasementTileMap targetBasementTilemap)
        {
            //가스관련 업데이트//나중엔 가스말고 점막같은것도 가능할듯함

            //탑뷰
            //모든좌표를 랜덤으로 체크
            var tempBible = targetBasementTilemap.calHexAeaInfoBible;
            //이미계산했으면 넘어가기
            if (tempBible.ContainsKey(hexAreaInfo))
            {
                return;
            }
            tempBible.Add(hexAreaInfo, false);

            //건축물(방해물)이면 계산안함
            if (hexAreaInfo.GetHexTileObject.BatchUnitObject != null)
            {
                var structUnit = hexAreaInfo.GetHexTileObject.BatchUnitObject as StructureUnitObject;
                if (structUnit != null)
                {
                    return;
                }
            }

            //가스상태가 아니면 넘어가기
            if (!hexAreaInfo.isGasArea)
            {
                return;
            }

            HexTileData tempHexTileData = null;
            HexBasementTileMap tempBasementTilemap = targetBasementTilemap;
            var calCashGasBible = tempBasementTilemap.calCashGasBible;

            //근처타일들을 다 체크함
            for (int i = 0; i < hexAreaInfo.NearCheckAreaArray.Length; i++)
            {
                int index = i;
                if (hexAreaInfo.NearCheckAreaArray[index])
                {
                    //있으면 메커니즘 작동
                    //주변 헥스타일

                    //존재여부체크
                    if (!tempBasementTilemap.GetHexAreaInfo(hexAreaInfo.NearAreaArray[index], out tempHexTileData))
                    {
                        continue;
                    }

                    //번져줄 타일들을 집어넣음
                    if (tempHexTileData.isGasArea)
                    {
                        //상대타일이 가스상태이면 넘어가기
                        continue;
                    }

                    //상대타일이 방해물이면 넘어가기
                    if (tempHexTileData.GetHexTileObject.BatchUnitObject != null)
                    {
                        var structUnit = tempHexTileData.GetHexTileObject.BatchUnitObject as StructureUnitObject;
                        if (structUnit != null)
                        {
                            continue;
                        }
                    }

                    //계산에 집어넣기
                    if (!calCashGasBible.ContainsKey(tempHexTileData.GetTilePos()))
                    {
                        calCashGasBible.Add(tempHexTileData.GetTilePos(), false);
                    }

                }
            }
        }

        public void SetGas(Vector3Int pos)
        {
            if (!hexAreaBible.ContainsKey(pos))
            {
                return;
            }
            hexAreaBible[pos].isGasArea = true;
            lLcroweUtil.SetTile(pos, Color.red, tilemap);
        }

        /// <summary>
        /// 해당위치의 헥스타일데이터를 가져옴
        /// </summary>
        /// <param name="pos">타일위치</param>
        /// <param name="hexTileData">헥스타일데이터</param>
        /// <returns></returns>
        public bool GetHexAreaInfo(Vector3Int pos, out HexTileData hexTileData)
        {
            return hexAreaBible.TryGetValue(pos, out hexTileData);
        }

        /// <summary>
        /// 타일맵 가져오기함수
        /// </summary>
        /// <returns>타일맵</returns>
        public Custom3DHexTileMap GetTileMap()
        {
            return tilemap;
        }
    }
}
