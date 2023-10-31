using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using lLCroweTool.Singleton;
using lLCroweTool.UI.Bar;
using lLCroweTool.DestroyManger;

namespace lLCroweTool.UI.Scene
{
    public class GameSceneLoadManager : MonoBehaviourSingleton<GameSceneLoadManager>
    {
        //나중에 로딩씬창은 프리팹화시켜서 리소스폴더에 짱박아놔야겠다

        //씬매니저//프로그램 시작에서부터 가지고 있음
        //씬을 변경하고 없에고 추가시키는 기능을 가진 함수        
        //게임로드씬매니저는 안부수고 게임끝날때까지 계속 가져감

        private bool isLoading = true;
          
        [Space]
        //타겟이 될 이미지 or 텍스트
        //(로딩바용)
        //로딩스크린에는 프로그래스바 백배경화면 텍스트로 구성되있음
        public GameObject loadingScreenObject;//로딩 오브젝트(커튼)

        public Sprite[] loadingImageSpriteArray = new Sprite[0];//여러로딩 스크린 이미지
        public Image loadingImageObject;//타겟 로딩 이미지
 
        //로딩관련
        public UIBar_Base targetProgressBarImage;//프로그레스바용

        //팁관련
        public GameSceneTipObjectScript gameSceneTipsData;//팁데이터
        public TextMeshProUGUI targetTipText;//팁표시용 텍스트//여기서 쓰는 타입은 보여주는 테스트로만 제한해야함//아니면 기능이 겹치니 합쳐버려도 무관할듯함        

        //애니키사용여부
        public bool isUsePushAnyKey = false; 
        public GameObject anyKeyPushTextObject;//아무킷입력으로

        //20211122
        //뭔가 이상하게 설계되서 변경
        //씬에 대한 로딩이 끝나면 작동되는 이벤트를 설정
        //해당이벤트는 Awake등에서 추가적으로 설정하거나 지우게 작업해줘야함

        //게임씬일을때만 작동되는 이벤트 
        public UnityEvent gameSceneDoneLoadEvent = new UnityEvent();//씬이 로딩이 다됫으면 작동되는 이벤트


        // 20200313
        // 오브젝트의 계층구조(중단)
        // Canvas = GameSceneLoadingCanvas
        //     LoadingObject <==로딩 오브젝트 
        //         Tip
        //         ProgressBar
        //         ProgressText

        //20230327 재작업
        // 오브젝트의 계층구조(새로제작)
        // Canvas = GameSceneLoadingCanvas
        //     LoadingObject <==로딩 오브젝트 
        //         Tip
        //         UIBar_Base(ProgressBar, ProgressText)

        //20230411 재작업
        // 오브젝트의 계층구조(재배치)
        // SceneManagerCanvas(GameSceneManager + canvas)
        //      LoadingObject (커튼, 인게임 쪽을 가리기 위한 용도)
        //          LoadingImageObject (로딩중에 보여줄 이미지를 보여주는 용도)
        //          TipObject (팁을 보여주기 위한 용도)
        //          UIBar_Base (ProgressBar, ProgressText)(로딩이 얼마나 됫는지 체크)
        //          AnyKeyPushTextObject (애니키 입력을 활성화했을때 보여주는 용도)

        protected override void Awake()
        {
            base.Awake();
            LoadingScreenDeActive();
        }

        /// <summary>
        /// 씬을 호출하는 함수
        /// </summary>
        /// <param name="sceneName">씬이름</param>
        /// <param name="doneEvent">작업완료시 이벤트</param>
        /// <param name="loadSceneMode">씬로드시할때 모드</param>
        public void CallScene(string sceneName, UnityAction doneEvent, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            if (doneEvent == null)
            {
                doneEvent = () => { };
            }
            gameSceneDoneLoadEvent.AddListener(doneEvent);            
            StartCoroutine(LoadScene(sceneName, loadSceneMode));            
        }
        
        /// <summary>
        /// 씬을 로딩하는 함수
        /// </summary>
        /// <param name="nextScene">로딩할 다음씬</param>
        /// <param name="loadSceneMode">로딩모드</param>
        private IEnumerator LoadScene(string nextScene, LoadSceneMode loadSceneMode)
        {
            //초기화

            //체크좀하자//빠르긴함
            //SceneManager.LoadScene(nextScene, loadSceneMode);
            //yield break;

            //느린데
            AsyncOperation op = SceneManager.LoadSceneAsync(nextScene, loadSceneMode);
            op.allowSceneActivation = false;
            loadingScreenObject.SetActive(true);
            isLoading = true;

            targetProgressBarImage.InitUIBar(0, 100, 0);
            //"맵환경을 불려오고 있습니다. - " + temp * 100f + "%" + " -"
            targetProgressBarImage.labelFormat = "맵환경을 불려오고 있습니다. - {cur}% -";
            var waitForEndOfFrame = new WaitForEndOfFrame();            

            //이미지체크
            if (loadingImageSpriteArray.Length == 0)
            {
                loadingImageObject?.gameObject.SetActive(false);
            }
            else
            {
                loadingImageObject?.gameObject.SetActive(true);
                loadingImageObject.sprite = loadingImageSpriteArray[Random.Range(0, loadingImageSpriteArray.Length)];
            }


            //팁처리 로직
            if (gameSceneTipsData != null)
            {
                StartCoroutine(GenerateTips());                
            }

            //애니키
            if (isUsePushAnyKey)
            {
                anyKeyPushTextObject.SetActive(false);
            }

            //로딩바처리 구간
            //while (!op.isDone)
            do
            {
                //0.9까지 오르고 안올라감//0.9가 최고 상태
                float value = Mathf.Clamp01(op.progress / 0.9f);
                targetProgressBarImage.SetCurValue(value * 10f);
                yield return waitForEndOfFrame;

                if (op.progress < 0.9f)
                {
                    continue;
                }

                //딜레이로딩처리
                float temp = 10f;
                while (temp < 100 && !op.isDone)
                {
                    temp += Time.deltaTime * 100f;
                    targetProgressBarImage.SetCurValue(temp);
                    yield return waitForEndOfFrame;
                }

                //씬로딩완료//장면이 준비되는 즉시 활성화되도록 허용
                op.allowSceneActivation = true;

                //애니키 미사용시 곧장 넘어감
                //씬로딩완료//키입력관련
                if (isUsePushAnyKey)
                {
                    if (!anyKeyPushTextObject.activeSelf)
                    {
                        anyKeyPushTextObject.SetActive(true);
                    }
                    while (true)
                    {
                        if (Input.anyKeyDown)
                        {
                            break;
                        }

                        //이걸로 안하면 인풋키 안먹힘
                        yield return null;
                    }
                }
            } while (!op.isDone);
            //메모리정리
            DestroyManager.AllocateForMemory();


            //작동되기위한 선언작동
            isLoading = false;            
            gameSceneDoneLoadEvent?.Invoke();
            gameSceneDoneLoadEvent.RemoveAllListeners();            
            LoadingScreenDeActive();
        }

        /// <summary>
        /// 팁을 보여주기 위한 코루틴
        /// </summary>
        /// <returns></returns>
        private IEnumerator GenerateTips()
        {
            targetTipText.gameObject.SetActive(true);
            int tipCount = Random.Range(0, gameSceneTipsData.tips.Length);            
            targetTipText.text = gameSceneTipsData.tips[tipCount];

            var WaitForSecondAlpha = new WaitForSeconds(5.0f);
            var WaitForSecondNextText = new WaitForSeconds(0.5f);

            while (loadingScreenObject.activeInHierarchy)
            {
                yield return WaitForSecondAlpha;
                //알파값줄여주는곳
                //Timing.RunCoroutine(_SetTextFadeOut(tipsData.tips[tipCount]));
                yield return WaitForSecondNextText;

                tipCount++;
                if (tipCount >= gameSceneTipsData.tips.Length)
                {
                    tipCount = 0;
                }
                targetTipText.text = gameSceneTipsData.tips[tipCount];
                //알파값늘려주는곳
                //Timing.RunCoroutine(_SetTextFadeIn(tipsData.tips[tipCount]));
            }
        }       

        //씬을 지우는 함수
        //필요없어보임
        public void RemoveScene(string sceneName)
        {
            //좀더 찾아볼껄
            SceneManager.UnloadSceneAsync(sceneName);
        }

        public void RemoveScene(int sceneIndex)
        {
            //좀더 찾아볼껄
            SceneManager.UnloadSceneAsync(sceneIndex);
        }       

        //인보그로 작동
        //로딩을 한다음 
        //로딩스크린을 지울때까지 시간을 멈추게 할예정
        //private void PauseStartGame()
        //{
        //    TimerModuleManager.Instance.GamePause();
        //}

        /// <summary>
        /// 씬이 로딩이 완료시 비활성화될 오브젝트들을 비활성화 하는 함수
        /// </summary>
        private void LoadingScreenDeActive()
        {   
            loadingScreenObject.SetActive(false);
            //WorldTimer.Instance.GamePause();
            //PauseStartGame();
            StopAllCoroutines();
        }
       
        //20200312
        //게임씬 인덱스
        //현재방식대로면 문제없어보이니 건들지않을예정
        //하지만 참고는 되니 주석으로만 처리
        //public enum EGameSceneIndexs
        //{
        //    MANAGER = 0,//매니저관리법에 따라 다르게 작동시켜야함
        //    //1. 매니저를 초반시작시 같이 둔다음 dontdestoryobject로 처리하면 싱글씬으로 해도 상관없음
        //    //2. 따로 매니저씬을 구상해논다음 더하기씬으로 집어넣는것도 좋음
        //    TITLE_SCREEN = 1,//메인메뉴            
        //    MAP = 2,//맵 GameMapManager를 통해 작업을 할 예쩡
        //}

        //씬들을 나열해보자
        //맵씬 
        //게임메인메뉴씬
        //게임오버씬
        //게임승리씬
        //등등



        //게임이 처음시작시에 작동되는 씬을 체크해보자
        //LogoScene
        //MainmenuScene
        //SpaceShipScene
        //MapScene ->계속 바꿔주는것

    }

}
