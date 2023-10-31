using lLCroweTool.TimerSystem;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace lLCroweTool.SkillSystem
{
    public class SkillSlotUI : CoolTimerModuleUI
    {
        //스킬슬롯을 캐싱하여 해당되는 스킬슬롯을 참조하여 UI에 표시해주는 컴포넌트
        //추가적인 역할로 스킬트리매니저를 켯을시 또는 일반적으로 드래그앤 드랍으로 자리를 이동할수 있게 변경
        //툴팁 추가
        //스킬트리UI에서 사용할 스킬트리슬롯
        //인벤토리슬롯과 사용방법은 동일
        //단 인벤토리슬롯과는 호환안됨

        //스킬슬롯을 교체하는 방법은 
        //1. 인게임에서 작동될때가 아닌 함선내에서 안전지대에 있을때 교체할수 있게하는방법이나        
        //2. 아니면 해당 스킬슬롯을 아예 제로해서 작동되게하는법


        //부모객체의 쿨타임 모듈 타겟을 스킬슬롯의 슬롯타겟으로 변경       
        [SerializeField] private bool isExistSkillSlot = false;
        [SerializeField] private int cashStackAmount;//스택량을 가져온후 업데이트때 비교하기위한것
        [SerializeField] private SkillSlot skillSlot;//스택을 체크하기위해 가져옴//스킬슬롯은 무조건 있지만 스킬데이터는 무조건 있지는 않음

        //스택표시관련
        public TextMeshProUGUI countTextObject;
        public Image countImageObject;

        //기능명시
        //자기자신 상대방
        //false false 일경우 해당
        //false true
        //true false
        //true true
        //에 있는 스킬데이터는 사용하고 있는 스킬오브젝트이다.//드래그를 할시 교체만
        protected override void UpdateCoolTimerModuleUI()
        {
            base.UpdateCoolTimerModuleUI();
            if (isExistSkillSlot && skillSlot.GetSkillData().isUseStack)
            {
                if (cashStackAmount != skillSlot.GetStackCurNum())
                {
                    cashStackAmount = skillSlot.GetStackCurNum();
                    countTextObject.text = cashStackAmount.ToString();
                }
            }
        }

        /// <summary>
        /// 스킬슬롯UI안의 스킬슬롯을 스왑하는 함수
        /// </summary>
        /// <param name="skillSlotUIA">스킬슬롯UIA</param>
        /// <param name="skillSlotUIB">스킬슬롯UIB</param>
        public static void SwapSkillSlotUI(SkillSlotUI skillSlotUIA, SkillSlotUI skillSlotUIB)
        {
            //스킬슬롯만 변경 UI는 그대로 둠
            SkillSlot a = skillSlotUIA.GetSkillSlot();
            SkillSlot b = skillSlotUIB.GetSkillSlot();

            SkillSlot.SwapSkillSlot(a, b);

            //UI재세팅
            skillSlotUIA.SetSkillSlotUI(a);
            skillSlotUIB.SetSkillSlotUI(b);
        }

        /// <summary>
        /// 스킬슬롯UI를 스킬슬롯에 맞게 세팅할때 사용하는 함수
        /// </summary>
        /// <param name="_skillSlot">타겟팅할 스킬슬롯</param>
        /// <param name="_index">단축키버튼 인덱스</param>
        public void SetSkillSlotUI(SkillSlot _skillSlot)
        {
            skillSlot = _skillSlot;
            SkillObjectScript skillData = _skillSlot.GetSkillData();

            if (ReferenceEquals(skillSlot, null))
            {
                isExistSkillSlot = false;
                ActiveTextObject(false);
            }
            else
            {
                isExistSkillSlot = true;
                
                //스킬슬롯안의 데이터가 있는지
                if (!ReferenceEquals(skillData, null))
                {
                    SetCoolTimerModuleUI(skillSlot.skillCooltimer);

                    //스택여부 체크
                    if (skillData.isUseStack)
                    {
                        //스택을 보여줌
                        cashStackAmount = skillSlot.GetStackCurNum();
                        ActiveTextObject(true);
                        countTextObject.text = cashStackAmount.ToString();
                    }
                    else
                    {
                        //스택을 안보여줌
                        ActiveTextObject(false);
                        cashStackAmount = 0;
                    }
                }

                //버튼에 이벤트 추가
                SetButtonEvent(delegate
                {
                    //키가져오기
                    //SkillManager.Instance.SetUseKeyCode(PlayerInPutKeySetting.Instance.NormalKeyBible[PlayerInPutKeySetting.Instance.skillToolBarKeys[index]]);
                    SkillManager.Instance.ActionSkill(_skillSlot, true);//AI가 쓰는 함수는 따로 제작하자//20221130
                }
                );
            }
        }

        /// <summary>
        /// 텍스트오브젝트 활성화기능관련 함수
        /// </summary>
        /// <param name="isShow">보여줄지</param>
        private void ActiveTextObject(bool isShow)
        {
            countTextObject.gameObject.SetActive(isShow);
            countImageObject.gameObject.SetActive(isShow);
        }

        /// <summary>
        /// 스킬슬롯이 존재여부를 가져오는 함수
        /// </summary>
        /// <returns>스킬슬롯존재여부</returns>
        public bool GetIsExistSkillSlot()
        {
            return isExistSkillSlot;
        }

        /// <summary>
        /// 스킬슬롯가져오기
        /// </summary>
        /// <returns>스킬슬롯</returns>
        public SkillSlot GetSkillSlot()
        {
            return skillSlot;
        }
    }
}