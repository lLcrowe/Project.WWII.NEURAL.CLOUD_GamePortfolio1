using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace lLCroweTool.Session
{
    public class SessionStarterAndEnder : MonoBehaviour
    {
        // Awake나 시작할시 등록하는것도 하나의 방법
        //구지 파인지드 하지 말고 등록하는 클래스를 각 씬에다 제작하는게 맞아보임

        //세션마다 특정기능을 시작하는 클래스를 만들어야할듯
        //매니저처럼 싱글톤형식이 아님
        //이거 같은경우//Start 했을시

        //세션엔더
        //세션이 변경됫을때
        //시작이 있으면 끝이 있음
        //파괴되야할 객체임

        [Header("세션스타터 설정")]
        public UnityEvent startActionEvent = new UnityEvent();//씬이 시작했을시 작동할 이벤트들
        public float starterTimer = 0.2f;

        [Space]
        [Header("세션엔더 설정")]
        public UnityEvent endActionEvent = new UnityEvent();


        private void Start()
        {
            SessionStarterAction();
            SceneManager.sceneUnloaded += SessionEnderAction;
        }

        [ButtonMethod]
        /// <summary>
        /// 세션스타터 이벤트작동
        /// </summary>
        public void SessionStarterAction()
        {
            //
            if (!SessionManager.CheckExistScene())
            {
                return;
            }
            StartCoroutine(SessionStarterCoroutine());
        }

        //세션스타터 시작
        private IEnumerator SessionStarterCoroutine()
        {
            yield return new WaitForSeconds(starterTimer);
            startActionEvent.Invoke();
        }

        /// <summary>
        /// 세션엔더 이벤트작동
        /// </summary>
        /// <param name="scene">씬</param>
        private void SessionEnderAction(Scene scene)
        {
            if (!SessionManager.CheckExistScene())
            {
                return;
            }
            endActionEvent.Invoke();
            SceneManager.sceneUnloaded -= SessionEnderAction;//기존에 있던 액션을 빼버림
        }
    }
}
