//using lLCroweTool.Localize;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//namespace lLCroweTool.QC.EditorOnly
//{
//    [CustomEditor(typeof(LocalizeDBObjectScript))]
//    public class LocalizeDataCSVEditor : Editor
//    {
//        LocalizeDBObjectScript targetLocalizeDBData;
//        private List<Dictionary<string, object>> targetCSVDataBible = new List<Dictionary<string, object>>();

//        private static string fileName = "";
//        private string path = "Resources";
//        private bool isExist = false;

//        void OnEnable()
//        {
//            targetLocalizeDBData = (LocalizeDBObjectScript)target;
//            //GetData();
//        }

//        private void GetData()
//        {
//            targetCSVDataBible.Clear();
//            //TextAsset[] textAssetArray = lLcroweUtilEditor.GetCustomUnityObject<TextAsset>.GetUnityObjectFile(path, "*.csv");
//            //string temp = "";
//            //if (textAssetArray.Length == 0)
//            //{
//            //    return;
//            //}
//            //else if(textAssetArray.Length == 1)
//            //{

//            //}
//            //else
//            //{
//            //    for (int i = 0; i < textAssetArray.Length; i++)
//            //    {
//            //        temp += textAssetArray[i].name + "\n";
//            //    }
//            //}
           

//            //Debug.Log(Application.dataPath + "/" + path + "/" + fileName);
//            targetCSVDataBible = lLcroweUtilEditor.CSVReader.Read("", fileName, ref isExist);//현재는 리소스에서됨
//        }

//        public override void OnInspectorGUI()
//        {
//            //objectName = serializedObject.FindProperty("objectName");
            
//            fileName = EditorGUILayout.TextField("파일이름", fileName);
//            path = EditorGUILayout.TextField("경로", path);

//            if (GUILayout.Button("CSV데이터 새로고침"))
//            {
//                GetData();
//            }
            

//            if (!isExist)
//            {
//                EditorGUILayout.HelpBox("경로에 데이터파일이 존재하지 않습니다.", MessageType.Warning);
//            }

//            //아이디string_ID
//            //한국어kor
//            //영어eng

//            //딕셔너리//딕셔너리로 가져옴
//            //줄, 헤더 데이터

//            GUILayout.BeginHorizontal();
//            if (GUILayout.Button("데이터 불려오기"))
//            {
//                //CSV데이터 불려오기
//                LocalizeDataBible localizeDataBible = targetLocalizeDBData.localizeDataBible;
//                localizeDataBible.Clear();

//                //줄수에맞게 카운트
//                for (int i = 0; i < targetCSVDataBible.Count; i++)
//                {
//                    int index = i;
//                    //Debug.Log(targetCSVDataBible[index]["string_ID"].ToString());
//                    if (localizeDataBible.ContainsKey(targetCSVDataBible[index]["string_ID"].ToString()))
//                    {
//                        //존재하는가
//                        //현재는 없음
//                    }
//                    else
//                    {
//                        //존재하지않는가
//                        //존재하지않으면 추가
//                        LocalizeData localizeData = new LocalizeData();
//                        localizeData.string_ID = targetCSVDataBible[index]["string_ID"].ToString();
//                        localizeData.kor = targetCSVDataBible[index]["kor"].ToString();
//                        localizeData.eng = targetCSVDataBible[index]["eng"].ToString();

//                        localizeDataBible.Add(localizeData.string_ID, localizeData);
//                    }
//                }
//            }

//            if (GUILayout.Button("저장하기"))
//            {
//                //기존거 위에 덮어쓰기
//                //기존거 안지움
//                //LocalizeDataBible => CSV
//                LocalizeDataBible localizeDataBible = targetLocalizeDBData.localizeDataBible;

//                //존재하면 현데이터를 해당라인의 CSV에 저장
//                List<string[]> lineDataList = new List<string[]>();
//                //헤더키값집어넣기
//                string[] tempID =
//                {
//                    "string_ID",
//                    "kor",
//                    "eng"
//                };
//                lineDataList.Add(tempID);

//                foreach (var localData in localizeDataBible)
//                {
//                    //신규데이터삽입
//                    string[] newTarget =
//                    {
//                        localData.Value.string_ID,
//                        localData.Value.kor,
//                        localData.Value.eng
//                    };

//                    lineDataList.Add(newTarget);
//                }
//                //Debug.Log(Application.dataPath + path + "/" + fileName);
//                lLcroweUtilEditor.CSVWritter.WriteCsv(lineDataList, Application.dataPath + "/" + path, fileName);
//                GetData();
              
//            }
//            GUILayout.EndHorizontal();
//            base.OnInspectorGUI();
//        }
//    }
//}
