using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.UI;
using static lLCroweTool.Camera3D.DirectorCamera;

namespace lLCroweTool.Camera3D
{
    public class ModelViewRoom : MonoBehaviour
    {
        //특정오브젝트를 돌려보기 하는 용도의 방?
        //캐릭터를 그대로 두고 카메라를 돌리자
        [Header("배치할 오브젝트")]
        public Transform batchObject;
        public float rotatePower = 1f;

        [Header("배치할 디렉터카메라")]
        public DirectorCamera directorCamera;

        [Space]
        [Header("배치할 설정")]
        public Transform batchPos;//배치오브젝트를 놓을 위치
        public Transform lookTarget;//카메라가 지속적으로 봐라볼 위치
        private Transform directorCameraTr;//디렉터카메라를 배치하는 위치

        //간단하게 하자
        //데이터덮어쓰기를 하자
        public CameraLimitInfo modelViewRoomCameraLimitInfo = new CameraLimitInfo();

        public bool isLive;
        public float yAxisValue;
        private float cashYRotateValuc;

        

        //원래값
        private Vector3 originDirectorCameraPos;
        private Vector3 originDirectorCameraRotate;
        //private Quaternion originDirectorCameraRotate;


        private float originZRailValue;

        private void Awake()
        {
            yAxisValue = batchPos.localEulerAngles.y;
            cashYRotateValuc = yAxisValue;
        }

        /// <summary>
        /// 모델뷰방을 초기화하는 함수
        /// </summary>
        /// <param name="targetBatchObject">배치할 오브젝트</param>
        public void InitModelViewRoom(Transform targetBatchObject)
        {
            batchObject = targetBatchObject;
            if (batchObject == null)
            {
                return;
            }
            batchObject.SetParent(batchPos);
            batchObject.SetPositionAndRotation(batchPos.position, Quaternion.identity);
            isLive = true;
                                    
            //카메라셋
            directorCamera.SetRotateLimit(modelViewRoomCameraLimitInfo);
            directorCameraTr = directorCamera.directorCamera.transform;

            //값저장
            originZRailValue = directorCamera.zRailValue;
            originDirectorCameraPos = directorCamera.moveFollowPos;
            originDirectorCameraRotate = new Vector3(directorCamera.xAxisValue, directorCamera.yAxisValue, directorCamera.zAxisValue);
            //originDirectorCameraRotate = Quaternion.Euler(new Vector3(tempInstance.xAxisValue, tempInstance.yAxisValue, tempInstance.zAxisValue));

            //저장후 변경
            directorCamera.zRailValue = -5f;
            directorCamera.moveFollowPos = batchPos.position;
            directorCamera.SetRotateCameraAxis(new Vector3(30, 30, 0));
        }

        public void ExitModelViewRoom()
        {
            isLive = false;
            directorCameraTr = null;                        
            directorCamera.ResetCameraAnglePose();

            //복구
            directorCamera.zRailValue = originZRailValue;
            directorCamera.moveFollowPos = originDirectorCameraPos;
            directorCamera.SetRotateCameraAxis(originDirectorCameraRotate);
        }

        private void Update()
        {
            if (isLive == false)
            {
                return;
            }


            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                directorCamera.MouseLeftKeyDown();
            }
            else if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                directorCamera.MouseLeftKeyUp();
            }

            float deltaTime = Time.deltaTime;
            

            if (Input.GetKey(KeyCode.Q))
            {
                yAxisValue += rotatePower * deltaTime;
            }
            if (Input.GetKey(KeyCode.E))
            {
                yAxisValue -= rotatePower * deltaTime;
            }


            cashYRotateValuc = Mathf.Lerp(cashYRotateValuc, yAxisValue, deltaTime);
            
            //배치오브젝트 
            Vector3 vecY  = batchPos.localEulerAngles;
            vecY.y = cashYRotateValuc;
            batchPos.localEulerAngles = vecY;
            //batchPos.localRotation = Quaternion.Euler(vecY);

            //카메라 회전
            directorCameraTr.LookAt(lookTarget.position);
        }
    }
}