
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using lLCroweTool.TimerSystem;

namespace lLCroweTool.ToolTipSystem
{
    [RequireComponent(typeof(Image))]
    public class ToolTipUITargetImage : UpdateTimerModule_Base, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler 
    {
        //툴팁 타겟//슬롯과는 별개로 작업해줘야함
        //툴팁을 사용하는 UI오브젝트에 붙착시켜서 들어가면 툴팁을 보여주게 해주는 오브젝트
        private Image imageObject;//이미지 오브젝트
        [SerializeField] private IconLabelBase objectDeScriptionData;//툴팁데이터용

        //일정시간후에 툴팁을 표시할수 있게
        public bool isUseWaitShow;
        public float waitTime = 1f;
        private float time;
        private bool OnPointerOn = false;
        
        protected override void Awake()
        {
            imageObject = GetComponent<Image>();
        }

        public override void UpdateTimerModuleFunc()
        {
            if (isUseWaitShow && OnPointerOn)
            {
                if (Time.time > waitTime + time)
                {
                    GlobalToolTipUiView.Instance.MoveToolTip(transform);
                    GlobalToolTipUiView.Instance.ShowText(objectDeScriptionData.labelID, objectDeScriptionData.description, objectDeScriptionData.icon);                  
                }
            }
        }

        public void SetToolTipUITarget(IconLabelBase _objectDeScriptionData)
        {
            if (ReferenceEquals(_objectDeScriptionData, null))
            {
                imageObject.sprite = null;
                objectDeScriptionData = null;
            }
            else
            {
                imageObject.sprite = _objectDeScriptionData.icon;
                objectDeScriptionData = _objectDeScriptionData;
            }
        }

        protected virtual string GetContentText()
        {
            return lLcroweUtil.GetCombineString(objectDeScriptionData.description);
        }

        /// <summary>
        /// 툴팁범위에 마우스를 올려놓을시 작동되는 기능
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (objectDeScriptionData == null)
            {
                return;
            }
            if (isUseWaitShow)
            {
                OnPointerOn = true;
                time = Time.time;               
            }
            else
            {
                GlobalToolTipUiView.Instance.MoveToolTip(transform);
                GlobalToolTipUiView.Instance.ShowText(objectDeScriptionData.labelID, GetContentText(), objectDeScriptionData.icon);
            }
        }

        

        /// <summary>
        /// 툴팁범위에 마우스를 꺼낼시 작동되는 기능
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (objectDeScriptionData == null)
            {
                return;
            }
            GlobalToolTipUiView.Instance.OffText();
            GlobalToolTipUiView.Instance.ClearText();
            OnPointerOn = false;
        }

        /// <summary>
        /// 툴팁범위에 마우스를 클릭할시 작동되는 기능
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerClick(PointerEventData eventData)
        {   
        }
    }
}