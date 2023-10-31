using UnityEngine;
using System.Collections.Generic;
//using Micosmo.SensorToolkit;
//using lLCroweTool.BoxColliderSystem.HitBoxSystem;
//using lLCroweTool.StatusModuleSystem;

namespace lLCroweTool.BuffSystem
{
    //다른걸로 변경예정
    //[RequireComponent(typeof(TriggerObjectEnter2D))]
    //[RequireComponent(typeof(TriggerObjectExit2D))]
    //[RequireComponent(typeof(TriggerSensor2D))]

    public class BuffInterectObject : MonoBehaviour
    {
        //제약조건에 따라 
        //버프를 주고 뺏어가는 상호작용오브젝트
        //제약조건은 다른 클래스를 사용함
        //사용하는 클래스 이름
        //1. TriggerObjectEnter2D
        //2. TriggerObjectExit2D
        //적용할 버프데이터를 집어넣으면 해당 버프데이터를 해당 스탯에 적용시켜줌
        //버프매니저로 통해서 오는걸 다이렉트로 변경시킴
        //그럼 버프매니저를 어디다 사용하는가?
        //버프매니저는 게임스킬쪽에서 사용할 예정

        //20220704//센서툴킷과 연동
        //트리거센서로 작동하게 함
        //나중에 성능이 안나오면 2DrangeSensor로 교환하기

        //적용할 버프데이터들
        public List<BuffInfo> applyBuffData = new List<BuffInfo>();


        private void Awake()
        {
            //TriggerSensor2D sensor = GetComponent<TriggerSensor2D>();
            //sensor.OnDetected.AddListener(AddInterectBuff);
            //sensor.OnLostDetection.AddListener(RemoveInterectBuff);
        }



        //버프를 주는 것
        //TriggerObjectEnter2D 에 집어넣을것
        public void AddInterectBuff(GameObject go/*d, Sensor sensor*/)
        {
            //if (go.TryGetComponent(out HitBox hitBox))
            //{
            //    if (!hitBox.GetWorldObject().isUseStat)
            //    {
            //        return;
            //    }
            //    UnitStatusModule unitStatus = hitBox.GetWorldObject().unitStatus;
            //    //버프요청
            //    //요청할때는 이름으로 매니저에서 찾아서 요청
            //    for (int i = 0; i < applyBuffData.Count; i++)
            //    {
            //        BuffManager.Instance.AddBuff(unitStatus, applyBuffData[i], null);
            //    }
            //}
            ////else
            ////{
            ////    Debug.Log(go.name + "는 스탯모듈이 없습니다.");
            ////}
        }

        //버프를 빼는것
        //TriggerObjectExit2D에 집어넣을것
        public void RemoveInterectBuff(GameObject go/*, Sensor sensor*/)
        {
            //if (go.TryGetComponent(out HitBox hitBox))
            //{
            //    if (!hitBox.GetWorldObject().isUseStat)
            //    {
            //        return;
            //    }
            //    UnitStatusModule unitStatus = hitBox.GetWorldObject().unitStatus;
            //    //버프삭제
            //    //삭제할때는 받은 버프를 없앰                
            //    //참조할시
            //    int tmpValue = applyBuffData.Count;
            //    for (int i = 0; i < tmpValue; i++)
            //    {
            //        BuffManager.Instance.RemoveBuff(unitStatus, applyBuffData[i]);
            //    }
            //}
            ////else
            ////{
            ////    Debug.Log(go.name + "는 스탯모듈이 없습니다.");
            ////}
        }

        private void OnDestroy()
        {
            applyBuffData.Clear();
        }
    }
}