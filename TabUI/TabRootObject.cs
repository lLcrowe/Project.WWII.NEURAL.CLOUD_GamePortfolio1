
using UnityEngine;


namespace lLCroweTool.UI.Tab
{

    public class TabRootObject : MonoBehaviour
    {
        //좀더 이용하기 쉽고 관리하기 쉽게
        //탑다운방식이 가장 편하다
        //현실로 치자면 원하는 문서를 맨위로 올리는것
        //그럼 원리는 간단하네
        //이걸 중앙으로 처리

        //탭루트오브젝트를 파일로 보면
        //탭오브젝트는 종이 한장한장들이다.

        // 계층구조
        // TabRootObject
        //     TabObject
        //       내용물
        //       내용물
        //     TabObject
        //       내용물
        //       내용물
        //     TabObject
        //       내용물
        //     TabObject
        //       내용물

        //이쪽은 사용할 기능이 없는데?
        //그냥 태그기능인가

        private void Awake()
        {

            TabObject[] tabObjectArray = GetComponentsInChildren<TabObject>();




            for (int i = 0; i < tabObjectArray.Length; i++)
            {
                int index = i;
                var tabGroup = tabObjectArray[index];
                tabGroup.Init();
            }
        }
    }
}