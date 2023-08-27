using lLCroweTool.Anim.GunRecoil;
using lLCroweTool.Anim.GunRecoil.Muzzle;
using UnityEditor;
using UnityEngine;

namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(GunRecoilAnim))]

    public class GunRecoilAnimInspectorEditor : Editor
    {
        private GunRecoilAnim targetObject;

        private void OnEnable()
        {
            targetObject = (GunRecoilAnim)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("자식에서 리코일타겟찾기"))
            {
                var tempTrArray  = lLcroweUtilEditor.GetChildComponent<GunRecoilAnimTarget>(targetObject);
                targetObject.gunRecoilAnimTargetArray = tempTrArray;
            }

            if (!Application.isPlaying)
            {
                return;
            }
            if (GUILayout.Button("ActionRecoil"))
            {
                targetObject.ActionRecoil();
            }
        }
    }
}