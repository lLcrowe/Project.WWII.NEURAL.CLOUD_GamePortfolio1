using lLCroweTool.Singleton;
using System.Collections.Generic;
using lLCroweTool.UI.Scene;
using UnityEngine;
using UnityEngine.Events;

namespace lLCroweTool.Session
{
    public class SessionManager : MonoBehaviourSingleton<SessionManager>
    {
        //세션끼리의 목적에 따라 연결을 유지하기 위한 용도
        //로딩처리하는구역과 연동됨

        //유닛.
        //뉴럴클라우드는 크게 두씬같다
        [SceneAttribute] public string mainMenuScene;//메인메뉴
        //[SceneTag] public string battleStationScene;//전장

        /// <summary>
        /// 해당씬으로 진입하는 함수
        /// </summary>
        /// <typeparam name="T">컴포넌트타입</typeparam>
        /// <param name="targetComponentList">타겟컴포넌트 리스트들</param>
        /// <param name="sceneName">씬이름</param>
        /// <param name="action">액션</param>
        public void LoadingTheTargetScene<T>(List<T> targetComponentList, string sceneName, UnityAction action) where T : Component
        {
            //해당유닛들의 위치를 자기자신하위로 보내 안사라지게 만드는 작업
            for (int i = 0; i < targetComponentList.Count; i++)
            {
                var temp = targetComponentList[i];
                temp.transform.parent = transform;
            }

            //보내버리기
            //로딩처리            
            GameSceneLoadManager.Instance.CallScene(sceneName, action);
        }

        /// <summary>
        /// 메인메뉴로 진입하는 함수
        /// </summary>
        public void LoadingMainMenu()
        {
            ObjectPoolManager.Instance.dynamicPoolBible.ResetCustomPoolBible();
            GameSceneLoadManager.Instance.CallScene(mainMenuScene, () => { });
        }
    }
}