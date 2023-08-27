using lLCroweTool.Singleton;
using UnityEngine;

namespace lLCroweTool.Effect
{
    public class EffectManager : MonoBehaviourSingleton<EffectManager>
    {
        //이팩트처리를 위한 매니저
        //이팩트는 여러종류가 있지만
        //그중에 사운드와 비쥬얼이팩트를 처리할예정



        //배경음은 하나니까 여기서처리

        //그외 사운드쪽은 다른곳에서처리
        //리스너로부터 멀어진곳에서 호출할시 무시하기
        private Transform audioListenerTr;


        //총시간은 VFX 그래프에서 사용할수 있다. 그걸 따로 프로퍼티로 빼와야되네


        protected override void Awake()
        {
            base.Awake();
            //audioListenerTr = FindAnyObjectByType<AudioListener>().transform;
        }

        /// <summary>
        /// 이팩트오브젝트를 요청하는 함수.오브젝트폴 매니저를 한번래핑해났음
        /// </summary>
        /// <param name="fXBaseObject"></param>
        /// <param name="targetPos"></param>
        /// <param name="isParent"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool RequestFXObject(EffectObject fXBaseObject, Transform targetPos, bool isParent, out EffectObject result)
        {
            //사운드일시
            result = null;
            //if (!lLcroweUtil.CheckDistance(audioListenerTr.position, targetPos.position,100))
            //{
            //    return false;
            //}

            var target = ObjectPoolManager.Instance.RequestDynamicComponentObject(fXBaseObject);
            if (isParent)
            {
                target.InitTrObjPrefab(targetPos, targetPos);
            }
            else
            {
                target.InitTrObjPrefab(targetPos, null);
            }
            result = target;
            return true;
        }
    }
}