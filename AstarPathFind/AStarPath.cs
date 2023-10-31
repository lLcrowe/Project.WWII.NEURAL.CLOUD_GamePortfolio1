using System.Collections.Generic;
using UnityEngine;
using lLCroweTool.Dictionary;
using System.Collections;
using lLCroweTool.PriorityQueue;
using lLCroweTool.ClassObjectPool;

namespace lLCroweTool.AstarPath
{
    public interface IAstarNode
    {
        //유닛이든 건물이든, 배치된 다른오브젝트든 갈수 없는곳은 방해물
        /// <summary>
        /// 장애물존재여부(IAstarNode)
        /// </summary>
        /// <returns>존재여부</returns>
        public bool CheckObstacleOrExistObject();

        /// <summary>
        /// 노드위치 가져오기(IAstarNode)
        /// </summary>
        /// <returns>노드위치</returns>
        public Vector3Int GetNodePos();

        /// <summary>
        /// 노드의 게임상의 월드위치(IAstarNode)
        /// </summary>
        /// <returns>노드의 게임상의 월드위치</returns>
        public Vector3 GetNodeWorldPos();

        /// <summary>
        /// 근처위치 값들을 가져오는 함수(IAstarNode)
        /// </summary>
        /// <returns></returns>
        public Vector3Int[] GetNearSidePosArray();

        /// <summary>
        /// 근처위치가 있는지 값들을 가져오는함수(IAstarNode)
        /// </summary>
        /// <returns></returns>
        public bool[] GetNearExistSidePosArray();
    }

    //노드의 코스트가 들어있는 데이터들
    public class NodeData : CustomClassPoolTarget
    {
        //public NodeData(Vector3Int pos)
        //{
        //    nodePos = pos;
        //    nodeGCost = 0;
        //    nodeHCost = 0;
        //    nodeCalCount = 0;
        //}

        public void Init(Vector3Int pos)
        {
            nodePos = pos;
            nodeGCost = 0;
            nodeHCost = 0;
            nodeCalCount = 0;
            SetIsUse(true);
        }

        //노드에 대한 데이터
        private Vector3Int nodePos;
        public Vector3Int NodePos { get => nodePos; }

        public int nodeGCost;
        public int nodeHCost;
        public int nodeCalCount;
        public int nodeFCost => nodeGCost + nodeHCost;



        public override string ToString()
        {
            string content = $"Pos: {nodePos}\n F: {nodeFCost}\n G:{nodeGCost} + H:{nodeHCost}\n Count: {nodeCalCount}";
            return content;
        }
    }


    [System.Serializable] public class IAstarNodeBible : CustomDictionary<Vector3Int, IAstarNode>{}

    [System.Serializable]
    public class AStarPath
    {
        //에이스타알고리즘//사용//맨허튼사용

        //20230620//문제발생인지
        //클로젯에 문제 있음//오픈 클로즈 구역


        //20230620//길찾기에 문제가있음//공식문제가 아니라//예외문제임 깔
        //야발
        //주변 배열을 앞으로돌리고 뒤로 돌리고
        //주변 배열을 랜덤으로 돌리고
        //체크해보니 바이블에 추가하는거 관련해서 문제가 있어보임

        //20230622
        //한번래핑시켜버림//열린결말 구역제작


        //20230721
        //F가 같으면 H기준으로 상하관계를 체크
        //


        //20230924 
        //오랜만에보니 한번 고쳐야할듯
        //몇개 안좋은게 눈에 뛴다



        //next구역도 체크


        //디버그 구역도 제작


        private IAstarNodeBible hexAreaBible;

        //열린결말//미래는 있지만 결말은 읍어//하나의 길을 따라갔을때 못찾았으면 꺼내서 체크//닫힌결말에 존재하면 안가져옴        
        //큐에 지속적으로 쌓아놓아서 처리할수 있게//주변노드들을 계속탐색할 대상
        //큐에서 집어넣을때와 뺄때 정렬됨
        private CustomPriorityQueue<NodeData> searchNodeQueue = new (new CustomNodeSort(), 300);
        private List<NodeData> openNodeList = new List<NodeData>(100);

        //private Queue<Vector3Int> searchHexNodeQueue = new (30);
        //private Stack<Vector3Int> searchHexNodeQueue = new Stack<Vector3Int>(30);

        
        //private Stack<Vector3Int> openPathStack = new (300);

        //닫힌결말//미래가 읍어//더이상 탐색안함
        private Dictionary<Vector3Int, bool> closePathBible = new (300);
        //private Dictionary<Vector3Int, int> nodeCheckCountBible = new(300);
        public Dictionary<Vector3Int, Vector3Int> parentNodeBible = new();//어디에서 어느위치로 옮겨갔는지
        public Dictionary<Vector3Int, NodeData> costNodeBible = new();

        public class NodeDataPool : CustomClassPool<NodeData> { }

        public NodeDataPool nodeDataPool = new NodeDataPool();
        
        //public Dictionary<Vector3Int, int> nodeGCostBible = new();//위치//G코스트
        //public Dictionary<Vector3Int, int> nodeHCostBible = new();//H휴리스틱코스트

        //최종적으로 반환할 경로//탐색이 끝난 노드들
        //private Dictionary<float, Vector3Int> pathPosBible = new (300);

        //최종적으로 반환할 경로//탐색이 끝난 노드들
        //찾다가 길이 안보이면..이라는건 그냥 검색수로 체크하는건가//여기 있네 문제점



        //요청을 받으면 킵해놓고 차례차례 보내줘야되겠는데
        //


        public enum EHeuristicsType
        {
            Manhattan,
            Diagonal,
            Euclidean,
            Custom,
        }

        //가는데 써지는 비용
        public bool isNodeWorldPos = false;
        public EHeuristicsType heuristicsType;
        public int straightCost = 10;
        public int diagonalCost = 14;//대각//왜 14냐면 //루트 == 1.414, 10 루트 == 14


        public AStarPath(IAstarNodeBible targetHexAreaBible)
        {
            hexAreaBible = targetHexAreaBible;
            heuristicsType = EHeuristicsType.Custom;//현재는 이게 제일 잘됨
        }

        /// <summary>
        /// 길을 찾는 함수
        /// </summary>
        /// <param name="startPos">시작위치</param>
        /// <param name="endPos">끝위치</param>
        /// <param name="pathPosList">돌려줄 경로위치들</param>
        /// <param name="addOnCheckAction">추가적인 체크함수</param>
        /// <param name="isIncludeStartPos">시작위치 포함여부</param>
        /// <returns>길찾기가 탐색됫는지 여부</returns>
        public bool Search(Vector3Int startPos, Vector3Int endPos, ref List<Vector3Int> pathPosList, System.Func<Vector3Int, Vector3Int, bool> addOnCheckAction = null, bool isIncludeStartPos = true)
        {
            //가야될위치와 시작위치가 동일하면
            if (startPos == endPos)
            {
                return false;
            }

            //시작위치가 존재하는지
            if (!hexAreaBible.TryGetValue(startPos,out IAstarNode startNode))
            {
                return false;
            }

            //끝위치가 존재하는지
            if (!hexAreaBible.TryGetValue(endPos, out IAstarNode endNode))
            {
                return false;
            }

            //마지막위치체크//찾지못하면 작동안하고 시작위치를 돌려줌
            if (!CheckNearSideTilePos(startNode, ref endNode))
            {
                return false;
            }

            //정리//초기화
            searchNodeQueue.Clear();
            costNodeBible.Clear();
            nodeDataPool.AllObjectDeActive();
            closePathBible.Clear();
            //openPathStack.Clear();
            //nodeCheckCountBible.Clear();

            parentNodeBible.Clear();
            //nodeGCostBible.Clear();
            //nodeHCostBible.Clear();

            //pathPosBible.Clear();
            pathPosList.Clear();
#if UNITY_EDITOR
            debugPosList.Clear();
#endif

            //Astat 로직작동
            bool check = Logic2(startNode, endNode, ref pathPosList, addOnCheckAction);

            //시작위치를 포함시키겠는가
            if (isIncludeStartPos == false & check)
            {
                pathPosList.RemoveAt(0);//요소삭제가 더빠름
            }
            return check;
        }

        public List<NodeData> GetNodeDataList()
        {
            return new List<NodeData>(costNodeBible.Values);
        }




        //        private bool Logic1(IAstarNode startNode, IAstarNode endNode, ref List<Vector3Int> pathPosList) 
        //        {
        //            //이걸로 제작중
        //            //시작지점 집어넣기
        //            Vector3Int startPos = startNode.GetNodePos();
        //            Vector3 startWorldPos = startNode.GetNodeWorldPos();
        //            searchHexNodeQueue.Enqueue(startPos);
        //            parentNodeBible[startPos] = startPos;
        //            nodeGCostBible[startPos] = 0;

        //            Vector3Int endPos = endNode.GetNodePos();
        //            Vector3 endWorldPos = endNode.GetNodeWorldPos();

        ////#if UNITY_EDITOR
        ////            AddDebugList(0, 0, startPos, parentNodeBible[startPos]);
        ////#endif

        //            do
        //            {
        //                Vector3Int targetCheckPos = searchHexNodeQueue.Dequeue();

        //                if (targetCheckPos == endPos)
        //                {
        //                    //도착
        //                    //부모쫒아가서 시작위치까지 찾기
        //                    do
        //                    {
        //                        pathPosList.Add(targetCheckPos);//마지막 도착길 추가
        //                        targetCheckPos = parentNodeBible[targetCheckPos];//부모찾기//

        //                        //부모노드와 동일하면 시작구역이므로 중지
        //                        if (targetCheckPos == parentNodeBible[targetCheckPos])
        //                        {
        //                            //시작구역까지 추가
        //                            pathPosList.Add(parentNodeBible[targetCheckPos]);

        //                            break;
        //                        }

        //                    } while (true);
        //                    pathPosList.Reverse();//앞뒤 뒤집기

        //                    return true;
        //                }


        //                //헥스타일정보가 있는지 체크
        //                if (!hexAreaBible.TryGetValue(targetCheckPos, out IAstarNode targetCheckNode))
        //                {
        //                    closePathBible.TryAdd(targetCheckPos, false);
        //                    continue;
        //                }

        //                if (closePathBible.ContainsKey(targetCheckPos))
        //                {
        //                    continue;
        //                }

        //                Vector3 targetCheckWorldPos = targetCheckNode.GetNodeWorldPos();
        //                Vector3Int[] nearSidePosArray = targetCheckNode.GetNearSidePosArray();
        //                nodeHCostBible[targetCheckPos] = GetHCost(targetCheckWorldPos, endWorldPos);

        //                //다음꺼랑 비교할려면 GCost를 추가해놔야됨
        //                float targetCheckFCost = nodeGCostBible[targetCheckPos] + nodeHCostBible[targetCheckPos] + straightCost;

        //#if UNITY_EDITOR
        //                AddDebugList(nodeGCostBible[targetCheckPos] + straightCost, nodeHCostBible[targetCheckPos], targetCheckPos, parentNodeBible[targetCheckPos]);
        //#endif          

        //                //int index = -1;
        //                //Vector3Int indexPos = Vector3Int.zero;

        //                for (int i = 0; i < nearSidePosArray.Length; i++)
        //                {
        //                    Vector3Int nearPos = nearSidePosArray[i];

        //                    //닫힌결말인지 체크
        //                    if (closePathBible.ContainsKey(nearPos))
        //                    {
        //                        continue;
        //                    }

        //                    //존재하는지 체크
        //                    if (!hexAreaBible.TryGetValue(nearPos, out IAstarNode nearNode))
        //                    {
        //                        closePathBible.TryAdd(targetCheckPos, false);
        //                        continue;
        //                    }

        //                    //주변 장애물을 체크
        //                    if (nearNode.CheckObstacleOrExistObject())
        //                    {
        //                        //존재하면 닫힌 결말로
        //                        closePathBible.TryAdd(nearPos, false);
        //                        continue;
        //                    }

        //                    //기존 G코스크 체크
        //                    if (!nodeGCostBible.ContainsKey(nearPos))
        //                    {
        //                        //없으면//신규로//GCost추기
        //                        nodeGCostBible[nearPos] = nodeGCostBible[targetCheckPos];
        //                    }
        //                    //있으면//G코스트추가//헥사라서 코스트올라가는 량은 같음
        //                    nodeGCostBible[nearPos] += straightCost;
        //                    nodeHCostBible[nearPos] = GetHCost(nearNode.GetNodeWorldPos(), endWorldPos);
        //                    int newCost = nodeGCostBible[nearPos] + nodeHCostBible[nearPos];

        //                    //작으면 열린노드에 집어넣고, 부모노드기록//<=
        //                    //흠여기가 잘안먹히는데//체크하는 자기자신의 G H체크이랑 비교// 생각해보니 G값이 변해서 좀 문제가 있을수도//체크해보기
        //                    //G값이 문제임 전에 있떤 G값을 가지고 와서 계산하니 문제 생긴것
        //                    //<가 안되는 이유는//전에 체크했던 구역의 G코스트를 가져와서 이제 새롭게 체크하는 G코스트와 동일한 문제때문에 안됫음
        //                    //되게 해결할려면 타겟을 미리 선점후 체크해야됨. //하지만 그럴경우 강제로 선점되기떄문에 주변 장애물이 다있을시 처리가 안됨
        //                    if (newCost <= targetCheckFCost  /*|| index == -1*/)
        //                    {
        //                        //index = i;
        //                        //indexPos = nearPos;
        //                        searchHexNodeQueue.Enqueue(nearPos);
        //                        parentNodeBible[nearPos] = targetCheckPos;//부모지정
        //                    }
        //                }

        //                //searchHexNodeQueue.Enqueue(indexPos);
        //                //parentNodeBible[indexPos] = targetCheckPos;//부모지정

        //                closePathBible.TryAdd(targetCheckPos, false);
        //            } while (searchHexNodeQueue.Count > 0);
        //            return false;
        //        }



        //private bool Logic(IAstarNode startNode, IAstarNode endNode, ref List<Vector3Int> pathPosList)
        //{
        //    //시작지점 집어넣기
        //    Vector3Int startPos = startNode.GetNodePos();

        //    //자동추가
        //    parentNodeBible[startPos] = startPos;//이게 길임
        //    nodeGCostBible[startPos] = 0;//이게 G비용이자//멀리갈수록 높아지는 비용

        //    Vector3Int endPos = endNode.GetNodePos();
        //    Vector3 endWorldPos = endNode.GetNodeWorldPos();



        //    searchHexNodeQueue.Enqueue(startPos);
        //    pathPosBible[0] = startPos;

        //    do
        //    {
        //        //하나씩꺼내먹어요~
        //        Vector3Int targetCheckPos = searchHexNodeQueue.Dequeue();

        //        //도착위치체크
        //        if (targetCheckPos == endPos)
        //        {

        //            pathPosList.AddRange(pathPosBible.Values);

        //            return true;
        //        }

        //        //노드가 있어야됨
        //        if (!hexAreaBible.TryGetValue(targetCheckPos,out IAstarNode targetCheckNode))
        //        {
        //            continue;
        //        }

        //        //주변이웃 체크
        //        Vector3Int[] nearPosArray = targetCheckNode.GetNearSidePosArray();
        //        Vector3 targetCheckWorldPos = targetCheckNode.GetNodeWorldPos();



        //        for (int i = 0; i < nearPosArray.Length; i++)
        //        {
        //            Vector3Int nearPos = nearPosArray[i];

        //            //존재하는지 체크
        //            if (!hexAreaBible.TryGetValue(nearPos, out IAstarNode nearNode))
        //            {
        //                continue;
        //            }
        //            int newCost = (int)(nodeGCostBible[targetCheckPos] + GetHCost(endWorldPos, nearNode.GetNodeWorldPos()));

        //            if (!nodeGCostBible.ContainsKey(nearPos) || newCost <= nodeGCostBible[nearPos])
        //            {
        //                //없으면 추가하고 코스트가 작아도 추가한다
        //                nodeGCostBible[nearPos] = newCost;

        //                searchHexNodeQueue.Enqueue(nearPos);
        //                pathPosBible[newCost] = nearPos;
        //                parentNodeBible[nearPos] = targetCheckPos;
        //            }
        //        }



        //    } while (searchHexNodeQueue.Count > 0);

        //    //못찾음
        //    return false;
        //}












        //20230721//Astar

        //G코스트를 구하는 이유는 또다른이유가 있는게 그게 장애물 때문이다.
        //F값이 같으면 H이 낮은걸로 기준점이 잡힘
        //열린구역을 한번계산후 그대로 둬야됨

        //장애물이 있으면 AStar같은경우 중구난방으로 기준노드가 걸린다.
        //기준노드가 갱신되면 주변노드의 G값을 갱신.

        //닫힌목록 => 기준노드였던것.//계산안됨


        //F 구역 먼저계산 H구역 계산
        //1.시작점과 타겟지점(완)

        //2.현재지점에서 상하좌우 대각선 방향의 이웃노드를 구함.(완)
        //이웃노드를 오픈된 목록에 저장.//문제가 있음. 안에있던 요소를 갱신안하고 넣어버리면 정렬이 안됨
        //이웃노드에 대한 부모를 현재지점 노드로 설정.(완)

        //3.이웃노드의 G코스트값(이것만 구해도 잘가긴하넹), H코스트값을 계산
        //오픈된 목록에 포함된 노드가 아니라면 계산된 값을 저장해주고,
        //오픈된 목록에 포함된 노드라면 현재값과 이전값을 비교해서 현재 계산된값이 더 작다면 이웃노드의 G, F 값을 갱신
        //이웃노드의 부모도 현재 기준노드로 변경

        //4.오픈된 목록에서 가장 작은 F코스트값을 찾아 새로운 기준 노드를 만들고,
        //오픈노드리스트에서 새로운 기준 노드를 삭제

        //5.위와같은 내용을 반복. 새로운 기준노드가 찾고자하는 노드라면 종료

        //위의 내용을 코드로 작성하면 됨

        //내가 문제된구역이 노빠구라 그런듯
        //F코스트가 같을시  휴리스틱비교도 안되있음

        private bool Logic2(IAstarNode startNode, IAstarNode endNode, ref List<Vector3Int> pathPosList, System.Func<Vector3Int, Vector3Int, bool> addOnCheckAction)
        {
            //이걸로 제작중
            //시작지점 집어넣기
            Vector3Int startPos = startNode.GetNodePos();
            Vector3 startWorldPos = startNode.GetNodeWorldPos();

            //노드데이터 초기화
            //NodeData nodeData = new NodeData(startPos);
            NodeData nodeData = nodeDataPool.RequestPrefab();
            nodeData.Init(startPos);

            searchNodeQueue.Enqueue(nodeData);
            costNodeBible.Add(startPos, nodeData);
            parentNodeBible[startPos] = startPos;

            Vector3Int endPos = endNode.GetNodePos();
            Vector3 endWorldPos = endNode.GetNodeWorldPos();

#if UNITY_EDITOR
            //AddDebugList(costNodeBible[targetCheckPos] + straightCost, costNodeBible[targetCheckPos], targetCheckPos, parentNodeBible[targetCheckPos]);
            AddDebugList(nodeData.nodeGCost, nodeData.nodeHCost, startPos, parentNodeBible[startPos]);
#endif

            //최대치 제한걸기
            int limitSearchCount = hexAreaBible.Count / 2;//맵크기 절반만큼 카운트//숫자가 클수록 작아짐
            int cashCount = 0;

            do
            {
                searchNodeQueue.Sort();//이거나중에 뮤조건 고치기//매번이러면 개느림
                nodeData = searchNodeQueue.Dequeue();//우선순위작동방식에 문제가 있는거 같음//
                Vector3Int targetCheckPos = nodeData.NodePos;

                //만약 기존 코스트값에 존재하는게 있으면 교체
                //클래스로 안해놔서 연동된구역들이 분리되있음
                //이걸 안해놓으면 costNodeBible은 갱신된값이 들어가 있고
                //우선순위 큐에는 갱신되기전값이 들어가 있어서 전값을 연산되는 문제점이 있음
                //클래스로 하면 이런문제가 없긴한데 그렇게 할 경우 
                //클래스를 오브젝트폴로 하는게 맞아보임
                //if (costNodeBible.ContainsKey(targetCheckPos))
                //{
                //    nodeData = costNodeBible[targetCheckPos];
                //}

                //도착구역 체크
                if (CheckEndNode(ref targetCheckPos, ref endPos, ref pathPosList, true))
                {
                    return true;
                }

                //헥스타일정보가 있는지 체크//없으면 맵이 없다는것이므로 닫힌 결말
                if (!hexAreaBible.TryGetValue(targetCheckPos, out IAstarNode targetCheckNode))
                {
                    closePathBible.TryAdd(targetCheckPos, false);
                    continue;
                }

                //닫힌 결말인지
                if (closePathBible.ContainsKey(targetCheckPos))
                {
                    continue;
                }
                
                //추가 액션 체크
                if (addOnCheckAction != null)
                {
                    //추가 액션 작동후
                    if (addOnCheckAction.Invoke(targetCheckPos, endPos))
                    {
                        //해당사향에 맞으면 타겟위치를 현재 타겟팅된 애로 고치고 넘겨버림 체크
                        return CheckEndNode(ref targetCheckPos, ref targetCheckPos, ref pathPosList, true);
                    }
                }


                Vector3 targetCheckWorldPos = targetCheckNode.GetNodeWorldPos();
                Vector3Int[] nearSidePosArray = targetCheckNode.GetNearSidePosArray();

                //휴리스틱 갱신
                if (isNodeWorldPos)
                {
                    nodeData.nodeHCost = GetHCost(targetCheckWorldPos, endWorldPos);
                }
                else
                {
                    nodeData.nodeHCost = GetHCost(targetCheckNode.GetNodePos(), endPos);
                }
                //costNodeBible[targetCheckPos] = nodeData;

#if UNITY_EDITOR
                //AddDebugList(costNodeBible[targetCheckPos] + straightCost, costNodeBible[targetCheckPos], targetCheckPos, parentNodeBible[targetCheckPos]);
                AddDebugList(costNodeBible[targetCheckPos].nodeGCost, costNodeBible[targetCheckPos].nodeHCost, targetCheckPos, parentNodeBible[targetCheckPos]);
#endif

              

                for (int i = 0; i < nearSidePosArray.Length; i++)
                {
                    Vector3Int nearPos = nearSidePosArray[i];


                    //닫힌결말인지 체크
                    if (closePathBible.ContainsKey(nearPos))
                    {
                        continue;
                    }

                    //헥스타일정보가 있는지 체크//없으면 맵이 없다는것이므로 닫힌 결말
                    if (!hexAreaBible.TryGetValue(nearPos, out IAstarNode nearNode))
                    {
                        closePathBible.TryAdd(targetCheckPos, false);
                        continue;
                    }

                    //주변 장애물을 체크
                    if (nearNode.CheckObstacleOrExistObject())
                    {
                        //도착구역 체크
                        if (CheckEndNode(ref nearPos, ref endPos, ref pathPosList, false))
                        {
                            return true;
                        }
                        continue;
                    }

                    //코스트노드바이블에 없으면 추가처리
                    if (!costNodeBible.ContainsKey(nearPos))
                    {
                        //없으면//신규로
                        //NodeData tempData = new NodeData(nearPos);
                        NodeData tempData = nodeDataPool.RequestPrefab();
                        tempData.Init(nearPos);
                        //tempData.nodeGCost = costNodeBible[targetCheckPos].nodeGCost;
                        costNodeBible[nearPos] = tempData;
                    }


                    //G코스트와 휴리스틱 갱신
                    nodeData = costNodeBible[nearPos];
                    nodeData.nodeGCost = costNodeBible[targetCheckPos].nodeGCost + straightCost;//헥사라서 코스트올라가는 량은 같음
                    nodeData.nodeCalCount++;

                    if (isNodeWorldPos)
                    {
                        nodeData.nodeHCost = GetHCost(nearNode.GetNodeWorldPos(), endWorldPos);
                    }
                    else
                    {
                        nodeData.nodeHCost = GetHCost(nearNode.GetNodePos(), endPos);
                    }
                                        
                    //costNodeBible[nearPos] = nodeData;//갱신

                    //갱신후에 큐에 집어넣기//큐에서 집어넣을때와 뺄때 정렬됨
                    searchNodeQueue.Enqueue(nodeData);
                    parentNodeBible[nearPos] = targetCheckPos;//부모지정
                }

                //기준이 된 노드는 닫힌 결말로
                closePathBible.TryAdd(targetCheckPos, false);

                //제한치체크
                if (cashCount > limitSearchCount)
                {
                    break;
                }

                cashCount++;
            } while (searchNodeQueue.Count > 0);

            
            return false;
        }


        /// <summary>
        /// 도착할 위치인지 체크하는 함수
        /// </summary>
        /// <param name="targetCheckPos">체크할 위치</param>
        /// <param name="endPos">도착할 위치</param>
        /// <param name="pathPosList">길을 적재할 리스트</param>
        /// <param name="isIncludeEndPos">마지막위치를 포함하는가</param>
        /// <returns>도착할 위치를 찾은 여부</returns>
        private bool CheckEndNode(ref Vector3Int targetCheckPos, ref Vector3Int endPos, ref List<Vector3Int> pathPosList, bool isIncludeEndPos)
        {
            if (targetCheckPos == endPos)
            {
                //도착
                //부모쫒아가서 시작위치까지 찾기
                do
                {
                    pathPosList.Add(targetCheckPos);//마지막 도착길 추가
                    targetCheckPos = parentNodeBible[targetCheckPos];//부모찾기//

                    //부모노드와 동일하면 시작구역이므로 중지
                    if (targetCheckPos == parentNodeBible[targetCheckPos])
                    {
                        //시작구역까지 추가
                        pathPosList.Add(parentNodeBible[targetCheckPos]);
                        break;
                    }

                } while (true);
                pathPosList.Reverse();//앞뒤 뒤집기
                                      
                if (!isIncludeEndPos)
                {
                    //마지막위치가 도착위치
                    //포함하지 않을시 마지막도착길 삭제
                    pathPosList.RemoveAt(pathPosList.Count - 1);
                }

                return true;
            }
            return false;
        }





        /// <summary>
        /// 끝위치가 비어있는지 체크하고 안비어있으면 근처노드에서 비어있고 가장가까운위치를 가져와서 세팅해주는 함수
        /// </summary>
        /// <param name="startNode">시작노드</param>
        /// <param name="endNode">끝노드</param>
        /// <param name="endPos">끝위치</param>
        /// <returns>결과가 나왔는지 체크여부</returns>
        private bool CheckNearSideTilePos(IAstarNode startNode, ref IAstarNode endNode)
        {
            //끝지점에 장애물이 없으면 그대로 돌려줌
            //아닐시 주변타일들을 탐색후 최단거리를 돌려줌
            if (!endNode.CheckObstacleOrExistObject())
            {
                return true;
            }

            Vector3Int[] posArray = endNode.GetNearSidePosArray();
            Vector3 startWorldPos = startNode.GetNodeWorldPos();


            IAstarNode minNode = endNode;//제일먼것을 최소노드로 진행후 차차 좁힘
            float minDistance = Vector3.Distance(startWorldPos, minNode.GetNodeWorldPos());

            for (int i = 0; i < posArray.Length; i++)
            {
                int index = i;
                //위치가 존재하는지
                if (!hexAreaBible.TryGetValue(posArray[index], out IAstarNode nearNode))
                {
                    continue;
                }

                //갈수 있는지
                if (nearNode.CheckObstacleOrExistObject())
                {
                    continue;
                }

                //거리체크
                float compareDistance = Vector3.Distance(startWorldPos, nearNode.GetNodeWorldPos());
                if (compareDistance < minDistance)
                {
                    minNode = nearNode;
                    minDistance = compareDistance;
                }
            }

            //만약 갈수 있는곳을 못찾았으면//시작위치를 돌려주기
            if (minNode == endNode)
            {   
                return false;
            }
            endNode = minNode;
            return true;
        }

        //경로 채점 (Path Scoring)
        //f = g + h

        //1. g => 시작점 a로부터 현재사격형까지의 경로를 따라 이동하는데 소요되는 비용
        //현재 상태의 비용
        //g는 움직인 거리
        //신규면그대로 집어넣으면됨
        //나로부터 있는걸 현재시점의 기준노드로 부터 갱신




        //2. h => 현재 위치에서 목적지 B까지의 예상 이동 비용
        //사이에 벽, 물 등으로 인해 실제 거리는 알지 못합니다.
        //그들을 무시하고 예상 거리를 산출합니다.
        //여러 방법이 있지만, 이 포스팅에서는 대각선 이동을 생각하지 않고, 가로 또는 세로로 이동하는 비용만 계산합니다.
        //사람이 미리 설정하는 코스트를 휴리스틱 코스트(Heuristic cost)

        //3. f => 현재까지 이동하는데 걸린 예상비용과 예상비용을 합친 총 비용
        //f = g + h 를 더한값으로 채점값으로 정의됨


        //Admissible heuristic
        //h = 절대값 (수평(x) - 수직(y)) + 10(이동,) //휴리스틱 함수 따라 성능이 극명하게 나누니 알아보기



        //가장작은 f비용을 가져옴
        //같은게 있으면 맘대루
        //어느 것이든 상관 없지만 마지막 추가한 노드를 탐색하는것이 빠름//...? 의미있나

        //float로 처리해되될거 같기도하구

      

       
        //스트레이트 비용


        //private int GetHCost(Vector3 checkWorldPos, Vector3 endWorldPos)
        //{
        //    return Mathf.RoundToInt(lLcroweUtil.GetDistance(checkWorldPos, endWorldPos));
        //}


        /// <summary>
        /// 휴리스틱을 구하는 함수
        /// </summary>
        /// <param name="checkWorldPos">체크하는 월드위치</param>
        /// <param name="endWorldPos">마지막 월드위치</param>
        /// <returns></returns>
        private int GetHCost(Vector3 checkWorldPos, Vector3 endWorldPos)
        {
            //휴리스틱은 여러가지 형태가 있다
            //크게 세가지 휴리스틱을 사용함
            //1. 맨하튼 방식
            //2. 유클리드, 다이어그아 방식 (같은거. 둘다 거리를 구하는 방식임. 하나는 정확하게, 나머지하나는 거리(길이)만)
            //3. 옥타일 방식
            float tempValue = 0;
            switch (heuristicsType)
            {
                case EHeuristicsType.Manhattan:
                    //1. 맨하든 거리 ㄴ형태
                    tempValue = Mathf.Abs(checkWorldPos.x - endWorldPos.x) + Mathf.Abs(checkWorldPos.y - endWorldPos.y);
                    break;
                case EHeuristicsType.Diagonal:
                    //2. 다이아그아 거리 /형태
                    //2-1
                    float distanceX = Mathf.Abs(checkWorldPos.x - endWorldPos.x);
                    float distanceY = Mathf.Abs(checkWorldPos.y - endWorldPos.y);
                    float reming = Mathf.Abs(distanceX - distanceY);
                    tempValue = diagonalCost * Mathf.Min(distanceX, distanceY) + straightCost * reming;//사각타일
                    //tempValue = straightCost * Mathf.Min(distanceX, distanceY) + straightCost * reming;//육각타일
                    break;
                case EHeuristicsType.Euclidean:
                    //3. 유클리드 거리 ㄱ형태
                    //3-1
                    //tempValue = Mathf.Sqrt(Mathf.Abs(checkWorldPos.x - endWorldPos.x) * 2 + Mathf.Abs(checkWorldPos.y - endWorldPos.y) * 2);
                    //3-2
                    tempValue = Mathf.Sqrt(Mathf.Pow(checkWorldPos.x - endWorldPos.x,2) + Mathf.Pow(checkWorldPos.y - endWorldPos.y,2));
                    break;
                case EHeuristicsType.Custom:
                    //4. 커스텀. 대충 위치에 대한 거리를 계산해서 던져주는게 났지않을까
                    Vector3 dir = endWorldPos - checkWorldPos;
                    tempValue = dir.sqrMagnitude;
                    break;
            }

            return Mathf.RoundToInt(tempValue);
        }





        /// <summary>
        /// 커스텀노드데이터 비교자
        /// </summary>
        public struct CustomNodeSort : IComparer<NodeData>
        {   
            public int Compare(NodeData leftNode, NodeData rightNode)
            {
                //F코스트를 먼저비교
                if (leftNode.nodeFCost < rightNode.nodeFCost)
                {
                    return -1;
                }
                else if (leftNode.nodeFCost > rightNode.nodeFCost)
                {
                    return 1;
                }
                else
                {
                    //F코스트가 동일할때는 휴리스틱 코스트로 비교
                    if (leftNode.nodeHCost < rightNode.nodeHCost)
                    {
                        return -1;
                    }
                    else if (leftNode.nodeHCost > rightNode.nodeHCost)
                    {
                        return 1;
                    }
                }
                return 0;
            }
        }

#if UNITY_EDITOR
        //디버그 구역

        //작동순서들을 다 집어넣고 싶은데
        //디버그용
        [Header("-디버그 기즈모-")]
        public bool isUseDebugGizmo = false;
        public List<DebugNodeData> debugPosList = new List<DebugNodeData>();
        public float gizmoSize = 0.5f;
        public Vector3 gizmoOffSet;
        [Min(-1)] public int debugListCount;

        //디버그리스트에 등록
        private void AddDebugList(int g, float h, Vector3Int pos, Vector3Int parentPos)
        {
            if (!isUseDebugGizmo)
            {
                return;
            }
            DebugNodeData nodeData = new DebugNodeData();
            nodeData.Set(g, h, pos, parentPos);
            debugPosList.Add(nodeData);
        }


        public void DebugGizmo()
        {
            if (!isUseDebugGizmo)
            {
                return;
            }
            //닫힌결말보기
            foreach (var item in closePathBible)
            {
                if (hexAreaBible.TryGetValue(item.Key, out IAstarNode astarNode))
                {
                    Vector3 worldPos = astarNode.GetNodeWorldPos() + gizmoOffSet;
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(worldPos, gizmoSize * 0.5f);
                }
            }

            //경로보기//부모
            //foreach (var item in parentNodeBible)
            //{
            //    if (!hexAreaBible.TryGetValue(item.Key, out IAstarNode parentNode))
            //    {
            //        continue;
            //    }
            //    if (!hexAreaBible.TryGetValue(item.Value, out IAstarNode childNode))
            //    {
            //        continue;
            //    }


            //    Vector3 parentNodePos = parentNode.GetNodeWorldPos() + gizmoOffSet;
            //    Vector3 childNodePos = childNode.GetNodeWorldPos() + gizmoOffSet;


            //    //Gizmos.color = Color.blue;
            //    //Gizmos.DrawWireSphere(parentNodePos, gizmoSize * 0.8f);
            //    //Gizmos.color = Color.cyan;
            //    //Gizmos.DrawWireSphere(childNodePos, gizmoSize * 0.9f);
                
            //    DrawArrow(parentNodePos, childNodePos, Color.yellow, Color.red);
            //}


            //타일들 계산된 G
            //foreach (var item in nodeGCostBible)
            //{
            //    if (!hexAreaBible.TryGetValue(item.Key, out IAstarNode countNode))
            //    {
            //        continue;
            //    }
            //    Gizmos.color = Color.cyan;
            //    Gizmos.DrawWireSphere(countNode.GetNodeWorldPos() + gizmoOffSet,gizmoSize * 0.7f);

            //    GUIStyle style = new GUIStyle();
            //    style.normal.textColor = Color.white;
            //    UnityEditor.Handles.Label(countNode.GetNodeWorldPos() + gizmoOffSet, $"좌표 : {item.Key.x}, {item.Key.y} G : {item.Value} H : {nodeHCostBible[item.Key]}", style);
            //}

            

            //진행도 체크
            if (debugPosList.Count == 0 || debugListCount < 0)
            {
                return;
            }
            if (debugListCount > debugPosList.Count - 1)
            {
                debugListCount = debugPosList.Count - 1;
                return;
            }

            DebugNodeData nodeData = debugPosList[debugListCount];

            //진행이 어덯게 됫는지 체크
            if (hexAreaBible.TryGetValue(nodeData.nodePos, out IAstarNode targetNode))
            {
                //진행목표//블루
                Vector3 worldPos = targetNode.GetNodeWorldPos() + gizmoOffSet;
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(worldPos, gizmoSize * 1.3f);

                //주변타일//레드
                var tempArray = targetNode.GetNearSidePosArray();
                for (int i = 0; i < tempArray.Length; i++)
                {
                    var temp = tempArray[i];
                    if (hexAreaBible.TryGetValue(temp, out IAstarNode tempNode))
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawSphere(tempNode.GetNodeWorldPos() + gizmoOffSet, gizmoSize * 1.5f);
                    }
                }
                string content = nodeData.ToString();
                UnityEditor.Handles.Label(worldPos + gizmoOffSet + Vector3.up * gizmoSize, content);

                //부모//노란
                if (hexAreaBible.TryGetValue(nodeData.parentNodePos, out IAstarNode parentNode))
                {
                    worldPos = parentNode.GetNodeWorldPos() + gizmoOffSet;
                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(worldPos, gizmoSize * 1.0f);
                }
            }
        }


        public void DrawArrow(Vector3 pos, Vector3 endPos, Color lineColor, Color arrowColor, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            if (pos == endPos)
            {
                return;
            }

            Gizmos.color = lineColor;
            Gizmos.DrawLine(pos, endPos);

            Gizmos.color = arrowColor;
            Vector3 right = Quaternion.LookRotation(endPos) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(endPos) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(endPos, right * arrowHeadLength);
            Gizmos.DrawRay(endPos, left * arrowHeadLength);
            Gizmos.DrawLine(endPos + right * arrowHeadLength, endPos + left * arrowHeadLength);
        }


        //이건 디버그용.
        //전에 
        [System.Serializable]
        public struct DebugNodeData
        {
            public Vector3Int nodePos;
            public float f;
            public int g;
            public float h;
            public Vector3Int parentNodePos;

            public void Set(int g, float h, Vector3Int nodePos, Vector3Int parentPos)
            {
                this.g = g;
                this.h = h;
                f = g + h;
                this.nodePos = nodePos;
                parentNodePos = parentPos;
            }

            public override string ToString()
            {
                return $"{f}={g}+{h}\n {parentNodePos}";
            }
        }
#endif
    }



    //테스트용

    //JSP체크
    //각 Grid의 충돌정보를 Bit형태로 저장하고
    //Bit Scan을 통해 빠르게 충돌위치정보를 검출해내자
    //라는 알고리즘

    //64bit 변수단위로 각Grid의 충돌정보를 Bit형태로 저장을 하게되면
    //1024 Grid는  1024 / 64 = 16 
    //그래서 1024 * 1024 = 1048576 계산을
    //16 * 16 = 256 만으로 계산가능
    //엄청난차이를 가짐

    //64비트의 저장된 변수에서의 충돌위치비트검출은
    //FFS(FindFirstSet) 같은 CPU가속을 받는 함수들로
    //순식간에 충돌정보를 BitScanning한다고 논문은 말해준다

    //이런걸 적용해서 구현해보니 평균연산시간 1ms 이내의 안전된 결과가 나오게 되었습니다.
    //이제 전쟁게임에서 길찾기 알고리즘은 JSP (B) 다 라고 말할수 있다



    public class JSP_B
    {




        //0000 0000  8bit

        //0000 0001 == 1 == 0x01
        //0000 0010 == 2 == 0x02
        //0000 0011 == 3 == 0x03
        //1111 1111 == 255 == 0xff

        //육각타일인거 체크
        //어덯게 아이덴디티를 가질까


        //1급=>
        //임의다변수함수는 1변수함수 콤비네이션=>

        public const byte ddd = 0x02;

        public enum ItemType
        {
            SWORD = 1,
            RIFLE = 2,
            BIGSWORD = 4
        }
        ItemType haveItems = ItemType.SWORD;

        
        public enum AnimationType
        {
            IDLE = 1,
            PATROL = 2,
            TRACE = 4,
            ATTACK = 8
        }


        byte[] data = new byte[]
        {
            0x00,//rightUp
            0x01,//leftDown
            0x02,//right
            0x03,//left
            0x04,//rightDown
            0x05,//leftUp
            0x06,

            0x07,//
            0x08,//
        };

        BitArray bitArray = new BitArray(4);


        public void Test()
        {
            int temp = data[0];
            if( (haveItems & ItemType.RIFLE) == ItemType.RIFLE)
            {

            }
        }


        //int로 연산하자
        //연산 최적화면 int
        //int가 표준인 이유가 int 연산이 가장 빠름
        //운영체제에 맞는 길이 
        //이거보다 길거나 짧으면
        //늘리거나 자르는데 연산이 들어김
        //그래서 int를 운영체제에 맞춰서 설정



        //8byte체계로 구성되있다.
        //8*8 = 64 bit
        //16 15 14 13  12  11   10   9   8  // 7  6	 5	 4	 3	 2	 1	 0
        //            4096 2047 1024 512 256//128 64 32	 16  8   4   2   1
        //									//최대 255

        //다시 말하면 환경에 따른 포인터에 대한 메모리최대치가 변경된다 이런건가
        //8btye == 64bit 체계
        //4byte == 32bit 체계	 
        // 
        //1byte == 8bit 
        // 이 8bit가 여러개 묶인걸 말하는거면 맞다
        // 그럼뭉크말하고 섞어보면 그냥 인트에서 처리하는게 맞고
        // 그상태로 비트연산하는게 더 빠르게 처리가 가능함
        // 그럼맵을 bit로 바꿔야되는데.
        // 1024라는 정형화된 수가 아닐경우 How라는 문제가 발생함
        // int가 64비트인지 32비트인 체계인지에 따라 다름
        // 그래도 넘쳐난다. 충분함

        //int temp = 0;
        //temp = 0x100;//이렇게 되넹//그럼 지정은 알겠고
        //temp = temp >> 1;//128//변경은 이렇게	
        //말그대로 비트를 뭘로 기준점을 잡고지정하느냐에 다르네



        public JSP_B(int tileAmount, IAstarNodeBible targetHexAreaBible)
        {
           
        }

        public bool Search(Vector3Int startPos, Vector3Int endPos, ref List<Vector3Int> pathPosList)
        {

            float speed = 1;
            float deltaTime = 0.01f;
            Vector2 dir = new Vector2(1, 0);

            Vector2 result = speed * deltaTime * dir;//4 8


            return false;
        }

        //int FindPostion(int num)
        //{
        //    while (num)
        //    {

        //    }
        //}



        //a.collision = collision1 | collision2 | collision3 | collision4;
        ////0b001111 == 0b000001 | 0b000010 | 0b000100 | 0b001000

        //#define way1 ~collision1
        ////0b111110 == ~0b000001

        //if ((a.collision | way) == way)
        ////0b001111 | 0b111110 == 0b111111
        //0b111111 != 0b111110 (false)


        void main()
        {
            int collision = Collision(0) | Collision(1) | Collision(2) | Collision(3);

            CollisionTest(collision, ~Collision(0));
            CollisionTest(collision, ~Collision(1));
            CollisionTest(collision, ~Collision(2));
            CollisionTest(collision, ~Collision(3));
            CollisionTest(collision, ~Collision(4));
            CollisionTest(collision, ~Collision(5));

        }

        private void CollisionTest(int c, int i)
        {
            if ((c | i) == i)
            {
                Debug.Log($"True: {i}");
            }
        }

        private int Collision(int index)
        {
            return 1 << index;
        }
    }

    public class Node
    {
        public int X;
        public int Y;
        public int GCost; // g-value (cost from start to this node)
        public int HCost; // h-value (heuristic cost from this node to the goal)

        public bool IsWall;

        public int FCost
        {
            get
            {
                return GCost + HCost;
            }
        } // f-value (G + H)
        public Node Parent;

        public Node(int x, int y)
        {
            X = x;
            Y = y;
            GCost = 0;
            HCost = 0;
            Parent = null;
        }
    }

    public class JumpPointSearch
    {
        private int[][] grid;
        private int gridSizeX;
        private int gridSizeY;
        private List<Node> openSet;
        private HashSet<Node> closedSet;

        public JumpPointSearch(int[][] grid)
        {
            this.grid = grid;
            gridSizeX = grid.Length;
            gridSizeY = grid[0].Length;
            openSet = new List<Node>();
            closedSet = new HashSet<Node>();
        }

        public List<Node> FindPath(Node startNode, Node goalNode)
        {
            openSet.Clear();
            closedSet.Clear();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = GetLowestFNode();
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == goalNode)
                {
                    return ReconstructPath(currentNode);
                }

                List<Node> successors = IdentifySuccessors(currentNode, goalNode);
                foreach (Node successor in successors)
                {
                    if (closedSet.Contains(successor))
                    {
                        continue;
                    }

                    int tentativeG = currentNode.GCost + GetDistance(currentNode, successor);
                    bool isBetterPath = false;

                    if (!openSet.Contains(successor))
                    {
                        openSet.Add(successor);
                        isBetterPath = true;
                    }
                    else if (tentativeG < successor.GCost)
                    {
                        isBetterPath = true;
                    }

                    if (isBetterPath)
                    {
                        successor.Parent = currentNode;
                        successor.GCost = tentativeG;
                        successor.HCost = GetDistance(successor, goalNode);
                    }
                }
            }

            // Path not found
            return null;
        }

        private List<Node> ReconstructPath(Node node)
        {
            List<Node> path = new List<Node>();
            Node current = node;

            while (current != null)
            {
                path.Insert(0, current);
                current = current.Parent;
            }

            return path;
        }

        private Node GetLowestFNode()
        {
            Node lowestFNode = openSet[0];

            foreach (Node node in openSet)
            {
                if (node.FCost < lowestFNode.FCost)
                {
                    lowestFNode = node;
                }
            }

            return lowestFNode;
        }

        private List<Node> IdentifySuccessors(Node currentNode, Node goalNode)
        {
            List<Node> successors = new List<Node>();

            // TODO: Implement Jump Point Search identification of successors

            return successors;
        }

        private int GetDistance(Node nodeA, Node nodeB)
        {
            int distX = Mathf.Abs(nodeA.X - nodeB.X);
            int distY = Mathf.Abs(nodeA.Y - nodeB.Y);

            // We can use Manhattan distance as the heuristic for grid-based maps
            return distX + distY;
        }
    }
}