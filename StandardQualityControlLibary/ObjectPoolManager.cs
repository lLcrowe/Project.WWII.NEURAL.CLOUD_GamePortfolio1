using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using lLCroweTool.Singleton;
using lLCroweTool.Dictionary;
using lLCroweTool.ObjectPool;
using lLCroweTool.PoolBible;


namespace lLCroweTool
{
    public class ObjectPoolManager : MonoBehaviourSingleton<ObjectPoolManager>
    {
        //[System.Serializable]
        //public class AttackBoxBasePoolBible : CustomPoolBible<AttackBox_Base> { }
        //public AttackBoxBasePoolBible attackBox_BasePoolBible = new AttackBoxBasePoolBible();

        //[System.Serializable]
        //public class EnemySpawnPoolBible : CustomPoolBible<EnemyUnitObject> { }
        //public EnemySpawnPoolBible enemySpawnPoolBible = new EnemySpawnPoolBible();

        //[System.Serializable]
        //public class EnemyTestSpawnPoolBible : TestCustomPoolBible<EnemyUnitObject> { }
        //public EnemyTestSpawnPoolBible enemyTestSpawnPoolBible = new EnemyTestSpawnPoolBible();

        //[System.Serializable]
        //public class EffectSpawnPoolBible : CustomPoolBible<EffectObject> { }
        //public EffectSpawnPoolBible effectSpawnPoolBible = new EffectSpawnPoolBible();

        //[System.Serializable]
        //public class UnitStatBarPoolBible : CustomPoolBible<UnitStatBar> { }
        //public UnitStatBarPoolBible unitStatBarPoolBible = new UnitStatBarPoolBible();



        //[System.Serializable]
        //public class SkillSelectButtonPoolBible : CustomPoolBible<SkillSelectButton> { }
        //public SkillSelectButtonPoolBible skillSelectButtonPoolBible = new SkillSelectButtonPoolBible();

        //[System.Serializable]
        //public class GrenadePoolBible : CustomPoolBible<Grenade> { }
        //public GrenadePoolBible grenadePoolBible = new GrenadePoolBible();


        ///// <summary>
        ///// 빈탄피용 커스텀폴
        ///// </summary>
        //[System.Serializable] public class EmtpyCaseBasePool : CustomObjectPool<EmtpyBulletCase> { }
        ///// <summary>
        ///// 빈탄창용 커스텀폴
        ///// </summary>
        //[System.Serializable] public class EmtpyMagazineBasePool : CustomObjectPool<EmtpyMagazine> { }


        //[System.Serializable]
        //public class UnitObjectPoolBible : CustomPoolBible<UnitObject_Base> { }
        //public UnitObjectPoolBible unitObjectPoolBible = new UnitObjectPoolBible();

        //[System.Serializable]
        //public class HexTileDetectObjectPoolBible : CustomPoolBible<HexTileObject> { }
        //public HexTileDetectObjectPoolBible hexTileDetectObjectPoolBible = new HexTileDetectObjectPoolBible();



        //Test결과//해쉬값으로 변경시키고 보는것보다 그냥 string으로 보는게 오히려빠르다
        //좀더 자세히 살펴보니 스트링을 헤수ㅢ코드로 변경후 딕셔너리가 더빠름
        [System.Serializable]
        public class DynamicPoolBible: CustomPoolBible<Component>{}
        public DynamicPoolBible dynamicPoolBible = new DynamicPoolBible();


        //public Canvas mainCanvas;//UI폴을 위한건데 구지 필요한가

        protected override void Awake()
        {
            base.Awake();
            //mainCanvas = FindObjectOfType<Canvas>();
        }

        //public HexTileObject RequestHexTileDectectObject(HexTileObject target)
        //{
        //    return hexTileDetectObjectPoolBible.RequestPrefab(target);
        //}

        //public UnitObject_Base RequestUnitObject(UnitObject_Base target)
        //{
        //    return unitObjectPoolBible.RequestPrefab(target);
        //}


        public T RequestDynamicComponentObject<T>(T target) where T : Component
        {   
            return dynamicPoolBible.RequestPrefab(target) as T;
        }

        public void ReturnDynamicComponentObject<T>(T target) where T : Component
        {
            dynamicPoolBible.ReturnPrefab(target);
        }

        /// <summary>
        /// 대미지박스를 요청하는 함수
        /// </summary>
        /// <param name="aEMB">사용성장비모듈베이스</param>
        /// <returns>대미지박스</returns>
        //public AttackBox_Base RequestAttackBox_Base(ActionEquipmentModuleBase aEMB)
        //{
        //    AttackBox_Base damageBox = attackBox_BasePoolBible.RequestPrefab(aEMB.GetAttackBox_Base());
        //    //damageBox.InitDamageBox(unitObject, weaponData);
        //    damageBox.InitAttackBox(aEMB.GetDamagePartData(), aEMB.GetProjectilePartData(), aEMB.GetUnitObject());
        //    damageBox.gameObject.SetActive(true);
        //    return damageBox;
        //}
        //public AttackBox_Base RequestAttackBox_Base(AttackObjectScript weaponData, TestWorldUnitObject unitObject )
        //{
        //    AttackBox_Base damageBox = attackBox_BasePoolBible.RequestPrefab(weaponData.projectilePartData.attackPrefab);
        //    //damageBox.InitDamageBox(unitObject, weaponData);
        //    damageBox.InitAttackBox(weaponData.damagePartData, weaponData.projectilePartData, unitObject);
        //    damageBox.gameObject.SetActive(true);
        //    return damageBox;
        //}

        ///// <summary>
        ///// 적군유닛을 요청하는 함수
        ///// </summary>
        ///// <param name="unitName">유닛이름</param>
        ///// <returns>적군유닛</returns>
        //public EnemyUnitObject RequestEnemyUnitObject(EnemyUnitObject enemyUnitObject)
        //{
        //    EnemyUnitObject enemyUnit = enemySpawnPoolBible.RequestPrefab(enemyUnitObject);
        //    enemyUnit.gameObject.SetActive(true);
        //    return enemyUnit;
        //}

        //public EnemyUnitObject RequestTestEnemyUnitObject(EnemyUnitObject enemyUnitObject)
        //{
        //    EnemyUnitObject enemyUnit = enemyTestSpawnPoolBible.RequestPrefab(enemyUnitObject);
        //    enemyUnit.gameObject.SetActive(true);
        //    return enemyUnit;
        //}
        /// <summary>
        /// 이팩트를 요청하는 함수
        /// </summary>
        /// <param name="curPos">현재위치</param>
        /// <param name="hitPos">히트위치</param>
        /// <param name="isChild">자식여부</param>
        ///// <param name="effectObject">이팩트오브젝트</param>
        //public void RequestEffectObject(Vector2 curPos, Transform hitPos, bool isChild, EffectObject effectObject)
        //{
        //    //20201102
        //    //나중에 이팩트종류마다 랜덤한 각도를 설정하거나 특정한 각도를 설정하게 작업 예정
        //    //20230225//GIR프로젝트 이식//각도이식끝

        //    EffectObject targetEffect = effectSpawnPoolBible.RequestPrefab(effectObject);
        //    targetEffect.gameObject.SetActive(true);
        //    targetEffect.Action(curPos, hitPos.position);
        //    targetEffect.transform.parent = isChild ? hitPos : null;
        //}

        ///// <summary>
        ///// 이팩트를 요청하는 함수
        ///// </summary>
        ///// <param name="curPos">현재위치</param>
        ///// <param name="hitPos">히트위치</param>
        ///// <param name="effectObject">이팩트오브젝트</param>
        //public void RequestEffectObject(Vector2 curPos, Vector2 hitPos, EffectObject effectObject)
        //{
        //    EffectObject targetEffect = effectSpawnPoolBible.RequestPrefab(effectObject);
        //    targetEffect.gameObject.SetActive(true);
        //    targetEffect.Action(curPos, hitPos);
        //    targetEffect.transform.parent = null;
        //}


        //public UnitStatBar RequestUnitStatBar(UnitStatBar unitStatBar)
        //{
        //    UnitStatBar unitStat = unitStatBarPoolBible.RequestPrefab(unitStatBar);
        //    unitStat.transform.SetParent(mainCanvas.transform);
        //    unitStat.gameObject.SetActive(true);
        //    return unitStat;
        //}

        ////public SkillSelectButton RequestSkillSelectButton(SkillSelectButton skillSelectButton)
        ////{
        ////    SkillSelectButton temp = skillSelectButtonPoolBible.RequestPrefab(skillSelectButton);
        ////    temp.gameObject.SetActive(true);
        ////    return temp;
        ////}

        //public Grenade RequestGrenade(Grenade grenade)
        //{
        //    Grenade temp = grenadePoolBible.RequestPrefab(grenade);
        //    temp.gameObject.SetActive(true);
        //    return temp;
        //}


        /// <summary>
        /// 모든 폴들에 있는 오브젝트를 비활성화
        /// </summary>
        public void AllDeActiveObjectPool()
        {
            //attackBox_BasePoolBible.AllObjectDeActive();
            //enemySpawnPoolBible.AllObjectDeActive();
            //enemyTestSpawnPoolBible.AllObjectDeActive();
            //effectSpawnPoolBible.AllObjectDeActive();
            //unitStatBarPoolBible.AllObjectDeActive();
            //skillSelectButtonPoolBible.AllObjectDeActive();

        }
    }
}