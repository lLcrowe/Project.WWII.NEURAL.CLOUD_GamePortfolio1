
using UnityEngine;
using lLCroweTool.SkillSystem;
#if UNITY_EDITOR
using UnityEditor;
#pragma warning disable 0618

namespace lLCroweTool.QC.EditorOnly
{
    public class SkillDataEditor : CustomDataWindowEditor<SkillObjectScript>
    {
        //데이터폴더 네이밍
        //스킬이름_Active_Targeting
        //스킬이름_Active_Myself
        //스킬이름_Passive_Targeting
        //스킬이름_Passive_Myself

        [MenuItem("lLcroweTool/SkillDataEditor")]
        public static void ShowWindow()
        {
            SetShowWindowSetting(typeof(SkillDataEditor));
        }

        protected override void SetDataContentName(ref string dataContentName)
        {
            dataContentName = "스킬";
        }

        protected override void SetSaveFileData(ref string labelNameOrTitle, ref string tag, ref string folderName)
        {
            labelNameOrTitle = targetData.labelNameOrTitle;
            tag = targetData.skillCATType.ToString();
            folderName = "SkillDataFolder";
        }

        protected override void DataDisplaySection(ref SkillObjectScript targetData)
        {
            lLcroweUtilEditor.IconLabelBaseDataShow("스킬", targetData);
            //targetData.labelNameOrTitle = EditorGUILayout.TextField("스킬데이터 이름", targetData.labelNameOrTitle);
            //EditorGUILayout.LabelField("스킬데이터 아이콘");
            //targetData.icon = (Sprite)EditorGUILayout.ObjectField(targetData.icon, typeof(Sprite));
            //EditorGUILayout.LabelField("스킬데이터 상세설명");
            //targetData.description = EditorGUILayout.TextArea(targetData.description, GUILayout.Height(100));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("스킬 세팅");
            targetData.skillCATType = (SkillCATType)EditorGUILayout.EnumPopup("스킬카테고리", targetData.skillCATType);
            targetData.skillCooltime = EditorGUILayout.FloatField("스킬 재사용시간", targetData.skillCooltime);
            targetData.isMoveCancel = EditorGUILayout.Toggle("움직일시 캔슬되는 여부", targetData.isMoveCancel);
            //skillRange = EditorGUILayout.FloatField("스킬 사거리", skillRange);
            //skillAreaSize = EditorGUILayout.FloatField("스킬범위 크기", skillAreaSize);//실질적인 스킬크기와 다르니 스킬포인터에서 다른방식으로 변경해보자

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("스택관련");
            targetData.isUseStack = EditorGUILayout.Toggle("스택을 사용하는가 여부", targetData.isUseStack);
            if (targetData.isUseStack)
            {
                targetData.stackMaxNum = EditorGUILayout.IntField("스택 최대량", targetData.stackMaxNum);
                targetData.stackingTimer = EditorGUILayout.FloatField("스택 쿨타임", targetData.stackingTimer);
            }


            EditorGUILayout.Space();
            targetData.castingTime = EditorGUILayout.FloatField("캐스팅 시간", targetData.castingTime);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("적용할 스킬오브젝트 프리팹");
            targetData.targetSkillPrefab = (Skill_Base)EditorGUILayout.ObjectField(targetData.targetSkillPrefab, typeof(Skill_Base));
        }
    }
}
#endif
