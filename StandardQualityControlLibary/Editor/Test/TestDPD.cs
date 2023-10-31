using lLCroweTool.Dictionary;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEditorInternal;
using UnityEngine;

namespace Assets.A0.QC
{
    //딕셔너리를 프로퍼티로 보여줄려고 제작한거 같은데
    //PropertyDrawer를 집어던짐. 안씀

    //[CustomPropertyDrawer(typeof(CustomDictionary<,>), true)]
    public abstract class TestDPD<T1,T2> : PropertyDrawer
    {
        public T1 key;
        public T2 value;

        

        //public ReorderableList

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //여기서는 타겟팅 된거 보여주는거나하자
            //property.name
            EditorGUI.LabelField(position, property.displayName +  "Test진행중");
            //EditorGUILayout//=>자동으로 높이값을 설정해줌
        }


        //public void DrawList<T1,T2>(T1)
        //{
        //}

        
    }

    //[CustomPropertyDrawer(typeof(CustomDictionary<string,string>), true)]
    public class TestDicDrawer : TestDPD<Object, string>
    {
    
         bool fold = false;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);

            //값들을 가져와서 보여주고 
            //값을 추가하고 삭제 수정(? 몰루)할수 있는
           

            //var list = property.FindPropertyRelative("list");

            var keylist = property.FindPropertyRelative("keyList");
            var valuelist = property.FindPropertyRelative("valueList");
            var key = property.FindPropertyRelative("key");
            var value = property.FindPropertyRelative("value");


            var keyCount = keylist.FindPropertyRelative("Array.size");

            // Apply any modifications to the serialized object
            //serializedObject.ApplyModifiedProperties();





            EditorGUILayout.BeginHorizontal();
            label.text = $"{key.type}:Value";
            EditorGUILayout.PropertyField(key, label);
            label.text = $"{value.type}:Value";
            EditorGUILayout.PropertyField(value, label);
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("추가하기"))
            {
                //list.arraySize++;
                keylist.arraySize++;
                valuelist.arraySize++;
                var keyElement = keylist.GetArrayElementAtIndex(keylist.arraySize - 1);
                var valueElement = valuelist.GetArrayElementAtIndex(valuelist.arraySize - 1);

                keyElement.managedReferenceValue = key.managedReferenceValue;
                valueElement.managedReferenceValue = value.managedReferenceValue;


                property.serializedObject.ApplyModifiedProperties();


                //list.InsertArrayElementAtIndex(list.arraySize - 1);
            }


            for (int i = 0; i < keylist.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                //var index = list.GetArrayElementAtIndex(i);
                //var key = index.FindPropertyRelative("key");
                //var value = index.FindPropertyRelative("value");

                var style = GUILayout.MinWidth(50);

                EditorGUILayout.PropertyField(key, style);
                EditorGUILayout.PropertyField(value, style);

                //testDic.list.Array.data[1]


                if (GUILayout.Button("삭제", style))
                {
                    keylist.DeleteArrayElementAtIndex(i);
                    valuelist.DeleteArrayElementAtIndex(i);
                    EditorGUILayout.EndHorizontal();
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }

            keylist.serializedObject.ApplyModifiedProperties();
        }
    }
}
