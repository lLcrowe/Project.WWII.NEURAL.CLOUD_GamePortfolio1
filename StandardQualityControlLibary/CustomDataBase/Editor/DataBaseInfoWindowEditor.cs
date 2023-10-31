using lLCroweTool.Ability;
using lLCroweTool.Ability.Util;
using lLCroweTool.DataBase;
using lLCroweTool.Effect;
using lLCroweTool.ScoreSystem;
using lLCroweTool.UI.Bar;
using lLCroweTool.UI.UIThema;
using lLCroweTool.UnitBatch;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using static lLCroweTool.DataBase.AchievementInfo;

namespace lLCroweTool.QC.EditorOnly
{
    public class DataBaseInfoWindowEditor : EditorWindow
    {
        //주목적은 
        //DataBaseInfo에 CSV파일에 있는 데이터를 임포트시켜서
        //신규오브젝트들을 생성해주는 것

        //다른곳에 팝업을 할예정이긴함
        //그냥 인스팩터에디터로 하는게 편할듯

        protected Vector2 scollPos;
        public static DataBaseInfo targetDataBaseInfo;

        //저장데이터
        protected string tag;
        protected string folderName;

        List<string> pathTypeContentList = new List<string>();
        string findSearch = "NullSprite";

        [MenuItem("lLcroweTool/DataBaseInfoWindowEditor")]
        public static void ShowWindow()
        {
            EditorWindow editorWindow = GetWindow(typeof(DataBaseInfoWindowEditor));
            editorWindow.titleContent.text = "게임데이터베이스 관리자";
            editorWindow.minSize = new Vector2(515, 570);
            editorWindow.maxSize = new Vector2(515, 570);
        }
   

        private void OnEnable()
        {
            folderName = "GameDataBaseFolder";
            pathTypeContentList = lLcroweUtil.GetEnumDefineStringData<lLcroweUtil.CATPathType>();
        }

        private void OnGUI()
        {
            //데이터어덯게할지체크
            targetDataBaseInfo = (DataBaseInfo)EditorGUILayout.ObjectField("현재선택된 게임 데이터베이스", targetDataBaseInfo, typeof(DataBaseInfo), true);
            if (targetDataBaseInfo == null)
            {
                return;
            }
            scollPos = EditorGUILayout.BeginScrollView(scollPos, GUILayout.Height(545));
            DataDisplaySection();
            EditorGUILayout.EndScrollView();
        }

        //데이터보는구역
        protected void DataDisplaySection()
        {
            //이거 나중에 제대로 처리하자구

            string contentPath = $"리소스폴더에서 사용하는 경로\n" +
                $"Default : Resources\n" +
                $"Sprite : Resources/SpriteFolder\n";

            for (int i = 0; i < pathTypeContentList.Count; i++)
            {
                string content = pathTypeContentList[i];
                contentPath += $"{content} : Resources/Prefab/{content}Folder\n";
            }

            contentPath = contentPath.Substring(0, contentPath.Length - 1);
            EditorGUILayout.HelpBox(contentPath, MessageType.Info);


            //GUI.backgroundColor = Color.red;
            targetDataBaseInfo.itemInfoTextSheet = (TextAsset)EditorGUILayout.ObjectField("아이템CSV", targetDataBaseInfo.itemInfoTextSheet, typeof(TextAsset), false);
            targetDataBaseInfo.recordActionInfoTextSheet = (TextAsset)EditorGUILayout.ObjectField("기록소CSV", targetDataBaseInfo.recordActionInfoTextSheet, typeof(TextAsset), false);
            //GUI.backgroundColor = new Color32(194, 194, 194, 255);//new Color32(56, 56, 56, 255);
            targetDataBaseInfo.achievementInfoTextSheet = (TextAsset)EditorGUILayout.ObjectField("업적CSV", targetDataBaseInfo.achievementInfoTextSheet, typeof(TextAsset), false);
            targetDataBaseInfo.achievementConditionInfoTextSheet = (TextAsset)EditorGUILayout.ObjectField("업적조건CSV", targetDataBaseInfo.achievementConditionInfoTextSheet, typeof(TextAsset), false);            
            targetDataBaseInfo.achievementRewardTextSheet = (TextAsset)EditorGUILayout.ObjectField("업적보상CSV", targetDataBaseInfo.achievementRewardTextSheet, typeof(TextAsset), false);

            targetDataBaseInfo.searchMapInfoTextSheet = (TextAsset)EditorGUILayout.ObjectField("탐색맵CSV", targetDataBaseInfo.searchMapInfoTextSheet, typeof(TextAsset), false);
            targetDataBaseInfo.searchMapRewardTextSheet = (TextAsset)EditorGUILayout.ObjectField("탐색보상CSV", targetDataBaseInfo.searchMapRewardTextSheet, typeof(TextAsset), false);


            targetDataBaseInfo.MVPTextSheet = (TextAsset)EditorGUILayout.ObjectField("MVPCSV", targetDataBaseInfo.MVPTextSheet, typeof(TextAsset), false);
            targetDataBaseInfo.storeTextSheet = (TextAsset)EditorGUILayout.ObjectField("상점CSV", targetDataBaseInfo.storeTextSheet, typeof(TextAsset), false);
            targetDataBaseInfo.storePullSheet = (TextAsset)EditorGUILayout.ObjectField("상점뽑기CSV", targetDataBaseInfo.storePullSheet, typeof(TextAsset), false);
            targetDataBaseInfo.unitObjectTextSheet = (TextAsset)EditorGUILayout.ObjectField("유닛CSV", targetDataBaseInfo.unitObjectTextSheet, typeof(TextAsset), false);

            targetDataBaseInfo.iconPresetTextSheet = (TextAsset)EditorGUILayout.ObjectField("아이콘프리셋데이터", targetDataBaseInfo.iconPresetTextSheet, typeof(TextAsset), false);
            targetDataBaseInfo.uiThemaTextSheet = (TextAsset)EditorGUILayout.ObjectField("UI테마데이터", targetDataBaseInfo.uiThemaTextSheet, typeof(TextAsset), false);

            EditorGUILayout.Space();
            findSearch = EditorGUILayout.TextField("찾을거", findSearch);
            if (GUILayout.Button("CheckFind"))
            {
                bool check = targetDataBaseInfo.resourcesSpriteBible.TryGetValue(findSearch, out var target);
                string content = check ? target.GetType().ToString() : "Null";
                string combineContent = $"SpriteResources : {check} {content}";

                check = targetDataBaseInfo.resourcesObjectBible.TryGetValue(findSearch, out target);
                content = check ? target.GetType().ToString() : "Null";
                combineContent = lLcroweUtil.GetCombineString(combineContent, $"\nObjectResources : {check} {content}");

                Debug.Log(combineContent);
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("GetCSV"))
            {
                //추가할때마다 여기서 처리해주면 됨//20230920
                //13개
                //시트이름//파일이름
                var SheetDataArray = new SheetData[]
                {
                new SheetData("아이템시트","포폴1_아이템CSV", SheetType.Item),
                new SheetData("기록소", "포폴1_기록소CSV", SheetType.Record),

                new SheetData("업적데이터", "포폴1_업적CSV", SheetType.AchievementInfo),
                new SheetData("업적조건데이터", "포폴1_업적조건CSV",SheetType.AchievementCondition),
                new SheetData("업적보상데이터", "포폴1_업적보상CSV", SheetType.AchievementReward),

                new SheetData("탐색맵시트", "포폴1_탐색맵CSV", SheetType.SearchMap),
                new SheetData("탐색보상시트", "포폴1_탐색보상CSV",SheetType.SearchMapReward),

                new SheetData("MVP데이터", "포폴1_MVPCSV", SheetType.MVP),
                new SheetData("상점시트", "포폴1_상점CSV", SheetType.Store),
                new SheetData("상점뽑기시트", "포폴1_상점뽑기CSV", SheetType.StorePull),
                new SheetData("유닛데이터", "포폴1_유닛CSV", SheetType.UnitStat),

                new SheetData("IconPreset데이터", "포폴1_아이콘프리셋CSV", SheetType.Icon),
                new SheetData("UI테마데이터", "포폴1_UI테마CSV", SheetType.UIThema),
                };

                //리스트로 처리하기//동적임
                foreach (var item in SheetDataArray)
                {
                    var editorCoroutine = new EditorCoroutine(DataUpdate(item));
                }
            }

            if (GUILayout.Button("FindAllResources"))
            {
                targetDataBaseInfo.resourcesSpriteBible.Clear();
                targetDataBaseInfo.resourcesObjectBible.Clear();
                
                var typeList = lLcroweUtil.GetEnumDefineData<ResourcesSearchClassType>();

                foreach (var classType in typeList)
                {
                    DataBaseInfo.ResourcesBible tempBible = null;

                    //각타입에 맞게 처리
                    switch (classType)
                    {
                        case ResourcesSearchClassType.Sprite:
                            tempBible = lLcroweUtilEditor.GetResourcesForAllSearch<Sprite>();
                            foreach (var item in tempBible)
                            {
                                targetDataBaseInfo.resourcesSpriteBible.Add(item.Key, item.Value);
                            }
                            targetDataBaseInfo.resourcesSpriteBible.SyncDictionaryToList();
                            break;
                        case ResourcesSearchClassType.GameObject:
                            tempBible = lLcroweUtilEditor.GetResourcesForAllSearch<Object>();
                            foreach (var item in tempBible)
                            {
                                targetDataBaseInfo.resourcesObjectBible.Add(item.Key, item.Value);
                            }
                            targetDataBaseInfo.resourcesObjectBible.SyncDictionaryToList();
                            break;
                    }
                }

                targetDataBaseInfo.allResourcesPathList.Clear();
                targetDataBaseInfo.allResourcesPathList = lLcroweUtilEditor.GetResourcesInAllPath();
                EditorUtility.SetDirty(targetDataBaseInfo);
            }

            if (GUILayout.Button("Import"))
            {
                Import();
            }
        }

        //DOCID와 동일//시트선택할때마다 안바뀌는부분 https://docs.google.com/spreadsheets/d/{DOCID}/
        static string key = "18EEOPPozjKlINIej0-e6c1XsVreTPMYGnIpx1zuKNMg";

        //적혀있던 숫자를 쓰는게 아님//시트이름 적어야됨
        //static string sheetName = "라벨(해석)시트";
        IEnumerator DataUpdate(SheetData sheetData)
        {
            string URL = $"https://docs.google.com/spreadsheets/d/{key}/gviz/tq?tqx=out:csv&sheet={sheetData.sheetName}";
            UnityWebRequest www = UnityWebRequest.Get(URL);
            var op = www.SendWebRequest();
            do
            {
                yield return op;
            } while (!op.isDone);

            //CSV에서 정리 해야됨//CSV칸이 남아있는거 그대로 가져오는 느낌이
            string text = www.downloadHandler.text;
            TextAsset textAsset = new TextAsset(text);
            lLcroweUtilEditor.CreateDataObject(ref textAsset, sheetData.fileName, null, null, false);
            SetTextAsset(targetDataBaseInfo, textAsset, sheetData.sheetType);
            yield break;
        }

        private class SheetData
        {
            public string sheetName;
            public string fileName;
            public SheetType sheetType;

            /// <summary>
            /// 시트데이터,
            /// </summary>
            /// <param name="sheetName">시트이름</param>
            /// <param name="fileName">생성한 파일이름</param>
            /// <param name="sheetType">시트타입</param>
            public SheetData(string sheetName, string fileName , SheetType sheetType)
            {
                this.sheetName = sheetName;
                this.fileName = fileName;
                this.sheetType = sheetType;
            }
        }

        //후에 LabelBase쪽거랑 연동해야됨//알트엔터에서 쉽게 추가가능
        private enum SheetType
        {
            Item,
            Record,

            AchievementInfo,
            AchievementCondition,
            AchievementReward,

            SearchMap,
            SearchMapReward,

            MVP,
            Store,
            StorePull,
            UnitStat,

            Icon,
            UIThema
        } 

        private void SetTextAsset(DataBaseInfo dataBaseInfo, TextAsset textAsset, SheetType sheetType)
        {       
            switch (sheetType)
            {
                case SheetType.Item:
                    dataBaseInfo.itemInfoTextSheet = textAsset;
                    break;
                case SheetType.Record:
                    dataBaseInfo.recordActionInfoTextSheet = textAsset;
                    break;
                case SheetType.AchievementInfo:
                    dataBaseInfo.achievementInfoTextSheet = textAsset;
                    break;
                case SheetType.AchievementCondition:
                    dataBaseInfo.achievementConditionInfoTextSheet = textAsset;
                    break;
                case SheetType.AchievementReward:
                    dataBaseInfo.achievementRewardTextSheet = textAsset;
                    break;
                case SheetType.SearchMap:
                    dataBaseInfo.searchMapInfoTextSheet = textAsset;
                    break;
                case SheetType.SearchMapReward:
                    dataBaseInfo.searchMapRewardTextSheet = textAsset;
                    break;
                case SheetType.MVP:
                    dataBaseInfo.MVPTextSheet = textAsset;
                    break;
                case SheetType.Store:
                    dataBaseInfo.storeTextSheet = textAsset;
                    break;
                case SheetType.StorePull:
                    dataBaseInfo.storePullSheet = textAsset;
                    break;
                case SheetType.UnitStat:
                    dataBaseInfo.unitObjectTextSheet = textAsset;
                    break;
                case SheetType.Icon:
                    dataBaseInfo.iconPresetTextSheet = textAsset;
                    break;
                case SheetType.UIThema:
                    dataBaseInfo.uiThemaTextSheet = textAsset;
                    break;
            }
            
            Debug.Log($"연동끝");
            EditorUtility.SetDirty(dataBaseInfo);//갱신//수정이 된상태일때 그것을 적용하는것
        }



        //간단히 멀티스레드 이용해볼까//그냥하자 에디터쪽이 멀티스레딩 지원한다는 보장도 없잖아
        public void Import()
        {
            //각 데이터쉬트에 맞게 각 스크립터블 오브젝트를 생성해서 폴더에 저장후
            //데이터베이스의 리스트에 다 집어넣기
            //List<Thread> threadList = new List<Thread>();

            //threadList.Add(new Thread(new ThreadStart(ImportAchievementConditionInfoTextSheet)));//업적조건
            //threadList.Add(new Thread(new ThreadStart(ImportAchievementInfoTextSheet)));//업적
            //threadList.Add(new Thread(new ThreadStart(ImportItemInfoTextSheet)));//아이템
            //threadList.Add(new Thread(new ThreadStart(ImportRecordActionInfoTextSheet)));//기록
            //threadList.Add(new Thread(new ThreadStart(ImportRewardInfoTextSheet)));//보상

            //for (int i = 0; i < threadList.Count; i++)
            //{
            //    threadList[i].Start();
            //}

            //설정용
            //ImportAchievementInfoTextSheet();

            //CSV와 네이밍규칙을 같게 지켜야됨//첫글자를 대문자? 소문자로 할지//그래야지 임포트하는 함수 작업이 편함

            //기록
            ImportCSVTextSheet("record", targetDataBaseInfo.recordActionInfoTextSheet, null, targetDataBaseInfo.recordActionInfoList);
            //아이템
            ImportCSVTextSheet("item", targetDataBaseInfo.itemInfoTextSheet, itemInfoImport, targetDataBaseInfo.itemInfoList);
            //업적
            ImportCSVTextSheet("achievement", targetDataBaseInfo.achievementInfoTextSheet, AchievementInfoImport, targetDataBaseInfo.achievementInfoList);
            //업적조건
            ImportCSVTextSheet("achievementCondition", targetDataBaseInfo.achievementConditionInfoTextSheet, AchievementConditionInfoImport, targetDataBaseInfo.achievementConditionInfoList);
            //업적보상
            ImportCSVTextSheet("achievementReward", targetDataBaseInfo.achievementRewardTextSheet, RewardInfoImport, targetDataBaseInfo.achievementRewardInfoList);
            //탐색맵
            ImportCSVTextSheet("searchMap", targetDataBaseInfo.searchMapInfoTextSheet, SearchMapInfoImport, targetDataBaseInfo.searchMapInfoList);
            //업적보상
            ImportCSVTextSheet("searchMapReward", targetDataBaseInfo.searchMapRewardTextSheet, RewardInfoImport, targetDataBaseInfo.searchMapRewardInfoList);

            //MVP
            ImportCSVTextSheet("MVP", targetDataBaseInfo.MVPTextSheet, MVPInfoImport, targetDataBaseInfo.mvpInfoList);
            //상점
            ImportCSVTextSheet("store", targetDataBaseInfo.storeTextSheet, StoreInfoImport, targetDataBaseInfo.storeInfoList);
            //상점뽑기
            ImportCSVTextSheet("storePull", targetDataBaseInfo.storePullSheet, StorePullInfoImport, targetDataBaseInfo.storePullInfoList);
            //유닛
            ImportCSVTextSheet("UnitObject", targetDataBaseInfo.unitObjectTextSheet, UnitObjectInfoImport, targetDataBaseInfo.unitObjectInfoList);

            //UI테마//6개인데
            //음?//형식 아예다른데 흐음
            //UI테마에 들어가는 CSV가 6개로 분할되있다.
            //원래쓰던건 단일 CSV
            ImportCSVTextSheet("IconPreset", targetDataBaseInfo.iconPresetTextSheet, IconPresetInfoImport, targetDataBaseInfo.iconPresetInfoList);

            //UIThema는 별개로 돌아감
            ImportCSVTextSheet("UIThema", targetDataBaseInfo.uiThemaTextSheet, UIThemaInfoImport, targetDataBaseInfo.uIThemaInfoList);


            //유닛정보들의 프리팹들 처리
            //임포트할떄 처리하면 덮어쓰기할떄 적용이 안됨
            foreach (var item in targetDataBaseInfo.unitObjectInfoList)
            {
                if (item.unitPrefab != null)
                {
                    item.unitPrefab.unitObjectInfo = item;
                    //프리팹변경된 메모리들 저장처리
                    //안해두면 메모리에만 적재되있고 하드드라이브에는 적용이 안됨
                    PrefabUtility.SavePrefabAsset(item.unitPrefab.gameObject);
                }
            }

            Selection.activeObject = targetDataBaseInfo;//선택
            EditorUtility.SetDirty(targetDataBaseInfo);//갱신//수정이 된상태일때 그것을 적용하는것

            Debug.Log("임포트가 완료되었습니다.");
            //EditorUtility.DisplayDialog("임포트 안내창", "임포트가 완료되었습니다.", "OK");
        }

        /// <summary>
        /// 라벨데이터용 CSV 임포터
        /// </summary>
        /// <typeparam name="T">라벨데이터상속한 타입</typeparam>
        /// <param name="labelName">라벨이름</param>
        /// <param name="textSheet">CSV텍스트</param>
        /// <param name="importNotDuplicatedCSVAction">추가로 작동될</param>
        /// <param name="infoList"></param>
        private void ImportCSVTextSheet<T>(string labelName, TextAsset textSheet, System.Action<Dictionary<string, object>, T> importNotDuplicatedCSVAction, List<T> infoList) where T : LabelBase , new()
        {
            List<T> newList = new List<T>();
            List<Dictionary<string, object>> dataList = lLcroweUtilEditor.CSVReader.ReadTextAsset(textSheet);
            string labelID = $"{labelName}ID";

            //폴더이름 처리//첫글자만 대문자로 처리
            //TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;//이건 한단어에서 첫글자만 대문자고 나머지는 다 소문자
            //labelName = textInfo.ToTitleCase(labelName);

            //원하는건 첫글자만 대문자고 나머지는 그대로 남기기
            labelName = char.ToUpper(labelName[0]) + labelName.Substring(1);
            string path = lLcroweUtil.GetCombineString(folderName, $"/{labelName}Info");

            //int count = 0;
            foreach (var item in dataList)
            {
                T newInfoData = new T();
                string fileName = newInfoData.labelID = item.GetConvertString(labelID);//ID로 파일이름정하다가 이름있으면 이름으로변경

                //디버그용
                //Debug.Log($"{labelID}: {fileName}: {count}개");
                //count++;

                //아이콘라벨용 처리
                IconLabelBase iconLabelBase = newInfoData as IconLabelBase;
                if (iconLabelBase)
                {
                    fileName = iconLabelBase.labelNameOrTitle = item.GetConvertString("NameOrTitle");//이거 어덯게 바꿀까//대문자? 소문자?
                    iconLabelBase.icon = item.GetResourcesForSprite("iconSprite");
                    iconLabelBase.description = item.GetConvertString("description");
                }

                //그외 데이터용 처리
                importNotDuplicatedCSVAction?.Invoke(item, newInfoData);

                //제작
                lLcroweUtilEditor.CreateDataObject(ref newInfoData, fileName, path, null, false);
                newList.Add(newInfoData);
            }
            AddNewList<T>(ref newList, ref infoList);
        }

        //===============================================================
        //각 고유의 데이터를 임포트하기 위한 함수구역들//인포임포트
        //===============================================================

        private void AchievementInfoImport(Dictionary<string, object> item, AchievementInfo achievementInfo)
        {
            achievementInfo.achievementActionType = item.GetConvertEnum<EAchievementActionType>("achievementActionType");//테스트해보기
            //achievementInfo.achievementActionType = (EAchievementActionTagType)Enum.Parse(typeof(EAchievementActionTagType), (string)item["achievementActionType"]);
            achievementInfo.isAutoUnlock = item.GetConvertBool("isAutoComplete");
            achievementInfo.uiBarPrefab = item.GetResourcesForComponent<UIBar_Base>("UIBarPrefabName", lLcroweUtil.CATPathType.UIBar);
            achievementInfo.rewardUIPrefab = item.GetResourcesForComponent<RewardUI>("RewardUIPrefabName", lLcroweUtil.CATPathType.UI);
        }   

        List<string> tempStringList = new List<string>(3);
        List<int> tempIntList = new List<int>(3);
        private void AchievementConditionInfoImport(Dictionary<string, object> item, AchievementConditionInfo achievementConditionInfo)
        {
            string idContent = "checkRecordID";
            string valueContent = "checkValue";

            for (int i = 1; i < 4; i++)
            {
                int index = i;
                //tempStringList.Add((string)item[idContent + index]);
                tempStringList.Add(item.GetConvertString(idContent + index));

                //int temp = 0;
                //if (int.TryParse(item[valueContent + index].ToString(), out temp))                
                //{
                    //Debug.Log($"ID: {valueContent + index}, Value: {item[valueContent + index]}");
                //}
                tempIntList.Add(item.GetConvertInt(valueContent + index));
            }
            achievementConditionInfo.checkRecordIDArray = tempStringList.ToArray();
            achievementConditionInfo.checkValueArray = tempIntList.ToArray();

            tempStringList.Clear();
            tempIntList.Clear();
        }

        private void itemInfoImport(Dictionary<string, object> item, ItemInfo itemInfo)
        {
            //itemInfo.itemType = (EItemType)Enum.Parse(typeof(EItemType), (string)item["itemType"]);
            //itemInfo.gradeType = (EGradeType)Enum.Parse(typeof(EGradeType), (string)item["gradeType"]);

            itemInfo.itemType = item.GetConvertEnum<EItemType>("itemType");
            itemInfo.gradeType = item.GetConvertEnum<EGradeType>("gradeType");
            //itemInfo.backGroundImage = lLcroweUtilEditor.GetCustomUnityObject<Sprite>.GetUnityObjectFile();
        }
    
        private void RewardInfoImport(Dictionary<string, object> item, RewardInfo rewardInfo)
        {
            string idContent = "itemName";
            string valueContent = "amount";

            for (int i = 1; i < 4; i++)
            {
                int index = i;
                //tempStringList.Add((string)item[idContent + index]);
                tempStringList.Add(item.GetConvertString(idContent + index));

                //int temp = 0;
                //if (int.TryParse(item[valueContent + index].ToString(), out temp))
                //{
                //    //Debug.Log($"ID: {valueContent + index}, Value: {item[valueContent + index]}");
                //}

                tempIntList.Add(item.GetConvertInt(valueContent + index));
            }
            rewardInfo.itemNameArray = tempStringList.ToArray();
            rewardInfo.amountArray = tempIntList.ToArray();
            tempStringList.Clear();
            tempIntList.Clear();
        }

        private void SearchMapInfoImport(Dictionary<string, object> item, SearchMapInfo searchMapInfo)
        {
            searchMapInfo.seed = item.GetConvertInt("seed");
            searchMapInfo.needSupply = item.GetConvertInt("needSupply");
            searchMapInfo.stageName = item.GetConvertString("stageName");
        }

        //각 데이터관련해서 처리해주기 
        private void MVPInfoImport(Dictionary<string, object> item, MVPInfo mVPInfo)
        {
            mVPInfo.scoreType = item.GetConvertEnum<ScoreType>("ScoreType");
            string hexColor = item.GetConvertString("colorMVPBackGroundColor_Hex");
            Color color = Color.red;
            lLcroweUtil.ConvertHexColorToRGBAColor(hexColor, out color);
            mVPInfo.colorMVPBackGroundColor_Hex = color;

            mVPInfo.bunusScale = item.GetConvertFloat("bunusScale");
            mVPInfo.scoreMVPAlias = item.GetConvertString("scoreMVPAlias");
            mVPInfo.scoreMVPDescription = item.GetConvertString("scoreMVPDescription");
        }

        private void StoreInfoImport(Dictionary<string, object> item, StoreInfo storeInfo)
        {
            //없음
        }

        private void StorePullInfoImport(Dictionary<string, object> item, StorePullInfo storePullInfo)
        {
            storePullInfo.unitObjectID = item.GetConvertString("UnitObjectID");
            storePullInfo.pullEnable = item.GetConvertBool("PullEnable");
        }

        private void UnitObjectInfoImport(Dictionary<string, object> item, UnitObjectInfo unitObjectInfo)
        {
            unitObjectInfo.unitClassType = item.GetConvertEnum<UnitClassType>("unitClassType");
            unitObjectInfo.classIcon = item.GetResourcesForSprite("classIcon");

            unitObjectInfo.damageObjectPrefab = item.GetResourcesForComponent<DamageObject>("damageObjectPrefab", lLcroweUtil.CATPathType.DamageObject);
            unitObjectInfo.unitPrefab = item.GetResourcesForComponent<UnitObject_Base>("unitPrefab", lLcroweUtil.CATPathType.UnitObject);
            unitObjectInfo.unitCardUI = item.GetResourcesForComponent<UnitBatchCardUI>("unitCardUI", lLcroweUtil.CATPathType.UnitCard);
            unitObjectInfo.fireEffectPrefab = item.GetResourcesForComponent<EffectObject>("fireEffectPrefab",lLcroweUtil.CATPathType.Effect);

            //자고 일어나기//유닛데이터임포트처리
            //item기준으로 있으면 집어넣어버리는거
            //스탯//상태
            StatusData statusData = unitObjectInfo.statusData;
            List<UnitStatusValue> unitStatusValueList = new List<UnitStatusValue>();
            var unitStatusTypeList = lLcroweUtil.GetEnumDefineData<UnitStatusType>();
            for (int i = 0; i < unitStatusTypeList.Count; i++)
            {
                var unitStatusType = unitStatusTypeList[i];
                float result = 0;
                bool check = false;
                switch (AbilityUtil.GetUnitStatusValueCATType(unitStatusType))
                {
                    case UnitStatusValueCATType.Int:
                        check = item.GetTryConvertInt(unitStatusType.ToString(), out int intResult);
                        result = intResult;
                        break;
                    case UnitStatusValueCATType.Float:
                        check = item.GetTryConvertFloat(unitStatusType.ToString(), out result);
                        break;
                }

                //존재하지않으면 넘어감
                if (!check)
                {
                    continue;
                }

                UnitStatusValue unitStatusValue = new UnitStatusValue();
                unitStatusValue.unitStatusType = unitStatusType;
                unitStatusValue.value = result;
                unitStatusValueList.Add(unitStatusValue);
            }
            statusData.unitStatusArray = unitStatusValueList.ToArray();

            //상태처리
            var stateTypeList = lLcroweUtil.GetEnumDefineData<UnitStateType>();
            var infoUnitStateApplyBible = unitObjectInfo.statusData.infoUnitStateApplyBible;
            for (int i = 0; i < stateTypeList.Count; i++)
            {
                var stateType = stateTypeList[i];

                //변환이 가능하면 작동
                if (!item.GetTryConvertBool(stateType.ToString(),out bool result))
                {
                    continue;
                }

                infoUnitStateApplyBible.Add(stateType, result);
            }
            infoUnitStateApplyBible.SyncDictionaryToList();

            //업적처리//수동처리나중에 처리혜정
            unitObjectInfo.recordID_KillEnemy = "KillEnemy";
            unitObjectInfo.recordID_KillIInfantryman = "KillIInfantryman";
            unitObjectInfo.recordID_DestroyTank = "DestroyTank";
        }

        private void IconPresetInfoImport(Dictionary<string, object> item, IconPresetInfo iconPresetInfo)
        {
            //나중에 처리 예정
            //2번부터 처리해야됨//1번은 
            int i = 0;
            foreach (var data in item)
            {   
                if (i == 0)
                {
                    i++;
                    continue;
                }
                                
                //CSV열부분 닉네임을 가져와야됨
                IconPresetData tempData = new IconPresetData();
                //네임을 아이콘이 들어갈 대상으로 수정됨//아이콘스프라이트는 
                string key = data.Key;
                tempData.iconName = key;//키//헤더가져옴
                tempData.iconSprite = item.GetResourcesForSprite(key);//키(헤더)값으로 스프라이트가져옴

                iconPresetInfo.iconDataList.Add(tempData);
                i++;
            }
        }

        private void UIThemaInfoImport(Dictionary<string, object> item, UIThemaInfo uIThemaInfo)
        {
            //여기서 다른 CSV들 처리후 가져오기

            //UIThema CSV에 있는것들 먼저 처리
            //아이콘데이터는 따로 제작해야할듯

            //합치자 귀찮다
            //이미 new로 다되있음

            //기본프리셋
            UIThemaPreset("panel", item, uIThemaInfo.panelUIThemaPreset);

            //아이콘프리셋
            UIThemaPreset("icon", item, uIThemaInfo.iconUIThemaPreset);

            //버튼프리셋            
            UIThemaPreset("button", item, uIThemaInfo.buttonUIThemaPreset);
            SetButtonColorPreset(item, uIThemaInfo.buttonColorPreset);
            SetButtonSpriteSwapPreset(item, uIThemaInfo.buttonSpriteSwapPreset);

            //텍스트프리셋            
            UIThemaPreset("text", item, uIThemaInfo.textUIThemaPreset);
            var font = item.GetResourcesForObject<TMP_FontAsset>("fontAsset", lLcroweUtil.CATPathType.Font);//FontFolder로 제작
            if (font == null)
            {
                uIThemaInfo.textFontPreset.fontAsset = font;
            }

            if (lLcroweUtil.ConvertHexColorToRGBAColor("fontColor", out Color color))
            {
                uIThemaInfo.textFontPreset.fontColor = color;
            }
        }

        private void UIThemaPreset(string addCellID, Dictionary<string, object> item, UIThemaPreset uIThemaPreset)
        {
            SetSpritePreset($"{addCellID}-panel",item, uIThemaPreset.panelSpritePreset, true);

            uIThemaPreset.innerPanelBorderPreset.borderTopValue = item.GetConvertFloat($"{addCellID}-innerPanelBorderTopValue");
            uIThemaPreset.innerPanelBorderPreset.borderBottomValue = item.GetConvertFloat($"{addCellID}-innerPanelBorderBottomValue");
            uIThemaPreset.innerPanelBorderPreset.borderLeftValue = item.GetConvertFloat($"{addCellID}-innerPanelBorderLeftValue");
            uIThemaPreset.innerPanelBorderPreset.borderRightValue = item.GetConvertFloat($"{addCellID}-innerPanelBorderRightValue");

            SetSpritePreset($"{addCellID}-innerPanel", item, uIThemaPreset.innerPanelSpritePreset, false);
        }

        private void SetSpritePreset(string cellID, Dictionary<string, object> item, SpritePreset spritePreset, bool isAllowNullSprite)
        {
            if (lLcroweUtil.ConvertHexColorToRGBAColor(item[$"{cellID}Color"].ToString(), out Color color))
            {
                spritePreset.color = color;
            }

            spritePreset.sprite = item.GetResourcesForSprite($"{cellID}Sprite");
            if (!isAllowNullSprite)
            {
                spritePreset.sprite = null;
            }
            spritePreset.uiMaterial = item.GetResourcesForObject<Material>($"{cellID}UIMaterial", lLcroweUtil.CATPathType.UIMaterial);//UIMeterial을 제작하기
            spritePreset.isUseRaycastTarget = item.GetConvertBool($"{cellID}IsUseRaycastTarget");
        }

        private void SetButtonColorPreset(Dictionary<string, object> item, ButtonColorPreset buttonColorPreset)
        {
            Color color = Color.red;
            if (lLcroweUtil.ConvertHexColorToRGBAColor("highLightColor", out color))
            {
                buttonColorPreset.highLightColor = color;
            }
            if (lLcroweUtil.ConvertHexColorToRGBAColor("pressedColor", out color))
            {
                buttonColorPreset.pressedColor = color;
            }
            if (lLcroweUtil.ConvertHexColorToRGBAColor("selectedColor", out color))
            {
                buttonColorPreset.selectedColor = color;
            }
            if (lLcroweUtil.ConvertHexColorToRGBAColor("disabledColor", out color))
            {
                buttonColorPreset.disabledColor = color;
            }

            float value = 0;
            if (item.GetTryConvertFloat("colorMultiplier",out value))
            {
                buttonColorPreset.colorMultiplier = value;
            }
            if (item.GetTryConvertFloat("fadeDuration", out value))
            {
                buttonColorPreset.fadeDuration = value;
            }
        }

        private void SetButtonSpriteSwapPreset(Dictionary<string, object> item, ButtonSpriteSwapPreset buttonSpriteSwapPreset)
        {
            buttonSpriteSwapPreset.highLightSprite = item.GetResourcesForSprite($"highLightSprite");
            buttonSpriteSwapPreset.pressedSprite = item.GetResourcesForSprite($"pressedSprite");
            buttonSpriteSwapPreset.selectedSprite = item.GetResourcesForSprite($"selectedSprite");
            buttonSpriteSwapPreset.disabledSprite = item.GetResourcesForSprite($"disabledSprite");
        }

        //===============================================================
        //===============================================================

        /// <summary>
        /// 데이터베이스에 중복없이 새롭게 만든 종류들을 추가해주는 함수
        /// </summary>
        /// <typeparam name="T">리스트</typeparam>
        /// <param name="newList">신규로 추가할리스트</param>
        /// <param name="dataBaseList">데이터베이스에 있는 리스트</param>
        private void AddNewList<T>(ref List<T> newList, ref List<T> dataBaseList)
        {
            dataBaseList.Clear();
            for (int i = 0; i < newList.Count; i++)
            {
                int index = i;
                if (dataBaseList.Contains(newList[index]))
                {
                    Debug.Log("중복존재");
                    continue;
                }
                dataBaseList.Add(newList[index]);
            }            
        }
    }
}
