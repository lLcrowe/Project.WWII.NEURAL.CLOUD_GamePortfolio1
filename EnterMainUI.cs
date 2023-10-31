using DG.Tweening;
using lLCroweTool.Cinemachine;
using lLCroweTool.Session;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace lLCroweTool
{
    public class EnterMainUI : MonoBehaviour
    {
        //게임처음시작시 나오는 씬에서 작동될 코드
        public Button gameStartButton;
        public CustomCinemachine loopCinemachine;
        public Image fadeImage;
        private bool isClick = false;

        private void Awake()
        {
            gameStartButton.onClick.AddListener(() =>
            {
                if (isClick)
                {
                    return;
                }
                isClick = true;
                StartCoroutine(FadeAndGoMainMenu());
            });
        }

        private void Start()
        {
            loopCinemachine.ActionCamera();
        }

        private IEnumerator FadeAndGoMainMenu()
        {
            float fadeTime = 0.5f;
            fadeImage.DOColor(new Color(0, 0, 0, 1), fadeTime);
            yield return new WaitForSeconds(fadeTime);
            SessionManager.Instance.LoadingMainMenu();
        }
    }
}


