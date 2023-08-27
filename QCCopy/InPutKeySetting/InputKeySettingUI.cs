using lLCroweTool.LogSystem;
using lLCroweTool.SingletonUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace lLCroweTool.UI.MainMenu
{

    /// <summary>
    /// 키세팅을 바꿔줄때 사용하는 UI클래스
    /// </summary>    
    public class InputKeySettingUI : NotMainmenu
    {   
        public InputKeySettingButton inputKeySettingButtonPrefab;   //인풋세팅UI카드 프리팹오브젝트
        public Transform[] buttonPosArray = new Transform[2];       //인풋세팅UI카드의 위치들3개
        public Color selectColor = Color.yellow;                    //선택했을시 버튼의 바탕색이 변경될 색

        [SerializeField] private bool isInit = false;//초기화여부
        private bool isSetting = false;//키변경버튼을 누를시부터 작동되는 세팅중 여부
        private bool isCancel = false;//취소여부
        private KeyCode targetKeyCode;//타겟팅할 키코드


        protected override void Awake()
        {
            base.Awake();
            LogManager.Register(name, name, true, true);
        }

        //private void Start()
        //{
        //    //세팅이 안되있으면 각레이어마다 세팅되게 돌린다
        //    if (!isInit)
        //    {
        //        InitInputSettingUI();
        //    }
        //}

        /// <summary>
        /// 인풋키세팅UI를 초기화하는 함수
        /// </summary>
        public void InitInputSettingUI()
        {
            foreach (KeyValuePair<ECustomKeyCode, KeyCode> item in PlayerInPutKeySetting.Instance.NormalKeyBible)
            {
                InputKeySettingButton temp = Instantiate(inputKeySettingButtonPrefab, buttonPosArray[0]);
                temp.InitInputSettingUICard(item.Key.ToString(), item.Value, delegate { InputKeySettingButtonFunc(item.Key, temp, PlayerInPutKeySetting.Instance.NormalKeyBible); }, selectColor);
            }

            foreach (KeyValuePair<ECustomKeyCode, KeyCode> item in PlayerInPutKeySetting.Instance.SecondaryKeyBible)
            {
                InputKeySettingButton temp = Instantiate(inputKeySettingButtonPrefab, buttonPosArray[1]);
                temp.InitInputSettingUICard(item.Key.ToString(), item.Value, delegate { InputKeySettingButtonFunc(item.Key, temp, PlayerInPutKeySetting.Instance.SecondaryKeyBible); }, selectColor);
            }
            isInit = true;
        }

        public override void ShowUIView()
        {
            if (!isInit)
            {
                InitInputSettingUI();
            }
            base.ShowUIView();
        }

        /// <summary>
        /// 인풋키세팅버튼 기능
        /// </summary>
        /// <param name="keyID">세팅당할 키코드</param>
        /// <param name="inputSettingButton">인풋키세팅버튼</param>
        /// <param name="targetKeyBible">키 사전</param>        
        private void InputKeySettingButtonFunc(ECustomKeyCode keyID, InputKeySettingButton inputSettingButton, Dictionary<ECustomKeyCode, KeyCode> targetKeyBible)
        {
            if (isSetting)
            {
                return;
            }        
            isSetting = true;
            StartCoroutine(UpdateKeySettingCoroutine(keyID, inputSettingButton, targetKeyBible));
        }

        private void OnGUI()
        {
            //코루틴으로 인해 작동되고 있으면 그때부터 작동
            if (isSetting)
            {
                Event evnet = Event.current;
                if (evnet.isKey)
                {
                    targetKeyCode = evnet.keyCode;
                    isSetting = false;
                    if (targetKeyCode == KeyCode.Escape)
                    {
                        isCancel = true;
                    }
                }
                else if (evnet.isMouse)
                {   
                    targetKeyCode = (KeyCode)evnet.button + 323;
                    isSetting = false;
                }
            }
        }

        private IEnumerator UpdateKeySettingCoroutine(ECustomKeyCode keyID, InputKeySettingButton inputSettingButton, Dictionary<ECustomKeyCode, KeyCode> targetKeyBible)
        {
            WaitForEndOfFrame wait = new WaitForEndOfFrame();
            do
            {
                //OnGUI에서 세팅이 되면 넘어감
                //여기서 이벤트받아오니 이벤트가 제대로 안넘어옴
                if (isCancel)
                {
                    isCancel = false;
                    break;
                }


                if (!isSetting)
                {   
                    //해당되는 키 사전에서 중복체크
                    if (targetKeyBible.ContainsValue(targetKeyCode))
                    {
                        //중복됨
                        //나중에 컨펌창 띄워주기 구역                    
                        LogManager.Log(typeof(InputKeySettingUI), "중복됩니다", inputSettingButton.gameObject, LogManager.LogType.Info);
                        break;
                    }
                    else
                    {
                        targetKeyBible[keyID] = targetKeyCode;
                        inputSettingButton.ChangeButtonText(targetKeyCode.ToString());
                        break;
                    }
                }
                yield return wait;
            } while (true);
            inputSettingButton.ChangeButtonColor(Color.white);
        }
    }
}
