using lLCroweTool.UI.InfinityScroll.Scroll;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(CustomInfinityScroll))]
    //[CanEditMultipleObjects]
    public class CustomInfinityScrollEditor : Editor
    {
        private CustomInfinityScroll customScroll;
        private ScrollRect scroll;
        private RectTransform contentRect;
        private RectTransform viewportRect;

        private void OnEnable()
        {
            customScroll = (CustomInfinityScroll)target;

            if (customScroll.TryGetComponent(out scroll))
            {
                contentRect = scroll.content;
                viewportRect = scroll.viewport;
            }

            if (viewportRect != null)
            {
                viewportRect.anchorMin = Vector2.up;
                viewportRect.anchorMax = Vector2.up;
            }
            if (contentRect != null)
            {
                contentRect.anchorMin = Vector2.up;
                contentRect.anchorMax = Vector2.up;
            }
        }


        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("옐로 : 보여주는 위치\n레드 : 마스크크기\n그린 : 컨텐츠크기", MessageType.Info);
            //EditorGUILayout.HelpBox("옐로 : 보여주는 위치", MessageType.Info);
            //EditorGUILayout.HelpBox("레드 : 마스크크기", MessageType.Info);
            //EditorGUILayout.HelpBox("그린 : 컨텐츠크기", MessageType.Info);
            base.OnInspectorGUI();
        }

        private void OnSceneGUI()
        {

            if (scroll == null)
            {
                return;
            }

            if (contentRect == null)
            {
                return;
            }

            if (viewportRect == null)
            {
                return;
            }

            if (customScroll.cellPrefab == null)
            {
                return;
            }

            RectTransform cellRect = customScroll.cellPrefab.transform as RectTransform;
            float cellHeight = cellRect.sizeDelta.y;
            float cellWidth = cellRect.sizeDelta.x;

            float spacing = customScroll.spacingValue;

            int cellCapacity = customScroll.cellCapacity;
            int showCellLineAmount = customScroll.showCellLineAmount;
            int showCellAmount = customScroll.showCellAmount;



            //노란색이 보여주는 위치들
            //빨간색이 뷰포트
            //초록색이 컨텐츠

            //수평수직에따라
            //높이 넓이에 따라
            //보여주는 라인에 따라




            Handles.color = Color.yellow;
            switch (customScroll.scrollType)
            {
                case CustomInfinityScroll.EScrollType.Vertical:
                    float y = ((cellHeight + spacing) * showCellAmount) / showCellLineAmount;
                    contentRect.sizeDelta = new Vector2(cellWidth * showCellLineAmount + spacing, y);
                    break;
                case CustomInfinityScroll.EScrollType.Horizontal:
                    float x = ((cellWidth + spacing) * showCellAmount) / showCellLineAmount;
                    contentRect.sizeDelta = new Vector2(x, cellHeight * showCellLineAmount + spacing);
                    break;
            }
            Handles.DrawAAPolyLine(lLcroweUtilEditor.ConvertBoxVectorForAA(contentRect));

            //viewPort
            Handles.color = Color.red;
            Handles.DrawAAPolyLine(lLcroweUtilEditor.ConvertBoxVectorForAA(viewportRect));

            //content
            Vector2 size = Vector2.zero;
            bool isOdd = false;
            if (showCellLineAmount != 1)
            {
                isOdd = showCellLineAmount % 2 != 0 ? true : false;
            }
            float addValue = 0;
            switch (customScroll.scrollType)
            {
                case CustomInfinityScroll.EScrollType.Vertical:
                    
                    float y = ((cellHeight + spacing) * cellCapacity) / showCellLineAmount;
                    //여기서 어덯게 처리해서 고쳐야됨 특정 홀수일때 문제발생되는거//딱맞게 안맞음
                    addValue = isOdd ? /*(cellHeight + spacing) -*/ (cellHeight + spacing) : 0; 

                    size = new Vector2(cellWidth * showCellLineAmount + spacing, y + addValue);
                    break;
                case CustomInfinityScroll.EScrollType.Horizontal:
                    float x = ((cellWidth + spacing) * cellCapacity) / showCellLineAmount;
                    addValue = isOdd ?/* (cellWidth + spacing) -*/ (cellWidth + spacing / showCellLineAmount) : 0;

                    size = new Vector2(x + addValue, cellHeight * showCellLineAmount + spacing);
                    break;
            }
            contentRect.sizeDelta = size;

            Handles.color = Color.green;
            Handles.DrawAAPolyLine(lLcroweUtilEditor.ConvertBoxVectorForAA(contentRect));
        }
    }
}