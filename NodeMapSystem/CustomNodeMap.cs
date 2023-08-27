using System.Collections.Generic;
using UnityEngine;
using lLCroweTool.SingletonUI;
using lLCroweTool.AstarPath;
using UnityEngine.UI;
namespace lLCroweTool.NodeMapSystem
{
    /// <summary>
    /// 커스텀노드용 
    /// 랜더링 컴포넌트(필수)
    /// </summary>
    public interface ICustomNode
    {
        /// <summary>
        /// 랜더링을 가져오는 함수//Awake에서 사용
        /// </summary>
        /// <returns>랜더링 컴포넌트</returns>
        Renderer GetNodeRenderer();

        /// <summary>
        /// 새로운 타입을 주는 메터리얼을 세팅해주는 함수
        /// </summary>
        /// <param name="targetMaterial">세팅해줄 메터리얼</param>
        void SetNodeMat(Material targetMaterial);

        /// <summary>
        /// 커스텀노드를 클릭작동되는 기능
        /// </summary>
        void ActionButton();

        /// <summary>
        /// 노드에서 작동시킬 특유이벤트ID를 가져오는 함수
        /// </summary>
        /// <returns>메커니즘ID</returns>
        string GetNodeEventID();

        /// <summary>
        /// 노드타입을 가져오는 함수
        /// </summary>
        /// <returns>노드타입</returns>
        NodeType GetNodeType();
    }

    /// <summary>
    /// 노드타입을 정의
    /// </summary>
    public enum NodeType
    {   
        BattleNode,         //배틀
        BattleBossNode,     //보스배틀
        RandomNode,         //랜덤
        RecorveryNode,      //회복
        SupplyNode,         //보급
        StoreNode,          //상점
        StartNode,          //시작
        EndNode,            //마지막
    }

    public class CustomNodeMap : MonoBehaviourSingletonUIView<CustomNodeMap>
    {
        //노드맵 기능
        //노드들은 ICustomNode IAstarNode 를 상속받아서 데이터를 저장


        //노드 그래프 길찾기는 에이스타로 가능하다고 함

        //중간지점도 중간지점 찍고 해당구역 에이스타 
        public List<ICustomNode> childCustomNodeList = new List<ICustomNode>();

        public int height;//높이
        public int width;//폭

        public ICustomNode curNode;//현재노드지점

        public ScrollRect scroll;



        public Material newColor;
        public Material originColor;

        public IAstarNodeBible astarNodeBible = new IAstarNodeBible();
        public AStarPath aStarPath;


        //길찾기 라인
        protected override void Awake()
        {
            base.Awake();
            List<Transform> tempTrList = new List<Transform>(GetComponentsInChildren<Transform>());

            for (int i = 0; i < tempTrList.Count; i++)
            {
                Transform tempTr = tempTrList[i];
                if (tempTr == transform)
                {
                    continue;
                }

                //에이스타노드
                if (tempTr.TryGetComponent(out IAstarNode iAstarNode))
                {
                    astarNodeBible.Add(iAstarNode.GetNodePos(), iAstarNode);
                }

                //커스텀노드
                if (tempTr.TryGetComponent(out ICustomNode iCustomNode))
                {
                    childCustomNodeList.Add(iCustomNode);
                }
            }

            aStarPath = new AStarPath(astarNodeBible);
        }

        public void InitNodeMap()
        {
            for (int i = 0; i < childCustomNodeList.Count; i++)
            {
                if (childCustomNodeList[i].GetNodeType() == NodeType.StartNode)
                {
                    curNode = childCustomNodeList[i];
                }
            }

            //카메라세팅을 해당구역부터시작
        }

        public void OnClickNode(ICustomNode node)
        {
            //노드를 클릭했을떄 기능
            //노드맵에 따른 작동

            //순서1
            //이동
            //작동

            switch (node.GetNodeType())
            {
                case NodeType.BattleNode:
                    break;
                case NodeType.BattleBossNode:
                    break;
                case NodeType.RandomNode:
                    break;
                case NodeType.RecorveryNode:
                    break;
                case NodeType.SupplyNode:
                    break;
                case NodeType.StoreNode:
                    break;
                case NodeType.StartNode:
                    //시작노드



                    break;
                case NodeType.EndNode:
                    //마지막노드
                    //게임결산

                    break;
            }
        }

        //노드에 해당되는 메커니즘 작동
        public void ActionNodeMechanism(ICustomNode customNode)
        {
            //특정아이디를 통해 작동
            string contentID = customNode.GetNodeEventID();
        }






        //스크롤뷰 체크





        //public void ScrollToTargetElement()
        //{
        //    // Get the index of the target element
        //    int targetIndex = -1;
        //    for (int i = 0; i < scrollRect.content.childCount; i++)
        //    {
        //        if (scrollRect.content.GetChild(i) == targetElement)
        //        {
        //            targetIndex = i;
        //            break;
        //        }
        //    }
        //    if (targetIndex == -1)
        //    {
        //        Debug.LogError("Target element not found in ScrollRect content!");
        //        return;
        //    }

        //    // Get the position of the target element relative to the content
        //    Vector2 targetPosition = targetElement.anchoredPosition;

        //    // Snap the scroll position to the target element
        //    float snapY = 1f / (scrollRect.content.childCount - 1) * targetIndex;
        //    scrollRect.verticalNormalizedPosition = Mathf.Clamp01(1f - snapY);

        //    // You can also scroll to the target position directly with a coroutine
        //    StartCoroutine(ScrollToPosition(targetPosition));
        //}

        //private IEnumerator ScrollToPosition(Vector2 position)
        //{
        //    // calculate the duration of the scroll based on the distance to the target position
        //    float duration = Vector2.Distance(scrollRect.content.anchoredPosition, position) / 500f;
        //    float elapsed = 0f;

        //    while (elapsed < duration)
        //    {
        //        scrollRect.content.anchoredPosition = Vector2.Lerp(scrollRect.content.anchoredPosition, position, elapsed / duration);
        //        elapsed += Time.deltaTime;
        //        yield return null;
        //    }

        //    // make sure the final position is exact
        //    scrollRect.content.anchoredPosition = position;
        //}

    }
}