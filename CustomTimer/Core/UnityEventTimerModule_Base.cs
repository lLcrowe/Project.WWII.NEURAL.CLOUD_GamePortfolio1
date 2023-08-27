using UnityEngine;
using UnityEngine.Events;

namespace lLCroweTool.TimerSystem
{   
    public abstract class UnityEventTimerModule_Base : TimerModule_Base
    {
        //유니티이벤트를 사용하는 타임모듈 베이스 클래스

        //이벤트호출용
        //원하는 시간이 될때마다 이벤트호출용도
        //모듈로서 사용할려면 사용할려는 오브젝트에서
        //따로 호출할려는 이벤트들을 생성해야 하며
        //해당객체도 시간이 필요하면 카운트함수를 제작해야함
        [Header("-=3. 사용할 이벤트를 설정")]
        public UnityEvent unityEvent = new UnityEvent();//호출할시 GC가 쌓임//단 매프레임마다 쌓이진 않고 일정시간마다 쌓임//점진적GC 켜서그런듯함//32B

        protected sealed override void Awake()
        {
            base.Awake();
            lLcroweUtil.GetAddUnitEvent(ref unityEvent);
        }

        /// <summary>
        /// 작동될 이벤트 추가(세팅)
        /// </summary>
        /// <param name="action">함수</param>
        public void AddUnityEvent(UnityAction action)
        {
            //AddListener(delegate{함수();})
            unityEvent.AddListener(action);
        }

        /// <summary>
        /// 모든이벤트삭제
        /// </summary>
        public void RemoveAllUnityEvent()
        {
            unityEvent.RemoveAllListeners();
        }

        /// <summary>
        /// 원하는 이벤트만 삭제
        /// </summary>
        /// <param name="action"></param>
        public void RemoveUnityEvent(UnityAction action)
        {
            unityEvent.RemoveListener(action);
        }

        protected virtual void OnDestroy()
        {
            unityEvent = null;
        }
    }
}