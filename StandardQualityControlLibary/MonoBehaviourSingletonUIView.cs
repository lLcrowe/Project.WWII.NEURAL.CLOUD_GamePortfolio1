using Doozy.Engine.UI;
using lLCroweTool.LogSystem;
using UnityEngine;

namespace lLCroweTool.SingletonUI
{
    //public class MonoBehaviourSingletonUIView<T> : MonoBehaviourSingleton<MonoBehaviourSingletonUIView<T>> where T : MonoBehaviourSingletonUIView<T>
    public class MonoBehaviourSingletonUIView<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (ReferenceEquals(instance, null))
                {
                    instance = FindObjectOfType<T>();
                    if (ReferenceEquals(instance, null))
                    {   
                        LogManager.Register("MonoBehaviourSingletonUIView", "MonoBehaviourSingletonUIView", true, true);
                        LogManager.Log("MonoBehaviourSingletonUIView", "UI싱글턴을 사용하는 친구들은 미리세팅되어야합니다.", null, LogManager.LogType.Error);
                    }
                }
                return instance;
            }
        }

        private UIView targetUIView;//타겟이 될 UIView

        protected virtual void Awake()
        {
            instance = this as T;
            targetUIView = GetComponent<UIView>();
            DontDestroyOnLoad(gameObject);
            OffUIView();
        }

        /// <summary>
        /// UI 뷰 숨기기
        /// </summary>
        public virtual void ShowUIView()
        {
            if (!targetUIView.IsShowing)
            {
                targetUIView.Show();
            }
        }

        /// <summary>
        /// UI 뷰 숨기기
        /// </summary>
        public virtual void OffUIView()
        {
            if (!targetUIView.IsHiding)
            {
                targetUIView.Hide();
            }
        }
    }
}