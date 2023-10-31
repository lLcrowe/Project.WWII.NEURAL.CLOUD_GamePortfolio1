using lLCroweTool.UI.UIThema.UIThemaButton;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace lLCroweTool.QC.EditorOnly
{    
    public class ThemaUIButtonMigrationWindowEditor : EditorWindow
    {
        //버튼을 UI테마 버튼으로 마이그레이션을 위한 처리
        //배치에디터 체크하기
        //스크롤로 보여주고
        
        public class CheckButton
        {
            public bool migrationCheck;
            public Button button;
        }

        public class RectTransformPreset
        {
            Vector2 anchorPos = Vector2.zero;
            Vector2 size = Vector2.zero;
            Vector2 anchorMin = Vector2.zero;
            Vector2 anchorMax = Vector2.zero;

            public RectTransformPreset(RectTransform rect)
            {   
                anchorPos = rect.anchoredPosition;
                size = rect.sizeDelta;
                anchorMin = rect.anchorMin;
                anchorMax = rect.anchorMax;
            }
           
            public void Copy(RectTransform rect)
            {
                rect.anchoredPosition = anchorPos;
                rect.sizeDelta = size;
                rect.anchorMin = anchorMin;
                rect.anchorMax = anchorMax;
            }
        }

        protected static List<CheckButton> checkButtonList = new();
        private Vector2 scollPos;
        private bool allSelect = false;


        [MenuItem("lLcroweTool/ButtonToUIThemaMigrationEditor")]
        public static void ShowWindow()
        {
            EditorWindow editorWindow = GetWindow(typeof(ThemaUIButtonMigrationWindowEditor));
            editorWindow.titleContent.text = "버튼마이그레이션 에디터";
            editorWindow.minSize = new Vector2(325, 625);
            editorWindow.maxSize = new Vector2(325, 625);

        }


        private void OnEnable()
        {
            SearchButton();
        }

        private void SearchButton()
        {
            allSelect = false;
            //현재하이어라키에 있는 타일맵들을 가져와서 처리
            Button[] buttonArray = Transform.FindObjectsOfType<Button>();
            checkButtonList.Clear();
            for (int i = 0; i < buttonArray.Length; i++)
            {
                CheckButton checkButton = new CheckButton();
                checkButton.migrationCheck = false;
                checkButton.button = buttonArray[i];
                checkButtonList.Add(checkButton);
            }
        }

        private void OnGUI()
        {
            //감지된 UI테마 수
            EditorGUILayout.LabelField($"감지된 버튼 수 : {checkButtonList.Count}개");
            ScrollView(ref scollPos, () =>
            {
                for (int i = 0; i < checkButtonList.Count; i++)
                {
                    var checkButton = checkButtonList[i];
                    if (checkButton.button == null)
                    {
                        checkButtonList.RemoveAt(i);
                        --i;
                        continue;
                    }
                    ButtonShow(checkButton);
                }
            }, 560);


            if (GUILayout.Button("버튼 탐색"))
            {
                SearchButton();
            }

            lLcroweUtilEditor.EditorGUILayoutHorizontal(() =>
            {
                if (GUILayout.Button("전체 선택", GUILayout.Width(80)))
                {
                    allSelect = !allSelect;
                    foreach (var item in checkButtonList)
                    {
                        item.migrationCheck = allSelect;
                    }
                }

                //마이그레이션 처리를 위한 버튼
                if (GUILayout.Button("마이그레이션 시작하기"))
                {
                    MigrationButton(checkButtonList);
                }
            });
        }

        private static void ButtonShow(CheckButton checkButton)
        {
            //배치되서 초기화됫는지 체크
            //어덯게 만들까
            bool migrationCheck = checkButton.migrationCheck;
            Button button = checkButton.button;
            bool check = false;

            //UI테마가 있는지 여부
            if (!button.TryGetComponent(out ThemaUIButton themaUIButton))
            {
                //없으면 처음부터 초기화처리
                check = true;
            }
            else
            {
                //있어도 제대로 배치되있는지 체크
                if (CheckThemeUIButton(themaUIButton))
                {
                    //안되있으면 처리
                    check = true;
                }
            }

            lLcroweUtilEditor.EditorGUILayoutHorizontal(() =>
            {
                //완료됫으면 그린처리로 하고
                //안됫으면 레드처리하자
                Color targetColor = check ? Color.red : Color.green;

                lLcroweUtilEditor.OnGUIBackGroundColorLayout(targetColor ,() =>
                {
                    checkButton.migrationCheck = EditorGUILayout.Toggle(checkButton.migrationCheck);
                    EditorGUILayout.LabelField($"{button.name}");
                    if (GUILayout.Button("Select"))
                    {
                        Selection.activeGameObject = button.gameObject;
                    }
                });
            });
        }

        //마이그레이션 시작
        public static void MigrationButton(List<CheckButton> checkButtonList)
        {
            //check 사항을 만들고 처리할 대상을 지정후 처리하자
            foreach (var item in checkButtonList)
            {
                if (item.migrationCheck == false)
                {
                    continue;
                }
                Button button = item.button;
                
                bool isExistText = false;
                string textContent = "Null";
                Color textColor = Color.black;
                RectTransformPreset textRect = null;

                bool isExistImage = false;
                Sprite sprite = null; 
                Color spriteColor = Color.white;
                RectTransformPreset imageRect = null;


                if (!button.TryGetComponent(out ThemaUIButton themaUIButton))
                {
                    //없으면 처음부터 초기화처리
                    //컴포넌트 추가
                    lLcroweUtilEditor.NullAddComonent(button.transform, ref themaUIButton);


                    //기본적인 버튼은 버튼 + 아래텍스트

                    //텍스트가 존재할시 기억해둠
                    var text = lLcroweUtilEditor.GetChildComponentToParent<TextMeshProUGUI>(button, button.transform, true);
                    //var text = button.GetComponentInChildren<TextMeshProUGUI>();
                    if (text != null)
                    {
                        isExistText = true;
                        themaUIButton.isUseText = true;
                        textContent = text.text;
                        textColor = text.color;         

                        var rect = text.rectTransform;
                        textRect = new RectTransformPreset(rect);

                        DestroyImmediate(text.gameObject);
                    }

                    //이미지를 가지고 있을시 기억해둠
                    var image = lLcroweUtilEditor.GetChildComponentToParent<Image>(button, button.transform, true);
                    if (image != null)
                    {
                        isExistImage = true;
                        themaUIButton.isUseIcon = true;
                        sprite = image.sprite;
                        spriteColor = image.color;

                        var rect = image.rectTransform;
                        imageRect = new RectTransformPreset(rect);

                        DestroyImmediate(image.gameObject);
                    }

                }
                //else
                //{
                //    if (themaUIButton.isUseText)
                //    {
                //        textContent = themaUIButton.textThemaUI.textObject.text;
                //    }
                //}

                //있어도 제대로 배치되있는지 체크
                if (CheckThemeUIButton(themaUIButton))
                {
                    //안되있으면 생성
                    ThemaUIButtonInspectorEditor.GenerateUIThemaButton(themaUIButton);
                }

                //존재하게 처리하기
                if (isExistText)
                {
                    var rect = themaUIButton.textThemaUI.panelImage.rectTransform;
                    textRect.Copy(rect);

                    themaUIButton.SetColorText(textColor);
                    themaUIButton.SetText(textContent);
                }

                if (isExistImage)
                {
                    var rect = themaUIButton.iconThemaUI.panelImage.rectTransform;
                    imageRect.Copy(rect);
                    
                    themaUIButton.SetIconColor(spriteColor);
                    themaUIButton.SetIconImage(sprite);
                }
            }
        }


        private static bool CheckThemeUIButton(ThemaUIButton themaUIButton)
        {
            bool check = false;
            if (themaUIButton.buttonThemaUI == null)
            { 
                check = true;
            }

            if (themaUIButton.isUseIcon)
            {
                if (themaUIButton.iconThemaUI == null)
                {   
                    check = true;
                }
            }
            else
            {
                if (themaUIButton.iconThemaUI)
                {   
                    check = true;
                }
            }

            if (themaUIButton.isUseText)
            {
                if (themaUIButton.textThemaUI == null)
                {   
                    check = true;
                }
            }
            else
            {
                if (themaUIButton.textThemaUI)
                {   
                    check = true;
                }
            }
            return check;
        }



        private static void ScrollView(ref Vector2 scollPos, System.Action action, float height)
        {
            scollPos = GUILayout.BeginScrollView(scollPos, false, true, GUILayout.Height(height));
            action?.Invoke();
            EditorGUILayout.EndScrollView();
        }
    }
}
