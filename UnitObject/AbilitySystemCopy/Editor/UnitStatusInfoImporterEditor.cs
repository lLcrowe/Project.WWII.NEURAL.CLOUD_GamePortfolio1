//20230813
//여기는 더이상 필요는 없는데
//일단 쓸곳이 있을거 같아서 놓아둠




//using lLCroweTool.Ability;
//using lLCroweTool.Ability.Util;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//namespace lLCroweTool.QC.EditorOnly
//{
//    public class UnitStatusInfoImporterEditor : EditorWindow
//    {
//        //어빌리티만 따로 하기 위해 에디터를 분리
//        //저장데이터
//        protected string tag;
//        protected string folderName;
//        protected TextAsset unitStatusInfoTextAsset;
//        private bool isRunCoroutine = false;

//        [MenuItem("lLcroweTool/UnitStatusInfoImporterEditor")]
//        public static void ShowWindow()
//        {
//            EditorWindow editorWindow = GetWindow(typeof(UnitStatusInfoImporterEditor));
//            editorWindow.titleContent.text = "유닛스탯정보 임포터";
//            editorWindow.minSize = new Vector2(250, 60);
//            editorWindow.maxSize = new Vector2(250, 60);
//        }

//        private void OnGUI()
//        {   
//            DataDisplaySection();
//        }

//        //데이터보는구역
//        protected void DataDisplaySection()
//        {   
//            unitStatusInfoTextAsset= (TextAsset)EditorGUILayout.ObjectField("유닛스탯정보 텍스트파일", unitStatusInfoTextAsset, typeof(TextAsset), true);

//            if (GUILayout.Button("Import"))
//            {
//                if (isRunCoroutine)
//                {
//                    return;
//                }
//                EditorCoroutine editorCoroutine = new EditorCoroutine(ImportUnitStatusInfoTextSheet());
//            }
//        }

//        /// <summary>
//        /// 임포트함수
//        /// </summary>
//        private IEnumerator ImportUnitStatusInfoTextSheet()
//        {
//            isRunCoroutine = true;
//            List<Dictionary<string, object>> dataList = lLcroweUtilEditor.CSVReader.Read(unitStatusInfoTextAsset);
//            var statusTypeArray = lLcroweUtil.GetEnumDefineData<UnitStatusType>();
//            List<UnitStatusValue> unitStatusValueList = new List<UnitStatusValue>(100);

//            var stateTypeArray = lLcroweUtil.GetEnumDefineData<UnitStateType>();

//            foreach (var item in dataList)
//            {
//                //정의부분
//                UnitStatusInfo unitStatusInfo = new UnitStatusInfo();
//                unitStatusInfo.labelID = (string)item["UnitObjectID"];
//                unitStatusInfo.icon = (Sprite)item["iconSprite"];
//                unitStatusInfo.labelNameOrTitle = (string)item["objectName"];
//                unitStatusInfo.description = (string)item["description"];



//                //이부분들은 엑셀로 데이터를 만들지 않으면 감이 잘 안오는데
//                //일단 만들고 하자

//                //20230720
//                //만들다 만거 같고 
//                //데이터부분을 체크해보니
//                //없는구역은 집어넣지 않게 처리
//                //엑셀부분에서 스탯부분과 설명부분을 분리시켯났으니 이걸합치는 CSV구역도 필요한거같다.
//                //그래야지만 여기서 임포트가능


//                //스탯부분//존재하는 구역만 집어넣자

//                unitStatusValueList.Clear();
                
//                for (int i = 0; i < statusTypeArray.Count; i++)
//                {
//                    string indexID = statusTypeArray[i].ToString();



//                    if (!item.TryGetValue(indexID, out object value))
//                    {
//                        continue;
//                    }


//                    var tempType = AbilityUtil.GetUnitStatusValueCATType(statusTypeArray[i]);
//                    float fValue = 0;                    
//                    switch (tempType)
//                    {
//                        case UnitStatusValueCATType.Int:
//                            if (!int.TryParse((string)value, out int intValue))
//                            {
//                                continue;
//                            }
//                            fValue = intValue;
//                            break;
//                        case UnitStatusValueCATType.Float:
//                            if (!float.TryParse((string)value, out float floatValue))
//                            {
//                                continue;
//                            }
//                            fValue = floatValue;
//                            break;
//                    }
//                    UnitStatusValue tempValue = new UnitStatusValue();
//                    tempValue.unitStatusType = statusTypeArray[i];
//                    tempValue.value = fValue;

//                    unitStatusValueList.Add(tempValue);
//                }
//                unitStatusInfo.unitStatusArray = unitStatusValueList.ToArray();


//                //상태부분//이건 그냥 다 집어넣자
//                for (int i = 0; i < stateTypeArray.Count; i++)
//                {
//                    unitStatusInfo.infoUnitStateApplyBible.Add(stateTypeArray[i], false);
//                }




//                //제작
//                lLcroweUtilEditor.CreateDataObject(ref unitStatusInfo, unitStatusInfo.labelNameOrTitle, "GameDataBaseFolder/UnitStatusInfo", null, false);
//                yield return null;
//            }
//            isRunCoroutine = false;
//        }
//    }
//}
