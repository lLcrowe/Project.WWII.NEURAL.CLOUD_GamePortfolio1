using Doozy.Engine.Progress;
using lLCroweTool.UI.Bar;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(UIBar_Doozy))]
    public class UIBar_DoozyInspectorEditor : UIBarInspectorEditor
    {
        private UIBar_Doozy targetUIBar_Doozy;
        //프로그래셔 이미지는 필수
        protected override void InitAddFunc()
        {
            base.InitAddFunc();
            targetUIBar_Doozy = (UIBar_Doozy)target;
        }

        protected override bool CheckAutoGenerate(ref string content)
        {
            bool isCheck = base.CheckAutoGenerate(ref content);

            content += "-=UIBar_Doozy 필요사항=-\n";

            if (targetUIBar_Doozy.progressor == null)
            {
                content += "프로그래서가 없습니다.\n";
                isCheck = true;
            }
            else
            {
                if (targetUIBar_Doozy.progressor.ProgressTargets == null)
                {
                    content += "프로그래서 리스트 타겟이 비어있습니다.\n";
                    isCheck = true;
                }
                else if (targetUIBar_Doozy.progressor.ProgressTargets.Count == 0) 
                {
                    content += "프로그래서 리스트에 아무것도 없습니다.\n";
                    isCheck = true;
                }
            }


            if (targetUIBar_Doozy.fillProgressTarget == null)
            {
                content += "채우기프로그래스 타겟이 없습니다.\n";
                isCheck = true;
            }
            else
            {
                if (targetUIBar_Doozy.fillProgressTarget.Image == null)
                {
                    content += "채우기프로그래스 타겟에 이미지가 설정이 안되있습니다.\n";
                    isCheck = true;
                }
            }

            return isCheck;
        }

        protected override void AutoGenerateSection()
        {
            base.AutoGenerateSection();
            targetUIBar_Doozy.gameObject.name = "UIBar_Doozy";

            AutoGenerateUIBarDoozy(targetUIBar_Doozy);
        }

        public static void AutoGenerateUIBarDoozy(UIBar_Doozy uIBar_Doozy)
        {
            //프로그래셔
            if (uIBar_Doozy.progressor == null)
            {
                uIBar_Doozy.progressor = uIBar_Doozy.gameObject.AddComponent<Progressor>();
                uIBar_Doozy.progressor.ProgressTargets = new System.Collections.Generic.List<ProgressTarget>();
            }

            //프로그래서타겟
            if (uIBar_Doozy.fillProgressTarget == null)
            {
                //새로생성할때 해당게임오브젝트의 이미지컴포넌트를 자동 타겟팅함
                uIBar_Doozy.fillProgressTarget = uIBar_Doozy.fill.gameObject.AddComponent<ProgressTargetImage>();
            }
            if (uIBar_Doozy.fillProgressTarget.Image == null)
            {
                uIBar_Doozy.fillProgressTarget.Image = uIBar_Doozy.fill;
            }

            if (uIBar_Doozy.progressor.ProgressTargets.Count == 0)
            {
                //리스트처리
                uIBar_Doozy.progressor.ProgressTargets.Add(uIBar_Doozy.fillProgressTarget);
            }
        }

        protected override void DisplaySection()
        {
            base.DisplaySection();
            EditorGUILayout.HelpBox("필요에 따라 프로그래스타겟을 추가하여 비쥬얼적 요소를 줄것", MessageType.Info);
        }
    }
}