using lLCroweTool.UI.CustomUIButton;
using UnityEngine;

namespace lLCroweTool.UI
{
    [RequireComponent(typeof(CustomButton))]
    public class EnterPointerAnimToButton : MonoBehaviour
    {
        //버튼
        public string enterStateName = "Transition";
        public string exitStateName = "Stop";
        public Animator animator;
        private CustomButton customButton;


        private void Awake()
        {
            customButton = GetComponent<CustomButton>();
            customButton.onPointerEnter += EnterAnim;
            customButton.onPointerExit += ExitAnim;
        }

        private void OnEnable()
        {
            //애니메이션 초기화
            ExitAnim();
        }

        public void EnterAnim()
        {
            if (animator != null)
            {
                //if (animator.enabled == false)
                //{
                //    animator.enabled = true;
                //}
                animator.Play(enterStateName, 0, 0);
            }
        }
        public void ExitAnim()
        {
            if (animator != null)
            {
                //if (animator.enabled == false)
                //{
                //    animator.enabled = true;
                //}
                animator.ResetTrigger(exitStateName);
            }
        }
    }
}

