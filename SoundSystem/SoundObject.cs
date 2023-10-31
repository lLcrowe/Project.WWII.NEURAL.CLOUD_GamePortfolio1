using lLCroweTool.DataBase;
using lLCroweTool.TimerSystem;
using UnityEngine;

namespace lLCroweTool.Sound
{
    //사운드오브젝트
    public class SoundObject : MonoBehaviour
    {
        public AudioSource audioSource;
        public TimerModule_Element disableTimer;

        //쫒아가기용
        public bool isFollowObject;
        public Transform targetFollowObject;
        private Transform tr;

        private void Awake()
        {
            audioSource = gameObject.GetAddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            tr = transform;
        }

        public void Action(AudioClip audioClip, Transform followObject = null)
        {
            targetFollowObject = followObject;
            isFollowObject = followObject != null;
            audioSource.clip = audioClip;
            audioSource.Play();
            ActionDiable(audioClip);
        }

        public void Action(SoundData soundData, Transform followObject = null)
        {
            targetFollowObject = followObject;
            isFollowObject = followObject != null;
            if (soundData.isPlayOneShot)
            {
                audioSource.PlayOneShot(soundData.audioClip);
            }
            else
            {                
                SoundManager.SettingAudioSource(audioSource, soundData);
                audioSource.Play();
            }
            
            ActionDiable(soundData.audioClip);
        }

        private void LateUpdate()
        {
            if (!isFollowObject)
            {
                return;
            }

            tr.position = targetFollowObject.position;
        }

        private void OnDisable()
        {
            targetFollowObject = null;
        }

        //비활성화처리함수
        private void ActionDiable(AudioClip audioClip)
        {   
            //사운드끝나면 꺼지게 만들기
            float totalTimer = audioClip.length;
            disableTimer.SetTimer(totalTimer);
            disableTimer.ResetTime();
            StartCoroutine(lLcroweUtil.ActionAndDisable(this, disableTimer));
        }

        //[ButtonMethod]
        //public void Test()
        //{

        //    Debug.Log($"1:{audioSource.time}2:{audioSource.clip.length}3:{audioSource.timeSamples}");
        //}
    }
}