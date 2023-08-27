using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#pragma warning disable 0618
namespace lLCroweTool.QC.EditorOnly
{
    //확인하기
    //public class CustomDataEditorShow<T> where T : ObjectDeScription_Base
    //{
    //    //해당되는 데이터
    //    private T targetData;
    //    private T targetLoadData;


    //}

    //윈도우창 샘플
    //[MenuItem("lLcroweTool/SkillDataEditor")]
    //public static void ShowWindow()
    //{
    //    EditorWindow editorWindow = GetWindow(typeof(SkillDataEditor));
    //    editorWindow.titleContent.text = "스킬데이터 생성 관리자";
    //    editorWindow.minSize = new Vector2(700, 515);
    //    editorWindow.maxSize = new Vector2(700, 515);
    //}

    /// <summary>
    /// 데이터생성용 윈도우에디터 초기상태
    /// </summary>
    /// <typeparam name="T">스크립터블 데이터타입</typeparam>
    public abstract class CustomDataWindowEditor<T> : EditorWindow where T : ScriptableObject, new()
    {
        private string dataName = "";
        private bool isNewData = false;//새로운데이터인지 로드데이터인지 체크
        private Vector2 scollPos;//스크롤 좌표
        private string temp = "";

        //저장할떄 경로데이터
        private string tag;
        private string folderName;

        //해당되는 데이터
        public static T targetData;
        public static bool isLoadData = false;//Load데이터인지,로드가 아닌 리셋인지
        private T targetLoadData;

        private static string dataContentName;//데이터컨텐츠이름

        protected static Vector2 windowMinSize = new Vector2(700, 515);
        protected static Vector2 windowMaxSize = new Vector2(700, 515);

        public string DataContentName { get => dataContentName; set => dataContentName = value; }

        //각각 윈도우에디터에 써줘야 나오는것
        //[MenuItem("lLcroweTool/ClassName_DataEditor")]
        //public static void ShowWindow()
        //{
        //    SetShowWindowSetting(typeof(ClassName_));
        //}



        //OnEnable이먼저옴
        //그다음 ShowWindow

        private void OnEnable()
        {
            isNewData = false;
            if (isLoadData)
            {
                //로드
                targetLoadData = targetData;
                if (targetLoadData != null)
                {
                    targetData = Instantiate(targetLoadData);
                    targetData.name = targetLoadData.name;
                    dataName = targetData.name;
                    isNewData = false;
                }
            }
            else
            {
                targetData = null;
                targetLoadData = null;
            }
            isLoadData = false;
            SetDataContentName(ref dataContentName);
        }

        /// <summary>
        /// ShowWindow() 함수에 집어넣는 함수
        /// ShowWindow() 상단에 [MenuItem("lLcroweTool/type의 이름이 들어감")] 집어넣기
        /// </summary>
        /// <param name="type">상속받은 자식클래스</param>
        public static void SetShowWindowSetting(Type type)
        {
            EditorWindow editorWindow = GetWindow(type);
            editorWindow.titleContent.text = $"{dataContentName} 생성 관리자";
            editorWindow.minSize = windowMinSize;
            editorWindow.maxSize = windowMaxSize;
        }

        /// <summary>
        /// 데이터컨텐츠이름 세팅하는 함수(표시해줄 대상데이터)
        /// OnEnable에서 작동
        /// </summary>
        /// <param name="dataContentName">무엇에 대한 컨텐츠이름</param>
        protected abstract void SetDataContentName(ref string dataContentName);

        /// <summary>
        /// 저장하기 전 폴더이름과 태그를 설정하는 함수
        /// </summary>
        /// <param name="labelNameOrTitle">데이터이름or타이틀</param>
        /// <param name="tag">태그</param>
        /// <param name="folderName">폴더이름 (/폴더/폴더가능)</param>
        protected abstract void SetSaveFileData(ref string labelNameOrTitle, ref string tag, ref string folderName);

        /// <summary>
        /// 데이터표시구간
        /// </summary>
        /// <param name="targetData">타겟팅된 데이터</param>
        protected abstract void DataDisplaySection(ref T targetData);

        /// <summary>
        /// 데이터리셋
        /// </summary>
        private void ResetData()
        {
            OnEnable();
        }        

        private void OnGUI()
        {
            scollPos = EditorGUILayout.BeginScrollView(scollPos, GUILayout.Height(515));

            if (DataCreateLoadSection())
            {
                EditorGUILayout.Space();
                UpDownResetAndMoreUI();
                if (!ReferenceEquals(targetData, null))
                {
                    DataDisplaySection(ref targetData);
                    UpDownResetAndMoreUI();
                }
            }
            EditorGUILayout.EndScrollView();
        }
        
        /// <summary>
        /// 데이터제작로드 섹션
        /// </summary>
        /// <returns>데이터대상 존재여부</returns>
        private bool DataCreateLoadSection()
        {
            EditorGUILayout.BeginVertical();

            if (targetData == null)
            {
                if (GUILayout.Button(dataContentName + "데이터 생성"))
                {
                    targetData = new T();                    
                    isNewData = true;
                }
            }
            else
            {
                if (GUILayout.Button("리셋"))
                {
                    ResetData();
                }
            }

            EditorGUILayout.BeginHorizontal();
            targetLoadData = (T)EditorGUILayout.ObjectField("현재선택된 " + dataContentName + "데이터", targetLoadData, typeof(T), true);
            if (GUILayout.Button(dataContentName + "데이터 로드"))
            {
                if (targetLoadData != null)
                {
                    targetData = Instantiate(targetLoadData);
                    targetData.name = targetLoadData.name;
                    dataName = targetData.name;
                    isNewData = false;
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            bool isInit = !ReferenceEquals(targetData, null);//초기화상태체크
            //존재하면
            if (isInit)
            {
                EditorGUILayout.HelpBox("현재 선택된 데이터 : " + targetData.name, MessageType.Info);

                if (!isNewData)
                {
                    //로드데이터일시
                    EditorGUILayout.HelpBox("현재 로드된 데이터 : " + targetLoadData.name + "\n현재 선택된 데이터의 이름과 다름이름으로 변경시 새로 생성됩니다.", MessageType.Warning);
                }

                if (isNewData)
                {
                    temp = "생성";
                }
                else
                {
                    if (targetLoadData == null)
                    {
                        ResetData();
                    }
                    else
                    {
                        if (targetLoadData.name == targetData.name)
                        {
                            temp = "덮어쓰기";
                        }
                        else
                        {
                            temp = "새로 생성";
                        }
                    }
                }
            }

            return isInit;
        }

        /// <summary>
        /// 데이터표시구간후와 시작부분에 보여주는 UI관련
        /// </summary>
        private void UpDownResetAndMoreUI()
        {
            if (GUILayout.Button(temp + " " + dataContentName + "데이터"))
            {
                SetSaveFileData(ref dataName, ref tag, ref folderName);//폴더이름//태그
                lLcroweUtilEditor.CreateDataObject(ref targetData, dataName, folderName, tag);

                //만들고 새롭게 로드시킴
                targetLoadData = targetData;
                targetData = Instantiate(targetLoadData);
                targetData.name = targetLoadData.name;
                dataName = targetData.name;
                isNewData = false;
            }
        }
    }
}
#endif
