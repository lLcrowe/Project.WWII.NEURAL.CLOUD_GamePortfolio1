using lLCroweTool.QC.EditorOnly;
using lLCroweTool.UI.Bar;
using UnityEditor;
using UnityEngine;

namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(NeuralCloudUIBar))]
    public class NeuralCloudUIBarInspectorEditor : UIBar_SlideInspectorEditor
    {

        private NeuralCloudUIBar targetUIBar_Slide;

        protected override void InitAddFunc()
        {
            base.InitAddFunc();
            targetUIBar_Slide = (NeuralCloudUIBar)target;
        }

        protected override bool CheckAutoGenerate(ref string content)
        {
            bool isCheck = base.CheckAutoGenerate(ref content);

            content += "-=NeuralCloudUIBar 필요사항=-\n";

            if (targetUIBar_Slide.followfill == null)
            {
                content += "쫒아가는 이미지가 없습니다.(첫번째이미지로 배치)";
                isCheck = true;
            }

            return isCheck;
        }

        protected override void AutoGenerateSection()
        {
            base.AutoGenerateSection();
            targetUIBar_Slide.gameObject.name = "NeuralCloudUIBar";
        }

        protected override void DisplaySection()
        {
            base.DisplaySection();
        }

    }
}