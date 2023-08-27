using UnityEngine;
using lLCroweTool.TileMap.HexTileMap;
using lLCroweTool.Ability;
using lLCroweTool.ScoreSystem;
using lLCroweTool.GamePlayRuleSystem;
using lLCroweTool.DataBase;

namespace lLCroweTool
{
    public abstract class UnitObject_Base : MonoBehaviour, IGameRuleManagingTarget
    {
        [Header("유닛 데이터 관련")]        
        public UnitObjectInfo unitObjectInfo;//적용할 유닛정보데이터
        public UnitAbilityModule unitAbilityModule;
        public HexTileObject curHexTileObject;//현재 있는 헥스타일오브젝트

        //컴포넌트
        protected Transform tr;

        public Transform Tr { get => tr; }

        //UI관련//월드스페이스처리
        public UnitObjectUI unitUI;

        protected virtual void Awake()
        {
            tr = transform;

            if (TryGetComponent(out unitAbilityModule))
            {
                //기본스탯처리
                unitAbilityModule.InitUnitAbilityModule(unitObjectInfo.statusData);

                //변경되었을시 이벤트처리
                unitAbilityModule.AddStatusChangeEvent(UnitStatusType.HealthPoint, UpdateUI);
                unitAbilityModule.AddStatusChangeEvent(UnitStatusType.HealthMaxPoint, UpdateUI);
            }

            //유닛UI처리
            unitUI?.InitBattleUnitUI(this);
            unitUI?.SetActive(false);            
        }

        //protected virtual void Start()
        //{
        //    unitUI.InitBattleUnitUI(this);
        //}

        protected abstract void Update();

        public Vector3Int GetBatchTilePos()
        {
            return curHexTileObject.GetHexTileData.GetTilePos();
        }

        public HexTileObject GetBatchTile()
        {
            return curHexTileObject;
        }

        private void OnMouseDown()
        {
            //툴팁표현
        }

        private void OnMouseEnter()
        {
            //툴팁표현
        }
        //여기아래에 있는 종류들 다 Protect로 만드는게 좋아보임

        /// <summary>
        /// 유닛의 아이돌상태
        /// </summary>
        public virtual void Idle(){}     

        /// <summary>
        /// 유닛히트
        /// </summary>
        public virtual void Hit(UnitObject_Base attackUnit)
        {
            HitCal(attackUnit, this);
        }

        protected void UpdateUI(float temp)
        {
            if (unitUI == null)
            {
                return;
            }
            unitUI.UpdateUnitUIBar(true);
        }


        /// <summary>
        /// 유닛이 히트했을시 계산되는 기능
        /// </summary>
        /// <param name="attackUnit">공격 유닛</param>
        /// <param name="hitUnit">맞는 유닛</param>
        public static void HitCal(UnitObject_Base attackUnit, UnitObject_Base hitUnit)
        {
            UnitAbilityModule attackUnitAbility = attackUnit.unitAbilityModule;
            UnitAbilityModule hitUnitAbility = hitUnit.unitAbilityModule;

            //20230612~20230613
            //확인할 사항 순서체크
            //1. 회피처리
            //2. 대미지와 스킬대미지의 크리티컬 찬스계산
            //3. 아머와 아머관통의 계산후
            //4. 대미지와 스킬대미지 빼기
            //5. 피해량증폭과 피해차감을 뺀후
            //6. 대미지와 스킬대미지에 최종피해량증폭을 계산 후 
            //7. 맞는 유닛에 스탯을 계산
            //8. 점수판 기록

            //체력체크
            if (!hitUnitAbility.GetUnitStatusValue(UnitStatusType.HealthPoint, out var healthPoint))
            {
                return;
            }

            //회피체크
            if (hitUnitAbility.GetUnitStatusValue(UnitStatusType.DodgeChanceRatio,out var valueRatio))
            {
                if (lLcroweUtil.ProbabilityCal(lLcroweUtil.RoundToInt(valueRatio)))
                {
                    //닷..!지!
                    
                    //해도 히트수는 올라가지?
                    if (hitUnit.TryGetComponent(out ScoreTarget unitScore))
                    {
                        //맞는자 점수판
                        unitScore.AddScore(ScoreType.HitCount, 1);
                    }
                    return;
                }
            }
            

            //대미지
            attackUnitAbility.GetUnitStatusValue(UnitStatusType.Damage,out float damage);
            attackUnitAbility.GetUnitStatusValue(UnitStatusType.SkillDamage, out float skillDamage);

            //크리티컬찬스와 스케일링
            if (attackUnitAbility.GetUnitStatusValue(UnitStatusType.CriticalChance,out var chance))
            {
                if (lLcroweUtil.ProbabilityCal(lLcroweUtil.RoundToInt(chance)))
                {
                    attackUnitAbility.GetUnitStatusValue(UnitStatusType.CriticalScaling, out var scale);
                    damage *= scale;
                    skillDamage *= scale;
                }
            }

            //아머와 아머관통처리
            hitUnitAbility.GetUnitStatusValue(UnitStatusType.DamageArmor, out var armor);
            hitUnitAbility.GetUnitStatusValue(UnitStatusType.SkillArmor, out var skillArmor);
            attackUnitAbility.GetUnitStatusValue(UnitStatusType.DamageArmorPenetration, out var armorPenetration);
            attackUnitAbility.GetUnitStatusValue(UnitStatusType.SkillArmorPenetration, out var skillArmorPenetration);
            
            float resultArmor = armor - armorPenetration;
            float resultSkillArmor = skillArmor - skillArmorPenetration;

            //대미지와 스킬대미지빼기
            damage -= resultArmor;
            skillDamage -= resultSkillArmor;

            //0미만으로 내리지않게 제한
            if (damage < 0)
            {
                damage = 0;
            }

            if (skillDamage < 0)
            {
                skillDamage = 0;
            }

            //피해량증폭과 피해차감을 뺀후
            attackUnitAbility.GetUnitStatusValue(UnitStatusType.IncreaseGiveDamageRatio, out var increaseGiveDamageRatio);
            hitUnitAbility.GetUnitStatusValue(UnitStatusType.DecreaseTakenDamageRatio, out var decreaseTakenDamageRatio);
            float increaseDamageRatio = increaseGiveDamageRatio - decreaseTakenDamageRatio;

            //대미지와 스킬대미지에 최종피해량증폭을 계산 후 
            damage += lLcroweUtil.GetPercentForValue(damage, increaseDamageRatio);
            skillDamage += lLcroweUtil.GetPercentForValue(skillDamage, increaseDamageRatio);


            //맞는 유닛에 스탯을 계산
            //이팩트가 나올수도
            healthPoint -= damage;
            healthPoint -= skillDamage;

            bool isDeadDamage = false;

            //죽음처리
            if (healthPoint < 0f)
            {   
                isDeadDamage = true;
                healthPoint = 0;
            }

            hitUnitAbility.SetUnitStatusValue(UnitStatusType.HealthPoint, healthPoint);

            // 점수판 기록
            if (attackUnit.TryGetComponent(out ScoreTarget attackUnitScore))
            {
                //공격자 점수판
                if (isDeadDamage)
                {
                    attackUnitScore.AddScore(ScoreType.KillCount, 1);
                }

                attackUnitScore.AddScore(ScoreType.GiveDamage, Mathf.RoundToInt(damage + skillDamage));
            }
            if (hitUnit.TryGetComponent(out ScoreTarget hitUnitScore))
            {
                //맞는자 점수판
                hitUnitScore.AddScore(ScoreType.HitCount, 1);
                hitUnitScore.AddScore(ScoreType.TakenDamage, Mathf.RoundToInt(damage + skillDamage));
            }

            //이팩트처리

        }

        public abstract TeamType GetTeamType();

        public Transform GetTr()
        {
            return tr;
        }

        public abstract bool GetGameTeamRule(out TeamRuleData teamRuleData);

        public abstract TeamRole GetTeamRole();

        public abstract bool GetTeamRolePriorityBible(out TeamRolePriorityBible teamRolePriorityBible);

        public bool IsAlive()
        {
            if (unitAbilityModule.GetUnitStatusValue(UnitStatusType.HealthPoint, out var value))
            {
                return value > 0;
            }
            return false;
        }
    }
}