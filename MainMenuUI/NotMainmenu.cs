using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace lLCroweTool.UI.MainMenu
{
    public abstract class NotMainmenu : UIBase
    {
        public Button backButton;

        protected override void Awake()
        {
            base.Awake();
            backButton.onClick.AddListener(() => ShowUI(false));
        }

        public void AddOverrideBackAction(UnityAction unityAction,bool isOverride = true)
        {
            if (isOverride)
            {
                backButton.onClick.RemoveAllListeners();
            }
            backButton.onClick.AddListener(unityAction);
        }


    }
}