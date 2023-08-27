namespace lLCroweTool.TimerSystem
{
    //[RequireComponent(typeof(UpdateTimerModule))]//20221021필요없게 변함
    public class CoolTimerModule : UpdateTimerModule_Base
    {
        //쿨타임 타이머
        //사용법
        //버튼에 해당컴포넌트를 집어넣는다.Or 따로 게임오브젝트에 집어넣는다.
        //사용처 : 버튼스킬, 버튼 인벤소모, 무버튼 스킬쿨 등등

        //필요할떄만 UI에 집어넣어사용하며 기본적으로 
        //그냥 아무곳이나 집어넣고 참조만할수 있게한느거 좋을거 같음
        //private UpdateNonUnityEventTimerModule timerModule;//타이머모듈

        public CoolTimerModule_Element coolTimerModule = new CoolTimerModule_Element();

        protected override void Awake()
        {
            base.Awake();
            SetTimer(0.02f);
        }

        public override void UpdateTimerModuleFunc()
        {
            CoolTimerModule_Element.UpdateCoolTimer(coolTimerModule);
        }

        private void OnDestroy()
        {
            coolTimerModule = null;
        }
    }
}

