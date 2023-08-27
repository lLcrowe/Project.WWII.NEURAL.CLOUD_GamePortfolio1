using lLCroweTool.TimerSystem;
using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace lLCroweTool.UnitBatch
{
    [RequireComponent(typeof(BoxCollider))]
    public class UnitBatchObject : MonoBehaviour
    {
        //박스콜라이더 존재
        //유닛을 배치하는 오브젝트
        //트리거형태로 가지고 있어야할듯함
        //네모나게해서 특정 쪽을 체크하게
        public BattleUnitObject targetBatchObject;//배치된 오브젝트

        public GameObject ShineObject;//이팩트용 오브젝트//껏다키는 오브젝트
    

        public void EnableShine()
        {   
            //이팩트키기
            ShineObject.gameObject.SetActive(true);
        }
        private void DisableShine()
        {
            //이팩트끄기
            ShineObject.gameObject.SetActive(false);
        }

        /// <summary>
        /// 유닛오브젝트를 세팅
        /// </summary>
        /// <param name="unitObject">유닛오브젝트</param>
        public void SetUnitObject(BattleUnitObject unitObject)
        {
            targetBatchObject = unitObject;

            if (targetBatchObject == null)
            {
                return;
            }


            //위치,회전만 그대로 두고 옮기기
            //그래야지 이동느낌이남//배치매니저쪽에서 정렬해줌
            targetBatchObject.transform.InitTrObjPrefab(targetBatchObject.transform.position, targetBatchObject.transform.rotation, transform);
        }

        /// <summary>
        /// 유닛오브젝트를 가져옴
        /// </summary>
        /// <returns></returns>
        public BattleUnitObject GetUnitObject()
        {
            return targetBatchObject;
        }

        /// <summary>
        /// 유닛배치내역을 리셋 
        /// </summary>
        public void ResetUnitBatch()
        {
            targetBatchObject.gameObject.SetActive(false);
            targetBatchObject = null;
        }

        private void OnMouseDown()
        {
            if (targetBatchObject == null)
            {
                return;
            }

            UnitBatchUIManager.Instance.SetUnitBatchUI(UnitBatchUIManager.UnitBatchStateType.SelectUnitObject, targetBatchObject.transform, transform, targetBatchObject.unitObjectInfo);
        }

        private void OnMouseEnter()
        {
            EnableShine();
        }

        private void OnMouseExit()
        {
            DisableShine();
        }
    }
}