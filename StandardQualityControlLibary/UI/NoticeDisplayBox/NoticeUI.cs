using DG.Tweening;
using TMPro;
using UnityEngine;
using static lLCroweTool.NoticeDisplay.NoticeDisplayUI;

namespace lLCroweTool.NoticeDisplay
{
    [RequireComponent(typeof(CanvasGroup))]
    public class NoticeUI : MonoBehaviour
    {
        //상속처리하여 다른 기능추가하기
        public TextMeshProUGUI text;
        private float time;
        private CanvasGroup canvasGroup;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnDisable()
        {
            if (gameObject.activeSelf)
            {
                this.SetActive(false);
            }
        }

        /// <summary>
        /// 알람UI 보여주는함수
        /// </summary>
        /// <param name="content">컨텐츠내용</param>
        /// <param name="showSetting">Show세팅</param>
        /// <param name="batchPos">초기 배치위치(로컬)</param>
        public void ShowNoticeUI(string content, DoSetting showSetting , Vector2 batchPos)
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
            
            transform.localScale = showSetting.startSize;
            transform.DOScale(showSetting.endSize, showSetting.scaleSpeed).SetEase(showSetting.scaleEase);
            
            transform.localPosition = showSetting.batchPos + batchPos;
            transform.DOLocalMove(batchPos, showSetting.moveSpeed).SetEase(showSetting.moveEase);
            
            canvasGroup.alpha = showSetting.startFade;
            canvasGroup.DOFade(showSetting.endFade, showSetting.fadeSpeed).SetEase(showSetting.fadeEase);

            text.text = content;
            time = Time.time;
        }

        /// <summary>
        /// 알람UI 숨기는 함수
        /// </summary>
        /// <param name="hideSetting">Hide세팅</param>
        public void HideNoticeUI(DoSetting hideSetting)
        {
            transform.localScale = hideSetting.startSize;
            transform.DOScale(hideSetting.endSize, hideSetting.scaleSpeed).SetEase(hideSetting.scaleEase);
                       
            transform.DOLocalMove(transform.localPosition + (Vector3)hideSetting.batchPos, hideSetting.moveSpeed).SetEase(hideSetting.moveEase);

            canvasGroup.alpha = hideSetting.startFade;
            canvasGroup.DOFade(hideSetting.endFade, hideSetting.fadeSpeed).SetEase(hideSetting.fadeEase);
        }

        public float GetTime()
        {
            return time;
        }
    }
}