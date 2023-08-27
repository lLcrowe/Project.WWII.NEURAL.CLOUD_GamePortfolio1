using System.Collections.Generic;
using UnityEngine;
using lLCroweTool.AstarPath;


namespace lLCroweTool.NodeMapSystem
{   
    public class CustomNode : MonoBehaviour, IAstarNode, ICustomNode
    {   
        //게임오브젝트//콜라이더로 작동
        public List<CustomNode> connectCustomNodeList = new List<CustomNode>();//연결된 노드들

        public string nodeMechanismID;//노드메커니즘아이디
        public NodeType nodeType;//노드에 따른 작동방식변경

        private Renderer curRenderer;//랜더러
        private Vector3Int[] nearSidePosArray;
        private bool[] nearExistSidePosArray;

        private void Awake()
        {
            curRenderer = GetComponent<Renderer>();

            List<Vector3Int> tempVecList = new List<Vector3Int>();
            List<bool> tempBoolList = new List<bool>();

            for (int i = 0; i < connectCustomNodeList.Count; i++)
            {
                IAstarNode tempNode = connectCustomNodeList[i];
                tempVecList.Add(tempNode.GetNodePos());
                tempBoolList.Add(tempNode.CheckObstacleOrExistObject());
            }

            nearSidePosArray = tempVecList.ToArray();
            nearExistSidePosArray = tempBoolList.ToArray();
        }


        /// <summary>
        /// 연결된 노드들을 가져오는 함수
        /// </summary>
        /// <returns>연결된 노드들</returns>
        public CustomNode[] GetConnectCustomNode()
        {
            return connectCustomNodeList.ToArray();
        }

        public bool CheckObstacleOrExistObject()
        {
            return false;
        }

        public Vector3Int GetNodePos()
        {   
            return Vector3Int.CeilToInt(transform.position);
        }

        public Vector3 GetNodeWorldPos()
        {
            return transform.position;
        }

        public Vector3Int[] GetNearSidePosArray()
        {
            return nearSidePosArray;
        }

        public bool[] GetNearExistSidePosArray()
        {
            return nearExistSidePosArray;
        }

        private void OnDrawGizmos()
        {
            for (int i = 0; i < connectCustomNodeList.Count; i++)
            {
                if (connectCustomNodeList[i] != null)
                {
                    Gizmos.DrawLine(transform.position, connectCustomNodeList[i].transform.position);
                }
            }   
        }

        public Renderer GetNodeRenderer()
        {
            return curRenderer;
        }

        public void SetNodeMat(Material targetMaterial)
        {
            curRenderer.material = targetMaterial;
        }

        public void ActionButton()
        {
            CustomNodeMap.Instance.OnClickNode(this);
        }

        public string GetNodeEventID()
        {
            return nodeMechanismID;
        }

        public NodeType GetNodeType()
        {
            return nodeType;
        }
    }
}