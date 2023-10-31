
using UnityEngine;
using UnityEditor;
using lLCroweTool.LevelSystem;
using System.Collections.Generic;
using System.Linq;

namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(LevelInfo))]
    public class LevelInfoEditor : CustomDataWindowEditor<LevelInfo>
    {
        private Vector2 levelScroll;
       
        GUILayoutOption[] levelCostScrollOptions = new[] {
        
        GUILayout.Height (300)
         };
        private bool showCheck = false;
        private AdditionalLevelUpData additionalLevelUpData = new AdditionalLevelUpData();
        private LabelBase needTargetData;
        private int needAmount;

        [MenuItem("lLcroweTool/LevelInfoEditor")]
        public static void ShowWindow()
        {
            SetShowWindowSetting(typeof(LevelInfoEditor));            
        }

        protected override void SetDataContentName(ref string dataContent)
        {
            dataContent = "레벨정보";
        }

        protected override void SetSaveFileData(ref string labelNameOrTitle, ref string tag, ref string folderName)
        {
            labelNameOrTitle = targetData.name;
            tag = "";
            folderName = "LevelInfoFolder";
        }

        protected override void DataDisplaySection(ref LevelInfo targetData)
        {           
            EditorGUILayout.Space();

            targetData.name = EditorGUILayout.TextField("레벨정보 이름(아이디)", targetData.name);
            EditorGUILayout.LabelField("레벨업시 타겟팅할 데이터");
            targetData.needExpTargetData = (LabelBase)EditorGUILayout.ObjectField(targetData.needExpTargetData, typeof(LabelBase), false);

            lLcroweUtilEditor.CurveDataShow(ref targetData.curveData, "경험치량");
            EditorGUILayout.Space();
            

            //레벨업 추가데이터관련
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("레벨업시 니드데이터관련");
            additionalLevelUpData.targetOverLapLevel = Mathf.Clamp(additionalLevelUpData.targetOverLapLevel, 1, targetData.curveData.MaxLevel);//제한체크

            EditorGUILayout.BeginHorizontal();
            additionalLevelUpData.targetOverLapLevel = EditorGUILayout.IntField("타겟팅될 레벨", additionalLevelUpData.targetOverLapLevel);
            additionalLevelUpData.isOverLap = EditorGUILayout.Toggle("덮어쓰기여부", additionalLevelUpData.isOverLap);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            needTargetData = (LabelBase)EditorGUILayout.ObjectField("필요데이터", needTargetData, typeof(LabelBase), false);
            needAmount = EditorGUILayout.IntField("필요 수량", needAmount);
            EditorGUILayout.EndHorizontal();

            var needDataGroup = additionalLevelUpData.needDataGroup;

            lLcroweUtilEditor.ArrayDataShow(needTargetData,needAmount,ref needDataGroup.needDataArray, ref needDataGroup.needAmountArray, "개");


            //if (GUILayout.Button("조건추가"))
            //{
            //    if (needDataGroup.needDataArray.Contains(needTargetData))
            //    {
            //        Debug.Log("이미존재하는 데이터입니다.");
            //        return;
            //    }

            //    if (needTargetData == null)
            //    {
            //        Debug.Log("비어있는 데이터입니다.");
            //        return;
            //    }

            //    List<LabelBase> tempList = new List<LabelBase>();
            //    List<int> tempIntList = new List<int>();

            //    tempList.AddRange(needDataGroup.needDataArray);
            //    tempIntList.AddRange(needDataGroup.needAmountArray);

            //    tempList.Add(needTargetData);
            //    tempIntList.Add(needAmount);

            //    needDataGroup.needDataArray = tempList.ToArray();
            //    needDataGroup.needAmountArray = tempIntList.ToArray();
            //}

            //EditorGUILayout.LabelField("-----------Start");
            //for (int i = 0; i < needDataGroup.needDataArray.Length; i++)
            //{
            //    //EditorGUILayout.BeginHorizontal();
            //    EditorGUILayout.LabelField(needDataGroup.needDataArray[i].ToString() + ":" + needDataGroup.needAmountArray[i] + "개");
            //    if (GUILayout.Button("삭제"))
            //    {
            //        List<LabelBase> tempList = new List<LabelBase>();
            //        List<int> tempIntList = new List<int>();

            //        tempList.AddRange(needDataGroup.needDataArray);
            //        tempIntList.AddRange(needDataGroup.needAmountArray);

            //        int value = i;
            //        tempList.RemoveAt(value);
            //        tempIntList.RemoveAt(value);

            //        needDataGroup.needDataArray = tempList.ToArray();
            //        needDataGroup.needAmountArray = tempIntList.ToArray();
            //    }
            //    //EditorGUILayout.EndHorizontal();
            //}            
            //EditorGUILayout.LabelField("-----------End");

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("바이블 추가"))
            {
                if (targetData.additionalLvUpBible.ContainsKey(additionalLevelUpData.targetOverLapLevel))
                {
                    Debug.Log("이미 중복된 레벨이 있습니다");
                    return;
                }
                AdditionalLevelUpData copyData = new AdditionalLevelUpData();
                copyData = lLcroweUtil.GetCopyOf(additionalLevelUpData, copyData);
                targetData.additionalLvUpBible.Add(copyData.targetOverLapLevel, copyData);
            }
            EditorGUILayout.EndHorizontal();




            if (GUILayout.Button("그래프데이터 보기"))
            {
                showCheck = !showCheck;
            }

            if (showCheck)
            {
                levelScroll = EditorGUILayout.BeginScrollView(levelScroll, levelCostScrollOptions);
                for (int i = 1; i <= targetData.curveData.MaxLevel; i++)
                {
                    EditorGUILayout.BeginHorizontal("Box");

                    EditorGUILayout.LabelField("Lv " + i);
                    EditorGUILayout.LabelField((int)targetData.GetLevelCurveIntValue(i) + "XP");

                    //바이블에 해당레벨이 있으면 보여주기//삭제하기 기능도 추가
                    //데이터보여주기
                    if (targetData.additionalLvUpBible.TryGetValue(i, out var data))
                    {

                        string arrayContent = "필요데이터\n";
                        for (int j = 0; j < data.needDataGroup.needAmountArray.Length; j++)
                        {
                            var tempData = data.needDataGroup.needDataArray[j];
                            string name = tempData == null ? "비어있습니다." : tempData.name;
                            arrayContent += $"{name} : {data.needDataGroup.needAmountArray[j]}개\n";
                        }

                        string levelContent = $"덮어쓸 레벨 : {data.targetOverLapLevel}";
                        string overLapContent = data.isOverLap ? "덮어쓰기" : "추가적";

                        //string content = $"{levelContent}, 덮어쓰기상태 : {overLapContent} -={arrayContent}";
                        string content = $"덮어쓰기상태 : {overLapContent} -={arrayContent}";
                        content = content.Substring(0, content.Length - 1);

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.HelpBox(content, MessageType.Info);

                        if (GUILayout.Button("바이블삭제"))
                        {
                            targetData.additionalLvUpBible.Remove(data.targetOverLapLevel);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
            }


            EditorGUILayout.Space();
            targetData.levelText = EditorGUILayout.TextField("포맷:레벨", targetData.levelText);
            targetData.maxLevelText = EditorGUILayout.TextField("포맷:최대레벨", targetData.maxLevelText);
            EditorGUILayout.LabelField("----------------------------");

            
          

            //데이터보여주기
            //foreach (var item in targetData.additionalLvUpBible)
            //{
            //    var data = item.Value;

            //    string arrayContent = "필요데이터\n";
            //    for (int i = 0; i < data.needDataGroup.needAmountArray.Length; i++)
            //    {
            //        var tempData = data.needDataGroup.needDataArray[i];
            //        string name = tempData == null ? "비어있습니다." : tempData.name;
            //        arrayContent += $"{name} : {data.needDataGroup.needAmountArray[i]}개\n";
            //    }

            //    string content = $"덮어쓸 레벨 : {data.targetOverLapLevel}, 덮어쓰기여부 : {data.isOverLap} \n{arrayContent}";
            //    content = content.Substring(0, content.Length - 1);

            //    EditorGUILayout.BeginHorizontal();
            //    EditorGUILayout.HelpBox(content, MessageType.Info);

            //    if (GUILayout.Button("바이블삭제"))
            //    {
            //        targetData.additionalLvUpBible.Remove(data.targetOverLapLevel);
            //        break;
            //    }
            //    EditorGUILayout.EndHorizontal();
            //}
        }
    }
}
