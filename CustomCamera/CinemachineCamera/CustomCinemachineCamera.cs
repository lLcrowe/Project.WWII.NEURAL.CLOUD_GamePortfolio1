using System.Collections;
using UnityEngine;

namespace lLCroweTool.Cinemachine
{
    [RequireComponent(typeof(Camera))]
    public class CustomCinemachineCamera : MonoBehaviour
    {
        //시네머신에서 카메라를 제어할때 편의성을 줄려고 만든 컴포넌트
        private Camera targetCamera;

        //캐싱용도//로컬기준
        public Transform originParent;
        public Vector3 originPos;
        public Quaternion originQuaternion;

        private void Awake()
        {
            targetCamera = GetComponent<Camera>();
            CashCurTransform();
        }

        /// <summary>
        /// 현재 트랜스폼값을 캐싱시키는 함수
        /// </summary>
        private void CashCurTransform()
        {
            Transform tr = targetCamera.transform;
            originParent = tr.parent;
            originPos = tr.localPosition;
            originQuaternion = tr.localRotation;
        }

        /// <summary>
        /// 위치를 지정하고 기존값을 캐싱해주는 함수
        /// </summary>
        /// <param name="parent"></param>
        public void SetPositionAndCash(Transform parent)
        {
            //캐싱
            CashCurTransform();

            Transform tr = targetCamera.transform;
            tr.parent = parent;
            tr.SetPositionAndRotation(parent.position, parent.rotation);
        }

        /// <summary>
        /// 카메라위치를 원래상태로 복귀
        /// </summary>
        public void RecorveryCameraTransform()
        {
            Transform tr = targetCamera.transform;

            tr.parent = originParent;
            tr.SetLocalPositionAndRotation(originPos,originQuaternion);
        }
    }
}