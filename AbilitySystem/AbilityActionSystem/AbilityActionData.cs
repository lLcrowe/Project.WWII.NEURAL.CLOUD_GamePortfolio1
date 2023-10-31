using System.Collections.Generic;
using UnityEngine;

namespace lLCroweTool.Ability
{
    //능력과 1대1 연동
    [System.Serializable]
    public class AbilityActionData
    {
        public AbilityInfo abilityInfo;

        [Header("타겟이 될 오브젝트or위치")]//다시 생각해볼것//단일로 하자
        public List<UnitObject_Base> targetWorldObjectList = new List<UnitObject_Base>();//프라이비트 예정//스킬과 연동한 특정한 위치나 오브젝트
        public List<Vector2> targetPosList = new List<Vector2>();


        //작동되고 있는 작동행위정보
        public AbilityActionInfo actionBehaviorInfo;// 배열로되있는거에 대한 인덱스로 가져오기
        public int actionBehaviorIndex = 0;



        public void InitAbilityData(AbilityInfo abilityInfo)
        {

        }


    }
}
