using UnityEngine;


namespace lLCroweTool
{
    //제약조건이 있는 오브젝트들은 해당 클래스를 참조할것
    [System.Serializable]
    public sealed class ConstraintsCondition
    {  
        public bool useTagCondition;
        /*[HideInInspector]*/[Tag] public string[] interectTags = new string[0];
      
        public bool useLayerCondition;
        /*[HideInInspector] */public LayerMask interectLayer;

        //20220726
        //ContactFilter2D에서 레이어처리를 간단히할수있다
        //근데왜 레이어마스크를 배열로 했지?

        ~ConstraintsCondition()
        {
            interectTags = null;
            //interectLayers = null;
        }
    }
}

