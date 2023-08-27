using System.Collections;
using UnityEngine;

namespace lLCroweTool.ToolTipSystem
{
    public class ToolTipTargetGameObject : MonoBehaviour
    {

        //건축물오브젝트의 정보들을 볼수 있는 HUD
        //스트럭쳐오브젝트에 마우스를 올려두면 보이게할 예정 
        //지금은 안함

        //스트력처의 기능 타입에 대한 아이콘 이미지//UI에 표시용도//상속받은역할에 따라 나누자
        //public Sprite buildingTypeIcon;




        //20220822//체크후 제작
        //UI오브젝트가 아닌 게임오브젝트 위에 마우스포인터를 올려놓으면
        //툴팁이 보이게 하는 오브젝트//트리거로 체크

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }


        //private bool isExistStructureObject;
        //private TestWorldStructureObject targetStructureObject;


        //private bool isExsistDesireSystem;
        //private DesireSystem targetDesireSystem;

        //public void UpdateStuctureWorldObjectUI()
        //{
        //    if (isExistStructureObject)
        //    {

        //    }
        //    if (isExsistDesireSystem)
        //    {

        //    }
        //}

        //public void SetStuctureWorldObjectUI(TestWorldStructureObject testWorldStructureObject)
        //{
        //    if (ReferenceEquals(testWorldStructureObject, null))
        //    {
        //        //존재하지않으면
        //        isExistStructureObject = false;
        //        isExsistDesireSystem = false;
        //    }
        //    else
        //    {
        //        //존재하면
        //        isExistStructureObject = true;
        //    }

        //    if (ReferenceEquals(testWorldStructureObject.unitStatus.unitDesireSystem, null))
        //    {
        //        //존재하지않으면
        //        isExsistDesireSystem = false;
        //    }
        //    else
        //    {
        //        //존재하면
        //        isExsistDesireSystem = true;
        //        targetDesireSystem = testWorldStructureObject.unitStatus.unitDesireSystem;
        //    }
        //}
    }
}