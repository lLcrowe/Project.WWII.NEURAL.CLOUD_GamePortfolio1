using UnityEngine;

namespace lLCroweTool.TimerSystem
{
    public abstract class UpdateTimerModule_Base : TimerModule_Base
    {
        //20220730
        //모노비헤이비어 컴포넌트를 만들때 상속받아서 쓰는 타이머모듈
        //유니티이벤트가 몇초마다 GC를 걸려내길래 혹시나 하는 마음으로 제작
        //20221112//구조변경
        //20221115//구조변경GC나는거 체크
        //20221116//구조변경 완료

        protected sealed override void ConnectTimerModuleManager()
        {
            TimerModuleManager.Instance.AddTimeModule(this, UpdateTimerModuleType.Update);
        }

        protected sealed override void DeconnectTimerModuleManager()
        {
            TimerModuleManager.Instance.RemoveTimeModule(this, UpdateTimerModuleType.Update);
        }
    }
}