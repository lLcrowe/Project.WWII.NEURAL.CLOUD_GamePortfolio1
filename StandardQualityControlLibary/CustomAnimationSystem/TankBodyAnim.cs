using UnityEngine;

namespace lLCroweTool.Anim
{
    public class TankBodyAnim : MonoBehaviour
    {
        //특정오브젝트에 대해 현 오브젝트를 수평을 맞추고 싶으면
        //회전의 대상이 될 터렛의 특정방향에 대한 값을 반대로 주면 됨        
        public Vector2 offSet;//오프셋
        public Transform parentBody;//부모가 될 대상
        public Transform turretBody;//터렛
        private Vector3 prevDir;
        //구조
        //Tank (GameObject)
        //  Turret
        //  TankBody(TankBody Component)
        //  track

        //parentBody == Tank
        //turretBody == Turret

        // Update is called once per frame
        void Update()
        {
            //Vector3 dir = ((turretBody.position + new Vector3(offSet.x, 0, offSet.y)) - transform.position);
            //각도문제가 있어서 각도 관련추가 처리
            Vector3 dir = ((turretBody.position + (parentBody.rotation * new Vector3(offSet.x, 0, offSet.y))) - transform.position);

            if (prevDir == dir)
            {
                return;
            }
            prevDir = dir;

            //LookRotation(방향, 회전축)
            //기본형태는 특정방향으로 y축 회전이다.
            //상대적인 회전을 원한다면 부모트랜스폼의 forward, up 등으로 가져오기

            //Quaternion rot = Quaternion.LookRotation(dir, -parentBody.forward);//축을 변경
            //rot *= Quaternion.AngleAxis(90, Vector3.right);//90도 이동으로 Y축으로 해당방향을 봐라봄//회전값을 합친후
            //transform.rotation = rot;

            transform.rotation = Quaternion.LookRotation(dir, -parentBody.forward) * Quaternion.AngleAxis(90, Vector3.right);
        }
    }
}