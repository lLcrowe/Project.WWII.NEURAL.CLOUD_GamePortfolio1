using lLCroweTool.TimerSystem;
using lLCroweTool.UI.Bar;
using System.Collections;
using TMPro;
using UnityEngine;

namespace lLCroweTool.Ability.UI
{
    public class UnitStatusBibleUIBar : MonoBehaviour
    {
        //해당 걸 찾아와서 캐싱
        public UnitStatusType maxUnitStatusType;//최대치
        public UnitStatusType curUnitStatusType;//현대치
        public UIBar_Base uIBar;
        public TextMeshProUGUI text;

        //레이트관련 보여줄지에 대한 설정
        public bool isShowRateValue;
        public UnitStatusType rateUnitStatusType;
        public TextMeshProUGUI rateText;
        private float rateTextValue;

        public TimerModule_Element timerModule;

        private UnitStatusBible targetUnitStatusBible;

        public void Init(UnitStatusBible unitStatusBible)
        {
            targetUnitStatusBible = unitStatusBible;
            if (targetUnitStatusBible == null)
            {
                uIBar.InitUIBar(0, 0, 0);
                return;
            }

            //최대,현재 체력관련
            if (!targetUnitStatusBible.TryGetValue(maxUnitStatusType, out var maxData))
            {
                uIBar.InitUIBar(0, 0, 0);
                return;
            }
            float maxValue = maxData.value;

            if (!targetUnitStatusBible.TryGetValue(curUnitStatusType, out var curData))
            {   
                return;
            }
            float curValue = curData.value;
            uIBar.InitUIBar(0, maxValue, curValue);

            //레이트관련
            if (!isShowRateValue)
            {
                return;
            }
            
            if (!targetUnitStatusBible.TryGetValue(rateUnitStatusType, out var rateData))
            {
                return;
            }

            rateText.text = rateData.value.ToString();
        }

        public void UpdateUI()
        {
            if (!timerModule.CheckTimer())
            {
                return;
            }

            //최대관련
            if (targetUnitStatusBible.TryGetValue(maxUnitStatusType, out var maxData))
            {
                if (uIBar.GetMaxValue() != maxData.value)
                {
                    uIBar.SetMaxValue(maxData.value);
                }
            }
            
            //현재관련
            if (targetUnitStatusBible.TryGetValue(curUnitStatusType, out var curData))
            {
                if (uIBar.GetCurValue()!= curData.value)
                {
                    uIBar.SetCurValue(curData.value);
                }
            }

            //레이트관련
            if (isShowRateValue)
            {
                if (!targetUnitStatusBible.TryGetValue(rateUnitStatusType, out var rateData))
                {
                    return;
                }

                if (rateTextValue != rateData.value)
                {
                    rateText.text = rateData.value.ToString();
                }
            }

            



        } 

        



        
    }
}