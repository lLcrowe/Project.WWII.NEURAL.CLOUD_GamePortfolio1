using lLCroweTool.Dictionary;
using lLCroweTool.Singleton;

namespace lLCroweTool.NoticeDisplay.Manager
{
    public class NoticeDisplayUIManager : MonoBehaviourSingleton<NoticeDisplayUIManager>
    {
        //여러알림디스플레이UI를 관리하기위한 매니저
        //NoticeDisplayUIRegisterTarget에서 초반세팅
        //필요할때 요청해서 처리

        /// <summary>
        /// 알림디스플레이UI타입
        /// </summary>
        public enum ENoticeDisplayUIType
        {
            //원하는 타입을 집어넣고 돌리기

            MiddleInfo, //중앙 정보디스플레이
            SideInfo,   //사이드 정보디스플레이
            Achievement,//업적
        }



        public class NoticeDisplayUIBible: CustomDictionary<ENoticeDisplayUIType, NoticeDisplayUI> {}
        public NoticeDisplayUIBible noticeDisplayUIBible = new NoticeDisplayUIBible();

        /// <summary>
        /// 알림디스플레이UI를 등록하는 함수
        /// </summary>
        /// <param name="noticeDisplayUIType">타입</param>
        /// <param name="noticeDisplayUI">알림디스플레이UI</param>
        public void Register(ENoticeDisplayUIType noticeDisplayUIType, NoticeDisplayUI noticeDisplayUI)
        {
            noticeDisplayUIBible.TryAdd(noticeDisplayUIType, noticeDisplayUI);
        }

        /// <summary>
        /// 타입에 대한 알림디스플레이UI를 요청하는 함수
        /// </summary>
        /// <param name="noticeDisplayUIType">타입</param>
        /// <returns>타입에 대한 알림디스플레이</returns>
        public NoticeDisplayUI RequestNoticeDisplayUI(ENoticeDisplayUIType noticeDisplayUIType)
        {
            noticeDisplayUIBible.TryGetValue(noticeDisplayUIType, out NoticeDisplayUI value);
            return value;
        }

    }
}