using lLCroweTool.QC.EditorOnly;
using UnityEditor;
using UnityEngine;

namespace lLCroweTool.Test
{
    //keyvalueStruct Drawer제작
    //[CustomPropertyDrawer (typeof(KeyValueStruct<,>))]
    
    public class KeyValueStructPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.LabelField(position, property.displayName);



            if (!property.GetPropery("key", out SerializedProperty key))
            {
                //여기서 타입에 맞게 가져오는게
                return;
            }
            if (!property.GetPropery("value", out SerializedProperty value))
            {
                //여기서 타입에 맞게 가져오는게
                return;
            }

            var style = GUILayout.MinWidth(50);


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(key, style);
            EditorGUILayout.PropertyField(value, style);
            EditorGUILayout.EndHorizontal();
        }
    }


    //제네릭은 해당걸 시리얼라이징해야 볼수 있음    
    //[CustomPropertyDrawer(typeof(CustomDictionary<,>), true)]
    public class CustomDictionaryPropertyDrawer : PropertyDrawer
    {
        private static float lineHeight = EditorGUIUtility.singleLineHeight;
        private static float vertSpace = EditorGUIUtility.standardVerticalSpacing;

        //여기는 테스트를 다한후 옮기기
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);
            //property.displayName;//인스팩터에서 보여주는거
            //property.name;//필드이름
            //property.type;
            EditorGUI.LabelField(position, property.displayName + "Test진행중");
           

         
            if (!property.GetPropery("keyValueList", out SerializedProperty keyValueList))
            {
                //여기서 타입에 맞게 가져오는게
                return;
            }

            ////집어넣는 종류
            //if (!keyValueList.GetPropery("Key", out SerializedProperty key))
            //{
            //    return;
            //}
            //if (!keyValueList.GetPropery("Value", out SerializedProperty value))
            //{
            //    //여기서 타입에 맞게 가져오는게
            //    return;
            //}
            //집어넣는 종류
            if (!property.GetPropery("editorKeyValue", out SerializedProperty editorKeyValue))
            {
                return;
            }

            //EditorGUILayout.PropertyField(key);
            //EditorGUILayout.PropertyField(value);
            //EditorGUILayout.PropertyField(keyValueList);
            EditorGUILayout.PropertyField(editorKeyValue);

            

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("추가하기"))
            {
                keyValueList.arraySize++;

                var element = keyValueList.GetArrayElementAtIndex(keyValueList.arraySize - 1);
                element = editorKeyValue.Copy();

                property.serializedObject.ApplyModifiedProperties();
            }
            if (GUILayout.Button("삭제하기"))
            {
            }
            EditorGUILayout.EndHorizontal();



            //보여주는곳
            //if (GUILayout.Button("추가하기"))
            //{
                
            //    keyValueList.arraySize++;

            //    var element = keyValueList.GetArrayElementAtIndex(keyValueList.arraySize - 1);
            //    element = editorKeyValue.Copy();

            //    property.serializedObject.ApplyModifiedProperties();
            //}


            //for (int i = 0; i < keyValueList.arraySize; i++)
            //{
            //    EditorGUILayout.BeginHorizontal();
            //    //var index = list.GetArrayElementAtIndex(i);
            //    //var key = index.FindPropertyRelative("key");
            //    //var value = index.FindPropertyRelative("value");

            //    var style = GUILayout.MinWidth(50);

            //    EditorGUILayout.PropertyField(keyValueList.GetArrayElementAtIndex(i), style);
                

            //    //testDic.list.Array.data[1]


            //    if (GUILayout.Button("삭제", style))
            //    {
            //        keyValueList.DeleteArrayElementAtIndex(i);


            //        property.serializedObject.ApplyModifiedProperties();
            //        EditorGUILayout.EndHorizontal();
            //        break;
            //    }
            //    EditorGUILayout.EndHorizontal();
            //}
        }

        //public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        //{
        //    //그냥 쓰고 있던방식 다들고와서 높이값을 추가하자
        //    float height = 0;
        //    height += property.GetPropertyHeight( "keyList");
        //    height += property.GetPropertyHeight( "valueList");
        //    return height;
        //}

    }
}
