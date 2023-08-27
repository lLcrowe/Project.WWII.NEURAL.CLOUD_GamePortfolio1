using UnityEngine;

namespace lLCroweTool.TimerSystem
{
    /// <summary>
    /// 타이머모듈 요소
    /// </summary>
    [System.Serializable]
    public struct TimerModule_Element
    {
        //컴포넌트식이 아닌 요소로 붙착시킴
        [Header("-=1. 몇초마다 이벤트를 발생할것인가.")]
        //몇초에 리셋될건지 해주는 타이머
        [SerializeField] private float timer;//작동될 타이머 : 0.02~ 0.05 정도

        [Header("-=2. 독립적인 타이머인가?")]
        //월드타이머와 별개로 돌아간건지
        public bool indieTimer;

        //기존의 돌아가는 타이머
        private float time;

        public TimerModule_Element(float timer, bool indieTimer = false)
        {
            this.timer = timer;
            this.indieTimer = indieTimer;
            time = -1;
        }

        /// <summary>
        /// 타이머모듈의 타이머체크
        /// </summary>
        /// <returns>작동할 시간이 됫는지 여부</returns>
        public bool CheckTimer()
        {
            //시간체크
            bool check = TimerModuleManager.Instance.CheckTimer(this);//작동할시간이 됫는지 여부
            if (check)
            {
                ResetTime();//타이머시간 리셋
            }
            return check;
        }

        /// <summary>
        /// 타이머 세팅(시간초)
        /// </summary>
        /// <param name="value">시간</param>
        public void SetTimer(float value)
        {
            timer = value;
        }

        /// <summary>
        /// 세팅한 타이머를 가져오는 함수
        /// </summary>
        /// <returns>세팅된 타이머</returns>
        public float GetTimer()
        {
            return timer;
        }

        /// <summary>
        /// 현재 저장된 시간을 가져오기
        /// </summary>
        /// <returns></returns>
        public float GetTime()
        {
            return time;
        }

        /// <summary>
        /// 시간을 현재 시간으로 초기화
        /// </summary>
        public void ResetTime()
        {
            time = Time.time;
        }
    }
}
