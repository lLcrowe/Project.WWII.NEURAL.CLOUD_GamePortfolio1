using UnityEngine;
using DG.Tweening;
//using System.Collections.Generic;//MEC 관련
using System.Collections;
using lLCroweTool.Anim.GunRecoil.Muzzle;
using System;

namespace lLCroweTool.Anim.GunRecoil
{
    public class GunRecoilAnim : MonoBehaviour
    {
        //건의 주퇴복좌기를 구현하기 위한 클래스
        //현재 오브젝트를 쏘고난뒤 돌아오는 애니메이션
        //주가되는 오브젝트에 붙착시켜서 사용할것
        //GunRecoilAnimTarget보다 위(부모)로 있어야됨


        //20230612//3D도 작동되게 변경

        [Header("동시발사 설정")]
        [Tooltip("동시발사 여부")]
        public bool isSolvoShot = false;
        [SerializeField][HideInInspector] private int firePosCount = 0;

        [Header("포 발사시 뒤로 가는 설정")]
        [Tooltip("뒤로 후퇴하는거리")]
        public float backDistnace = 0;//뒤로 후퇴하는거리
        [Tooltip("뒤로 후퇴하는 시간")]
        public float backIntervalTime = 0;//뒤로 후퇴하는 시간

        [Header("포 발사 위치설정")]
        [Tooltip("되돌아오는 시간")]
        public float resetIntervalTime = 0.3f;//되돌아오는 시간
        [Tooltip("되돌아오는 시간동안 코루틴이 대기하는 여부")]
        public bool isWaitResetTime = false;//되돌아오는 시간동안 코루틴이 대기하는 여부


        [Tooltip("포 위치는 현스크립트로 설정됩니다.")]
        private Vector3[] originPosArray = new Vector3[0];//원래위치//현재위치를 기준함//원할시 런타임에서 세팅후 복붙하기
        public GunRecoilAnimTarget[] gunRecoilAnimTargetArray = new GunRecoilAnimTarget[0];

        //이벤트처리
        public Action<Transform> gunMuzzleAction;

        private bool checkCoroutine = false;
        public GunAxisType gunAxisType;
        public enum GunAxisType{Y,Z}

        private void Awake()
        {   
            originPosArray = new Vector3[gunRecoilAnimTargetArray.Length];
            for (int i = 0; i < gunRecoilAnimTargetArray.Length; i++)
            {
                originPosArray[i] = gunRecoilAnimTargetArray[i].transform.localPosition;
            }
        }

        /// <summary>
        /// 건리코일애님이 작동될떄 작동할 이벤트를 등록하는 함수
        /// </summary>
        /// <param name="action">이벤트</param>
        public void InitGunMuzzleAction(Action<Transform> action)
        {
            gunMuzzleAction = action;
        }
                
        /// <summary>
        /// 리코일 액션
        /// </summary>
        public void ActionRecoil()
        {
            if (checkCoroutine)
            {
                return;
            }
            StartCoroutine(ActionAnim(this));
        }

        public int GetActionFireCount()
        {
            //-1은 일제사격
            if (isSolvoShot)
            {
                return -1;
            }
            return firePosCount;
        }

        public Transform GetFirePos()
        {
            return gunRecoilAnimTargetArray[firePosCount].GetMuzzleTr();
        }

        /// <summary>
        /// 건리코일애니메이션처리를 위한 코루틴
        /// </summary>
        /// <param name="gunRecoilAnim">건리코일애님모듈</param>
        private static IEnumerator ActionAnim(GunRecoilAnim gunRecoilAnim)
        {
            gunRecoilAnim.checkCoroutine = true;
            float backIntervalTime = gunRecoilAnim.backIntervalTime;
            float backDistnace = gunRecoilAnim.backDistnace;
            float resetIntervalTime = gunRecoilAnim.resetIntervalTime;
            GunAxisType gunAxisType = gunRecoilAnim.gunAxisType;
            var gunMuzzleAction = gunRecoilAnim.gunMuzzleAction;
            int i;

            if (gunRecoilAnim.isSolvoShot)
            {
                for (i = 0; i < gunRecoilAnim.gunRecoilAnimTargetArray.Length; i++)
                {
                    var recoilTarget = gunRecoilAnim.gunRecoilAnimTargetArray[i];
                    Transform tr = recoilTarget.transform;
                    switch (gunAxisType)
                    {
                        case GunAxisType.Y:
                            //tr.DOLocalMove(tr.up * -backDistnace, backIntervalTime);
                            tr.DOMove(tr.position + tr.up * -backDistnace, backIntervalTime);
                            break;
                        case GunAxisType.Z:
                            tr.DOMove(tr.position + tr.forward * -backDistnace, backIntervalTime);
                            //tr.DOLocalMove(tr.forward * -backDistnace, backIntervalTime);
                            break;
                    }

                    //이벤트작동
                    gunMuzzleAction?.Invoke(recoilTarget.GetMuzzleTr());
                }


                yield return new WaitForSeconds(backIntervalTime);
                for (i = 0; i < gunRecoilAnim.gunRecoilAnimTargetArray.Length; i++)
                {
                    Transform tr = gunRecoilAnim.gunRecoilAnimTargetArray[i].transform;
                    tr.DOLocalMove(gunRecoilAnim.originPosArray[i], resetIntervalTime);
                }
            }
            else
            {
                int posCount = gunRecoilAnim.firePosCount;
                var recoilTarget = gunRecoilAnim.gunRecoilAnimTargetArray[posCount];
                Transform tr = recoilTarget.transform;
                Vector3 originPos = gunRecoilAnim.originPosArray[posCount];
                switch (gunAxisType)
                {
                    case GunAxisType.Y:
                        tr.DOMove(tr.position + tr.up * -backDistnace, backIntervalTime);                        
                        break;
                    case GunAxisType.Z:
                        tr.DOMove(tr.position + tr.forward * -backDistnace, backIntervalTime);
                        //tr.DOLocalMove((tr.localRotation * Vector3.forward) * -backDistnace, backIntervalTime);//안됨                        
                        break;
                }

                //이벤트작동
                gunMuzzleAction?.Invoke(recoilTarget.GetMuzzleTr());

                yield return new WaitForSeconds(backIntervalTime);
                tr.DOLocalMove(originPos, resetIntervalTime);

                gunRecoilAnim.firePosCount++;
                if (gunRecoilAnim.firePosCount >= gunRecoilAnim.gunRecoilAnimTargetArray.Length)
                {
                    gunRecoilAnim.firePosCount = 0;
                }
            }

            if (!gunRecoilAnim.isWaitResetTime)
            {
                yield return new WaitForSeconds(resetIntervalTime);
            }

            gunRecoilAnim.checkCoroutine = false;
        }


        private void OnDrawGizmos()
        {
            for (int i = 0; i < gunRecoilAnimTargetArray.Length; i++)
            {
                //부모위치 기준으로 보여줘야지 제대로 보여줌
                //회전은 대상
                var targetTr = gunRecoilAnimTargetArray[i];
                if (targetTr == null)
                {
                    continue;
                }


                Vector3 nor = Vector3.zero;
                switch (gunAxisType)
                {
                    case GunAxisType.Y:
                        nor = targetTr.transform.up;
                        break;
                    case GunAxisType.Z:
                        nor = targetTr.transform.forward;
                        break;
                }

                //원래위치
                Gizmos.color = Color.green;
                Vector3 startPos = targetTr.transform.position;
                Gizmos.DrawWireSphere(startPos, 0.2f);

                //후퇴위치
                Gizmos.color = Color.red;
                Vector3 endPos = targetTr.transform.position + (nor * -backDistnace);
                Gizmos.DrawWireSphere(endPos, 0.2f);
            }
        }
    }
}