using lLCroweTool.DataBase;
using lLCroweTool.Sound;
using lLCroweTool.TimerSystem;
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
        public TimerModule_Element timer;

        //비쥬얼관련
        //이미션
        public VisualEffect[] visualEffectArray = new VisualEffect[0];
        public ParticleSystem[] particleSystemArray = new ParticleSystem[0];

        //사운드관련
        public SoundInfo[] soundInfoArray = new SoundInfo[0];

        public void Init()
        {
            //비활성화 시간정하기
            float disableTime = 10f;

            int i;
            //VFX//임의로 처리해야됨//이벤트로 집어넣어서 처리하라는데 흠 그러면 다 분리시켜야되는거 아닌가
            //for (i = 0; i < visualEffectArray.Length; i++)
            //{
            //    var temp = visualEffectArray[i];
            //    temp..
            //    float totalTime = temp.GetFloat("");

            //    if (totalTime < disableTime)
            //    {
            //        continue;
            //    }
            //    disableTime = totalTime;
            //}

            //파티클시스템
            for (i = 0; i < particleSystemArray.Length; i++)
            {
                var temp = particleSystemArray[i];
                float totalTime = temp.main.duration;
                if (totalTime < disableTime)
                {
                    continue;
                }
                disableTime = totalTime;
            }

            //시간초기화
            timer.SetTimer(disableTime);
            timer.ResetTime();
        }

        /// <summary>
        /// 붙착형 이팩트 작동함수
        /// </summary>
        public void JointPlay(Transform soundTr)
        {
            VisualPlay();
            PlaySound(soundTr);
        }

        /// <summary>
        /// 이팩트 작동함수
        /// </summary>
        /// <param name="effectTr">이팩트위치 지정용</param>
        /// <param name="curPos">현재위치</param>
        /// <param name="directionPos">방향</param>
        /// <param name="soundTr">사운드위치</param>
        public void Play(Transform effectTr, Vector3 curPos, Vector3 directionPos)
        {
            //회전과 이동처리
            //스크립트에서 이팩트처리보기
            var dir = Quaternion.LookRotation(directionPos - curPos);
            effectTr.InitTrObjPrefab(curPos, dir);

            VisualPlay();
            PlaySound(effectTr);
        }

        private void VisualPlay()
        {
            int i;
            //VFX
            for (i = 0; i < visualEffectArray.Length; i++)
            {
                var temp = visualEffectArray[i];
                temp.Play();
            }

            //파티클시스템
            for (i = 0; i < particleSystemArray.Length; i++)
            {
                var temp = particleSystemArray[i];
                temp.Play();
            }
        }

        private void PlaySound(Transform soundTr)
        {
            //사운드
            var instance = SoundManager.Instance;
            for (int i = 0; i < soundInfoArray.Length; i++)
            {
                var soundObject = instance.RequestSoundObject();
                var soundData = soundInfoArray[i].soundData;
                soundObject.InitTrObjPrefab(soundTr);
                soundObject.Action(soundData);
            }
        }






        public void Stop()
        {
            int i;
            //VFX
            for (i = 0; i < visualEffectArray.Length; i++)
            {
                var temp = visualEffectArray[i];
                temp.Stop();
            }

            //파티클시스템
            for (i = 0; i < particleSystemArray.Length; i++)
            {
                var temp = particleSystemArray[i];
                temp.Stop();
            }
        }
    }

    public class EffectObject : MonoBehaviour
    {
        //이팩트오브젝트의 베이스
        //이팩트는 두종류로 작동됨

        //비쥬얼요소와 사운드요소
        //비쥬얼은 파티클시스템과 VFX 그래프
        //사운드는 오디오클립을 사용하면 되니 통합하자
        public EffectGroup mainEffectGroup = new EffectGroup();

        private void Awake()
        {
            mainEffectGroup.Init();
        }

        /// <summary>
        /// 이팩트오브젝트 작동
        /// </summary>
        /// <param name="effectTr"></param>
        /// <param name="curPos"></param>
        /// <param name="directionPos"></param>
        /// <param name="soundTr"></param>        
        public void Action(Vector3 curPos, Vector3 directionPos)
        {
            //이팩트그룹 작동
            mainEffectGroup.Play(transform, curPos, directionPos);
            StartCoroutine(lLcroweUtil.ActionAndDisable(this, mainEffectGroup.timer));
        }
    }
}