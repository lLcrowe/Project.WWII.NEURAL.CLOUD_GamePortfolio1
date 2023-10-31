using UnityEngine;

namespace lLCroweTool
{
    public class PlayerUIDistanceCheckModule : MonoBehaviour
    {
        //플레이어가 특정오브젝트에 상호작용했을시
        //특정오브젝트가 가진 UI에서
        //플레이어캐릭터가 일정거리 이상일시 UI가 꺼지는 기능을 가짐
        //UI컴포넌트에서 컴포넌트모듈로서 사용

        //다른 UI컴포넌트에서 현컴포넌트를 참조하여 체크
        //현컴포넌트는 UI객체에 붙착

        public float checkDistince = 0.5f;//확인할 거리
        private bool isExistUnitObject = false;
        private Transform playerCharecterTransform;

        private Transform targetUIInterectObject;//상호작용하여 UI가 켜진 오브젝트

        /// <summary>
        /// 플레이어와 거리체크를 위해 세팅해주는 함수
        /// </summary>
        /// <param name="playerUnitObject">플레이어캐릭터 트랜스폼</param>
        /// <param name="targetInterectObject">UI가 켜지는 오브젝트</param>
        public void InitUIDistanceChecker(Transform playerUnitObject, Transform targetInterectObject)
        {
            playerCharecterTransform = playerUnitObject;
            isExistUnitObject = !ReferenceEquals(playerCharecterTransform, null);
            targetUIInterectObject = targetInterectObject;
        }

        /// <summary>
        /// 거리를 확인해주는 함수
        /// </summary>
        /// <returns>거리가 넘는지?</returns>
        public bool UpdateCheckDistance()
        {
            if (!isExistUnitObject)
            {
                return false; 
            }
            
            bool isDone = !lLcroweUtil.CheckDistance(playerCharecterTransform.position, targetUIInterectObject.position, checkDistince);
            return isDone;
        }

        /// <summary>
        /// 유닛 존재여부를 가져오는함수
        /// </summary>
        /// <returns>유닛존재여부</returns>
        public bool GetIsExistUnitObject()
        {
            return isExistUnitObject;
        }
    }
}