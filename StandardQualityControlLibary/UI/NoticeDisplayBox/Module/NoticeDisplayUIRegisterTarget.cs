using UnityEngine;
using static lLCroweTool.NoticeDisplay.Manager.NoticeDisplayUIManager;

namespace lLCroweTool.NoticeDisplay.Manager
{
    public class NoticeDisplayUIRegisterTarget : MonoBehaviour
    {
        [Header("알림 디스플레이 UI 타입")]
        public ENoticeDisplayUIType noticeDisplayUIType;
        private void Awake()
        {
            if (TryGetComponent(out NoticeDisplayUI noticeDisplayUI))
            {
                NoticeDisplayUIManager.Instance.Register(noticeDisplayUIType, noticeDisplayUI);
            }
        }
        private void Start()
        {
            Destroy(this);
        }
    }
}