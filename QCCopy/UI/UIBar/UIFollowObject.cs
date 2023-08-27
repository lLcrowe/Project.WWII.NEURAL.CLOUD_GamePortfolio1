using lLCroweTool.TimerSystem;
using UnityEngine;

namespace lLCroweTool.UI.Follow
{
    /// <summary>
    /// UI에 사용할 팔로우기능을 담당하는 클래스
    /// </summary>
    public class UIFollowObject : UpdateTimerModule_Base
    {
        //※주의점
        //월드스페이스 사용시 루트 오브젝트의 스케일을 조심할것
        //해당스케일만큼 크기가 정해져버리니 확인하고 처리하기


        [Header("따라갈 유닛")]
        public Transform followObject;//대상이 될 유닛
        [SerializeField] private Transform controlTr;//제어할 대상//자기자신

        [Space]
        [Header("움직임관련")]
        //움직임관련
        public bool isUseMove;
        public Vector3 moveOffSet;//1~5 사이가 편함

        [Space]
        [Header("회전관련")]
        //회전관련
        public bool isUseRotate;

        //랜더모드관련 처리
        private Canvas parentCanvas;

        //메인카메라관련
        private static Camera mainCamera;
        private static Transform mainCameraTr;

        protected override void Awake()
        {
            base.Awake();
            controlTr = transform;
            parentCanvas = GetComponentInParent<Canvas>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!ReferenceEquals(mainCamera, null))
            {
                return;
            }
            mainCamera = Camera.main;//이거 Awake에서 어덯게 된지//되긴하는데 씬넘어갔을때 문제생김//일단 밑에거 고치고 해보기
            mainCameraTr = mainCamera.transform;
        }

        public void OnValidate()
        {
            var parentCanvas = GetComponentInParent<Canvas>();
            controlTr = transform;
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                print($"{name}메인카메라가 존재하지않음. 메인카메라 태그를 확인하기");
                return;
            }
            mainCameraTr = mainCamera.transform;
            if (parentCanvas == null)
            {
                print($"{name}캔버스가 존재하지않음");
                return;
            }
            if (followObject == null)
            {
                print($"따라갈 타겟이 존재하지 않습니다. 인스팩터를 확인해주세요.");
                return;
            }


            var renderMode = parentCanvas.renderMode;
            if (isUseMove)
            {
                MoveUI(renderMode);
            }
            if (isUseRotate)
            {
                RotateUI(renderMode);
            }
        }


        public override void UpdateTimerModuleFunc()
        {
            //음 이상하네 생각한게 맞는데 문제가 생기네
            //집가서 해보기

            //캔버스 랜더모드에 따른 작동방식 변경
            var renderMode = parentCanvas.renderMode;

            if (isUseMove)
            {
                MoveUI(renderMode);
            }
            if (isUseRotate)
            {
                RotateUI(renderMode);
            }
        }

        public void MoveUI(RenderMode renderMode)
        {
            switch (renderMode)
            {
                case RenderMode.ScreenSpaceOverlay:
                    //원래쓰던거
                    //회전할 필요가 없긴함//맨앞에 랜더링
                    //controlTr.position = mainCamera.WorldToScreenPoint(followObject.position + moveOffSet);//특정오브젝트의 z위로 보여줌(고정)
                    controlTr.position = mainCamera.WorldToScreenPoint(followObject.position) + (moveOffSet * 40);//회전해도 특정오브젝트 위로 보여줌
                    break;
                case RenderMode.ScreenSpaceCamera:
                    //원근감을 줄수 있음//맨앞 랜더링이 아님//카메라 화면기준 랜더링
                    //카메라가 없으면 위의 ScreenSpaceOverlay이고 있으면 WorldSpace로 처리
                    if (parentCanvas.worldCamera == null)
                    {
                        controlTr.position = mainCamera.WorldToScreenPoint(followObject.position) + (moveOffSet * 40);
                    }
                    else
                    {
                        //월드스페이스
                        controlTr.position = followObject.position + (mainCameraTr.rotation * moveOffSet);
                    }
                    break;
                case RenderMode.WorldSpace:
                    //원근감을 줄수 있음
                    //월드공간에 존재함
                    //스크린의 pos x, y 의 크기와는 40 차이남
                    //Z축을 -1로 해놔서 UI가 겹치지않게 처리
                    //controlTr.position = followObject.position + moveOffSet;
                    controlTr.position = followObject.position + (mainCameraTr.rotation * moveOffSet);
                    break;
            }
        }


        public void RotateUI(RenderMode renderMode)
        {
            switch (renderMode)
            {
                case RenderMode.ScreenSpaceOverlay:
                    //이상하다//여기는 필요할까?
                    //회전을 줘도 Z만 줘야됨
                    break;
                case RenderMode.ScreenSpaceCamera:

                    //월드스페이스 상태만 회전
                    if (parentCanvas.worldCamera)
                    {
                        //월드스페이스
                        controlTr.rotation = mainCameraTr.rotation;
                    }
                    break;
                case RenderMode.WorldSpace:
                    //돌아가긴하는데
                    //controlTr.LookAt(controlTr.position - mainCameraTr.position);//Z축회전할시에는 안돌아감

                    //기존거
                    //controlTr.LookAt(controlTr.position - mainCameraTr.position);
                    //controlTr.rotation *= Quaternion.AngleAxis(mainCameraTr.eulerAngles.z, Vector3.forward);//z축회전

                    //이동시키면 문제가 발생
                    //controlTr.LookAt(mainCameraTr.position);
                    //controlTr.rotation *= Quaternion.AngleAxis(mainCameraTr.eulerAngles.z, Vector3.forward);//z축회전을 상대

                    //어느각도가면 이상하게 변하는데//안되는거
                    //controlTr.rotation = Quaternion.LookRotation(controlTr.position - mainCameraTr.position) * Quaternion.AngleAxis(mainCameraTr.eulerAngles.z, Vector3.forward);//돌아가게 처리

                    //안되는거
                    //controlTr.rotation = Quaternion.LookRotation(controlTr.position - mainCameraTr.position, Vector3.up);//문제가 있네 이방식이


                    controlTr.rotation = mainCameraTr.rotation;
                    break;
            }
        }
    }
}
