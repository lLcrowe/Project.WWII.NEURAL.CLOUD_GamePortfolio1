using lLCroweTool.UI.Line;
using UnityEditor;
using UnityEngine;

namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(ImageLine))]
    public class ImageLineInspectorEditor : Editor
    {
        private ImageLine targetImageLine;
        private RectTransform imageRect;

        private bool isModify = false;
        private void OnEnable()
        {
            targetImageLine = target as ImageLine;
            imageRect = targetImageLine.GetComponent<RectTransform>();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (imageRect == null)
            {
                return;
            }

            string content = isModify ? "Cancel" : "Modify";

            if (GUILayout.Button(content))
            {
                isModify = !isModify;
            }
            SetSize(imageRect, targetImageLine.point, targetImageLine.lineWidth);
            SceneView.RepaintAll();
        }

        public static void SetSize(RectTransform rectTransform, Vector3 targetPoint, float width)
        {
            Vector3 dir = GetRectDir(rectTransform, targetPoint);
            rectTransform.sizeDelta = new Vector2(dir.magnitude, width);    //width는 길이//height는 라인의 폭
            rectTransform.pivot = new Vector2(0, 0.5f);                     //피봇설정
        }

        public static void SetAngle(RectTransform rectTransform, Vector3 targetPoint)
        {
            //각도처리
            Vector3 dir = GetRectDir(rectTransform, targetPoint);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            rectTransform.rotation = Quaternion.Euler(0, 0, angle);
        }

        private static Vector3 GetRectDir(RectTransform rectTransform, Vector3 targetPoint)
        {
            Vector3 originPos = rectTransform.transform.position;
            return (originPos + targetPoint) - originPos;
        }


        private void OnSceneGUI()
        {
            if (isModify)
            {
                Vector3 originPos = targetImageLine.transform.position;
                targetImageLine.point = Handles.DoPositionHandle(originPos + targetImageLine.point, Quaternion.identity);
                targetImageLine.point -= originPos;
                SetAngle(imageRect, targetImageLine.point);
            }
        }
    }
}
