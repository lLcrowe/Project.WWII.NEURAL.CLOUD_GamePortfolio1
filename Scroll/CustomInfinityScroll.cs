using lLCroweTool.UI.SmoothScroll;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace lLCroweTool.UI.InfinityScroll.Scroll
{
    [RequireComponent(typeof(ScrollSmoothMove))]
    public class CustomInfinityScroll : MonoBehaviour
    {
        //컴포넌트
        [Header("컴포넌트")]
        //셀버튼들 고정축초기화해야됨
        public CellUI_Base cellPrefab;//프리팹처리
        public EScrollType scrollType;

        //기초설정관련
        [Space]
        [Header("보여주는거 설정")]
        [Min(1)]
        public int showCellAmount = 4;//보여주는 CellAmount
        [Min(1)]
        public int showCellLineAmount = 1;//해당라인의 셀들을 보여주는 용도//디폴트1
        public float spacingValue = 20;//간격처리

        [Space]
        [Header("데이터 설정")]
        [Min(0)]
        public int cellCapacity;//셀용량들//외부에서 받아오는 데이터들로 정해짐//

        //기준값//스탠다드라고 정할까
        public List<CellData> cellDataList = new List<CellData>();//셀데이터

        [Header("UI처리설정")]
        //UI처리를 위한
        private List<CellUI_Base> cellUIList = new List<CellUI_Base>();
        private int cellUIAmount;//리스트용량캐싱//리스트카운트

        //인덱스 기준으로 잡아놔야지 데이터랑 연동할때 편할거 같은데
        //일단 뭐가 되야지 해먹지
        private int currentIndex = 0;//첫번째 기준
        private int lastIndex;//마지막으로 변한 인덱스//바뀌었을때 갱신용//트리거
        private ScrollRect scrollRect;//스크롤렉트
        private RectTransform contentRect;//전체적인 크기

        /// <summary>
        /// 스크롤의 작동방식
        /// </summary>
        public enum EScrollType
        {
            Vertical,//수직
            Horizontal,//수평
        }

        private void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();
            scrollRect.scrollSensitivity = 100;//기본 100을 잡으면 적당함
            scrollRect.onValueChanged.AddListener(UpdateScroll);
            contentRect = scrollRect.content;

            //활성화처리
            switch (scrollType)
            {
                case EScrollType.Vertical:
                    scrollRect.horizontal = false;
                    //contentRect.anchorMin = new Vector2(0, 1);
                    //contentRect.anchorMax = new Vector2(1, 1);
                    if (scrollRect.horizontalScrollbar != null)
                    {
                        DestroyImmediate(scrollRect.horizontalScrollbar.gameObject);
                    }
                    break;
                case EScrollType.Horizontal:
                    scrollRect.vertical = false;
                    //contentRect.anchorMin = new Vector2(0, 0);
                    //contentRect.anchorMax = new Vector2(0, 1);
                    if(scrollRect.verticalScrollbar != null)
                    {
                        DestroyImmediate(scrollRect.verticalScrollbar.gameObject);
                    }
                    break;
            }
        }

        /// <summary>
        /// 데이터세팅
        /// </summary>
        /// <param name="newCellDataList">신규데이터목록들</param>
        public void Init<T>(List<T> newCellDataList) where T: CellData
        {
            //기존거 프리팹 비활성화
            for (int i = 0; i < cellUIList.Count; i++)
            {
                cellUIList[i].gameObject.SetActive(false);
            }
            cellUIList.Clear();

            if (newCellDataList != null)
            {
                cellDataList.Clear();
                cellDataList.AddRange(newCellDataList);
            }
            RefreshContentSize();

            //프리팹 소환
            if (showCellAmount != 0)
            {
                //소환만//보여주는 쉘수 * 라인에서 보여주는 양
                for (int i = 0; i < showCellAmount * showCellLineAmount; i++)
                {
                    CellUI_Base cellObject = ObjectPoolManager.Instance.RequestDynamicComponentObject(cellPrefab);
                    cellObject.transform.InitTrObjPrefab(Vector2.zero, Quaternion.identity, contentRect, false);
                    cellObject.InitCellUI(cellObject.GetHeight(), cellObject.GetWidth(), Vector2.zero);

                    cellUIList.Add(cellObject);
                }

                //RefreshContentSize();
            }


            //세팅//인덱스처리를 그대로 유지해야된다면?//리플래쉬에서 해야됨
            cellUIAmount = cellUIList.Count - 1;
            currentIndex = 0;
            lastIndex = currentIndex;
            contentRect.anchoredPosition = Vector2.zero;


            //데이터동기화
            RefreshData();
        }

        public void AddData<T>(T newCellData) where T : CellData
        {
            cellDataList.Add(newCellData);
            RefreshContentSize();

            //위치 재선정//마지막 인덱스로//확인해보기=>//컨텐츠렉트를 움직여야지 나머지는 자동적으로 움직임
            CellUI_Base startCell = cellUIList[0];
            switch (scrollType)
            {
                case EScrollType.Vertical:
                    contentRect.anchoredPosition = new Vector2(0, startCell.GetHeight() * cellCapacity * showCellAmount);
                    break;
                case EScrollType.Horizontal:
                    contentRect.anchoredPosition = new Vector2(startCell.GetWidth() * cellCapacity * showCellAmount, 0);
                    break;
            }

            RefreshData();
        }

        public void ClearCellData()
        {
            //기존거 프리팹 비활성화
            for (int i = 0; i < cellUIList.Count; i++)
            {
                cellUIList[i].gameObject.SetActive(false);
            }
            cellUIList.Clear();
            cellDataList.Clear();
            RefreshContentSize();
        }
        
        /// <summary>
        /// 컨텐츠사이즈를 데이터량에 맞게 변경하는 함수.
        /// </summary>
        private void RefreshContentSize()
        {
            cellCapacity = cellDataList.Count;

            //컨텐츠 길이 세팅//현재상태는 고정만 가능//만약여러개로 한다면//데이터를먼저 작업해야됨
            RectTransform cellRect = cellPrefab.transform as RectTransform;
            float cellHeight = cellRect.sizeDelta.y;
            float cellWidth = cellRect.sizeDelta.x;

            Vector2 size = Vector2.zero;
            bool isOdd = false;
            if (showCellLineAmount != 1)
            {
                isOdd = showCellLineAmount % 2 != 0 ? true : false;
            }
            float addValue = 0;

            switch (scrollType)
            {
                case EScrollType.Vertical:

                    float y = ((cellHeight + spacingValue) * cellCapacity) / showCellLineAmount;
                    //여기서 어덯게 처리해서 고쳐야됨 특정 홀수일때 문제발생되는거//딱맞게 안맞음
                    addValue = isOdd ? /*(cellHeight + spacing) -*/ (cellHeight + spacingValue) : 0;

                    size = new Vector2(cellWidth * showCellLineAmount + spacingValue, y + addValue - spacingValue);
                    break;
                case EScrollType.Horizontal:
                    float x = ((cellWidth + spacingValue) * cellCapacity) / showCellLineAmount;
                    addValue = isOdd ?/* (cellWidth + spacing) -*/ (cellWidth + spacingValue / showCellLineAmount) : 0;

                    size = new Vector2(x + addValue - spacingValue, cellHeight * showCellLineAmount + spacingValue);
                    break;
            }
            contentRect.sizeDelta = size;
        }

        /// <summary>
        /// 현 설정사항에 대한 셀 인덱스를 가져오는 함수
        /// </summary>
        /// <param name="scrollType">스크롤타입</param>
        /// <param name="cellUI">해당되는 cellUI</param>
        /// <returns>셀크기</returns>
        private int GetCalCellIndex(CellUI_Base cellUI)
        {
            switch (scrollType)
            {
                case EScrollType.Vertical:
                    return (int)(contentRect.anchoredPosition.y / (cellUI.GetHeight() + spacingValue));
                case EScrollType.Horizontal:
                    return (int)(contentRect.anchoredPosition.x / (cellUI.GetWidth() + spacingValue)) * -1;//반대방향
            }
            return 0;
        } 

        /// <summary>
        /// 스크롤 업데이트//스크롤이벤트에 등록
        /// </summary>
        /// <param name="temp">좌표</param>
        private void UpdateScroll(Vector2 temp)
        {
            if (cellUIList.Count == 0)
            {
                return;
            }

            //체크할 시작셀과 끝셀
            CellUI_Base startCell = cellUIList[0];

            //인덱스체크
            //현재 움직인 컨텐츠앵커위치 / 방향에따른 높이 or 넓이 + 간격
            currentIndex = GetCalCellIndex(startCell) * showCellLineAmount;
            //print($"{currentIndex}");
            currentIndex = currentIndex < 0 ? 0 : currentIndex;//0이하처리

            //같으면 넘김
            if (lastIndex == currentIndex)
            {
                return;
            }

            //위로가는지 아래로 가는지 체크
            bool isUpScroll = lastIndex > currentIndex ? true : false;
            lastIndex = currentIndex;

            //데이터동기화
            if (isUpScroll)
            {
                //위로 올라가면
                //맨밑에 있는 친구들을 위로 올림
                //Debug.Log($"위로 가는중");

                //라인만큼
                for (int i = 0; i < showCellLineAmount; i++)
                {
                    CellUI_Base endCell = cellUIList[cellUIAmount];

                    //리스트로 인덱스변경
                    //endCell.transform.SetAsFirstSibling();//이거 필요하니?
                    cellUIList.Remove(endCell);
                    cellUIList.Insert(0, endCell);
                }
            }
            else
            {
                //아래로 내려가면
                //맨 위에 있는 친구를 밑으로 내림
                //Debug.Log($"밑으로 가는중");

                //라인만큼
                for (int i = 0; i < showCellLineAmount; i++)
                {
                    startCell = cellUIList[0];

                    cellUIList.Remove(startCell);
                    cellUIList.Add(startCell);
                }
            }

            //위치동기화 + 데이터처리
            RefreshData();
        }


        /// <summary>
        ///UI위치동기화, UI데이터처리하는 함수
        /// </summary>
        private void RefreshData()
        {
            //드래그 길게하면 위치값에 문제가 있어서 별수 없이 이걸로 처리
            //위아래만 이동시키는 최적화 처리하지말기
            
            int curLine = lastIndex / showCellLineAmount;//현재 축이 될 라인줄 체크//추가된라인에서 나눔
            int checkAddLine = 0;//추가되는 라인줄 체크
            //탁 타타탁 으로 변경시키자

            for (int i = 0; i < showCellAmount * showCellLineAmount; i++)
            {
                CellUI_Base cellUI = cellUIList[i];
                int dataIndex = i + lastIndex;//데이터인덱스 == 마지막인덱스 + 반복 횟수               
                
                //각 셀들은 아래방향 * 간격 + (높이 or 넓이) * 셀리스트인덱스 + 마지막인덱스
                if (dataIndex >= cellDataList.Count)
                {
                    //데이터가 없으니//꺼버리기
                    if (cellUI.gameObject.activeSelf)
                    {
                        cellUI.gameObject.SetActive(false);
                    }
                    continue;
                }
                else
                {
                    //데이터가 있으니 보여주기
                    if (!cellUI.gameObject.activeSelf)
                    {
                        cellUI.gameObject.SetActive(true);
                    }

                    //데이터캐싱
                    float cellHeight = cellUI.GetHeight();
                    float cellWidth = cellUI.GetWidth();
                    CellData cellData = cellDataList[dataIndex];
                    Vector2 pos = Vector2.zero;

                    //위치관련
                    switch (scrollType)
                    {
                        case EScrollType.Vertical:
                            //첫위치 기준
                            pos = Vector2.down * (cellHeight + spacingValue) * curLine;


                            //이거 홀수일때 처리하는거 제대로 맞추는거  간단하긴한데 지금은아니다//나중에 시간되면
                            //1일시만 중간
                            if (showCellLineAmount == 1)
                            {
                                pos += Vector2.right * (spacingValue * 0.5f);
                            }

                            //우측으로 늘리는거 체크
                            if (checkAddLine != 0)
                            {
                                //0번아 아니면 우측으로 한개식
                                pos += Vector2.right * (cellWidth + spacingValue) * checkAddLine;
                            }
                            checkAddLine++;

                            //라인에서 보여주는 량과 동일해지면 
                            if (checkAddLine >= showCellLineAmount)
                            {   
                                curLine++;//다음라인으로 진행
                                checkAddLine = 0;//라인에 추가되는 량 체크하는거 리셋
                            }
                            break;
                        case EScrollType.Horizontal:
                            //첫위치 기준
                            pos = Vector2.right * (cellWidth + spacingValue) * curLine;

                            //아래로 늘리는거 체크
                            if (checkAddLine != 0)
                            {
                                //0번이 아니면 아래로 한개식
                                pos += Vector2.down * (cellHeight + spacingValue) * checkAddLine;
                            }
                            checkAddLine++;

                            //라인에서 보여주는 량과 동일해지면 
                            if (checkAddLine >= showCellLineAmount)
                            {
                                curLine++;//다음라인으로 진행
                                checkAddLine = 0;//라인에 추가되는 량 체크하는거 리셋
                            }
                            break;
                    }

                    //넓이,높이 & 최종위치 & 데이터처리
                    cellUI.InitCellUI(cellHeight, cellWidth, pos);
                    cellUI.SetData(cellData);//데이터처리
                }
            }
        }
    }
}


