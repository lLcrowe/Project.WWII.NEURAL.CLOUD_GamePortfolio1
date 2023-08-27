using UnityEngine;

namespace lLCroweTool.Ability
{
    public class DamageObject : MonoBehaviour
    {
        //대미지 오브젝트
        //유닛내의 정보를 사용하여 처리
        //어차피 이게임은 타겟팅임
        //투사체 처리
                
        public float hitDistance = 0.1f;//일정거리안으로 들어오면 히트로 침
        public float projectileSpeed = 0.5f;
        public int attackRange = 0;
        [SerializeField] private UnitObject_Base targetAttackUnit;
        [SerializeField] private UnitObject_Base targetObject;


        private Transform tr;

        

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
                Quaternion rot = Quaternion.LookRotation(targetObject.Tr.position - tr.position);
                //rot = Quaternion.Lerp(rot, target.Tr.rotation, projectileSpeed * deltaTime * 25);
                tr.SetPositionAndRotation(projectileSpeed * deltaTime * tr.forward + tr.position, rot);
            }

            //거리체크
            if (!lLcroweUtil.CheckDistance(targetObject.Tr.position, tr.position, hitDistance))
            {
                return;
            }

            //공격계산
            //범위처리

            var hexTileData = targetObject.curHexTileObject.GetHexTileData;
            BattleManager instance = BattleManager.Instance;
            var tileMap =  instance.hexBasementTileMap.GetTileMap();

            //타일맵에서 가져와서 존재하면 대상을 처리
            Vector3Int[] rangePosArray = lLcroweUtil.GetNearRangePos(hexTileData.GetTilePos(), attackRange,tileMap);

            for (int i = 0; i < rangePosArray.Length; i++)
            {
                var pos = rangePosArray[i];
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

            //폭발이팩트처리
        }
    }
}
