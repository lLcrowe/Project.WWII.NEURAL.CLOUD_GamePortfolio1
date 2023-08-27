using lLCroweTool.TimerSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

namespace lLCroweTool.Effect
{
    /// <summary>
    /// 이팩트그룹
    /// </summary>
    [System.Serializable]
    public class EffectGroup
    {
        public VisualEffect[] visualEffectArray = new VisualEffect[0];
        public ParticleSystem[] particleSystemArray = new ParticleSystem[0];

        public AudioClip[] audioClipArray = new AudioClip[0];

        
        public void Play(AudioSource audioSource)
        {
            int i;
            //VFX
            for (i = 0; i < visualEffectArray.Length; i++)
            {
                var temp = visualEffectArray[i];
                temp.SetActive(true);                
                temp.Play();
            }

            //파티클시스템
            for (i = 0; i < particleSystemArray.Length; i++)
            {
                var temp = particleSystemArray[i];
                temp.SetActive(true);
                temp.Play();
            }

            //사운드
            for (i = 0; i < audioClipArray.Length; i++)
            {
                audioSource.PlayOneShot(audioClipArray[i]);
            }
        }

        public void Stop()
        {
            int i;
            //VFX
            for (i = 0; i < visualEffectArray.Length; i++)
            {
                var temp = visualEffectArray[i];
                temp.SetActive(false);
                temp.Stop();
                

            }

            //파티클시스템
            for (i = 0; i < particleSystemArray.Length; i++)
            {
                var temp = particleSystemArray[i];
                temp.SetActive(false);
                temp.Stop();
            }
        }

        //오브젝트온오프
    }

    public class EffectObject : MonoBehaviour
    {
        //이팩트오브젝트의 베이스
        //이팩트는 두종류로 작동됨

        //비쥬얼요소와 사운드요소
        //비쥬얼은 파티클시스템과 VFX 그래프
        //사운드는 오디오클립을 사용하면 되니 통합하자

        public EffectGroup mainEffectGroup = new EffectGroup();
        public AudioSource audioSource;
        public TimerModule_Element timer;
        private bool actionState;

        private void Awake()
        {
            if (mainEffectGroup.audioClipArray.Length > 0)
            {
                audioSource = gameObject.GetAddComponent<AudioSource>();
            }
        }
        public void Action(Vector3 curPos, Vector3 hitPos, float disableTime)
        {
            //뭔가 매니저에서 해야되면 안되는걸 하는걸


            //회전과 이동처리//스크립트에서 이팩트처리보기 
            transform.position = curPos;
            //transform.rotation = ;

            if (actionState)
            {
                return;
            }
            timer.SetTimer(disableTime);
            timer.ResetTime();
            StartCoroutine(Action());
        }
        public IEnumerator Action()
        {
            actionState = true;
            do
            {
                yield return null;
                if (timer.CheckTimer())
                {
                    break;
                }
            } while (true);
            actionState = false;
            EndAction();
        }


        public void EndAction()
        {
            this.SetActive(false);
        }
    }
}