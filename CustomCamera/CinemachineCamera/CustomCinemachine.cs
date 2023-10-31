using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;

namespace lLCroweTool.Cinemachine
{
    public class CustomCinemachine : MonoBehaviour
    {
        //이벤트가 발생시 카메라를 특정 방향으로 이동시켜주는 용도
        //뭔가 좀더 해놓으면 키프레임 박아버리듯이 작업해서 툴처럼 할거 같긴한데 넘어가자
        
        [Header("시네머신 고유의 아이디")]
        public string cinemachineID;//외부에서 찾아서 사용할용도

        [Space]
        [Header("시네머신 상태설정")]
        public bool isUseEntryLerpMode = false;//현 시네머신에 진입하여 작동시킬떄 러프로 진입후에 시네머신이 작동될 여부인지
        public float lerpDurationTime = 1f;        
        public AnimationCurve lerpCurve = AnimationCurve.Linear(0, 0, 1, 1);
        private float lerpTime = 0;
        private bool isActionEntryLerpCoroutine = false;

        public bool isLoof = false;//루프여부
        [SerializeField] private bool isRun = false;//플레이 여부
        //public TimerModule_Element timerModule;

        //총시간과 현재시간
        public float timer;//0 ~ totalTimer
        public float totalTimer;

        //시작시 이벤트
        public UnityEvent startEvent = new UnityEvent();
        //끝났을시 이벤트
        public UnityEvent endEvent = new UnityEvent();
        
        public List<ObjectBatchInfo> cameraBatchList = new List<ObjectBatchInfo> ();
        //private int batchCount = 0;


        private void Awake()
        {
            totalTimer = GetTotalTimer();
        }


        private void Update()
        {
            if (isActionEntryLerpCoroutine)
            {
                return;
            }

            if (!isRun)
            {
                return;
            }

            //if (!timerModule.CheckTimer())
            //{
            //    return;
            //}

            float deltaTime = Time.deltaTime;
            timer += deltaTime;

            //모든배치를 돌려버림
            for (int i = 0; i < cameraBatchList.Count; i++)
            {
                ObjectBatchInfo temp = cameraBatchList[i];
                temp.ActionBatchInfoCamera(timer);
            }

            //20230328//기능변경//순차적이 아닌 전체적으로 작동될수 있게 처리
            //ObjectBatchInfo cameraBatchInfo = cameraBatchList[batchCount];
            //if (cameraBatchInfo.ActionBatchInfoCamera(timer) == false)
            //{
            //    //값올리기
            //    batchCount++;
                
            //    //끝났는지 체크
            //    if (batchCount == cameraBatchList.Count)
            //    {
            //        isRun = false;
            //        endEvent.Invoke();
            //        //Debug.Log("End");
            //    }
            //    else
            //    {
            //        //Debug.Log("Next");
            //        cameraBatchList[batchCount].InitCameraBatch();
            //    }
            //    return;
            //}

            //끝나버림
            if (timer > totalTimer)//완전히 진행되게 넉넉히 줌
            {
                //모든배치를 총시간으로 한번더 재배치
                //for (int i = 0; i < cameraBatchList.Count; i++)
                //{
                //    ObjectBatchInfo temp = cameraBatchList[i];
                //    temp.ActionBatchInfoCamera(timer);
                //}



                isRun = false;
                endEvent.Invoke();
                //Debug.Log($"CineEnd {cinemachineID}");

                //루프일시
                if (isLoof)
                {
                    ActionCamera();
                }
            }
        }

        /// <summary>
        /// 이벤트세팅 함수
        /// </summary>
        /// <param name="startAction">시작이벤트</param>
        /// <param name="endAction">끝이벤트</param>
        public void SetActionEvent(UnityAction startAction, UnityAction endAction)
        {
            startEvent.RemoveAllListeners();
            endEvent.RemoveAllListeners();
            startEvent.AddListener(startAction);
            endEvent.AddListener(endAction);
        }

        /// <summary>
        /// 시네머신카메라 작동로직
        /// </summary>
        private void ActionCameraLogic()
        {
            //다 지났으면 처음부터 시작
            //if (cameraBatchList.Count == batchCount)
            if (timer >= totalTimer)
            {
                //Debug.Log($"시간초기화 {cinemachineID}");
                timer = 0;
                //batchCount = 0;
                //모든배치를 초기화
                for (int i = 0; i < cameraBatchList.Count; i++)
                {
                    ObjectBatchInfo temp = cameraBatchList[i];
                    temp.InitControlTargetBatch(true);
                }
            }

            //일시정지와 플레이
            isRun = !isRun;
            if (isRun)
            {
                //총시간 구하기
                totalTimer = GetTotalTimer();

                //처음 시작시 타이머체크 
                if (timer < 0.01f)
                {
                    //Debug.Log($"CineStart {cinemachineID}");
                    startEvent.Invoke();
                }
            }
        }

        /// <summary>
        /// 엔트리러프모드가 켜질시 러프로 이동하는 구역
        /// </summary>        
        private IEnumerator EntryLerp()
        {
            //컨트롤대상을 진입점까지 이동
            isActionEntryLerpCoroutine = true;
            
            //0번기준점
            ObjectBatchInfo objectBatchInfo = cameraBatchList[0];

          
            Vector3 entryPos = objectBatchInfo.startTr.position;
            Quaternion entryRot = objectBatchInfo.startTr.rotation;//이렇게 바꾸면 원하는 되로 작동 잘됨//앞으로 그렇게 할것

            ///룩엣 오브젝트 체크후//각도변경            
            if (objectBatchInfo.LookAtObject != null)
            {
                Vector3 upAxis = Vector3.up;
                //if (objectBatchInfo.FollowObject != null)
                //{
                //    upAxis = objectBatchInfo.FollowObject.up;
                //}

                Vector3 lookPos = objectBatchInfo.LookAtObject.position;
                Vector3 resultLookPos = lookPos + objectBatchInfo.lookAtOffset;
                Vector3 dir = resultLookPos - entryPos;
                //entryRot = Quaternion.LookRotation(dir, upAxis).eulerAngles;
                entryRot = Quaternion.LookRotation(dir, upAxis);
            }

            Transform targetTr = objectBatchInfo.ControlTarget;
            lerpTime = 0;

            //Quaternion targetRot = Quaternion.Euler(entryRot);

            WaitForEndOfFrame wait = new WaitForEndOfFrame();
            do
            {
                yield return wait;
                float norValue = lLcroweUtil.MinMaxNormalize(0, lerpDurationTime, lerpTime);
                float curveValue = lerpCurve.Evaluate(norValue);

                //왜 안될까 흠//10같은긴거에서 문제


                targetTr.rotation = Quaternion.Lerp(targetTr.rotation, entryRot, curveValue);//쿼터니온
                //targetTr.rotation = Quaternion.Euler(Vector3.Lerp(targetTr.rotation.eulerAngles, entryRot, curveValue));//원래거//작동잘됨

                targetTr.position = Vector3.Lerp(targetTr.position, entryPos, curveValue);

                //print($"{curveValue}, {lerpTime}");
                lerpTime += Time.deltaTime;
                
            } while (lerpTime < lerpDurationTime);

            isActionEntryLerpCoroutine = false;
            ActionCameraLogic();
        }

        /// <summary>
        /// 시네머신 카메라작동
        /// </summary>
        public void ActionCamera()
        {
            this.SetActive(true);


            //배치오브젝트가 0일시 넘어감
            if (cameraBatchList.Count == 0)
            {
                return;
            }
            
            if (isUseEntryLerpMode)
            {
                if (isActionEntryLerpCoroutine)
                {
                    return;
                }
                //처음 시작시//진입점까지 러프하게 간다
                StartCoroutine(EntryLerp());
                return;
            }
            //아닐시 그냥 작동
            ActionCameraLogic();
        }

        /// <summary>
        /// 시네머신 카메라작동
        /// </summary>
        /// <param name="batchIndex">배치 인덱스</param>
        public void ActionCamera(int batchIndex)
        {   
            //현재 배치카운트보다 크면 작동
            if (cameraBatchList.Count < batchIndex)
            {
                return;
            }

            isRun = true;
            timer = cameraBatchList[batchIndex].startTime;
            cameraBatchList[batchIndex].InitControlTargetBatch(true);
            //batchCount = batchIndex;
        }

        /// <summary>
        /// 시네머신 정지
        /// </summary>
        public void Stop()
        {
            if (cameraBatchList.Count == 0)
            {
                return;
            }

            isRun = false;
            timer = 0;
            //batchCount = 0;
            for (int i = 0; i < cameraBatchList.Count; i++)
            {
                cameraBatchList[i].InitControlTargetBatch(true);//배치초기화
            }

            //총시간 구하기
            totalTimer = GetTotalTimer();
        }

        /// <summary>
        /// 총플레이 시간을 가져오는 함수
        /// </summary>
        /// <returns></returns>
        private float GetTotalTimer()
        {
            float tempValue = 0;

            if (cameraBatchList.Count == 0)
            {
                return tempValue;
            }

            ObjectBatchInfo cameraBatchInfo = cameraBatchList[cameraBatchList.Count - 1];
            tempValue = cameraBatchInfo.endTime;
            return tempValue;
        }

        /// <summary>
        /// 현재 작동되고 있는 시네머신을 스킵하는 함수
        /// </summary>
        public void Skip()
        {   
            lerpTime = lerpDurationTime;
            timer = totalTimer;
            
            //모든배치를 돌려버림
            for (int i = 0; i < cameraBatchList.Count; i++)
            {
                ObjectBatchInfo temp = cameraBatchList[i];
                temp.ActionBatchInfoCamera(timer);
            }
        }

        public bool IsRun()
        {
            return isActionEntryLerpCoroutine | isRun;
        }
    }

    /// <summary>
    /// 위치를 움직이는 타입
    /// </summary>
    public enum MovePosType
    {
        Linear,//직선
        Bezier,//곡선
    }
}