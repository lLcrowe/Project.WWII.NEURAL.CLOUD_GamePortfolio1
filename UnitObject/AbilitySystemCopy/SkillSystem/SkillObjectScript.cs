using UnityEngine;

namespace lLCroweTool.SkillSystem
{
    [CreateAssetMenu(fileName = "New SkillData", menuName = "lLcroweTool/New SkillData")]

    public class SkillObjectScript : IconLabelBase
    {
        //데이터폴더 네이밍
        //스킬이름_Active_Targeting
        //스킬이름_Active_Myself
        //스킬이름_Passive_Targeting
        //스킬이름_Passive_Myself

        //220201213 개편
        //ActiveSkill_Myself,//액티브스킬 자기자신에게 사용
        //ActiveSkill_Other_Pos,//액티브스킬 다른 위치에 사용
        //ActiveSkill_Other_Object,//액티브스킬 다른 오브젝트에 사용
        //PassiveSkill,//패시브스킬

        //-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
        //스킬이름
        //스킬아이콘
        //스킬 짧은설명(애니메이션 이름)
        //스킬설명
        //공통
        
        [Header("스킬 세팅")]        
        public SkillCATType skillCATType;//스킬카테고리
        public float skillCooltime;//스킬 재사용시간
        public bool isMoveCancel;//움직일시 캔슬되는 여부

        //20201212 밑에 두개를 스킬에 이전
        //public float skillRange;//스킬 사거리
        //public float skillAreaSize = 0.2f;//스킬범위 크기//실질적인 스킬크기와 다르니 스킬포인터에서 다른방식으로 변경해보자

        [Header("스택관련")]
        public bool isUseStack = false;
        public int stackMaxNum = 2;
        public float stackingTimer = 1.5f; //스택시스템을 이용할시 쿨타임보다는 커야함//20201214 왜?

        [Header("캐스팅관련")]
        public float castingTime = 0.2f;

        //[Header("버프 관련")]//스킬을 
        //public BuffInfo buffInfo;

        [Header("작동시킬 스킬 프리팹")]//RespawnModule사용예정//사용안함 스킬에다 하기로 결정. 관리가 힘듬//자체적으로 스킬에 집어넣기로 함
        public Skill_Base targetSkillPrefab;//스폰할 오브젝트  

        public override LabelBaseDataType GetLabelDataType()
        {
            return LabelBaseDataType.SkillData;
        }
    }
}
