using UnityEngine;
using UnityEngine.Events;

namespace lLCroweTool.TimerSystem
{
    public static class CoolTimerModule_Element_Extend
    {
        public static void UpdateCoolTimer(this CoolTimerModule_Element coolTimerModule)
        {
            CoolTimerModule_Element.UpdateCoolTimer(coolTimerModule);
        }
    }

    [System.Serializable]
    public class CoolTimerModule_Element
    {
        //이벤트관련
        private bool enabled = false;//업데이트여부
        private bool enableSkill = true;//스킬활성화여부
        public UnityEvent SkillActionEvent;//작동시켯을 때 이벤트
        public UnityEvent readyToCoolEvent;//쿨타임이 완료되었을때 이벤트

        //반복관련
        public bool isUseRepeat = false;//우클릭작동불가쪽
        public bool isRepeat = false;//반복하는중인가
        public float skillCoolTime = 3f;//스킬쿨타임 시간
        [Header("현 쿨타임")]
        [SerializeField] protected float timeValue = 0;//쿨타임용
        public float time = -1;

        public CoolTimerModule_Element()
        {
            enableSkill = true;
            lLcroweUtil.GetAddUnitEvent(ref SkillActionEvent);
            lLcroweUtil.GetAddUnitEvent(ref readyToCoolEvent);
        }

        ~CoolTimerModule_Element()
        {
            SkillActionEvent = null;
            readyToCoolEvent = null;
        }

        /// <summary>
        /// 쿨타이머모듈에서 쓰는 업데이트
        /// </summary>
        /// <param name="coolTimerModule">타겟이 될 쿨타이머모듈</param>
        public static void UpdateCoolTimer(CoolTimerModule_Element coolTimerModule)
        {
            if (!coolTimerModule.enabled)
            {
                return;
            }

            //스킬(종합) 업데이트 //프레임또는 일정시간마다 호출//스킬시간 업데이트
            coolTimerModule.SetTimeValue(coolTimerModule.GetTimeValue() + 1 / coolTimerModule.skillCoolTime * (Time.time - coolTimerModule.GetTime()));//작동됨 작동잘됨          
            if (coolTimerModule.GetTimeValue() > 1)
            {
                //쿨타임이 시간이 다됫으면 업데이트를 종료시키고
                //스킬을 활성화시킨다.
                coolTimerModule.enabled = false;
                coolTimerModule.SetEnableSkill(true);
                //coolTimerModule.SetTimeUIValue(coolTimerModule.GetTimeValue());
                coolTimerModule.SetTimeValue(0);
                //쿨이 완료되었을때 작동되는 함수
                coolTimerModule.readyToCoolEvent.Invoke();
            }

            coolTimerModule.ResetTime();
            if (coolTimerModule.isRepeat && coolTimerModule.GetEnableSkill())
            {
                //여기 문제있을 확률이 있음
                //어차피 한 프레임지나면 다작동되고 빠지니 상관없을 듯하고..
                coolTimerModule.StartSkill();
            }
        }

        //스킬쿨타임시작 
        //버튼클릭이벤트와 키세팅했을때 이벤트
        //다른곳에서 쓸 함수를 따로 해놔야할듯

        //스킬작동만 다루는곳
        //외부에서는 setskill함수와  startskill함수를 쓰면됨
        //UI버튼과 플레이어컨트롤러 그리고 AI가 사용하면 되는함수
        /// <summary>
        /// 쿨타임 작동 액션 활성 플레이
        /// </summary>
        public void StartSkill()
        {
            if (enableSkill)
            {
                SkillActionEvent.Invoke();//작동
                ResetTime();//시간을 초기화
                enabled = true;//타이머작동
                enableSkill = false;//두번작동못하게
            }
        }

        /// <summary>
        /// 쿨타이머가 작동되는걸 취소시키는 함수
        /// readyToCoolEvent 이벤트가 작동되기전에 사용
        /// </summary>
        public void CancelCoolTime()
        {
            enabled = false;
            enableSkill = true;
            timeValue = 0;
        }

        //쿨타임모듈세팅하기
        //무기에 사용됨
        //외부에서 사용하는 함수

        ///<summary>쿨타임모듈의 쿨타임 세팅</summary>
        public void SetCoolTime(float coolTime)
        {
            //세팅
            skillCoolTime = coolTime;
            //timerModule.SetTimer(0.1f);
            isRepeat = false;
        }
        ///<summary>쿨타임작동했을시의 이벤트 세팅</summary>
        public void SetActionEvent(UnityAction action)
        {
            //이벤트초기화
            SkillActionEvent.RemoveAllListeners();

            //세팅
            SkillActionEvent.AddListener(action);
        }

        ///<summary>쿨타임이 다돌았을시 이벤트 세팅함수</summary>
        public void SetReadyToCoolEvent(UnityAction action)
        {
            //이벤트초기화
            readyToCoolEvent.RemoveAllListeners();

            //세팅
            readyToCoolEvent.AddListener(action);
        }

        /// <summary>
        /// 스킬쿨타임의 활성화여부 가져오기
        /// </summary>
        /// <returns>스킬을 사용할수 있는지 여부</returns>
        public bool GetEnableSkill()
        {
            return enableSkill;
        }

        public void SetEnableSkill(bool _enableSkill)
        {
            enableSkill = _enableSkill;
        }

        public void ResetTime()
        {
            time = Time.time;
        }

        public float GetTime()
        {
            return time;
        }

        public float GetTimeValue()
        {
            return timeValue;
        }
        public void SetTimeValue(float _value)
        {
            timeValue = _value;
        }
    }
}
