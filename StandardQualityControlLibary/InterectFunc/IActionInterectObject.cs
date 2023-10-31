using UnityEngine;

namespace lLCroweTool
{
    //인터렉트 오브젝트
    //인터렉트 오브젝트는 체크해야할 충돌체를 가진다
    //트리거 형태로 작동
    //기존의 인터렉트 오브젝트를 보완해서 제작
    //인터렉트오브젝트를 사용하기위한 인터페이스
    /// <summary>
    /// 현 인터페이스는 상호작용이 필요한 클래스가 사용한다
    /// 해당 오브젝트는 콜라이더를 가져야한다.
    /// </summary>
    public interface IActionInterectObject
    {
        /// <summary>
        /// 상호작용하기전에 상호작용이 가능한지 체크해주는 함수(순서1)
        /// </summary>
        /// <param name="targetObject">해당 오브젝트에서 체크해줄수 있는 오브젝트를 집어넣음</param>
        /// <returns>인터렉션 가능한지 여부</returns>
        bool CheckInterectObject(GameObject targetObject);

        /// <summary>
        /// 상호작용에 사용할 함수를 정의(순서2)
        /// </summary>
        /// <param name="targetObject">게임오브젝트를 집어넣고 게임오브젝트에서 컴포넌트 추출해서 작동을 권장</param>    
        void InterectObjectAction(GameObject targetObject);

        /// <summary>
        /// 상호작용할떄 어떠한 행동을 줄것인가에 대한 텍스트
        /// </summary>
        /// <returns>상호작용할때 행동할 텍스트</returns>
        string GetInterectText();
    }
}
