using DG.Tweening;
using lLCroweTool.NoticeDisplay;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using static lLCroweTool.NoticeDisplay.NoticeDisplayUI;

namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(NoticeDisplayUI))]
    public class NoticeDisplayUIInspectorEditor : Editor
    {
        private NoticeDisplayUI noticeDisplayUI;
        private bool isModifyShowPos;
        private bool isModifyShowSettingPos;
        private bool isModifyHideSettingPos;


        private void OnEnable()
        {
            noticeDisplayUI = target as NoticeDisplayUI;
        }
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("옐로 : 보여주는 위치\n레드 : 시작위치\n그린 : 숨는위치", MessageType.Info);
            //if (noticeDisplayUI.transform != noticeDisplayUI.showPos)
            //{
            //    ShowModifyButton("Pos ", ref isModifyShowPos);
            //}            
            ShowModifyButton("Show ", ref isModifyShowSettingPos);
            ShowModifyButton("Hide ", ref isModifyHideSettingPos);
            base.OnInspectorGUI();
            SceneView.RepaintAll();
        }

        private void ShowModifyButton(string content, ref bool value)
        {            
            string temp = value ? "On" : "Off";

            if (GUILayout.Button($"{content} Modify -={temp}=-"))
            {
                value = !value;
            }
        }

        private void OnSceneGUI()
        {
            Transform showPos = noticeDisplayUI.transform;
            if (showPos == null)
            {
                return;
            }
            
            if (isModifyShowPos)
            {
                showPos.position = Handles.DoPositionHandle(showPos.position, Quaternion.identity);
            }

            if (noticeDisplayUI.noticeUIPrefab == null)
            {
                return;
            }
            RectTransform prefabRect = noticeDisplayUI.noticeUIPrefab.transform as RectTransform;
            

            Vector2 pos = showPos.position;
            Vector2 size = prefabRect.sizeDelta;

            //로컬임
            //시작할떄 어디서 보여주는지
            DoSetting showSetting = noticeDisplayUI.showSetting;            
            Vector2 batchPos = showSetting.batchPos;
            Vector2 showBatchPos = showSetting.batchPos;
            Vector2 startSize = showSetting.startSize;
            Vector2 endSize = showSetting.endSize;

            float distance = noticeDisplayUI.distance;
            Vector2 dir = GetDirectionNorVector(noticeDisplayUI.showMoveType);


            Vector2 addPos = dir * distance * (noticeDisplayUI.showMaxCount - 1);
            if (isModifyShowSettingPos)
            {
                showSetting.batchPos = Handles.DoPositionHandle(pos + showSetting.batchPos + addPos, Quaternion.identity) - (Vector3)pos - (Vector3)addPos;
            }


            //보이는 위치들
            Handles.color = Color.yellow;
            
            for (int i = 0; i <noticeDisplayUI.showMaxCount; i++)
            {   
                Vector2 newPos = pos + (dir * i * distance);
                Handles.Label(newPos - Vector2.right * 100, $"{i} 세션");
                Handles.DrawWireCube(newPos, size);
            }

            //사라질때 어디로 사라지는지
            DoSetting hideSetting = noticeDisplayUI.hideSetting;
            batchPos = hideSetting.batchPos;
            startSize = hideSetting.startSize;
            endSize = hideSetting.endSize;

            if (isModifyHideSettingPos)
            {
                hideSetting.batchPos = Handles.DoPositionHandle(pos + hideSetting.batchPos, Quaternion.identity) - (Vector3)pos;                
            }

            Handles.Label(pos + showBatchPos + addPos + Vector2.right * 100, "Show 구역");
            Handles.color = Color.red;//시작
            Handles.DrawWireCube(pos + showBatchPos + addPos, size * startSize);

            Handles.Label(pos + batchPos + Vector2.right * 100, "Hide 구역");
            Handles.color = Color.green;//마지막
            Handles.DrawWireCube(pos + batchPos, size * endSize);
        }
    }
}