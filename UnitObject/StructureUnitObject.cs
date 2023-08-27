using lLCroweTool.GamePlayRuleSystem;


namespace lLCroweTool
{
    public class StructureUnitObject : UnitObject_Base
    {
        public override bool GetGameTeamRule(out TeamRuleData teamRuleData)
        {
            teamRuleData = null;
            return false;
        }

        public override TeamRole GetTeamRole()
        {
            return TeamRole.Wall;
        }

        public override bool GetTeamRolePriorityBible(out TeamRolePriorityBible teamRolePriorityBible)
        {
            teamRolePriorityBible = null;
            return false;
        }

        public override TeamType GetTeamType()
        {
            return TeamType.Neutrality;
        }

        //벽같은 구조물 유닛오브젝트
        protected override void Update()
        {
            
        }
    }
}