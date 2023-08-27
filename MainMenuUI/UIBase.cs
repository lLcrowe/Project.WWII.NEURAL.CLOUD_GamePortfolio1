using Doozy.Engine.UI;
using UnityEngine;

namespace lLCroweTool.UI.MainMenu
{
    public abstract class UIBase : MonoBehaviour
    {
        //UIView들을 사용하는 UI들을 위한 클래스

        private UIView targetUIView;//타겟이 될 UIView

        protected virtual void Awake()
        {
            targetUIView = GetComponent<UIView>();
            OffUIView();
        }


        public void ShowUI(bool isActive)
        {
            if (isActive)
            {
                ShowUIView();
            }
            else
            {
                OffUIView();
            }
        }


        /// <summary>
        /// UI 뷰 보여주기
        /// </summary>
        public virtual void ShowUIView()
        {
            if (targetUIView == null)
            {
                targetUIView = GetComponent<UIView>();
            }
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
            if (targetUIView == null)
            {
                targetUIView = GetComponent<UIView>();
            }
            if (!targetUIView.IsHiding)
            {
                targetUIView.Hide();
            }
        }
    }
}