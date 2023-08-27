using lLCroweTool.Anim;
using lLCroweTool.Anim.GunRecoil;
using lLCroweTool.Anim.GunRecoil.Muzzle;
using lLCroweTool.GamePlayRuleSystem;
using UnityEngine;

namespace lLCroweTool
{
    public class SoliderUnitObject : BattleUnitObject
    {
        public CustomAnimController_AniMancer anim;
        public GunRecoilAnim recoilAnim;

        protected override void Awake()
        {
            base.Awake();
            anim = GetComponent<CustomAnimController_AniMancer>();
            recoilAnim = GetComponent<GunRecoilAnim>();
            recoilAnim.InitGunMuzzleAction(ActionFireEffect);
        }

        public override void Idle()
        {
            base.Idle();
            anim.ActionAnim(CustomAnimController_AniMancer.UnitAnimationStateType.Idle);
        }

        public override void AttackAction(BattleUnitObject attackUnitObject)
        {
            base.AttackAction(attackUnitObject);
            anim.ActionAnim(CustomAnimController_AniMancer.UnitAnimationStateType.Attack);
            recoilAnim?.ActionRecoil();
        }

        public override void Move(Vector3 target)
        {
            base.Move(target);
            anim.ActionAnim(CustomAnimController_AniMancer.UnitAnimationStateType.Move);
        }

        public override Transform GetMuzzleTransform()
        {
            return recoilAnim.GetFirePos();
        }

        public override TeamRole GetTeamRole()
        {
            return TeamRole.Rifle;
        }
    }
}