using UnityEngine;
using TMPro;
using Doozy.Engine.UI;
using Doozy.Engine.Progress;
using UnityEngine.Audio;
using System.Collections.Generic;
using UnityEngine.UI;

using lLCroweTool.UI.MainMenu;

namespace lLCroweTool.UI.Option
{
    public class OptionSettingMenu : NotMainmenu
    {
        //게임설정에 대한 메뉴설정창이다

        //해상도관련
        //Brackeys쪽의 세팅메뉴 영상을 참고함
        //https://www.youtube.com/watch?v=YOaYQrN1oYQ

        private Resolution[] resolutions;
        public TMP_Dropdown resolutionDropdown;
        public UIToggle fullDisplayToggle;
        public TMP_Dropdown qualityDropdown;

        //초기 사운드믹스 초기화관련 메모해주기 //20230330
        //AudioMixer를 클릭해서 오디오믹서 창을 열고
        //Master를 클릭해보면 인스팩터창에서 볼륨 우클릭하여 Expose 뭐시갱이하면
        //거기에 해당되는 스크립트로 추출됨. 이름을 변경한후 사용하는데
        //String으로 받아서 처리함. 그 string하고 오디오변경이랑 동일하게 처리하면 작동
        
        //오디오관련
        public AudioMixer audioMixer;

        //오디오토글
        public UIToggle audioToggle;

        //해당 프로그래서 안에 슬라이더가 같이 포함된상태로 세팅해줘야함
        //마스터볼륨
        //public UIToggle masterVolumeToggle;
        //public Progressor masterVolumeProgressor;
        public Slider masterVolumeSlider;
        //음악볼륨
        //public UIToggle musicVolumeToggle;
        //public Progressor musicVolumeProgressor;
        public Slider musicVolumeSlider;
        //이팩트볼륨
        //public UIToggle sFXVolumeToggle;
        //public Progressor sFXVolumeProgressor;
        public Slider sFXVolumeSlider;
        //UI볼륨
        //public UIToggle uiVolumeToggle;
        //public Progressor uiVolumeProgressor;
        public Slider uiVolumeSlider;
        //환경음
        //public UIToggle ambienceVolumeToggle;
        //public Progressor ambienceVolumeProgressor;
        public Slider ambienceVolumeSlider;

        //public Progressor frameProgressor;
        public Slider frameSlider;

        //키값세팅관련
        public UIButton showInputKeySettingButton;//키값세팅메뉴를 열기위한 버튼
        public InputKeySettingUI inputKeySettingUI;//키값세팅UI

        //그래픽관련
        protected override void Awake()
        {
            base.Awake();
        
            //그래픽세팅 체크
            //해상도체크
            resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();

            //리스트 문자열에 각솔루션의 문자들을 집어넣음
            List<string> options = new List<string>();
            int currentResolutionIndex = 0;

            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " X " + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }

            //dropdown UI의 옵션에 리스트 문자열을 더해줌
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
            resolutionDropdown.onValueChanged.AddListener(SetResolution);
            //템플릿에 스크롤이 있음
            if (resolutionDropdown.template.TryGetComponent(out ScrollRect scroll))
            {
                scroll.scrollSensitivity = 100;
            }

            //전체화면 토글체크
            fullDisplayToggle.OnClick.OnToggleOn.Event.AddListener(delegate { SetFullScreenToggle(true); });
            fullDisplayToggle.OnClick.OnToggleOff.Event.AddListener(delegate { SetFullScreenToggle(false); });
            fullDisplayToggle.IsOn = Screen.fullScreen;


            //퀼리티세팅
            //프로젝트 세팅 -> 퀄리티
            options.Clear();
            options.AddRange(QualitySettings.names);

            qualityDropdown.ClearOptions();
            qualityDropdown.AddOptions(options);            
            qualityDropdown.value = QualitySettings.GetQualityLevel();
            qualityDropdown.RefreshShownValue();
            qualityDropdown.onValueChanged.AddListener(SetQualityGame);
            //템플릿에 스크롤이 있음
            if (resolutionDropdown.template.TryGetComponent(out scroll))
            {
                scroll.scrollSensitivity = 100;
            }
            
            //오디오세팅
            //밸류체인지,슬라이더 체크
            //마스터
            int minValue = -80;
            int maxValue = 0;
            float powerValue = 0;//현재값

            audioToggle.OnClick.OnToggleOn.Event.AddListener(() => SetAudioToggle(true));
            audioToggle.OnClick.OnToggleOff.Event.AddListener(() => SetAudioToggle(false));

            audioMixer.GetFloat("Master", out powerValue);//값가져와서
            SetSliderInProgressor(masterVolumeSlider,minValue, maxValue, powerValue);//세팅
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);//변할수 있게 이벤트

            //음악
            audioMixer.GetFloat("Music", out powerValue);
            SetSliderInProgressor(musicVolumeSlider, minValue, maxValue, powerValue);
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);

            //이팩트
            audioMixer.GetFloat("SFX", out powerValue);
            SetSliderInProgressor(sFXVolumeSlider, minValue, maxValue, powerValue);
            sFXVolumeSlider.onValueChanged.AddListener(SetSFXVolume);

            //UI
            audioMixer.GetFloat("UI", out powerValue);
            SetSliderInProgressor(uiVolumeSlider, minValue, maxValue, powerValue);
            uiVolumeSlider.onValueChanged.AddListener(SetUIVolume);

            //환경음
            audioMixer.GetFloat("Ambience", out powerValue);
            SetSliderInProgressor(ambienceVolumeSlider, minValue, maxValue, powerValue);
            ambienceVolumeSlider.onValueChanged.AddListener(SetAmbienceVolume);

            //게임 프레임변경
            powerValue = Application.targetFrameRate;
            SetSliderInProgressor(frameSlider, 30, 144, powerValue);
            frameSlider.onValueChanged.AddListener(SetApplicationFrame);

            //키세팅버튼
            showInputKeySettingButton.Button.onClick.AddListener(()=>
            {
                inputKeySettingUI.ShowUI(true);//해당UI만키기
                ShowUI(false);//자기자신꺼버리기
            });
            inputKeySettingUI.AddOverrideBackAction(() =>
            {
                inputKeySettingUI.ShowUI(false);
                ShowUI(true);
            });

        }

        /// <summary>
        /// 슬라이드안에 프로그래서를 같이 세팅해주는 함수//20221103
        /// </summary>
        /// <param name="slider"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="curValue"></param>
        private static void SetSliderInProgressor(Slider slider, int min, int max, float curValue)
        {
            //사용법//SetSliderInProgressor(TestSlider, 0, 100 , 0);
            //확인해보니 프로그래스바에서는 말그대로 바-만 보여주기 떄문에
            //슬라이드는 못하는거 같음
            //고로 슬라이드에서 프로그래스바를 조종하는게 맞음
            //슬라이드 => 프로그래서 => 프로그래스 타겟
            //위치는 슬라이드게임오브젝트안의 프로그래서를 가져와서 호출

            slider.minValue = min;
            slider.maxValue = max;
            slider.value = curValue;
            

            if (!slider.TryGetComponent(out Progressor progressor))
            {
                return;
            }

            //프로그래서 처리
            slider.onValueChanged.AddListener(progressor.SetValue);
            progressor.SetMin(min);
            progressor.SetMax(max);
            progressor.SetValue(curValue);

            //프로그래서 타겟
            //각타겟은 직접 세팅해주기//왠만해선 슬라이드와 같은선상에 해주는게 좋아보임
            //자동화시키는것도 좋아보인다.//인스팩터에디터제작해야될듯함
            //이미지
            //텍스트=>100% 같은거 원하면 멀티플해주기
        }

        //해상도처리
        private void SetResolution(int resolutionIndex)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }


        //게임전체화면 
        //토글에 사용
        private void SetFullScreenToggle(bool onOff)
        {
            Screen.fullScreen = onOff;
        }

        //게임퀄리티세팅용        
        private void SetQualityGame(int qualityIndex)
        {
            //아직은 사용안할듯함
            QualitySettings.SetQualityLevel(qualityIndex);
        }

        //오디오 껏다켯다
        private void SetAudioToggle(bool onOff)
        {
            AudioListener.volume = onOff ? 1 : 0;
        }

        //볼륨크기를 정하는곳
        //UI의 슬라이더의 이벤트에 집어넣는다.        
        private void SetMasterVolume(float value)
        {
            Debug.Log("마스터볼륨 크기 : " + value);
            audioMixer.SetFloat("Master", value);
        }
        private void SetMusicVolume(float value)
        {
            Debug.Log("음악볼륨 크기 : " + value);
            audioMixer.SetFloat("Music", value);
        }
        private void SetSFXVolume(float value)
        {
            Debug.Log("SFX볼륨 크기 : " + value);
            audioMixer.SetFloat("SFX", value);
        }
        private void SetUIVolume(float value)
        {
            Debug.Log("UI볼륨 크기 : " + value);
            audioMixer.SetFloat("UI", value);
        }
        private void SetAmbienceVolume(float value)
        {
            Debug.Log("환경음 크기 : " + value);
            audioMixer.SetFloat("Ambience", value);
        }
        private void SetApplicationFrame(float value)
        {
            Debug.Log("프레임 변경 : " + value);
            Application.targetFrameRate = (int)value;
        }
    }
}