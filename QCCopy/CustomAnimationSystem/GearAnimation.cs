using lLCroweTool;
using System.Collections;
using UnityEngine;

namespace lLCroweTool.Anim
{
    public class GearAnimation : MonoBehaviour
    {
        //비율에 따른 애니메이션처리
        [System.Serializable]
        public class GearData
        {
            //https://kr.misumi-ec.com/special/gear/about/
            //회전비(속도 전달비)
            //맞물리는 한 쌍의 기어에서 회전비(i)는 기준원 직경에 반비례 합니다.
            //잇수와 회전수는 비례합니다.
            //기준원 직경：d 잇수：Z 회전수：n(rpm)
            //A:원동기어, B:피동기어

            //※ 회전비i == dB/dA == ZB/ZA == nA/nB

            //- 원동축(원동기어) : 동력이 들어오는 축(입력)
            //- 종동축(종동기어) : 동력이 나가는 축(출력)
            //- 가속기어 : 원동기어 크기 > 종동기어 크기//동력 나가는 축의 속도가 빨라빠르게 가속
            //- 감속기어 : 원동기어 크기 < 종동기어 크기//동력 나가는 축의 속도가 느려져 감속

            //크기가 크면 느리지?//간단히 처리
            public float scale = 1;
            public Transform target;
        }

        //자기자신이 원동기어임
        public float speed = 1;
        public float scale = 1;

        //연결된 기어들
        public GearData[] connectGearArray = new GearData[0];

        private Transform tr;
        private void Awake()
        {
            tr = transform;
        }


        private void Update()
        {
            float deltaTime = Time.deltaTime;
            RotateGear(tr,deltaTime, speed, scale);
            
            for (int i = 0; i < connectGearArray.Length; i++)
            {
                var data = connectGearArray[i];
                int value = (i & 1) == 1 ? 1 : -1;
                RotateGear(data.target, deltaTime, speed * value, data.scale);
            }
        }

        private static void RotateGear(Transform target, float deltaTime, float speed ,float scale)
        {
            //속도는 이게 맞음
            target.Rotate(Vector3.up, (speed / scale) * deltaTime);
        }
    }
}