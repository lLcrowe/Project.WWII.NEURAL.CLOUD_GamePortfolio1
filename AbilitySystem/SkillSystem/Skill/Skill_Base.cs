using lLCroweTool.SkillSystem;
using System.Collections;
using UnityEngine;

namespace lLCroweTool
{
    /// <summary>
    /// 스킬오브젝트가 사용할 부모클래스
    /// 초기화 함수와 작동함수가 동봉되있다.
    /// 프리팹화 하여 동봉해놀것
    /// 현 클래스를 상속받아서 할시 더이상의 추가상속으로 만들지말것
    /// 상속받은 클래스에서 모든걸 처리할것. 코드줄 최적화 고려X
    /// 해당 스킬마다 어떠한 스킬카테고리를 사용하는가를 주석처리(중요)
    /// </summary>
    public abstract class Skill_Base : MonoBehaviour
    {
        //최상단에 주석을 써주어야 될것
        //행동 : => 해당스킬이 무엇을 해주는지
        //사용하는 스킬카테고리 : ex=> ActiveSkill_Other_Pos
        //ActiveSkill_Myself,//액티브스킬 자기자신에게 사용
        //ActiveSkill_Other_Pos,//액티브스킬 다른 위치에 사용
        //ActiveSkill_Other_Object,//액티브스킬 다른 오브젝트에 사용
        //PassiveSkill,//패시브스킬

        protected SkillSlot targetSkillSlot;//스킬주최자가 있는 스킬슬롯
        [SerializeField] protected float skillRange;//스킬 사거리
        [SerializeField] protected float skillAreaSize = 0.2f;//스킬범위 크기//실질적인 스킬크기와 다르니 스킬포인터에서 다른방식으로 변경해보자

        [SerializeField] private int targetAmount = 1;//타겟량//스킬포인터에서 체크하여 작동//기본

        //참조크기는 0, 1 ,0 
        //매니저에 초기화, 리셋한개씩
        //액션은 2개 들어감

        /// <summary>
        /// 스킬을 처음 세팅시 초기화하는 함수 (순서1)
        /// </summary>
        public abstract void InitSkill();
        /// <summary>
        /// 스킬캐스트 작동될때 사용하는 함수
        /// </summary>
        public abstract void ActionSkillCast();
        /// <summary>
        /// 스킬캐스트를 취소할때 사용하는 함수
        /// </summary>
        public abstract void CancelSkillCast();
        /// <summary>
        /// 스킬을 작동하는 함수 (순서2)
        /// </summary>
        public abstract void ActionSkill();
        /// <summary>
        /// 해당 스킬을 리셋시킬때 사용하는 함수
        /// </summary>
        public abstract void ResetSkill();
        /// <summary>
        /// 스킬 사거리를 가져오는 함수
        /// </summary>
        /// <returns>스킬 사거리</returns>
        public float GetSkillRange()
        {
            return skillRange;
        }
        /// <summary>
        /// 스킬 범위를 가져오는 함수
        /// </summary>
        /// <returns>스킬 범위</returns>
        public float GetSkillAreaSize()
        {
            return skillAreaSize;
        }
        /// <summary>
        /// 위치를 지정하는 함수
        /// </summary>
        /// <param name="_pos">위치값</param>
        public abstract void SetPosition(Vector2 _pos);
        /// <summary>
        /// 스킬베이스에 타겟을 지정해주는 함수
        /// </summary>
        /// <param name="_targetSkillSlot">타겟이 될 스킬슬롯</param>
        public void SetTargetSkillSlot(SkillSlot _targetSkillSlot)
        {
            targetSkillSlot = _targetSkillSlot;
        }
        /// <summary>
        /// 스킬베이스에 타겟이된 스킬슬롯을 가져오는 함수
        /// </summary>
        /// <returns>스킬베이스의 스킬슬롯</returns>
        public SkillSlot GetTargetSkillSlot()
        {
            return targetSkillSlot;
        }
        /// <summary>
        /// 스킬의 타겟팅 수 기본 => 1
        /// </summary>
        /// <returns></returns>
        public int GetTargetAmount()
        {
            return targetAmount;
        }
        
    }
}