using UnityEngine;
using System;

namespace lLCroweTool.Camera3D
{
    public class DirectorCamera : MonoBehaviour
    {
        [System.Serializable]
        public class CameraLimitInfo
        {
            [Space]
            [Header("카메라제한 설정")]
            [Space]
            [Header("좌우회전관련")]
            //자기자신을 y축을 회전시켜 좌우회전을 시킴
            [Range(-360f, 360f)] public float minYRotateLimit = 0f;
            [Range(-360f, 360f)] public float maxYRotateLimit = 360f;
            public float yRotatePower = 20f;
            public float yRotateSensitivity = 1f;//민감도  

            [Space]
            [Header("위아래회전관련")]        
            //회전관련 제한
            [Range(-90f, 90f)] public float minXRotateLimit = 10f;
            [Range(-90f, 90f)] public float maxXRotateLimit = 80f;
            public float xRotatePower = 10f;
            public float xRotateSensitivity = 1f;//민감도

            [Space]
            [Header("양옆회전관련")]
            //z축이 회전하여 카메라를 기우둥시킴
            //회전관련 제한
            [Range(-360f, 360f)] public float minZRotateLimit = 0;
            [Range(-360f, 360)] public float maxZRotateLimit = 360f;
            public float zRotatePower = 10f;
            public float zRotateSensitivity = 1f;//민감도


            [Space]
            [Header("앞으로 뒤로 카메라이동 관련")]
            [Range(1f, 20f)] public float minZMoveLimit = 1;
            [Range(1f, 20f)] public float maxZMoveLimit = 20;
            public float zMovePower = 2f;
            public float zMoveForCameraSensitivity = 1f;//민감도


            //public CameraLimitInfo Clone()
            //{
            //    return MemberwiseClone() as CameraLimitInfo;//복제본넘김
            //}
        }

        //캐릭터 미리보기 만들기        
        //카메라 앵글처리 관련처리
        [Header("움직이면서 쫒아갈 대상오브젝트 좌표")]
        public Vector3 moveFollowPos;//쫒아갈대상
        public float moveFollowSensitivity = 1f;

        [Header("카메라 & 축 설정")]
        public Camera directorCamera;
        private Transform cameraObject;
        public Vector3 offSet;

        //x축이 회전하여 높낮이를 이동시킴
        //public Transform heightAxis;

        //카메라 제한 앵글정보
        public CameraLimitInfo cameraLimitInfo = new CameraLimitInfo();
        private CameraLimitInfo copyCameraLimitInfo;

        [Space]
        [Header("수동조절가능한 값")]
        public bool isControlCinemachine;//시네머신 컨트롤인지//리미트제한 해제됨
        public bool isCurrentScriptControl;//자체스크립트조절인지
        private bool isMouseRotate = false;//마우스로 회전인지


        public float xAxisValue;//x축회전
        public float yAxisValue;//y축회전
        public float zAxisValue;//z축회전
        public float zRailValue;//z축이동



        
        //내부에서만
        public float cashXValue;
        public float cashYValue;
        public float cashZValue;
        public float cashZRailValue;
        public Vector2 mouseNewPos;
        public Vector2 mousePrevPos;
        private Transform tr;

        private void Awake()
        {   
            tr = transform;
            moveFollowPos = tr.position;

            yAxisValue = tr.rotation.eulerAngles.y;
            cashYValue= yAxisValue;

            Vector3 rotateVec = tr.localRotation.eulerAngles;
            xAxisValue = rotateVec.x;
            cashXValue = xAxisValue;
            zAxisValue = rotateVec.z;
            cashZValue = zAxisValue;

            cameraObject = directorCamera.transform;
            zRailValue = cameraObject.localPosition.z;
            cashZRailValue = zRailValue;

            //하나복제해서 가지고 있음
            //copyCameraLimitInfo = cameraLimitInfo.Clone();

            copyCameraLimitInfo = new CameraLimitInfo();
            copyCameraLimitInfo = lLcroweUtil.GetCopyOf(cameraLimitInfo, copyCameraLimitInfo);

            //Debug.Log(ReferenceEquals(cameraLimitInfo, copyCameraLimitInfo));
        }

        void Update()
        {
            float deltaTime = Time.deltaTime;
            Vector2 dir = Vector3.zero;
            mouseNewPos = MousePointer.Instance.mouseScreenPosition;

            if (isCurrentScriptControl)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    MouseLeftKeyDown();
                }
                else if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    MouseLeftKeyUp();
                }              
            }
            

            //작동이 이상함
            //회전여부
            if (isMouseRotate)
            {
                dir = mouseNewPos - mousePrevPos;//마우스 위치처리
                mousePrevPos = mouseNewPos;
                //Debug.Log($"{dir}");

                //최대치 파워제한후 더해주기
                xAxisValue -= dir.y * cameraLimitInfo.xRotatePower * deltaTime;
                yAxisValue += dir.x * cameraLimitInfo.yRotatePower * deltaTime;
            }

            //확대는 Z축 이동
            float wheelInput = Input.GetAxis("Mouse ScrollWheel") * 1000;
            zRailValue += wheelInput * cameraLimitInfo.zMovePower * deltaTime;

            //20230710
            //나중에 여기 회전을 쿼터니언 러프로 처리하자
            //쿼터니언으로 하면 구지 축하나 더할필요가 없네
            //쿼터니언용 Clamp가 필요함



            //스무스처리
            cashXValue = Mathf.Lerp(cashXValue, xAxisValue, cameraLimitInfo.xRotateSensitivity * deltaTime);
            cashYValue = Mathf.Lerp(cashYValue, yAxisValue, cameraLimitInfo.yRotateSensitivity * deltaTime);
            cashZValue = Mathf.Lerp(cashZValue, zAxisValue, cameraLimitInfo.zRotateSensitivity * deltaTime);
            cashZRailValue = Mathf.Lerp(cashZRailValue, zRailValue, cameraLimitInfo.zMoveForCameraSensitivity * deltaTime);

            //시네머신이 컨트롤 하고 있는건지 체크//값제한 해체
            if (!isControlCinemachine)
            {
                xAxisValue = lLcroweUtil.ClampAngle(xAxisValue, cameraLimitInfo.minXRotateLimit, cameraLimitInfo.maxXRotateLimit);
                yAxisValue = lLcroweUtil.ClampAngle(yAxisValue, cameraLimitInfo.minYRotateLimit, cameraLimitInfo.maxYRotateLimit);
                zAxisValue = lLcroweUtil.ClampAngle(zAxisValue, cameraLimitInfo.minZRotateLimit, cameraLimitInfo.maxZRotateLimit);
                zRailValue = -Mathf.Clamp(-zRailValue, cameraLimitInfo.minZMoveLimit, cameraLimitInfo.maxZMoveLimit);//뒤집히는거 방지
            }

            tr.rotation = Quaternion.Euler(cashXValue, cashYValue, cashZValue);







            ////y축 회전(좌우회전)
            //Vector3 yVec = tr.localEulerAngles;
            //yVec.y = cashYValue;
            //tr.localEulerAngles = yVec;
            ////tr.localRotation = Quaternion.Euler(yVec);

            ////x축 회전 (업다운, 기우둥) 
            //Vector3 xVec = heightAxis.localEulerAngles;            
            //xVec.x = cashXValue;
            //xVec.z = cashZValue;
            //heightAxis.localEulerAngles = xVec;
            ////heightAxis.localRotation = Quaternion.Euler(xVec);



            //Z위치 움직임(완)            
            Vector3 zMoveVec = Vector3.zero;
            zMoveVec.z = cashZRailValue;
            cameraObject.transform.localPosition = offSet + zMoveVec;

            //이동관련
            tr.position = Vector3.Lerp(tr.position, moveFollowPos, moveFollowSensitivity * deltaTime);
        }

        public void MouseLeftKeyDown()
        {
            isMouseRotate = true;
            mousePrevPos = mouseNewPos;
        }
        public void MouseLeftKeyUp()
        {
            isMouseRotate = false;
        }

        /// <summary>
        /// 카메라 신규 회전축을 설정하는 함수
        /// </summary>
        /// <param name="newRotate">새롭게 갱신할 회전방향</param>
        public void SetRotateCameraAxis(Vector3 newRotate)
        {   
            xAxisValue = newRotate.x;//상하
            yAxisValue = newRotate.y;//좌우
            zAxisValue = newRotate.z;//기우뚱
        }

        public void SetRotateCameraAxis(Quaternion quaternion)
        {
            Vector3 newRotate = quaternion.eulerAngles;
            //Debug.Log($"{newRotate} {quaternion}");
            xAxisValue = newRotate.x;//상하
            yAxisValue = newRotate.y;//좌우
            zAxisValue = newRotate.z;//기우뚱
        }


        public void YRotateCamera(bool isRight)
        {
            yAxisValue += isRight ? cameraLimitInfo.yRotatePower * Time.deltaTime : -cameraLimitInfo.yRotatePower * Time.deltaTime;
        }

        public void XRotateCamera(bool isUp)
        {
            xAxisValue += isUp ? cameraLimitInfo.xRotatePower * Time.deltaTime : -cameraLimitInfo.xRotatePower * Time.deltaTime;
        }

        /// <summary>
        /// 카메라제한 정보를 세팅해주는 함수
        /// </summary>
        /// <param name="cameraLimitInfo"></param>
        public void SetRotateLimit(CameraLimitInfo cameraLimitInfo)
        {
            this.cameraLimitInfo = cameraLimitInfo;
        }

        /// <summary>
        /// 카메라 신규 회전축을 강제로 설정하는 함수
        /// </summary>
        /// <param name="newRotate">새롭게 갱신할 회전방향</param>
        public void SetCompulsionCameraAxis(Vector3 newRotate)
        {
            tr.rotation = Quaternion.Euler(newRotate);
        }

        /// <summary>
        /// 카메라오프셋을 설정하는 함수
        /// </summary>
        /// <param name="value">벡터3</param>
        public void SetMoveCameraOffSet(Vector3 value)
        {
            offSet = value;
        }

        /// <summary>
        /// 카메라Z축 이동
        /// </summary>
        /// <param name="addValue"></param>
        public void AddMoveCameraZDepth(float addValue)
        {
            zRailValue += addValue;
        }

        public void ResetCameraAnglePose()
        {
            //카메라의 돌아가야하지 말아야할 구역의 포즈를 리셋해주는 함수
            cameraObject.localEulerAngles = Vector3.zero;
            tr.localEulerAngles = new Vector3(0, tr.localEulerAngles.y, 0);
            cameraLimitInfo = copyCameraLimitInfo;
        }
    }
}