using UnityEditor;
using UnityEngine;

namespace lLCroweTool.QC.EditorOnly
{
    //[CustomEditor(typeof(T))]//수동으로 표시해주기
    //불가능//CustomEditor 어트리뷰트는 T를 사용못함//수동으로
    //[CustomEditor(typeof(CustomDataInspecterEditor))]형식으로 상속받을때마다 붙혀주기
    //[CustomEditor(typeof(CustomDataInspecterEditor), true)] 쓰면 해당 오브젝트에 필요한걸 못 체크하니 필요할때 만들어버리기
    public abstract class CustomDataInspecterEditor<T> : Editor where T : Component
    {
        //인스팩터에디터 제너릭화
        //20221118 재작업완료
        //3개의 구역으로 나눔
        //초기화구역
        //데이터표시구역
        //자동생성구역

        protected T targetObject;
        private string content = "";

        protected void OnEnable()
        {
            targetObject = (T)target;
            InitAddFunc();
        }

        public sealed override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("-= 인스팩터에디터 =-");

            //자동생성 체크
            content = "";
            if (CheckAutoGenerate(ref content))
            {
                EditorGUILayout.HelpBox(content, MessageType.Warning);
                if (GUILayout.Button("자동생성"))
                {
                    AutoGenerateSection();
                }
                return;
            }
            EditorGUILayout.Space();
            DisplaySection();
        }

        /// <summary>
        /// 활성화 초기화기능//스크립터블의 new하고 싶으면 월드에서 하는게 아니라 onEnable에서 처리해야하므로 현함수에서 new 해줄것
        /// </summary>
        protected abstract void InitAddFunc();

        /// <summary>
        /// 자동생성 여부를 체크하는 함수
        /// </summary>
        /// <param name="content">경고사항 내용</param>
        /// <returns>자동생성해야되는 여부</returns>
        protected abstract bool CheckAutoGenerate(ref string content);

        //예시
        //protected override bool CheckAutoGenerate(ref string content)
        //{
        //    bool isCheck = base.CheckAutoGenerate(ref content);

        //    content += "-=XXXXXXXX 필요사항=-\n";

        //    return isCheck;
        //}


        /// <summary>
        /// 자동생성 구역
        /// </summary>
        protected abstract void AutoGenerateSection();
        //XXXXXX.gameObject.name = "SkillSlotUI";//이름 지정

        /// <summary>
        /// 표시구간//생성이 다됫을시 보여줌
        /// </summary>
        protected abstract void DisplaySection();
    }
}