using DG.Tweening;
using lLCroweTool;
using System;
using System.Collections;
using UnityEngine;

namespace lLCroweTool.Effect.VFX
{
    public class ResourceSpreadVFX : MonoBehaviour
    {
        //UI자원리소스 비쥬얼이팩트용.


        //이름//
        //아이템먹었을때 보상받았을따 어느지점으로 쇼로로롱 가는거 만들기
        //일반적으로는 파티클로 만들어졋다함
        //촤아악 펼쳐지고 쇼로로롱 따로 감

        public SpreadVFXObject spreadVFXObjectPrefab;
        
        public float spawnTime = 0.05f;
        public int spawnAmount = 15;//스폰수        

        public float spreadTime = 0.5f;
        public float spreadForce = 100f;

        public float waitTime = 0.2f;

        public Transform targetMoveObject;//최종적으로 가야될 위치
        public float moveTargetTime = 0.5f;//시간

        public Ease ease;


        [ButtonMethod]
        public void Action()
        {
            StartCoroutine(ActionCoroutine());
        }

        private IEnumerator ActionCoroutine()
        {   
            Vector2 originPos = transform.position;
            var spawnTimeWait = new WaitForSeconds(spawnTime);
            for (int i = 0; i < spawnAmount; i++)
            {
                SpreadVFXObject target = ObjectPoolManager.Instance.RequestDynamicComponentObject(spreadVFXObjectPrefab);
                target.transform.InitTrObjPrefab(Vector2.zero, Quaternion.identity, transform, false);

                Vector2 randomPos = UnityEngine.Random.insideUnitCircle * spreadForce + originPos;
                ValueTuple<Vector2, Vector2, float, float, float, Ease> setting = new(randomPos, targetMoveObject.position, spreadTime, waitTime, moveTargetTime, ease);
                target.InitSpreadVFXObject(setting);
                yield return spawnTimeWait;
            }
        }
    }
}