using UnityEngine;

namespace lLCroweTool.TimerSystem
{
    /// <summary>
    /// 업데이트타이머 모듈
    /// </summary>
    public sealed class UpdateTimerModule : UnityEventTimerModule_Base
    {
        public sealed override void UpdateTimerModuleFunc()
        {
            unityEvent.Invoke();
        }

        protected sealed override void ConnectTimerModuleManager()
        {
            TimerModuleManager.Instance.AddTimeModule(this, UpdateTimerModuleType.UpdateEvent);
        }

        protected sealed override void DeconnectTimerModuleManager()
        {
            TimerModuleManager.Instance.RemoveTimeModule(this, UpdateTimerModuleType.UpdateEvent);
        }
    }
}
