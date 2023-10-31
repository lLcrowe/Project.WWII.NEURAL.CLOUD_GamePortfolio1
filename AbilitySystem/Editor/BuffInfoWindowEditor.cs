using lLCroweTool.Ability;
using lLCroweTool.BuffSystem;
using UnityEditor;
using UnityEngine;

namespace lLCroweTool.QC.EditorOnly
{
    public class BuffInfoWindowEditor : CustomDataWindowEditor<BuffInfo>
    {

        [MenuItem("lLcroweTool/BuffInfoWindowEditor")]
        public static void ShowWindow()
        {
            SetShowWindowSetting(typeof(BuffInfoWindowEditor));
        }


        private static BuffUnitStatusValue buffUnitStatusValue_Static = new ();
        private static BuffUnitStateValue buffUnitStateValue_Static = new ();

       


        protected override void DataDisplaySection(ref BuffInfo targetData)
        {
            lLcroweUtilEditor.IconLabelBaseDataShow(DataContentName, targetData);
            targetData.buffKategorie = (BuffKategorie)EditorGUILayout.EnumPopup("버프카테고리", targetData.buffKategorie);

            
            targetData.isUseApplyNeedCondition = EditorGUILayout.Toggle("버프를 받을수 있는 조건",targetData.isUseApplyNeedCondition);

            if (targetData.isUseApplyNeedCondition)
            {
                lLcroweUtilEditor.NeedBibleShow(targetData.applyNeedBible);
            }
            


            EditorGUILayout.BeginHorizontal();
            targetData.isUseTimeBuff = EditorGUILayout.Toggle("시간버프여부", targetData.isUseTimeBuff);

            if (targetData.isUseTimeBuff)
            {
                targetData.buffDurationTime = EditorGUILayout.FloatField("버프의 지속시간", targetData.buffDurationTime);
            }
            EditorGUILayout.EndHorizontal();


            if (targetData.isUseApplyNeedCondition)
            {   
                targetData.isUseTimeBuffDisableBuffCondition = EditorGUILayout.Toggle("시간버프 일시 특정 조건에 자동적으로 사라지는 설정", targetData.isUseTimeBuffDisableBuffCondition);

                if (targetData.isUseTimeBuffDisableBuffCondition)
                {
                    lLcroweUtilEditor.NeedBibleShow(targetData.disableBuffConditionBible);
                }
            }

            EditorGUILayout.BeginHorizontal();
            targetData.isStackBuff = EditorGUILayout.Toggle("스택버프여부", targetData.isStackBuff);

            if (targetData.isStackBuff)
            {
                //스택 최소 수량은 1
                if (targetData.maxStack < 1)
                {
                    targetData.maxStack = 1;
                }

                targetData.maxStack = EditorGUILayout.IntField("최대스택", targetData.maxStack);
            }
            EditorGUILayout.EndHorizontal();


            lLcroweUtilEditor.BuffUnitStatusValueShow(buffUnitStatusValue_Static);

            if (GUILayout.Button("적용할 스탯버프추가하기"))
            {
                if (targetData.buffStatusBible.ContainsKey(buffUnitStatusValue_Static.unitStatusValue.unitStatusType))
                {
                    Debug.Log("키가 중복됩니다");
                    return;
                }
                targetData.buffStatusBible.Add(buffUnitStatusValue_Static.unitStatusValue.unitStatusType, buffUnitStatusValue_Static);
                buffUnitStatusValue_Static = new();
                targetData.buffStatusBible.SyncDictionaryToList();
            }

            lLcroweUtilEditor.DictionaryShow(targetData.buffStatusBible);


            
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            buffUnitStateValue_Static.unitStateType = (UnitStateType)EditorGUILayout.EnumPopup("집어넣을 상태버프타입", buffUnitStateValue_Static.unitStateType);
            buffUnitStateValue_Static.value = EditorGUILayout.Toggle(buffUnitStateValue_Static.value);
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("적용할 상태버프추가하기"))
            {
                if (targetData.buffStateBible.ContainsKey(buffUnitStateValue_Static.unitStateType))
                {
                    Debug.Log("키가 중복됩니다");
                    return;
                }
                targetData.buffStateBible.Add(buffUnitStateValue_Static.unitStateType, buffUnitStateValue_Static);
                buffUnitStateValue_Static = new();
                targetData.buffStateBible.SyncDictionaryToList();
            }
            lLcroweUtilEditor.DictionaryShow(targetData.buffStateBible);

        }

        protected override void SetDataContentName(ref string dataContentName)
        {
            dataContentName = "버프정보";
        }

        protected override void SetSaveFileData(ref string labelNameOrTitle, ref string tag, ref string folderName)
        {
            labelNameOrTitle = targetData.labelNameOrTitle;
            tag = "";
            folderName = "BuffInfo";
        }
    }
}