using UnityEngine;
using System.Collections.Generic;
using lLCroweTool.SkillSystem;
using lLCroweTool.TimerSystem;
#if UNITY_EDITOR
using UnityEditor;
#pragma warning disable 0618
namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(SkillToolBar))]
    public class SkillToolBarInspecterEditor : Editor
    {
        private SkillToolBar targetSkillToolbar;
        private List<SkillSlot> skillSlotList = new List<SkillSlot>();

        private void OnEnable()
        {
            targetSkillToolbar = (SkillToolBar)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            skillSlotList.Clear();
            skillSlotList.AddRange(targetSkillToolbar.skillSlotArray);

            if (GUILayout.Button("스킬슬롯 추가하기"))
            {
                SkillSlot tempSlot = new SkillSlot();
                skillSlotList.Add(tempSlot);
                targetSkillToolbar.skillSlotArray = skillSlotList.ToArray();

                //모듈장착
                //tempSlot.skillCooltimer = targetSkillToolbar.gameObject.AddComponent<CoolTimerModule>();
                //tempSlot.skillStackTimer = targetSkillToolbar.gameObject.AddComponent<CoolTimerModule>();
                //tempSlot.skillCastTimer = targetSkillToolbar.gameObject.AddComponent<CoolTimerModule>();
            }

            string content;
            for (int i = 0; i < skillSlotList.Count; i++)
            {   
                if (skillSlotList[i].GetSkillData() == null)
                {
                    int temp = i;
                    content = temp + " 비어있는 스킬 슬롯";
                }
                else
                {
                    content = skillSlotList[i].GetSkillData().labelNameOrTitle;
                }

                if (GUILayout.Button(content + " 스킬슬롯 삭제하기"))
                {
                    SkillSlot tempSlot = skillSlotList[i];
                    skillSlotList.Remove(tempSlot);
                    targetSkillToolbar.skillSlotArray = skillSlotList.ToArray();

                    //모노비헤이비어 상속안받아서 그대로 없애도됨
                    //if (tempSlot.skillCooltimer != null)
                    //{
                    //    //쿨타임모듈
                    //    DestroyImmediate(tempSlot.skillCooltimer.gameObject);
                    //}
                    //if (tempSlot.skillStackTimer != null)
                    //{
                    //    //스택모듈
                    //    DestroyImmediate(tempSlot.skillStackTimer.gameObject);
                    //}
                    //if (tempSlot.skillCastTimer != null)
                    //{
                    //    //캐스팅모듈
                    //    DestroyImmediate(tempSlot.skillCastTimer.gameObject);
                    //}

                    //tempSlot.InitSkillSlot(null);
                    //tempSlot.skillData = null;
                    //tempSlot.skillCooltimer = null;
                    //tempSlot.skillStackTimer = null;
                    //tempSlot.skillCastTimer = null;
                    //tempSlot.SetSkillPrefab(null);
                    //tempSlot.targetWorldObjectList.Clear();
                }
            }
        }
    }
}
#endif