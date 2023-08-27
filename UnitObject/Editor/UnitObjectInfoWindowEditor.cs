using lLCroweTool.Ability;
using lLCroweTool.DataBase;
using lLCroweTool.SkillSystem;
using UnityEditor;

namespace lLCroweTool.QC.EditorOnly
{
    public class UnitObjectInfoWindowEditor : CustomDataWindowEditor<UnitObjectInfo>
    {
        public SkillObjectScript skillObjectScript;
        public UnitStatusValue newUnitStatusValue;
        public UnitStateType newUnitStateType;

        [MenuItem("lLcroweTool/UnitObjectEditor")]
        public static void ShowWindow()
        {
            SetShowWindowSetting(typeof(UnitObjectInfoWindowEditor));
        }

        protected override void SetDataContentName(ref string dataContentName)
        {
            dataContentName = "유닛";
        }
        
        protected override void SetSaveFileData(ref string labelNameOrTitle, ref string tag, ref string folderName)
        {
            labelNameOrTitle = targetData.labelNameOrTitle;
            tag = "";
            folderName = "UnitObjectInfo";//데이터베이스와 동일한 처리
        }

        protected override void DataDisplaySection(ref UnitObjectInfo targetData)
        {
            lLcroweUtilEditor.UnitObjectInfoShow(ref targetData, ref skillObjectScript, ref newUnitStatusValue, ref newUnitStateType);
        }
    }
}