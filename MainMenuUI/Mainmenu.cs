using Doozy.Engine.UI;
using lLCroweTool.Cinemachine;
using lLCroweTool.UI.Confirm;
using lLCroweTool.UI.Option;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace lLCroweTool.UI.MainMenu
{
    public class Mainmenu : MonoBehaviour
    {
        //메인화면에서 중간중간부분을 연결해주는 기능을 함
        //시네머신 ID룰
        //In => 어디로 들어가겠다
        //Out => 어디에서 빠져나가겠다

        //시네머신작동되는 버튼들
        public Button searchBtn;
        public Button basementBtn;
        public Button supplyBtn;
        public Button shopBtn;

        //샵과 보급소를왔다갔다하는 
        public Button shopToSupplyBtn;
        public Button supplyToShopBtn;

        //UI요소 + 일반버튼들

        //메인메뉴
        public UIView mainMenuUIView;        
        
        //전초기지
        public BasementUI basementUI;

        //탐색
        public SearchUI searchUI;

        //샵
        public ShopUI shopUI;        
        public PlayerSupplyDataUI playerSupplyDataUI;//재화처리

        //서플라이
        public SupplyUI supplyUI;

        //동료
        public Button showMyPartnersBtn;
        public MyPartnersUI myPartnersUI;

        //업적
        public Button achievementBtn;
        public AchievementUI achievementUI;

        //메인솔져 변경
        public Button changeMainScreenSoliderBtn;
        public ChangeMainScreentSoliderUI changeMainScreenSoliderUI;

        //옵션
        public Button showOptionBtn;
        public OptionSettingMenu OptionUI;


        [Header("시네머신관련")]
        public MenuPlaceType menuPlaceType;
        public CustomCinemachineManager cinemachineManager;//시네머신 매니저

        private CustomCinemachine targetCinemachine;
        public CustomCinemachine plane1Cinemachine;
        public CustomCinemachine plane2Cinemachine;

        //상태처리
        public bool showAllView = false;        

        /// <summary>
        /// 시네머신작동을 위한구역. 위치를 뜻함
        /// </summary>
        public enum MenuPlaceType
        {
            MainMenu,
            SearchMap,
            Basement,
            Supply,
            Shop,

            SuppyToShop,//보급소에서 상점으로
            ShopToSuppy,//상점에서 보급소로
        }

        protected void Awake()
        {  
            //버튼들 세팅
            //UI관련
            basementUI.showAllViewBtn.onClick.AddListener(() => ShowBasementUI(false, true));
            showMyPartnersBtn.onClick.AddListener(() =>
            {
                myPartnersUI.ShowUI(true);
                ShowMainMenuUI(false);
            });

            achievementBtn.onClick.AddListener(() => achievementUI.ShowUI(true));
            changeMainScreenSoliderBtn.onClick.AddListener(() => changeMainScreenSoliderUI.ShowUI(true));
            showOptionBtn.onClick.AddListener(() => OptionUI.ShowUI(true));

            shopUI.SetPlayerSupplyDataUI(playerSupplyDataUI);


            //시네머신관련            
            searchBtn.onClick.AddListener(() => GoTargetPlace(MenuPlaceType.SearchMap));
            basementBtn.onClick.AddListener(() => GoTargetPlace(MenuPlaceType.Basement));
            supplyBtn.onClick.AddListener(() => GoTargetPlace(MenuPlaceType.Supply));
            shopBtn.onClick.AddListener(() => GoTargetPlace(MenuPlaceType.Shop));

            shopToSupplyBtn.onClick.AddListener(() => GoTargetPlace(MenuPlaceType.ShopToSuppy));
            supplyToShopBtn.onClick.AddListener(() => GoTargetPlace(MenuPlaceType.SuppyToShop));

            //백버튼들
            basementUI.AddOverrideBackAction(() => GoTargetPlace(MenuPlaceType.MainMenu));
            searchUI.AddOverrideBackAction(() => GoTargetPlace(MenuPlaceType.MainMenu));
            shopUI.AddOverrideBackAction(() => GoTargetPlace(MenuPlaceType.MainMenu));
            supplyUI.AddOverrideBackAction(() => GoTargetPlace(MenuPlaceType.MainMenu));
            myPartnersUI.AddOverrideBackAction(() => 
            {
                myPartnersUI.ShowUI(false);
                ShowMainMenuUI(true);
            });
            //achievementUI.AddOverrideBackAction(() => achievementUI.ShowUI(false));
            //changeMainScreenSoliderUI.AddOverrideBackAction(() => changeMainScreenSoliderUI.ShowUI(false));


            //각각의 시네머신들에 이벤트추가//수동추가는 귀찮
            SetActionCinemachine("InSearchMap", () => ShowMainMenuUI(false), () => ShowSearchUI(true));
            SetActionCinemachine("OutSearchMap", () => ShowSearchUI(false), () => ShowMainMenuUI(true));

            SetActionCinemachine("InBasement", () => ShowMainMenuUI(false), () => ShowBasementUI(true, false));
            SetActionCinemachine("OutBasement", () => ShowBasementUI(false, false), () => ShowMainMenuUI(true));

            SetActionCinemachine("InSupply", () => ShowMainMenuUI(false), () => ShowSupplyUI(true));
            SetActionCinemachine("OutSupply", () => ShowSupplyUI(false), () => ShowMainMenuUI(true));

            SetActionCinemachine("InShop", () => ShowMainMenuUI(false), () => ShowShopUI(true));
            SetActionCinemachine("OutShop", () => ShowShopUI(false), () => ShowMainMenuUI(true));

            SetActionCinemachine("SuppyToShop", () => ShowSupplyUI(false), () => ShowShopUI(true));
            SetActionCinemachine("ShopToSuppy", () => ShowShopUI(false), () => ShowSupplyUI(true));
        }
        
        private IEnumerator Start()
        {
            // 초기카메라배치//처음에는 스킵 못하게 처리
            if (cinemachineManager.RequestCustomCinemachine("OutBasement", out CustomCinemachine outCinemachine))
            {
                //존재하면 가져와서 작동
                ActionCinemachineCamera(outCinemachine);
                //ActionCinemachineCamera(refCinemachine);//스킵
            }
            playerSupplyDataUI.SetActive(false);

            //비행기 시네머신작동
            yield return new WaitForSeconds(1);
            plane1Cinemachine.ActionCamera();
            yield return new WaitForSeconds(2);
            plane2Cinemachine.ActionCamera();
        }

        private void SetActionCinemachine(string id, UnityAction startAction, UnityAction endAction)
        {
            if (cinemachineManager.RequestCustomCinemachine(id, out CustomCinemachine refCinemachine))
            {
                //존재하면 가져와서 이벤트집어넣기
                refCinemachine.startEvent.AddListener(startAction);
                refCinemachine.endEvent.AddListener(endAction);
            }
        }

        private void Update()
        {
            if (showAllView)
            {
                if (Input.anyKey)
                {
                    ShowBasementUI(true, true);
                }
            }
            if (targetCinemachine != null)
            {
                if (targetCinemachine.IsRun())
                {
                    //Debug.Log("시네머신런");
                    if (Input.anyKey)
                    {
                        ActionCinemachineCamera(targetCinemachine);
                    }
                }
            }
        }


        /// <summary>
        /// 해당되는 구역으로 이동하는 함수
        /// </summary>
        /// <param name="targetMenuPlace">타겟메뉴 위치</param>
        public void GoTargetPlace(MenuPlaceType targetMenuPlace)
        {
            //같으면 아무처리안함
            if (menuPlaceType == targetMenuPlace)
            {
                return;
            }

            //각 아이디찾기
            string id = null;
            switch (targetMenuPlace)
            {
                case MenuPlaceType.MainMenu:
                    //돌아올때는 어덯게 할꼬 받아서처리
                    id = GetReturnKey(menuPlaceType);
                    break;
                case MenuPlaceType.SearchMap:
                    //맵탐색
                    //뉴럴의 탐색
                    id = "InSearchMap";
                    break;
                case MenuPlaceType.Basement:
                    //기지가는구역
                    //뉴럴의 오아시스
                    id = "InBasement";
                    break;
                case MenuPlaceType.Supply:
                    //보급소
                    //뉴럴의 제작소
                    id = "InSupply";
                    break;
                case MenuPlaceType.Shop:
                    //상점
                    //뉴럴의 상점UI대신 위치로
                    id = "InShop";
                    break;
                case MenuPlaceType.SuppyToShop:
                    //보급소에서 상점으로
                    id = "SuppyToShop";
                    targetMenuPlace = MenuPlaceType.Shop;
                    break;
                case MenuPlaceType.ShopToSuppy:
                    //상점에서 보급소으로
                    id = "ShopToSuppy";
                    targetMenuPlace = MenuPlaceType.Supply;
                    break;

            }
            menuPlaceType = targetMenuPlace;

            //시네머신작동
            if (cinemachineManager.RequestCustomCinemachine(id, out targetCinemachine))
            {              
                //존재하면 가져와서 작동
                ActionCinemachineCamera(targetCinemachine);
            }
        }

        /// <summary>
        /// 메인메뉴로 돌아가는 키값을 가져오는 함수
        /// </summary>
        /// <param name="targetMenuPlaceType"></param>
        /// <returns></returns>
        private string GetReturnKey(MenuPlaceType targetMenuPlaceType)
        {
            switch (targetMenuPlaceType)
            {
                case MenuPlaceType.SearchMap:
                    return "OutSearchMap";                    
                case MenuPlaceType.Basement:
                    return "OutBasement";                    
                case MenuPlaceType.Supply:
                    return "OutSupply";                    
                case MenuPlaceType.Shop:
                    return "OutShop";                    
                case MenuPlaceType.MainMenu:
                    return "";//사용안한것                    
            }
            return "";
        }

        /// <summary>
        /// 시네머신
        /// </summary>
        /// <param name="cinemachine"></param>
        private void ActionCinemachineCamera(CustomCinemachine cinemachine)
        {
            if (cinemachine.IsRun())
            {
                //작동중이면 스킵
                cinemachine.Skip();
            }
            else
            {
                //아니면 재생만
                cinemachine.ActionCamera();
            }
        }

        //UI관련======================================================================

        private void ShowMainMenuUI(bool isActive)
        {
            if (isActive)
            {
                mainMenuUIView.Show();
                playerSupplyDataUI.UpdateUI();
            }
            else
            {
                mainMenuUIView.Hide();
            }
            playerSupplyDataUI.SetActive(isActive);
        }

        private void ShowSearchUI(bool isActive)
        {
            searchUI.ShowUI(isActive);
        }

        private void ShowBasementUI(bool isActive, bool isChangeState)
        {
            basementUI.ShowUI(isActive);
            if (isChangeState)
            {
                showAllView = !isActive;
            }
        }

        private void ShowSupplyUI(bool isActive)
        {
            supplyUI.ShowUI(isActive);
            playerSupplyDataUI.SetActive(isActive);
        }

        private void ShowShopUI(bool isActive)
        {
            shopUI.ShowUI(isActive);
            playerSupplyDataUI.SetActive(isActive);
        }

        public void ShowMyPartnersUI(bool isActive)
        {
            //아군들 보기


        }

        public void ShowAchievementUI(bool isActive)
        {
            //업적
            //업적시스템
            //퀘스트시스템
            //모든걸 다 기록해야되는건지
        }

        public void ShowChangeMainScreenSoliderUI(bool isActive)
        {
            //메인화면 캐릭터배경 관련변경하기 위한 요소
        }

        public void ShowOptionUI(bool isActive)
        {

        }


        private void SignalToGameQuit()
        {
            //GlobalConfirmWindow.Instance.SetConfirmWindow(LocalizingManager.Instance.GetLocalLizeText("Game Exit?"), GameExit);            
            GlobalConfirmWindow.Instance.SetConfirmWindow("Game Exit?", GameExit);
        }

        //게임종료
        private void GameExit()
        {
            Debug.Log("Bye~");
            Application.Quit();
        }
    }
}