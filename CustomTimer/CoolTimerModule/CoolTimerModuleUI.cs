using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


namespace lLCroweTool.TimerSystem
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(UpdateTimerModule))]
    public class CoolTimerModuleUI : MonoBehaviour, IPointerClickHandler
    {
        private bool isExistCoolTimer = false;
        private CoolTimerModule_Element targetCoolTimer;//타겟이될 쿨타임
        private Button CoolTimerButton;//버튼가져오기<--자신
        
        //수동으로 기입
        public Image skillFillUI;//스킬이미지위에 검은 타이머<--자식    
        public Image skillRepeatIcon;//스킬반복을 했을시 동작함<--자식

        private UpdateTimerModule timerModule;

        //public override void CreateObjectInitSetting()
        //{
        //    if (GetIsInit())
        //    {
        //        Debug.Log("이미 초기세팅이 됫습니다. 다시 세팅할려면 리셋시켜주세요");
        //        return;
        //    }
        //    base.CreateObjectInitSetting();

        //    //스킬 쿨타임 차기위해 보여주는 이미지오브젝트
        //    GameObject gameObject = new GameObject();
        //    gameObject.transform.parent = transform;
        //    gameObject.transform.position = new Vector2(transform.position.x + 32, transform.position.y
        //         + 32);
        //    gameObject.name = "skillFillUI ImageObject";
        //    skillFillUI = gameObject.AddComponent<Image>();
        //    skillFillUI.TryGetComponent(out RectTransform rectTransform);
        //    rectTransform.sizeDelta = new Vector2(30, 30);

        //    //스킬 반복인지 보여주기 위한 이미지 오브젝트
        //    gameObject = new GameObject();
        //    gameObject.transform.parent = transform;
        //    gameObject.transform.position = transform.position;
        //    gameObject.name = "skillRepeatIcon ImageObject";
        //    skillRepeatIcon = gameObject.AddComponent<Image>();
        //    Color color = skillRepeatIcon.color;
        //    color.a = 0.5f;
        //    skillRepeatIcon.color = color;
        //}

        protected void Awake()
        { 
            CoolTimerButton = GetComponent<Button>();

            //UI표시 업데이트
            timerModule = GetComponent<UpdateTimerModule>();
            timerModule.SetTimer(0.05f);
            timerModule.AddUnityEvent(UpdateCoolTimerModuleUI);
        }

        protected virtual void UpdateCoolTimerModuleUI()
        {
            if (isExistCoolTimer)
            {
                if (skillFillUI.fillAmount != targetCoolTimer.GetTimeValue())
                {
                    skillFillUI.fillAmount = targetCoolTimer.GetTimeValue();
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                //마우스 우클릭
                if (targetCoolTimer.isUseRepeat)
                {
                    targetCoolTimer.isRepeat = !targetCoolTimer.isRepeat;
                    skillRepeatIcon.enabled = targetCoolTimer.isRepeat;
                    if (targetCoolTimer.GetEnableSkill())
                    {   
                        targetCoolTimer.StartSkill();
                    }
                }
            }
        }

        /// <summary>
        /// 쿨타이머내용을 보여주기 위해 쿨타이머UI에 세팅해주는 함수
        /// </summary>
        /// <param name="_coolTimerModule">타겟이 될 쿨타이머 모듈</param>
        public void SetCoolTimerModuleUI(CoolTimerModule_Element _coolTimerModule)
        {
            targetCoolTimer = _coolTimerModule;
            if (ReferenceEquals(targetCoolTimer, null))
            {
                isExistCoolTimer = false;
                skillRepeatIcon.enabled = false;
                Debug.Log("타겟이 될 쿨타이머가 비어있습니다.");
            }
            else
            {
                isExistCoolTimer = true;
                skillRepeatIcon.enabled = targetCoolTimer.isRepeat;
                SetButtonEvent(delegate { targetCoolTimer.StartSkill(); });
                skillFillUI.fillAmount = targetCoolTimer.GetTimeValue();
            }
        }

        /// <summary>
        /// 버튼에 특정이벤트를 삽입//삽입할때는 쿨타이머도 작동되는 기능도 넣을것
        /// </summary>
        /// <param name="unityAction">유니티액션</param>
        public void SetButtonEvent(UnityAction unityAction)
        {
            CoolTimerButton.onClick.RemoveAllListeners();
            CoolTimerButton.onClick.AddListener(unityAction);
        }


        protected void OnDestroy()
        {
            targetCoolTimer = null;
            CoolTimerButton = null;
            skillFillUI = null;
            skillRepeatIcon = null;
            timerModule = null;
        }
    }
}
