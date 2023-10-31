using lLCroweTool.DataBase;
using lLCroweTool.LogSystem;
using lLCroweTool.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace lLCroweTool.Sound
{
    public class SoundManager : MonoBehaviourSingleton<SoundManager>
    {
        //이팩트처리를 위한 매니저
        //이팩트는 여러종류가 있지만
        //그중에 사운드와 비쥬얼이팩트를 처리할예정
        //사운드와 비쥬얼을 어덯게 처리할건지에 대해 문제가 있음


        //이름변경
        //=>사운드 매니저

        //이팩트와 따로 돌림

        //배경음은 하나니까 여기서처리
        //이름이 이상한디

        //아 이거 사운드오브젝트가 괜찮을지 사운드FX가 괜찮을지 
        //FX는 배경음악에는 안들어가니 오브젝트로 하는게 맞아보이긴함



        public SoundInfo backGroundMusicInfo;
        public AudioSource audioSource;
        //음악 시작 끝 이벤트 들어가는것도?
        //중간에 신규음악들어가게하는것도? 블랜딩





        //그외 사운드쪽은 다른곳에서처리
        //리스너로부터 멀어진곳에서 호출할시 무시하기
        public SoundObject SoundObjectPrefab;//자동생성
        private Transform audioListenerTr;
        private string logKey = "Sound";

        





        protected override void Awake()
        {
            base.Awake();
            if (SoundObjectPrefab == null)
            {
                GameObject go = new GameObject("SoundObject", 
                    typeof(AudioSource), 
                    typeof(SoundObject));
                go.transform.SetParent(transform);
                SoundObjectPrefab = go.GetComponent<SoundObject>();
            }


            SceneManager.sceneLoaded += ChangeSceneAction;
            LogManager.Register(logKey, "SoundLog", false, true);

            audioSource = gameObject.GetAddComponent<AudioSource>();
            ActionBackGroundMusic();
        }

        public void ActionBackGroundMusic()
        {
            if (backGroundMusicInfo == null)
            {
                return;
            }
            SettingAudioSource(audioSource, backGroundMusicInfo.soundData);
            audioSource.Play();
            
        }

        //데이터설정
        public static void SettingAudioSource(AudioSource audioSource, SoundData soundData)
        {
            if (soundData == null)
            {
                return;
            }
            audioSource.clip = soundData.audioClip;
            
        }

        private void ChangeSceneAction(Scene scene, LoadSceneMode loadSceneMode)
        {
            //오디오리스너 찾기
            audioListenerTr = FindAnyObjectByType<AudioListener>().transform;
            LogManager.Log(logKey,$"변환된 씬이름: {scene.name}, 오디오리스너위치 찾은지 여부: {audioListenerTr == null}");
        }

        /// <summary>
        /// 사운드오브젝트 프리팹을 요청하는 함수
        /// </summary>
        /// <returns>사운드오브젝트</returns>
        public SoundObject RequestSoundObject()
        {
            var soundObject = ObjectPoolManager.Instance.RequestDynamicComponentObject(SoundObjectPrefab);
            return soundObject;
        }
    }
}