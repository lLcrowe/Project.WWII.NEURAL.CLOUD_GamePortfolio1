using UnityEngine.UI;
using UnityEngine;
using lLCroweTool.TimerSystem;
using Doozy.Engine.Progress;
namespace lLCroweTool.UI.Bar
{
    public class UIBar_Slide : UIBar_Base
    {   
        public Slider slider;//필수
        public Image iconHandle;//아이콘//따라다니는아이콘

        public override void InitUIBar(float min, float max, float value)
        {
            slider.minValue = min;
            slider.maxValue = max;
            slider.value = value;

            SetCurValue(value);
        }

        public override float GetCurValue()
        {   
            return slider.value;
        }

        protected override void SetCurBarValue(float value)
        {
            slider.value = value;
        }

        public override float GetMaxValue()
        {
            return slider.maxValue;
        }

        public override void SetMaxValue(float value)
        {
            slider.maxValue = value;
        }

        public override float GetMinValue()
        {
            return slider.minValue;
        }

        public override void SetMinValue(float value)
        {
            slider.minValue = value;
        }
    }
}