using System.Collections.Generic;
using lLCroweTool.DataBase;
using UnityEditor;
using UnityEngine;

namespace lLCroweTool.QC.EditorOnly
{
    [CustomPropertyDrawer(typeof(lLCroweTool.RecordIDTagAttribute))]
    public class RecordIDTagAttribute : PropertyDrawer
    {
        private int index;
        private List<string> groupNames = new List<string>();
        private List<string> groupNameOrigins = new List<string>();
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                if (!_checked)
                {
                    Warning(property);
                }
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            string path = "Assets/A0/Resources/GameDataBaseFolder";

            //게임디비는 하나만 존재해야됨
            DataBaseInfo[] dataBaseInfoArray = lLcroweUtilEditor.GetScriptableObjectFile<DataBaseInfo>(path, "*.asset");
            DataBaseInfo dataBaseInfo = null;

            if (dataBaseInfoArray.Length != 0)
            {
                dataBaseInfo = dataBaseInfoArray[0];
            }
            groupNames.Clear();

            if (dataBaseInfo != null)
            {
                List<string> tempList = new List<string>();
                List<string> tempList2 = new List<string>();
                foreach (var recordInfo in dataBaseInfo.recordActionInfoList)
                {
                    tempList.Add($"{recordInfo.labelID}-{recordInfo.labelNameOrTitle}");
                    tempList2.Add($"{recordInfo.labelID}");
                }
                groupNames.AddRange(tempList);
                groupNames.Sort();
                groupNames.Insert(0, "-None-");

                groupNameOrigins.AddRange(tempList2);
                groupNameOrigins.Sort();
                groupNameOrigins.Insert(0, "-None-");

            }
            else
            {
                groupNames.Insert(0, "(DataBaseManager not in A0/ResourceFolder)");
                groupNameOrigins.Insert(0, "(DataBaseManager not in A0/ResourceFolder)");
            }

            index = groupNames.IndexOf(property.stringValue);

            if (index == -1)
            {
                //체크

                //에러발생시 처리
                groupNames.Insert(0, property.stringValue);
                groupNameOrigins.Insert(0, property.stringValue);
                index = groupNameOrigins.IndexOf(property.stringValue);
                //에러아님! 작동잘됨!//20230408
                index = EditorGUI.Popup(position, $"RecordID:{label.text}", index, groupNames.ToArray());
                string groupErrorName = groupNames[index];

                //해당되는 프로퍼티에 대입
                property.stringValue = groupErrorName.Split('_')[0];
                ;
                return;
            }

            position.width -= 82;
            index = EditorGUI.Popup(position, "RecordID", index, groupNames.ToArray());
            string groupName = groupNames[index].Split('-')[0];

            //해당되는 프로퍼티에 대입
            property.stringValue = groupName;
        }


        //public void Test()
        //{

        //    if (questDBData != null)
        //    {
        //        List<string> tempList = new List<string>();
        //        List<string> tempList2 = new List<string>();
        //        for (int i = 0; i < questDBData.questBookDataArray.Length; i++)
        //        {
        //            tempList.Add(questDBData.questBookDataArray[i].questBookIDKey + "_제목:" + questDBData.questBookDataArray[i].questBookTitle);
        //            tempList2.Add(questDBData.questBookDataArray[i].questBookIDKey);
        //        }
        //        groupNames.AddRange(tempList);
        //        groupNames.Sort();
        //        groupNames.Insert(0, "-None-");

        //        groupNameOrigins.AddRange(tempList2);
        //        groupNameOrigins.Sort();
        //        groupNameOrigins.Insert(0, "-None-");
        //    }
        //    else
        //    {
        //        groupNames.Insert(0, "(QuestDBData not in ResourceFolder)");
        //        groupNameOrigins.Insert(0, " (QuestDBData not in ResourceFolder)");
        //    }

        //    //여러그룹을 사용할떄 각각의 그룹에 대한걸 가지고 있기위한 기능을 가짐
        //    index = groupNameOrigins.IndexOf(property.stringValue);

        //    if (index == -1)
        //    {
        //        //에러발생시 처리
        //        groupNames.Insert(0, property.stringValue);
        //        groupNameOrigins.Insert(0, property.stringValue);
        //        index = groupNameOrigins.IndexOf(property.stringValue);
        //        index = EditorGUI.Popup(position, "QuestBookID_Error", index, groupNames.ToArray());
        //        string groupErrorName = groupNames[index];

        //        //해당되는 프로퍼티에 대입
        //        property.stringValue = groupErrorName.Split('_')[0];
        //        return;
        //    }

        //    position.width -= 82;


        //    index = EditorGUI.Popup(position, "QuestBookID", index, groupNames.ToArray());
        //    string groupName = groupNames[index];

        //    //해당되는 프로퍼티에 대입
        //    property.stringValue = groupName.Split('_')[0];

        //}


        private bool _checked;

        private void Warning(SerializedProperty property)
        {
            Debug.LogWarning(string.Format("프로퍼티 <color=brown>{0}</color> in object <color=brown>{1}</color> 가 잘못된 타입입니다. 스트링 체크",
                property.name, property.serializedObject.targetObject));
            _checked = true;
        }
    }
}
