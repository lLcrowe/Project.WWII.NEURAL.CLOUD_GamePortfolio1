using lLCroweTool.Ability;
using lLCroweTool.Effect;
using lLCroweTool.SkillSystem;
using lLCroweTool.UnitBatch;
using UnityEngine;

namespace lLCroweTool.DataBase
{
    [CreateAssetMenu(fileName = "New UnitObjectInfo", menuName = "lLcroweTool/New UnitObjectInfo")]
    public class UnitObjectInfo : IconLabelBase
    {
        //여기가 아마 유닛데이터와 스탯둘다 가져와서 세팅해주는 구역임 체크하기
        //public UnitTagType[] unitTagArray = new UnitTagType[0];
        public UnitClassType unitClassType;
        public Sprite classIcon;
             
        //프리팹들 연동
        public DamageObject damageObjectPrefab;//대미지모듈관련
        public UnitObject_Base unitPrefab;//유닛프리팹
        public UnitBatchCardUI unitCardUI;//UI카드 프리팹

        //이팩트
        public EffectObject fireEffectPrefab;//발사


        //유닛스킬//아직안씀
        public SkillObjectScript[] skillDataArray = new SkillObjectScript[0];

        //유닛스탯
        public StatusData statusData = new StatusData();


        //작동될 업적처리
        [Header("업적처리")]        
        [RecordIDTagAttribute] public string recordID_KillEnemy;
        [RecordIDTagAttribute] public string recordID_DestroyTank;
        [RecordIDTagAttribute] public string recordID_KillIInfantryman;


        public override LabelBaseDataType GetLabelDataType()
        {
            return LabelBaseDataType.UnitData;
        }
    }

    ////태그
    //public enum UnitTagType
    //{
    //    Attacker,
    //    Tanker,
    //}

    //직업
    public enum UnitClassType
    {
        RifleMan,       //라이플맨
        Tank,           //탱크
        MachineGunMan,  //기관총
        Officer,        //장교
        Structure,      //건축물
    }

    public enum DamageType
    {
        NormalDamage,//공격
        SkillDamage,//스킬대미지
        HealDamage,//힐
        IgnoreArmrDamage,//방어무시
    }

 

    //이제 사용안함//스탯시스템이 제작됫음

    ///// <summary>
    ///// 유닛상태 데이터
    ///// </summary>
    //[System.Serializable]
    //public class UnitStateData
    //{
    //    //상태이상관련//능력과는 별개로 유닛의 상태를 체크하는 구역
    //    public bool stunnedState = false;//기절 상태
    //    public bool stealthState = false;//은신 상태
    //    public bool fascinationState = false;//매료 상태
    //    //public bool moveState;//움직이는 상태//필요할까?
    //    public bool superArmorState = false;//슈퍼아머
    //}

    ///// <summary>
    ///// 유닛스탯 데이터
    ///// </summary>
    //[System.Serializable]
    //public class UnitStatusData
    //{
    //    //뉴럴클라우드 데이터 체크
    //    public UnitTeamType teamType;

    //    //체력관련
    //    public int maxHealth;
    //    public int curHealth;

    //    //대미지관련
    //    public int damage;//일반공격, 스킬공격
    //    public int skillDamage;//스킬공격만 해당됨

    //    public float attackCoolTime;//공격쿨타임
    //    public float attackDistance;//공격사거리
    //    public DamageModule projectilePrefab;//공격투사체
    //    public DamageType damageType;//대미지타입

    //    public float projectileSpeed;//투사체 스피드
    //    public float damageRange;//대미지거리

    //    //이동관련
    //    public float moveSpeed;


    //    //아머관련
    //    public int damageArmor;
    //    public int skillArmor;

    //    //크리티컬관련
    //    public int criticalChance;//크리티컬 찬스
    //    public float criticalScaling;//크리티컬 배율

    //    //관통
    //    public int damageArmorPenetration;//아머관통
    //    public int skillArmorPenetration;//아머관통


    //    //회피율
    //    public int dodgeChanceRatio;

    //    //전투후 회복
    //    public int combatEndHill;

    //    //충전속도//스킬충전속도//스킬쪽하고 곱함
    //    public float skillChargeSpeed;

    //    //효과저항
    //    public int effectResistance;

    //    //받는 피해량증폭//피해를 주었을때 받는 대미지 상승
    //    public int increaseTakenDamageRatio;

    //    //피해차감//피해를 받았을시 차감
    //    public int decreaseTakenDamageRatio;

    //    //치료효과//치료받았을시 회복율
    //    public int increaseHillPowerRatio;
    //}

}