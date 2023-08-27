using System.Collections;
using UnityEngine;

namespace lLCroweTool.Cinemachine
{
    //A => 애니메이션커브 => B

    //쓰는방식을 좀 바꿔야할듯
    //간단한 툴형식 가져와야될듯하다
    //앵커느낌?


    /// <summary>
    /// 오브젝트 배치 정보
    /// </summary>
    public class ObjectBatchInfo : MonoBehaviour
    {
        //CustomCinemachine
        //tr로 변경하면서 기존구조에 문제가 있어서 방식을 변경

        //오브젝트 정보를 기준으로 처리할수 있게 처리
        //에디터도 새로제작
        //기존 트랜스폼의 좌표계는 로컬만 보이니 월드로 보이게 처리하는거 추가해야됨

        //start 부분을 자기자신으로 처리할까//아니다 할필요 없다
        //했다가 오히려 시네머신작업할때 손해만 입음

        //구조
        //CustomCinemachine
        //  ObjectBatchInfo_contentName1
        //      startTr
        //      endTr
        //      startHandleTr
        //      endHandleTr
        //  ObjectBatchInfo_contentName2
        //      startTr
        //      endTr
        //      startHandleTr
        //      endHandleTr


        public CustomCinemachine targetCinemachine;


        //배치컨텐츠 이름
        public string contentName;

        //카메라 배치와 앵글을 조절하기 위한 구역
        public Transform followObject;//로컬인지 월드인지 선택//존재하면 로컬//존재하지않으면월드
        public bool isFixedRotateForFollowObject = false;//팔로우오브젝트에 회전을 고정시킬건지 여부
        //나중에 회전축과 각도를 주어서 고정시킬방향에 대한걸 처리하는것도 좋아보임

        public Transform controlTarget;//움직일 대상//필수

        public Transform lookAtObject;//봐라볼 대상
        public Vector3 lookAtOffset;//봐라볼 오프셋


        //애니메이션커브
        public AnimationCurve curve = new AnimationCurve();

        //지속시간
        public bool isStartInit = false;
        public float startTime;
        public float endTime;

        //이동타입
        public MovePosType movePosType;


        //20230805//작업하기 편하게 tr로 변경시킴


        //핸들용
        public Transform startHandleTr;
        public Transform endHandleTr;

        public Transform startTr;
        public Transform endTr;

        //1차적 정리는 끝났고 

        //벡터들 처리후 트랜스폼하고 연동
        //덮어쓰기하던구역들 정리후 연동후
        //마무리작업남음

        public Transform FollowObject { get => followObject; }
        public Transform ControlTarget { get => controlTarget; }
        public Transform LookAtObject { get => lookAtObject; }

        /// <summary>
        /// 카메라배치 초기화
        /// </summary>
        public void InitControlTargetBatch(bool initToStart)
        {
            if (ReferenceEquals(controlTarget, null))
            {
                Debug.Log("controlTarget is Empty");
            }

            if (initToStart)
            {
                //controlTarget.SetPositionAndRotation(startPos, Quaternion.Euler(startAngle));
                controlTarget.SetPositionAndRotation(startTr.position, startTr.rotation);
            }
            else
            {
                //controlTarget.SetPositionAndRotation(endPos, Quaternion.Euler(endAngle));
                controlTarget.SetPositionAndRotation(endTr.position, endTr.rotation);
            }
        }

        /// <summary>
        /// 제어할 대상을 세팅하는 함수
        /// </summary>
        /// <param name="controlTarget">제어할 오브젝트(null허용안함)</param>
        public void SetControlTarget(Transform controlTarget)
        {
            this.controlTarget = controlTarget;
        }

        /// <summary>
        /// 따라갈 오브젝트를 세팅하는 함수
        /// </summary>
        /// <param name="followObject">따라갈 오브젝트</param>
        public void SetFollowObject(Transform followObject)
        {
            this.followObject = followObject;

            //작동방식이 변해서 팔로우오브젝트할시 해당 위치로 옮겨줘야됨
            if (followObject == null)
            {
                transform.SetParent(targetCinemachine.transform, true);
                transform.localPosition = Vector3.zero;
            }
            else
            {
                transform.SetParent(followObject.transform, true);
                transform.localPosition = Vector3.zero;
            }
        }

        /// <summary>
        /// 봐라볼 오브젝트를 세팅하는 함수
        /// </summary>
        /// <param name="lookAtObject">봐라볼 오브젝트</param>
        public void SetLookAtObject(Transform lookAtObject)
        {
            this.lookAtObject = lookAtObject;
        }

        /// <summary>
        /// 배치정보에 따른 카메라 작동
        /// </summary>
        /// <param name="timer">타이머</param>
        /// <returns>작동중인지 여부</returns>
        public bool ActionBatchInfoCamera(float timer)
        {
            if (timer > endTime)
            {
                //들어온 타이머가 크면 되돌리기  
                return false;
            }

            if (timer < startTime)
            {
                //시작 시간보다 작으면 트루를 리턴하되 작동은 안함
                return true;
            }

            if (!isStartInit)
            {
                //처음 시작하면 카메라배치를 초기화시킴
                InitControlTargetBatch(true);
                isStartInit = true;
            }


            float norValue = lLcroweUtil.MinMaxNormalize(startTime, endTime, timer);
            float curveValue = curve.Evaluate(norValue);

            Vector3 resultPos = Vector3.zero;
            Vector3 startPos = startTr.position;
            Vector3 endPos = endTr.position;
            Quaternion startAngle = startTr.rotation;
            Quaternion endAngle = endTr.rotation;
            Quaternion resultRot = Quaternion.identity;
            switch (movePosType)
            {
                case MovePosType.Linear:
                    //pos = Vector3.Lerp(startPos, endPos, curveValue);//위치를 베지어곡선을 줘야할듯한데
                    resultPos = Vector3.Lerp(startTr.position, endTr.position, curveValue);
                    
                    break;
                case MovePosType.Bezier:                    
                    Vector3 startHandlePos = startHandleTr.position;
                    Vector3 endHandlePos = endHandleTr.position;
                    resultPos.x = lLcroweUtil.FourPointBezier(startPos.x, startHandlePos.x, endHandlePos.x, endPos.x, curveValue);
                    resultPos.y = lLcroweUtil.FourPointBezier(startPos.y, startHandlePos.y, endHandlePos.y, endPos.y, curveValue);
                    resultPos.z = lLcroweUtil.FourPointBezier(startPos.z, startHandlePos.z, endHandlePos.z, endPos.z, curveValue);

                    //회전처리 필요하면 처리하자 일단 넘어감//할거면 눌러처리후 돌리는게 맞아보임
                    

                    break;
            }
            
            //쫒아갈 대상이 있는지
            if (FollowObject != null)
            {
                //회전관련 고정여부
                if (isFixedRotateForFollowObject)
                {
                    transform.rotation = Quaternion.identity;
                }
            }

            //봐라볼대상이 있는지
            if (LookAtObject != null)
            {
                //쫒아갈대상이 있으면 지정
                Vector3 upAxis = Vector3.up;
                if (FollowObject != null)
                {
                    upAxis = FollowObject.up;
                }

               
                Vector3 lookPos = LookAtObject.position;
                Vector3 resultLookPos = lookPos + lookAtOffset;
                Vector3 dir = resultLookPos - (startPos);
                startAngle = Quaternion.LookRotation(dir, upAxis);

                dir = resultLookPos - (endPos);
                endAngle = Quaternion.LookRotation(dir, upAxis);
            }

            //회전은 덮어쓰기 되고 위치는 추가됨
            resultRot = Quaternion.Lerp(startAngle, endAngle, curveValue);
            controlTarget.SetPositionAndRotation(resultPos, resultRot);

            return true;
        }

    }
}