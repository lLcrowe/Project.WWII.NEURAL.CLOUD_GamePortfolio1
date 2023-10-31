using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Engine.UI;
using TMPro;

namespace lLCroweTool.UI.Confirm
{
    public class ConfirmWindow : MonoBehaviour
    {
        //여기도 좀더 좋게 만들수 있을거 같긴함//20231030

        //확인창
        //버튼 두개 or 확인 버튼 한개
        //Yes//No
        //각각 이벤트설정후 돌려받을수 있음
        //이것저것찾아보니 Doozy PopUP이라는게 현재 이것의 기능을 대체하고 있음 체크해봐야함
        [SerializeField] private TextMeshProUGUI titleTextObject;
        [SerializeField] private Image titleImageObject;
        //밑의 두버튼은 호라이즌레이아웃 하단에 배치하여 확인창과 예아니오창을 겸인하고 작업하게 제작함
        [SerializeField] private UIButton confirmButtonObject;//확인버튼
        [SerializeField] private UIButton yesButtonObject;//등록된 이벤트를 작동시키고 동작정지
        [SerializeField] private UIButton noButtonObject;//아무행동을 안함

        private UnityEvent yesUnityEvent = new UnityEvent();
        private UnityEvent noUnityEvent = new UnityEvent();
        private UnityEvent confirmUnityEvent = new UnityEvent();
        private UIView targetUIView;

        protected virtual void Awake()
        {
            targetUIView = GetComponent<UIView>();
            yesButtonObject.Button.onClick.AddListener(()=> 
            {
                OffConfirmWindowUIView();
                yesUnityEvent.Invoke();
            });
            noButtonObject.Button.onClick.AddListener(()=> 
            {
                OffConfirmWindowUIView();
                noUnityEvent.Invoke();
            });
            confirmButtonObject.Button.onClick.AddListener(() =>
            {
                OffConfirmWindowUIView();
                confirmUnityEvent.Invoke();                
            });
        }

        /// <summary>
        /// 확인창을 세팅해주는 함수
        /// </summary>
        /// <param name="titleText">제목</param>
        /// <param name="confirmAction">확인할시 기능</param>
        /// <param name="confirmButtonText">확인버튼의 텍스트</param>
        /// <param name="titleSprite">제목스프라이트 이미지</param>
        public void SetConfirmWindow(string titleText, UnityAction confirmAction, string confirmButtonText = "Confirm", Sprite titleSprite = null)
        {
            ResetEvent();
            ShowConfirmWindowUIView();

            //알림창
            //알림창여부
            //확인만 보여줌
            confirmButtonObject.gameObject.SetActive(true);
            noButtonObject.gameObject.SetActive(false);
            yesButtonObject.gameObject.SetActive(false);

            titleImageObject.sprite = titleSprite;
            titleTextObject.text = titleText;
            confirmButtonObject.SetLabelText(confirmButtonText);

            if (confirmAction == null)
            {
                confirmAction = () => { };
            }
            confirmUnityEvent.AddListener(confirmAction);
        }

        /// <summary>
        /// 확인창을 세팅해주는 함수
        /// </summary>
        /// <param name="titleText">제목</param>
        /// <param name="yesAction">'예'할시 기능</param>
        /// <param name="noAction">'아니오'할시 기능</param>
        /// <param name="yesButtonText">Yes버튼의 텍스트</param>
        /// <param name="noButtonText">No버튼의 텍스트</param>
        /// <param name="titleSprite">제목스프라이트 이미지</param>
        public void SetConfirmWindow(string titleText, UnityAction yesAction, UnityAction noAction, string yesButtonText = "Yes", string noButtonText = "No", Sprite titleSprite = null)
        {
            ResetEvent();
            ShowConfirmWindowUIView();

            //YES NO 보여줌
            confirmButtonObject.gameObject.SetActive(false);
            noButtonObject.gameObject.SetActive(true);
            yesButtonObject.gameObject.SetActive(true);

            //titleTextObject.text = LocalizingManager.Instance.GetLocalLizeText(titleText);
            //yesButtonObject.SetLabelText(LocalizingManager.Instance.GetLocalLizeText(yesButtonText));
            //noButtonObject.SetLabelText(LocalizingManager.Instance.GetLocalLizeText(noButtonText));

            titleImageObject.sprite = titleSprite;
            titleTextObject.text = titleText;
            yesButtonObject.SetLabelText(yesButtonText);
            noButtonObject.SetLabelText(noButtonText);


            if (yesAction == null)
            {
                yesAction = () => { };
            }           
            yesUnityEvent.AddListener(yesAction);

            if (noAction == null)
            {
                noAction = () => { };
            }
            noUnityEvent.AddListener(noAction);
        }

        private void ResetEvent()
        {
            confirmUnityEvent.RemoveAllListeners();
            noUnityEvent.RemoveAllListeners();
            yesUnityEvent.RemoveAllListeners();
        }
    
        /// <summary>
        /// 확인창을 보여주는 함수
        /// </summary>
        public void ShowConfirmWindowUIView()
        {
            if (targetUIView.IsShowing)
            {
                return;
            }
            targetUIView.Show();
        }

        /// <summary>
        /// 확인창을 닫아주는 함수
        /// </summary>
        private void OffConfirmWindowUIView()
        {
            if (targetUIView.IsHiding)
            {
                return;
            }
            targetUIView.Hide();
        }
    }
}