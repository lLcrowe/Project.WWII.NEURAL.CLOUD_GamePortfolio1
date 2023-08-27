using lLCroweTool.Ability;
using lLCroweTool.Ability.Util;
using lLCroweTool.DataBase;
using lLCroweTool.Effect;
using lLCroweTool.ScoreSystem;
using lLCroweTool.UI.Bar;
using lLCroweTool.UnitBatch;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
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
     



        //private AchievementConditionInfo achievementConditionInfo;
        //private AchievementInfo achievementInfo;
        //private ItemInfo itemInfo;
        //private RecordActionInfo recordActionInfo;
        //private RewardInfo rewardInfo;


        //저장데이터
        protected string tag;
        protected string folderName;


        [MenuItem("lLcroweTool/DataBaseInfoWindowEditor")]
        public static void ShowWindow()
        {
            EditorWindow editorWindow = GetWindow(typeof(DataBaseInfoWindowEditor));
            editorWindow.titleContent.text = "게임데이터베이스 관리자";
            editorWindow.minSize = new Vector2(515, 700);
            editorWindow.maxSize = new Vector2(515, 700);
        }
   

        private void OnEnable()
        {
            folderName = "GameDataBaseFolder";
        }
        private void OnGUI()
        {
            //데이터어덯게할지체크
            targetDataBaseInfo = (DataBaseInfo)EditorGUILayout.ObjectField("현재선택된 게임 데이터베이스", targetDataBaseInfo, typeof(DataBaseInfo), true);
            if (targetDataBaseInfo == null)
            {
                return;
            }
            scollPos = EditorGUILayout.BeginScrollView(scollPos, GUILayout.Height(515));
            DataDisplaySection();
            EditorGUILayout.EndScrollView();

        }

        //데이터보는구역
        protected void DataDisplaySection()
        {   
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

            if (GUILayout.Button("Import"))
            {
                Import();
            }
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

            //CSV와 네이밍규칙을 같게 지켜야됨//첫글자를 대문자? 소문자로 할지//그래야지 임포트하느 함수 작업이 편함

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

        private void ImportCSVTextSheet<T>(string labelName, TextAsset textSheet, Action<Dictionary<string, object>, T> importNotDuplicatedCSVAction, List<T> infoList) where T : LabelBase , new()
        {
            List<T> newList = new List<T>();
            List<Dictionary<string, object>> dataList = lLcroweUtilEditor.CSVReader.Read(textSheet);
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
                //Debug.Log($"{labelID}:{count}개");
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


        //===============================================================
        //===============================================================

        /// <summary>
        /// 데이터베이스에 새롭게 만든 종류들을 추가해주는 함수
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