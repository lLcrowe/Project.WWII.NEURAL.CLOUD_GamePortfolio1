using lLCroweTool.QC.EditorOnly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace lLCroweTool.Test
{
    //제네릭은 해당걸 시리얼라이징해야 볼수 있음
    //[CustomPropertyDrawer(typeof(TestCustomDic<,>), true)]
    internal class TestDrawer : PropertyDrawer
    {


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(targetRect, property, label);

            //그럼 


            var list = property.FindPropertyRelative("keyList");
            string tempContent = list == null ? " 없음 " : " 존재 ";
            Debug.Log($"감지중 {tempContent}");
            label.text = "key";
            EditorGUI.PropertyField(position, list, label, true);
            list.NextLine(ref position);
            label.text = "value";
            var list2 = property.FindPropertyRelative("valueList");
            EditorGUI.PropertyField(position, list2, label, true);
            list2.NextLine(ref position);










        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //그냥 쓰고 있던방식 다들고와서 높이값을 추가하자
            float height = 0;
            height += property.GetPropertyHeight( "keyList");
            height += property.GetPropertyHeight( "valueList");
            return height;
        }
    }
}
