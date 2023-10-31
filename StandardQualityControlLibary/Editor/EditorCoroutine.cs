
using System.Collections;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
namespace lLCroweTool.QC.EditorOnly
{
    public struct EditorCoroutine
    {
        //에디터코루틴
        private IEnumerator coroutine;
        private bool isRun;
        public bool IsRun { get => isRun; }

        public EditorCoroutine(IEnumerator value)
        {
            //생성시키고 시작하기
            coroutine = value;
            isRun = false;
            Start();            
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (mode == LoadSceneMode.Single)
            {
                UnityEditor.EditorApplication.update -= UpdateCoroutine;
            }
        }

        private void Start()
        {
            UnityEditor.EditorApplication.update += UpdateCoroutine;
            SceneManager.sceneLoaded += OnSceneLoaded;
            isRun = true;
        }
        private void Stop()
        {
            UnityEditor.EditorApplication.update -= UpdateCoroutine;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            isRun = false;
        }

        private void UpdateCoroutine()
        {
            if (coroutine.MoveNext())
            {
                return;
            }
            Stop();
        }
    }

    //사용법
    //private IEnumerator Func()
    //{
    //    yield return null;
    //}
    //EditorCoroutine editorCoroutine = new EditorCoroutine(Func());
}
#endif
