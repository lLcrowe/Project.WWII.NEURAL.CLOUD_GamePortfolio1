using lLCroweTool.TimerSystem;

namespace lLCroweTool.Ability
{

    [System.Serializable]
    public class AbilityActionBehaviorData
    {
        //타이머 두개
        public CoolTimerModule_Element skillCastTimer = new CoolTimerModule_Element();//스킬캐스트타임모듈 //스킬에 대한 실질적인 작동
        public CoolTimerModule_Element abilityActionTimer = new CoolTimerModule_Element();//액션지속시간 타이머


        public void Init(AbilityActionInfo info)
        {
            skillCastTimer.SetCoolTime(info.castingTime);
            skillCastTimer.SetReadyToCoolEvent(() => { abilityActionTimer.StartSkill(); });

            abilityActionTimer.SetCoolTime(info.actionDurationTime);
            abilityActionTimer.SetActionEvent(() => { info.ActionStart(); });
            abilityActionTimer.SetReadyToCoolEvent(() => { info.ActionEnd(); });
        }

        public void Action()
        {
            skillCastTimer.StartSkill();
        }

        public void Abort()
        {
            skillCastTimer.CancelCoolTime();
        }

        public void UpdateAbilityData()
        {
            CoolTimerModule_Element.UpdateCoolTimer(skillCastTimer);
            CoolTimerModule_Element.UpdateCoolTimer(abilityActionTimer);
        }
    }
}
