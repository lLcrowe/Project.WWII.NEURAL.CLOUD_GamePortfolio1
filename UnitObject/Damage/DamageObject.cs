using lLCroweTool.Effect;
using UnityEngine;

namespace lLCroweTool.Ability
{
    public class DamageObject : MonoBehaviour
    {
        //대미지 오브젝트
        //유닛내의 정보를 사용하여 처리
        //어차피 이게임은 타겟팅임
        //투사체 처리
                
        public float hitDistance = 0.5f;//일정거리안으로 들어오면 히트로 침
        public float projectileSpeed = 0.5f;
        public int attackRange = 0;

        public Vector3 targetPos;
        public Quaternion rot;
        
        [SerializeField] private UnitObject_Base targetAttackUnit;
        [SerializeField] private UnitObject_Base targetObject;

        public EffectObject effectObjectPrefab;


        private Transform tr;

        //20231024
        //공격이팩트와, 공격히트이팩트 둘다 가지고 있는게 올바르어 보임
        //다음 제작할때는 설계에 집어넣기

        

        private void Awake()
        {
            tr = transform;
            
        }


        /// <summary>
        /// 대미지모듈 초기화
        /// </summary>
        /// <param name="attackUnit">공격한 유닛</param>
        /// <param name="targetUnit">공격하는 대상</param>
        public void InitDamageObject(BattleUnitObject attackUnit, BattleUnitObject targetUnit)
        {
            targetAttackUnit = attackUnit;
            targetObject = targetUnit;
            targetPos = targetObject.Tr.position;
            rot = Quaternion.LookRotation(targetObject.Tr.position - tr.position);

            attackUnit.unitAbilityModule.GetUnitStatusValue(UnitStatusType.ProjectileSpeed, out projectileSpeed);
            attackUnit.unitAbilityModule.GetUnitStatusValue(UnitStatusType.AttackRange, out var value);
            attackRange = lLcroweUtil.RoundToInt(value);
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            //비었으면 꺼버리기
            if (targetObject == null)
            {
                this.SetActive(false);
                return;
            }

            //이동회전처리
            if (projectileSpeed > 0.001f)
            {
                tr.SetPositionAndRotation(projectileSpeed * deltaTime * tr.forward + tr.position, rot);
            }
            

            //거리체크
            if (!lLcroweUtil.CheckDistance(targetPos, tr.position, hitDistance))
            {
                return;
            }

            //공격계산
            //범위처리
            var hexTileData = targetObject.curHexTileObject.GetHexTileData;
            BattleManager instance = BattleManager.Instance;
            var tileMap =  instance.hexBasementTileMap.GetTileMap();

            //타일맵에서 가져와서 존재하면 대상을 처리
            var rangePosList = lLcroweUtil.GetNearRangePos(hexTileData.GetTilePos(), attackRange,tileMap);

            for (int i = 0; i < rangePosList.Count; i++)
            {
                var pos = rangePosList[i];
                if (!tileMap.TryGetHexTile(pos, out var hexTileObject))
                {
                    continue;
                }

                var unit = hexTileObject.BatchUnitObject;
                if (unit == null)
                {
                    continue;
                }

                //같은유닛인지 체크
                if (unit.GetTeamType() == targetAttackUnit.GetTeamType())
                {
                    continue;
                }

                unit.Hit(targetAttackUnit);
            }
            this.SetActive(false);

            //공격히트 이팩트처리
            //가운데처리
            Vector3 trPos = tr.position;
            var effectObject = ObjectPoolManager.Instance.RequestDynamicComponentObject(effectObjectPrefab);
            effectObject.Action(trPos, targetAttackUnit.Tr.position);
        }
    }
}
