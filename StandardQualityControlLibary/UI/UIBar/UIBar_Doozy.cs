using UnityEngine.UI;
using UnityEngine;
using lLCroweTool.TimerSystem;
using Doozy.Engine.Progress;
namespace lLCroweTool.UI.Bar
{
    public class UIBar_Doozy : UIBar_Base
    {
        public Progressor progressor;//필수
        public ProgressTargetImage fillProgressTarget;//필수
        //텍스트타겟//조건

        public override void InitUIBar(float min, float max, float value)
        {
            progressor.SetMin(min);
            progressor.SetMax(max);
            progressor.SetValue(value);

            SetCurValue(value);
        }

        public override float GetCurValue()
        {
            return progressor.Value;
        }
        protected override void SetCurBarValue(float value)
        {
            progressor.SetValue(value);
        }

        public override float GetMaxValue()
        {
            return progressor.MaxValue;
        }

        public override void SetMaxValue(float value)
        {
            progressor.SetMax(value);
        }

        public override float GetMinValue()
        {
            return progressor.MinValue;
        }

        public override void SetMinValue(float value)
        {
            progressor.SetMin(value);
        }
    }
}