using UnityEngine;
using System.Collections.Generic;
//using MEC;
//using lLCroweTool.LogSystem;
using lLCroweTool.Singleton;
using UnityEngine.Profiling;
using DG.Tweening;

namespace lLCroweTool.TimerSystem
{
    public class TimerModuleManager : MonoBehaviourSingleton<TimerModuleManager>
    {
        //월드타이머와 병합됨
        //월드상에 한개 이상무조건있어야됨
        public bool isPause = false;                        //정지버튼
        [Range(0.0f, 2.0f)]
        ///<summary></summary>
        [SerializeField] private float timerScale = 1.0f;   //타임배율
        private float cashScale;                            //저장하는 스케일값
        [Range(2.0f, 0.25f)]
        ///<summary>
        ///업데이트모듈에서 사용하는 타임배율//숫자가 작을수록 빨라짐
        /// </summary>        
        [SerializeField] private float updateTimerScale = 1.0f;//0.25 0.5 0.75 1


        //update와 fixedupdate쪽에서 제어하면 0으로 변수가설정시
        //아예 멈추고 다음프레임으로 옮겨가지못하는 상황이 발생됨
        //그래서 LateUpdate쪽에서 작업하면 정상작동
        //코루틴생각하다가 LateUpdate도 괜찮다고 생각함
        //나중에 업데이트타이머모듈을 여러개 만들어야겠음
        //각각업데이트마다 작동되게
        //현재 Update만 만들어져있고
        //나중에 만들 LateUpdate
        //FixedUpdate가 있다

        //20221116(새벽)
        //이리저리 테스트해보니 2020버전부터는 코루틴말고 업데이트가 빠르며
        //각각 Mono에서 업데이트보다 한곳에서 업데이트하는게 더 빠르다
        //느낌상 C#에서 C++통신레이어에서 오버헤드가 생기니 그런듯함
        //고로 다시 체크후 분리하여 처리할예정 자고일어나서 처리하자

        //20221116(오후)
        //구조변경//상속으로 가자//유니티이벤트는 그대로 납두고 위치만 바꾸고
        //타이머모듈데이터는 상속만 쓰자//유니티이벤트 사용 안됨
        //구조변경완료 

        [Tooltip("업데이트모듈이 현황상태")]
        public int normalTimerModuleAmount = 0;
        public int unityEventTimerModuleAmount = 0;//유니티이벤트 사용하는 업데이트
        public int normalCoroutineModuleAmount = 0;
        public int unityEventCoroutineModuleAmount = 0;//유니티이벤트 사용하는 코루틴


        //인터페이스처리?//상속보다 느리긴해도 GC가 안생김//이벤트호출을 고치기전까지는 인터페이스나 상속으로 등록후 처리할 가능성높음(문제가 된다면)
        [Tooltip("업데이트모듈이 들어간 오브젝트들")]////모노비헤이비어 상속받은 모듈
        //업데이트모듈
        public List<TimerModule_Base> updateTimerModuleBaseList = new List<TimerModule_Base>();
        public TimerModule_Base[] updateTimerModuleBaseArray = new TimerModule_Base[0];

        //코루틴모듈//MEC로 처리하는중//중앙에서 체크하여 작동됫는지 확인용도//작동은 다른곳에서 됨
        public List<TimerModule_Base> coroutineTimerModuleList = new List<TimerModule_Base>();

        //private CoroutineHandle updateHandle;
        private static float timerValue;

        protected override void Awake()
        {
            base.Awake();
            //updateHandle = Timing.RunCoroutine(UpdateTimerModuleFunc());//MEC
        }

        private void LateUpdate()
        {
            Time.timeScale = timerScale;
            Time.fixedDeltaTime = 0.06f * Time.timeScale;
        }

        /// <summary>
        /// 게임정지(버튼용)
        /// </summary>
        public void GamePause()
        {
            //LogManager.Log(typeof(TimerModuleManager), "멈춤작동", gameObject, LogManager.LogType.Info);
            if (!isPause)
            {
                cashScale = timerScale;
                timerScale = 0;
                isPause = !isPause;
            }
            else
            {
                timerScale = cashScale;
                isPause = !isPause;
            }
        }

        /// <summary>
        /// -3~3까지 범위가 있음 -일수록 느려지고 +일수록 빨라짐
        /// </summary>
        /// <param name="_level">변하고자 하는 레벨</param>
        public void SetUpdateTimerScale(int _level)
        {
            switch (_level)
            {
                //느려짐
                case -3:
                    updateTimerScale = 2.0f;//2배느려짐
                    break;
                case -2:
                    updateTimerScale = 1.5f;//1.5배
                    break;
                case -1:
                    updateTimerScale = 1.25f;//1.25배
                    break;
                //빨라짐
                case 1:
                    updateTimerScale = 0.875f;//1.25배
                    break;
                case 2:
                    updateTimerScale = 0.75f;//1.5배
                    break;
                case 3:
                    updateTimerScale = 0.5f;//2배빨라짐
                    break;
                case 0:
                default:
                    updateTimerScale = 1f;
                    break;
            }
        }

        /// <summary>
        /// -3~3까지 범위가 있음 -일수록 느려지고 +일수록 빨라짐
        /// </summary>
        /// <param name="_level">변하고자 하는 레벨</param>
        public void SetTimerScale(int _level)
        {
            switch (_level)
            {
                //느려짐
                case -3:
                    timerScale = 2.0f;//2배느려짐
                    break;
                case -2:
                    timerScale = 1.5f;//1.5배
                    break;
                case -1:
                    timerScale = 1.25f;//1.25배
                    break;
                //빨라짐
                case 1:
                    timerScale = 0.875f;//1.25배
                    break;
                case 2:
                    timerScale = 0.75f;//1.5배
                    break;
                case 3:
                    timerScale = 0.5f;//2배빨라짐
                    break;
                case 0:
                default:
                    timerScale = 1f;
                    break;
            }
        }

        /// <summary>
        /// 타이머스케일값 지정하는 함수
        /// </summary>
        /// <param name="_timerScale">타이머스케일값</param>
        public void SetTimerScale(float _timerScale)
        {
            timerScale = _timerScale;
        }

        /// <summary>
        /// 타이머스케일값 지정하는 함수
        /// </summary>
        /// <param name="_timerScale">타이머스케일값</param>
        public void SetTimerScale(float _timerScale, float recoveryTimer)
        {
            timerScale = _timerScale;
            float value = 0;
            DOTween.To(() => value, x => value = x, 1, recoveryTimer)
                .OnUpdate(() => {
                    timerScale = value;
                });
        }

        /// <summary>
        /// 현재 타이머스케일 값 가져오기
        /// </summary>
        /// <returns>타이머 스케일값</returns>
        public float GetUpdateTimerScale()
        {
            return updateTimerScale;
        }

        /// <summary>
        /// 타이머모듈울 등록
        /// </summary>
        /// <param name="timerModule">추가할 업데이트타임모듈</param>
        public void AddTimeModule(TimerModule_Base timerModule, UpdateTimerModuleType timerModuleType)
        {
            switch (timerModuleType)
            {
                case UpdateTimerModuleType.Update:
                    if (!updateTimerModuleBaseList.Contains(timerModule))
                    {
                        updateTimerModuleBaseList.Add(timerModule);
                        updateTimerModuleBaseArray = updateTimerModuleBaseList.ToArray();
                        normalTimerModuleAmount++;
                    }
                    break;
                case UpdateTimerModuleType.UpdateEvent:
                    if (!updateTimerModuleBaseList.Contains(timerModule))
                    {
                        updateTimerModuleBaseList.Add(timerModule);
                        updateTimerModuleBaseArray = updateTimerModuleBaseList.ToArray();
                        unityEventTimerModuleAmount++;
                    }
                    break;
                case UpdateTimerModuleType.Coroutine:
                    if (!coroutineTimerModuleList.Contains(timerModule))
                    {
                        coroutineTimerModuleList.Add(timerModule);
                        normalCoroutineModuleAmount++;
                    }
                    break;

                case UpdateTimerModuleType.CoroutineEvent:
                    if (!coroutineTimerModuleList.Contains(timerModule))
                    {
                        coroutineTimerModuleList.Add(timerModule);
                        unityEventCoroutineModuleAmount++;
                    }
                    break;
            }
        }

        /// <summary>
        /// 등록한 타이머모듈 삭제
        /// </summary>
        /// <param name="timerModule">삭제할 타이머모듈</param>
        public void RemoveTimeModule(TimerModule_Base timerModule, UpdateTimerModuleType timerModuleType)
        {
            switch (timerModuleType)
            {
                case UpdateTimerModuleType.Update:
                    if (updateTimerModuleBaseList.Contains(timerModule))
                    {
                        updateTimerModuleBaseList.Remove(timerModule);
                        updateTimerModuleBaseArray = updateTimerModuleBaseList.ToArray();
                        normalTimerModuleAmount--;
                    }
                    break;
                case UpdateTimerModuleType.UpdateEvent:
                    if (updateTimerModuleBaseList.Contains(timerModule))
                    {
                        updateTimerModuleBaseList.Remove(timerModule);
                        updateTimerModuleBaseArray = updateTimerModuleBaseList.ToArray();
                        unityEventTimerModuleAmount--;
                    }
                    break;
                case UpdateTimerModuleType.Coroutine:
                    if (coroutineTimerModuleList.Contains(timerModule))
                    {
                        coroutineTimerModuleList.Remove(timerModule);
                        normalCoroutineModuleAmount--;
                    }
                    break;
                case UpdateTimerModuleType.CoroutineEvent:
                    if (coroutineTimerModuleList.Contains(timerModule))
                    {
                        coroutineTimerModuleList.Remove(timerModule);
                        unityEventCoroutineModuleAmount--;
                    }
                    break;
            }
        }

        //===================================================================
        //업데이트 관련=======================================================
        //===================================================================

        private void UpdateTimerModuleManager()
        {
            //업데이트 타이머모듈베이스(모노비헤이비어)
            Profiler.BeginSample("UpdateTimerModule");
            TimerModule_Base timerModule_Base = null;
            for (int i = 0; i < updateTimerModuleBaseArray.Length; i++)
            {
                timerModule_Base = updateTimerModuleBaseArray[i];
                //게임오브젝트, 스크립트가 비활성화상태이면 중지
                if (!timerModule_Base.isActiveAndEnabled)
                {
                    continue;
                }

                //시간체크
                if (!CheckTimer(timerModule_Base.timerModule_Element))
                {
                    continue;
                }

                timerModule_Base.UpdateTimerModuleFunc();//이벤트작동
                timerModule_Base.ResetTime();//타이머시간 리셋
            }
            Profiler.EndSample();
        }

        private void Update()
        {
            //유니티업데이트
            UpdateTimerModuleManager();
        }

        ///// <summary>
        ///// MEC로 호출하는 업데이트 함수//MEC
        ///// </summary>
        //private IEnumerator<float> UpdateTimerModuleFunc()//MEC
        //{
        //    //유니티 업데이트 대신 돌리기 위한 구역
        //    //테스트
        //    for (; ; )
        //    {
        //        UpdateTimerModuleManager();
        //        yield return Timing.WaitForOneFrame;
        //    }
        //}

        ///// <summary>
        ///// 타이머모듈들을 공통로직 함수(모노코루틴용)//MEC
        ///// </summary>
        ///// <param name="coroutineTimerModule">코루틴타이머모듈</param>
        ///// <param name="timer">지정된 타이머</param>
        //public IEnumerator<float> UpdateTimerModuleFunc(CoroutineTimerModule coroutineTimerModule)//MEC
        //{
        //    //20221113새로제작
        //    //20221114테스트진행 GC문제없음
        //    //20221116구조변경 코루틴은 코루틴쓰는 대상에서 처리

        //    for (; ; )
        //    {
        //        if (!coroutineTimerModule.isActiveAndEnabled)
        //        {
        //            continue;//문제있을거 같은데 모름
        //        }

        //        //Profiler.BeginSample("CoroutineTimerModule");
        //        coroutineTimerModule.UpdateTimerModuleFunc();//유니티이벤트32B//액션은 얼마나 쌓일까
        //        //Profiler.EndSample();
        //        //yield return Timing.WaitForSeconds(timer / WorldTimer.Instance.timerScale);
        //        //yield return Timing.WaitForSeconds(coroutineTimerModule.GetTimer() / (Mathf.Round(GetUpdateTimerScale() * 100) / 100));//뭔가 이상하다 체크해볼것
        //        //yield return Timing.WaitForSeconds(corotineTimerData.GetTimer() / ((GetUpdateTimerScale() * 100f) * 0.01f));//20220701변경
        //        yield return Timing.WaitForSeconds(coroutineTimerModule.GetTimer() / GetUpdateTimerScale());//20221113변경//원래이거였네
        //    }
        //}

        ///// <summary>
        ///// 타이머모듈들을 공통로직 함수(모노코루틴용)//MEC
        ///// </summary>
        ///// <param name="coroutineTimerModule">코루틴타이머모듈</param>
        ///// <param name="timer">지정된 타이머</param>
        //public IEnumerator<float> UpdateTimerModuleFunc(CoroutineTimerModule_Base coroutineTimerModule_Base)
        //{
        //    //20221113새로제작
        //    //20221114테스트진행 GC문제없음
        //    //20221116구조변경 코루틴은 코루틴쓰는 대상에서 처리

        //    for (; ; )
        //    {
        //        //Profiler.BeginSample("CoroutineTimerModule_Base");
        //        coroutineTimerModule_Base.UpdateTimerModuleFunc();
        //        //Profiler.EndSample();
        //        //yield return Timing.WaitForSeconds(timer / WorldTimer.Instance.timerScale);
        //        //yield return Timing.WaitForSeconds(coroutineTimerModule.GetTimer() / (Mathf.Round(GetUpdateTimerScale() * 100) / 100));//뭔가 이상하다 체크해볼것
        //        //yield return Timing.WaitForSeconds(corotineTimerData.GetTimer() / ((GetUpdateTimerScale() * 100f) * 0.01f));//20220701변경
        //        yield return Timing.WaitForSeconds(coroutineTimerModule_Base.GetTimer() / GetUpdateTimerScale());//20221113변경//원래이거였네
        //    }
        //}

        //===================================================================        
        //===================================================================

        ///// <summary>
        ///// 타이머모듈들을 공통로직 함수(일반용)
        ///// </summary>
        ///// <param name="updateAction">업데이트 행동</param>
        ///// <param name="ResetTimeAction">시간초기화 행동</param>
        ///// <param name="timer">타이머</param>
        ///// <param name="time">타임</param>
        ///// <param name="indieTimer">독립된 타이머여부</param>
        //private void UpdateTimerModuleFunc(System.Action updateAction, System.Action ResetTimeAction, float timer, float time, bool indieTimer)
        //{
        //    //20221113제작//공통로직 합동
        //    //20221114테스트진행 GC문제있음//왜고쳣지

        //    //퍼즈상태이면 중지
        //    if (isPause)
        //    {
        //        return;
        //    }

        //    timerValue = CalTimerValue(timer, time, indieTimer, GetUpdateTimerScale());

        //    if (Time.time > timerValue)
        //    {
        //        //Debug.Log("time : "+ time);

        //        //이벤트작동
        //        //Profiler.BeginSample("CommomTimerUpdate");
        //        updateAction();//업데이트
        //        //Profiler.EndSample();

        //        //Debug.Log("시간이 초기화되었습니다");
        //        ResetTimeAction();//리셋작동
        //    }
        //}

        /// <summary>
        /// 작동할 시간이 됫는지 체크해주는 함수
        /// </summary>
        ///<param name="timerModule_Element">타이머모듈_요소</param>
        /// <returns>작동할 시간이 됫는지 여부</returns>
        public bool CheckTimer(TimerModule_Element timerModule_Element)
        {
            //20221113제작//공통로직 합동
            //20221114테스트진행 GC문제있음
            //202216문제는 Action같은 Delegate문제. 여긴상관없음
            //20230220데이터부분을 분리
            if (isPause)
            {
                return false;
            }

            timerValue = CalTimerValue(timerModule_Element.GetTimer(), timerModule_Element.GetTime(), timerModule_Element.indieTimer, GetUpdateTimerScale());

            return Time.time > timerValue;
        }

        /// <summary>
        /// 타임밸류를 계산해주는 함수
        /// </summary>
        /// <param name="timer">지정된 타이머</param>
        /// <param name="time">저장된 시간</param>
        /// <param name="indieTimer">독립된 타이머여부</param>
        /// <param name="timerScale">시간 스케일</param>
        /// <returns>계산된 타임밸류</returns>
        private static float CalTimerValue(float timer, float time, bool indieTimer, float timerScale)
        {
            float tempValue = indieTimer ? timer + time : (timer * timerScale) + time;
            return tempValue;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            updateTimerModuleBaseList.Clear();
            coroutineTimerModuleList.Clear();
        }
    }

    //업데이트와 코루틴을 같이 작업하지않기떄문에 분리 해놈
    //중앙에서 볼수 있게

    /// <summary>
    /// 타이머모듈(모노) 타입
    /// </summary>
    public enum UpdateTimerModuleType
    {
        Update,
        UpdateEvent,
        Coroutine,
        CoroutineEvent,
    }
}
