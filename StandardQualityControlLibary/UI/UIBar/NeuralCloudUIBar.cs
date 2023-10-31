using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace lLCroweTool.UI.Bar
{
    public class NeuralCloudUIBar : UIBar_Slide
    {
        [Header("FollowFillImage Setting")]
        //뉴럴클라우드에서 사용되는 UIBar를 제작
        public Image followfill;//쫒아가서 채우는 이미지
        public float timer;//걸리는시간


        [Header("HitEffect Setting")]
        public Vector3 hitDesizeValue;
        public float desizeTime;
        public float recoveryTime;

        private Vector3 originSizeValue;
        private WaitForSeconds desizeWait;
        private Transform tr;

        private void Awake()
        {
            tr = transform;
            originSizeValue = transform.localScale;
            desizeWait = new WaitForSeconds(desizeTime);
        }

        public override void InitUIBar(float min, float max, float value)
        {
            base.InitUIBar(min, max, value);
            followfill.fillAmount = value;
        }


        protected override void SetCurBarValue(float value)
        {
            base.SetCurBarValue(value);
            followfill.DOFillAmount(fill.fillAmount, timer);
        }

        /// <summary>
        /// 히트했을시 UI에서 보여줄 이팩트
        /// </summary>
        public void HitUIEffectAction()
        {
            StopCoroutine(HitEffectAction());
            StartCoroutine(HitEffectAction());
        }

        private IEnumerator HitEffectAction()
        {
            tr.DOScale(hitDesizeValue, desizeTime);
            yield return desizeWait;
            tr.DOScale(originSizeValue, recoveryTime);
        }
    }
}