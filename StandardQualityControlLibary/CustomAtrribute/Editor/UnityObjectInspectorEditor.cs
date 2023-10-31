using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System;


#if UNITY_EDITOR

namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(Object), true), CanEditMultipleObjects]
    public class UnityObjectInspectorEditor : Editor
    {
        private ButtonMethodHandler _buttonMethod;

        private void OnEnable()
        {
            if (target == null) return;

            if (targets.Length == 0)
            {
                return;
            }
           
            _buttonMethod = new ButtonMethodHandler(targets);
        }

      

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            _buttonMethod?.OnBeforeInspectorGUI();

            _buttonMethod?.OnAfterInspectorGUI();
        }
    }


    public class ButtonMethodHandler
    {
        public readonly List<(MethodInfo Method, string Name, ButtonMethodDrawOrder order)> TargetMethods;
        public int Amount => TargetMethods?.Count ?? 0;

        private readonly Object[] _targetArray;

        public ButtonMethodHandler(Object[] targetArray)
        {
            _targetArray = targetArray;

            var type = targetArray[0].GetType();
            var bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            var members = type.GetMembers(bindings).Where(IsButtonMethod);

            foreach (var member in members)
            {
                var method = member as MethodInfo;
                if (method == null) continue;

                if (IsValidMember(method, member))
                {
                    var attribute = (ButtonMethodAttribute)Attribute.GetCustomAttribute(method, typeof(ButtonMethodAttribute));
                    if (TargetMethods == null) TargetMethods = new List<(MethodInfo, string, ButtonMethodDrawOrder)>();
                    TargetMethods.Add((method, SplitCamelCase(method.Name), attribute.DrawOrder));
                }
            }
        }

        public void OnBeforeInspectorGUI()
        {
            if (TargetMethods == null) return;

            foreach (var method in TargetMethods)
            {
                if (method.order != ButtonMethodDrawOrder.BeforeInspector) continue;


                if (GUILayout.Button(method.Name))
                {
                    for (int i = 0; i < _targetArray.Length; i++)
                    {
                        var _target = _targetArray[i];
                        InvokeMethod(_target, method.Method);
                    }
                    
                }
            }

            //GUILayout.Space(5);
            //EditorGUILayout.Space();
        }

        public void OnAfterInspectorGUI()
        {
            if (TargetMethods == null) return;
            //GUILayout.Space(5);
            //EditorGUILayout.Space();

            foreach (var method in TargetMethods)
            {
                if (method.order != ButtonMethodDrawOrder.AfterInspector) continue;



                if (GUILayout.Button(method.Name))
                {
                    for (int i = 0; i < _targetArray.Length; i++)
                    {
                        var _target = _targetArray[i];
                        InvokeMethod(_target, method.Method);
                    }
                }
                SceneView.RepaintAll();
            }
        }

        //public void Invoke(MethodInfo method) => InvokeMethod(_target, method);


        private void InvokeMethod(Object target, MethodInfo method)
        {
            var result = method.Invoke(target, null);

            if (result != null)
            {
                var message = $"{result} \n메서드 결과 '{method.Name}' 문제발생(결과null) {target.name}";
                Debug.Log(message, target);
            }
        }

        private bool IsButtonMethod(MemberInfo memberInfo)
        {
            return Attribute.IsDefined(memberInfo, typeof(ButtonMethodAttribute));
        }

        private bool IsValidMember(MethodInfo method, MemberInfo member)
        {
            if (method == null)
            {
                Debug.LogWarning(
                    $"Property <color=brown>{member.Name}</color>.이유: 구성원이 메서드가 아니지만 EditorButtonAttribute가 있습니다!");
                return false;
            }

            if (method.GetParameters().Length > 0)
            {
                Debug.LogWarning(
                    $"Method <color=brown>{method.Name}</color>.이유: 매개 변수가 있는 메서드는 EditorButtonAttribute에서 지원되지 않습니다!");
                return false;
            }

            return true;
        }
        private string SplitCamelCase(string inputCamelCaseString)
        {
            string sTemp = Regex.Replace(inputCamelCaseString, "([A-Z][a-z])", " $1", RegexOptions.Compiled).Trim();
            return Regex.Replace(sTemp, "([A-Z][A-Z])", " $1", RegexOptions.Compiled).Trim();
        }
    }

}

#endif