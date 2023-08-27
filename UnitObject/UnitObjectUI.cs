using lLCroweTool.TimerSystem;
using lLCroweTool.UI.Bar;
using lLCroweTool.UI.Follow;
using UnityEngine;
using UnityEngine.UI;

namespace lLCroweTool
{    
    public class UnitObjectUI : MonoBehaviour
    {
        //전장에 있는 유닛들에게 현재상태를 보여주는 UI

        //타겟팅된
        public UnitObject_Base targetUnit;

        public Image classIcon;
        
        public NeuralCloudUIBar hpBar;
        public NeuralCloudUIBar skillChargeBar;

        private bool isBattleUnit = false;
        public BattleUnitObject battleUnitObject;

        public Transform buffContentPos;

        public UIFollowObject uIBarFollowObject;

        public TimerModule_Element timer;


        public void InitBattleUnitUI(UnitObject_Base target)
        {
            targetUnit = target;
            if (ReferenceEquals(targetUnit, null))
            {
                this.SetActive(false);
                return;
            }

            
            uIBarFollowObject.followObject = targetUnit.transform;

            targetUnit.unitAbilityModule.GetUnitStatusValue(Ability.UnitStatusType.HealthPoint, out var point);
            targetUnit.unitAbilityModule.GetUnitStatusValue(Ability.UnitStatusType.HealthMaxPoint, out var pointMax);
            hpBar.InitUIBar(0,pointMax, point);

            battleUnitObject = targetUnit as BattleUnitObject;
            if (battleUnitObject == null)
            {
                isBattleUnit = false;
                skillChargeBar.SetActive(false);
            }
            else
            {
                isBattleUnit = true;


                classIcon.sprite = battleUnitObject.unitObjectInfo.classIcon;
                skillChargeBar.SetActive(true);
                point = battleUnitObject.skillChargeCurValue;
                pointMax = battleUnitObject.skillChargeMaxValue;
                skillChargeBar.InitUIBar(0, pointMax, point);
            }
        }

        public void UpdateUnitUIBar(bool isIgnoreTimer = false)
        {
            if (!isIgnoreTimer)
            {
                if (!timer.CheckTimer())
                {
                    return;
                }
            }

            //UI처리
            targetUnit.unitAbilityModule.GetUnitStatusValue(Ability.UnitStatusType.HealthPoint, out var point);
            targetUnit.unitAbilityModule.GetUnitStatusValue(Ability.UnitStatusType.HealthMaxPoint, out var pointMax);

            if (hpBar.GetCurValue() != point)
            {
                hpBar.SetCurValue(point);
            }
            if (hpBar.GetMaxValue() != pointMax)
            {
                hpBar.SetMaxValue(pointMax);
            }

            if (!isBattleUnit)
            {
                return;
            }

            point = battleUnitObject.skillChargeCurValue;
            if (skillChargeBar.GetCurValue() != point)
            {
                skillChargeBar.SetCurValue(point);
            }
        }
    }
}