using System.Collections;
using UnityEngine;
using DG.Tweening;
using lLCroweTool;
using System;

namespace lLCroweTool.Effect.VFX
{
    public class SpreadVFXObject : MonoBehaviour
    {
        /// <summary>
        /// 스프레드VFX 초기화함수
        /// </summary>
        /// <param name="setting">위치1 위치2 위치1타임 위치2타임 애니메이션타입</param>
        public void InitSpreadVFXObject(ValueTuple<Vector2, Vector2, float, float, float, Ease> setting)
        {
            this.SetActive(true);
            StartCoroutine(SpreadVFXObjectCoroutine(setting));
        }
        //코루틴처리
        private IEnumerator SpreadVFXObjectCoroutine(ValueTuple<Vector2, Vector2, float, float, float, Ease> setting)
        {
            var (firPos, secPos, firTime, waitTime, secTime, ease) = setting;

            transform.DOMove(firPos, firTime).SetEase(ease);
            yield return new WaitForSeconds(waitTime + firTime);
            transform.DOMove(secPos, secTime).SetEase(ease);
            yield return new WaitForSeconds(secTime);
            this.SetActive(false);
        }

        private void OnDisable()
        {
            if(gameObject.activeSelf)
            {
                this.SetActive(false);
            }
        }
    }
}