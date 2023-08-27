using lLCroweTool.Ability;
using lLCroweTool.GamePlayRuleSystem;
using UnityEngine;
using lLCroweTool.Anim.GunRecoil;

namespace lLCroweTool
{
    public class TankUnitObject : BattleUnitObject
    {
        //공격위치



        //포탑관련
        public float rotatePower = 50;
        public Transform turret;

        //기관포탑은 어덯게 처리할지 생각하기
        
        //캐터필러 관련
        public float caterpillarSpeed = 50f;
        public MeshRenderer[] caterpillarArray = new MeshRenderer[0];//캐터필러에 있는 메쉬랜더러의 메터리얼에서 offset - 1 방향이 전방//직접 집어넣기

        //절차애니메이션
        public GunRecoilAnim mainGunRecoil;
        //public GunRecoilAnim machineGunRecoil;

        protected override void Awake()
        {
            base.Awake();
            mainGunRecoil = GetComponent<GunRecoilAnim>();
            mainGunRecoil.InitGunMuzzleAction(ActionFireEffect);
        }

        public override void Idle()
        {
            if (!unitAbilityModule.GetUnitStatusValue(UnitStatusType.MoveSpeed,out var speed))
            {
                return;
            }
            float deltaTime = Time.deltaTime;
            Quaternion rot = Quaternion.LookRotation(tr.forward);//회전구역에 노말라이즈가 필요?//필요하네 왜일까
            speed *= deltaTime;
            rot = Quaternion.RotateTowards(turret.rotation, rot, speed * (rotatePower * 0.5f));
            turret.rotation = rot;
        }

        public override void Move(Vector3 target)
        {
            if (!unitAbilityModule.GetUnitStatusValue(UnitStatusType.MoveSpeed,out var rotateSpeed))
            {
                return;
            }

            //탱크는 회전먼저하고 움직임
            float deltaTime = Time.deltaTime;
            Vector3 dir = (target - tr.position).normalized;
            rotateSpeed *= deltaTime;
            Debug.DrawRay(tr.position, dir);

            Quaternion rot = Quaternion.LookRotation(dir);//회전구역에 노말라이즈가 필요?//필요하네 왜일까
            rot = Quaternion.RotateTowards(tr.rotation, rot, rotateSpeed * rotatePower);
            tr.rotation = rot;


            //캐터필러 움직임처리//애님
            for (int i = 0; i < caterpillarArray.Length; i++)
            {
                caterpillarArray[i].material.mainTextureOffset -= Vector2.up * deltaTime;
            }

            if (Vector3.Dot(tr.forward, dir) > 0.99f)//내적처리
            {
                //해당방향으로 다움직여야지 이동할수 있게
                tr.position += rotateSpeed * dir;
            }

            //탱크포신 방향도 변경
            rot = Quaternion.LookRotation(dir);//회전구역에 노말라이즈가 필요?//필요하네 왜일까
            rot = Quaternion.RotateTowards(turret.rotation, rot, rotateSpeed * (rotatePower * 0.5f));
            turret.rotation = rot;
        }

        public override void Attack(Vector3 target)
        {
            if (!unitAbilityModule.GetUnitStatusValue(UnitStatusType.MoveSpeed, out var speed))
            {
                return;
            }
            Vector3 dir = (target - tr.position).normalized;

            //회전처리후 발사//0.99로 나오는게 있어서 그거맞추어서 내려버림//기존은 1이었음
            if (Vector3.Dot(turret.forward, dir) < 0.999f)
            {   
                speed *= Time.deltaTime;
                Quaternion rot = Quaternion.LookRotation(dir);//회전구역에 노말라이즈가 필요?//필요하네 왜일까
                rot = Quaternion.RotateTowards(turret.rotation, rot, speed * 50);
                turret.rotation = rot;
                return;
            }
            attackCoolTimer.StartSkill();
        }

        public override void AttackAction(BattleUnitObject attackUnitObject)
        {
            base.AttackAction(attackUnitObject);

            mainGunRecoil?.ActionRecoil();
            //machineGunRecoil?.ActionRecoil();
        }

        public override Transform GetMuzzleTransform()
        {
            return mainGunRecoil.GetFirePos();
        }

        public override TeamRole GetTeamRole()
        {
            return TeamRole.Tank;
        }
    }
}