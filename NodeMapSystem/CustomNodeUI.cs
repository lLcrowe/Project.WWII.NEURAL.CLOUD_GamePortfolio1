using lLCroweTool.AstarPath;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace lLCroweTool.NodeMapSystem
{
    public class CustomNodeUI : MonoBehaviour, IAstarNode, ICustomNode
    {
        //UI에서 작동
        public List<CustomNodeUI> connectCustomUINodeList = new List<CustomNodeUI>();//연결된 노드들

        public string nodeMechanismID;//노드메커니즘아이디
        public NodeType nodeType;//노드에 따른 작동방식변경

        private Renderer curRenderer;//랜더러
        private Button button;
        private Vector3Int[] nearSidePosArray;
        private bool[] nearExistSidePosArray;

        private void Awake()
        {
            curRenderer = GetNodeRenderer();
            button = GetComponent<Button>();
            button.onClick.AddListener(ActionButton);

            List<Vector3Int> tempVecList = new List<Vector3Int>();
            List<bool> tempBoolList = new List<bool>();

            for (int i = 0; i < connectCustomUINodeList.Count; i++)
            {
                IAstarNode tempNode = connectCustomUINodeList[i];
                tempVecList.Add(tempNode.GetNodePos());
                tempBoolList.Add(tempNode.CheckObstacleOrExistObject());
            }

            nearSidePosArray = tempVecList.ToArray();
            nearExistSidePosArray = tempBoolList.ToArray();
        }

        public void ActionButton()
        {
            CustomNodeMap.Instance.OnClickNode(this);
        }

        public bool CheckObstacleOrExistObject()
        {
            return false;
        }

        public bool[] GetNearExistSidePosArray()
        {
            return nearExistSidePosArray;
        }

        public Vector3Int[] GetNearSidePosArray()
        {
            return nearSidePosArray;
        }

        public string GetNodeEventID()
        {
            return nodeMechanismID;
        }

        public Vector3Int GetNodePos()
        {
            return Vector3Int.CeilToInt(transform.position);
        }

        public NodeType GetNodeType()
        {
            return nodeType;
        }

        public Vector3 GetNodeWorldPos()
        {
            return transform.position;
        }
        private void OnDrawGizmos()
        {
            for (int i = 0; i < connectCustomUINodeList.Count; i++)
            {
                if (connectCustomUINodeList[i] != null)
                {
                    Gizmos.DrawLine(transform.position, connectCustomUINodeList[i].transform.position);
                }
            }
        }

        public Renderer GetNodeRenderer()
        {
            return GetComponent<Renderer>();
        }

        public void SetNodeMat(Material targetMaterial)
        {
            curRenderer.material = targetMaterial;
        }
    }
}