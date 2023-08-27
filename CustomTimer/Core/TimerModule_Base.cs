using System.Collections;
using UnityEngine;

namespace lLCroweTool.TimerSystem
{
    public abstract class TimerModule_Base : MonoBehaviour
    {
        //20221113
        //구조변경//타이머모듈들을 더 슬림하게 제작
        //20221116
        //구조변경//통합완료

        public TimerModule_Element timerModule_Element;

        /// <summary>
        /// 타임모듈매니저에 현 클래스를 등록하는 함수//앱작동시
        /// </summary>
        protected abstract void ConnectTimerModuleManager();

        /// <summary>
        /// 타임모듈매니저에 현 클래스를 삭제하는 함수//앱작동시
        /// </summary>
        protected abstract void DeconnectTimerModuleManager();

        /// <summary>
        /// 업데이트할 기능을 명시하는 함수//매니저에서 호출//활성화됫을시 작동
        /// </summary>
        public abstract void UpdateTimerModuleFunc();

        protected virtual void Awake()
        {
            if (timerModule_Element.GetTime() == -1)
            {
                ResetTime();
                //time = Time.time;
            }
        }

        protected virtual void OnEnable()
        {
            ConnectTimerModuleManager();
        }

        protected virtual void OnDisable()
        {
            if (checkQuit)
            {
                return;
            }
            DeconnectTimerModuleManager();
        }

        //private static bool checkQuit = false;//스태딕을 초기화안해주는 버그가 있어서 그냥 스태딕사용을 안할 예정
        private bool checkQuit = false;
        private void OnApplicationQuit()
        {
            checkQuit = true;
        }

        /// <summary>
        /// 타이머 세팅(시간초)
        /// </summary>
        /// <param name="value">시간</param>
        public void SetTimer(float value)
        {
            timerModule_Element.SetTimer(value);
        }

        /// <summary>
        /// 세팅한 타이머를 가져오는 함수
        /// </summary>
        /// <returns>세팅된 타이머</returns>
        public float GetTimer()
        {
            return timerModule_Element.GetTimer();
        }

        /// <summary>
        /// 현재 저장된 시간을 가져오기
        /// </summary>
        /// <returns></returns>
        public float GetTime()
        {
            return timerModule_Element.GetTime();
        }

        /// <summary>
        /// 시간을 현재 시간으로 초기화
        /// </summary>
        public void ResetTime()
        {
            timerModule_Element.ResetTime();
        }

#if UNITY_EDITOR
        /// <summary>
        ///타이머 디버그용//테스트할때 쓴다.
        /// </summary>
        public void TimerDebug()
        {
            Debug.Log(gameObject.name + " : 타이머이벤트가 호출되었습니다.");
        }
#endif
    }
}