using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace lLCroweTool.QC.EditorOnly
{
    [CustomPropertyDrawer(typeof(SceneAttribute))]
    public class SceneDrawer : PropertyDrawer
    {
        List<string> nameList = new List<string>();
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

            nameList.Clear();
            int index;
            string[] scenes = EditorBuildSettings.scenes
              .Where(scene => scene.enabled)
              .Select(scene => scene.path)
              .ToArray();


            for (int i = 0; i < scenes.Length; i++)
            {
                string[] temp = scenes[i].Split("/");
                temp = temp[temp.Length - 1].Split(".");
                nameList.Add(temp[0]);//이름만 추출
            }

            //빌드
            //for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            //{
            //    //var temp = SceneManager.GetSceneByBuildIndex(i);
            //}
            //nameList.AddRange(scenes);





            //여러그룹을 사용할떄 각각의 그룹에 대한걸 가지고 있기위한 기능을 가짐
            index = nameList.IndexOf(property.stringValue);

            string labelContent = label.text;
            if (index == -1)
            {
                //에러발생시 처리                
                nameList.Insert(0, property.stringValue);
                index = nameList.IndexOf(property.stringValue);
                index = EditorGUI.Popup(position, $"{label}", index, nameList.ToArray());
                string groupErrorName = nameList[index];

                //해당되는 프로퍼티에 대입
                property.stringValue = groupErrorName;
                return;
            }

            position.width -= 82;
            index = EditorGUI.Popup(position, $"{label}", index, nameList.ToArray());
            string groupName = nameList[index];

            //해당되는 프로퍼티에 대입
            property.stringValue = groupName;

        }

        private bool _checked;

        private void Warning(SerializedProperty property)
        {
            Debug.LogWarning(string.Format("프로퍼티 <color=brown>{0}</color> in object <color=brown>{1}</color> 가 잘못된 타입입니다. 스트링 체크",
                property.name, property.serializedObject.targetObject));
            _checked = true;
        }
    }
}
