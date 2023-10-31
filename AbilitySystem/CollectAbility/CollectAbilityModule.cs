using lLCroweTool.Dictionary;
using System.Collections.Generic;
using UnityEngine;

namespace lLCroweTool.Ability.Collect
{
    /// <summary>
    /// 세트능력들의 데이터//세트능력을 작용시키게 하고 싶으면 필요한 모듈
    /// </summary>
    [System.Serializable]
    public class CollectAbilityModule
    {
        private static AbilityInfo[] refAbilityArray;//참조용

        /// <summary>
        /// 세트능력정보와 모어논 세트부품정보 바이블
        /// </summary>
        //세트능력정보//타겟팅한 세트능력의 세트부품들
        //몇개를 모앗을시 특정 능력이 발동되게//여기안에 요소가 몇개 있으면 작동되는것들이 존재
        //세트정보들은 중복이 안되므로 바이블로 처리
        [System.Serializable]
        public class CollectAbilityBible : CustomDictionary<CollectAbilityInfo, List<CollectPartInfo_Base>> { }

        //세트아이템 능력처리//관리////UI작동할때도 필요//어차피능력쪽에서 UI로 볼텐데 구지 이걸?
        public CollectAbilityBible collectAbilityBible = new CollectAbilityBible();

        /// <summary>
        /// 세트능력 부속품추가
        /// </summary>
        /// <param name="collectPartInfo">콜렉션파츠 정보</param>
        public void AddCollectElement(CollectPartInfo_Base collectPartInfo)
        {
            if (collectPartInfo == null)
            {
                //콜렉트부속이 없음
                return;
            }

            if (collectPartInfo.TargetCollectAbilityInfo == null)
            {
                //세트능력정보가 없음
                return;
            }

            //세트정보 확인
            var key = collectPartInfo.TargetCollectAbilityInfo;

            if (!collectAbilityBible.ContainsKey(key))
            {
                //없을시 하나생성해서
                //바이블에 등록하고 
                //콜렉트에 집어넣기
                List<CollectPartInfo_Base> tempCollectPartInfoList = new List<CollectPartInfo_Base>();
                collectAbilityBible.Add(key, tempCollectPartInfoList);
            }

            var dataList = collectAbilityBible[key];

            //존재하는지 
            if (dataList.Contains(collectPartInfo))
            {
                return;
            }

            //존재안하면 모은 파츠들에 추가 후 능력추가
            dataList.Add(collectPartInfo);

            //세트아이템 능력이 존재하는지 체크
            if (!key.GetPartAmountAbility(dataList.Count, ref refAbilityArray))
            {
                return;
            }

            //존재하면 능력들 등록하기
            for (int i = 0; i < refAbilityArray.Length; i++)
            {
                var abilityInfo = refAbilityArray[i];
                //targetUnitAbility.AddAbility(abilityInfo);
                Debug.Log("적용중");
            }
        }

        /// <summary>
        /// 세트능력 부속품삭제
        /// </summary>
        /// <param name="collectPartInfo">콜렉션파츠 정보</param>
        public void RemoveCollectElement(CollectPartInfo_Base collectPartInfo)
        {
            if (collectPartInfo == null)
            {
                //콜렉트가 없음
                return;
            }

            if (collectPartInfo.TargetCollectAbilityInfo == null)
            {
                //세트능력이 없음
                return;
            }

            //세트정보 확인
            var key = collectPartInfo.TargetCollectAbilityInfo;

            if (!collectAbilityBible.ContainsKey(key))
            {
                //없을시 넘어감
                return;
            }

            var dataList = collectAbilityBible[key];

            //존재안하면 넘어감
            if (!dataList.Contains(collectPartInfo))
            {
                return;
            }

            //존재하면 현재 세팅된 능력을 체크후 빼기            

            //세트아이템 능력이 존재하는지 체크
            if (key.GetPartAmountAbility(dataList.Count, ref refAbilityArray))
            {
                //존재하면 능력들 빼기
                for (int i = 0; i < refAbilityArray.Length; i++)
                {
                    var abilityInfo = refAbilityArray[i];
                    //targetUnitAbility.RemoveAbility(abilityInfo);
                }
            }

            //데이터빼기
            dataList.Remove(collectPartInfo);
        }
    }
}
