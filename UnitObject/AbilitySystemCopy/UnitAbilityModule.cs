using lLCroweTool.Ability.Collect;
using lLCroweTool.BuffSystem;
using lLCroweTool.Dictionary;
using lLCroweTool.TimerSystem;
using UnityEngine;

namespace lLCroweTool.Ability
{
    public class UnitAbilityModule : MonoBehaviour
    {
        public StatusData statusData;
   
        //스탯만 관리하는 모듈로 쓰자//구지 모노상속안받아도될듯함
        [SerializeField] protected UnitStatusBible unitStatus = new UnitStatusBible();
        [SerializeField] protected UnitStateBible unitState = new UnitStateBible();
        [SerializeField] protected BuffBible buffBible = new BuffBible();

        //세트아이템데이터
        public CollectAbilityModule collectAbilityData = new CollectAbilityModule();
        
        public TimerModule_Element healthPointRateTimer;

        [System.Serializable]
        public class StatusChangeEventBible : CustomDictionary<UnitStatusType, System.Action<float>> { }
        public StatusChangeEventBible statusChangeEventBible = new();



        public BuffInfo buffInfo;
        [ButtonMethod]
        public void ApplyBuffInfo()
        {
            BuffManager.Instance.AddBuff(ref unitStatus, ref unitState, ref buffBible, buffInfo, statusData.infoUnitStateApplyBible, null);
        }


        //private void Awake()
        //{
        //    InitUnitAbilityModule(unitStatusInfo);
        //}


        /// <summary>
        /// 유닛능력모듈 초기화
        /// </summary>
        /// <param name="targetStatusData">유닛스탯 데이터</param>
        public void InitUnitAbilityModule(StatusData targetStatusData)
        {
            statusData = targetStatusData;

            if (statusData == null)
            {
                return;
            }

            //스탯처리//콜백이벤트처리
            //unitStatus.Clear();
            foreach (var item in statusData.unitStatusArray)
            {
                unitStatus.Add(item.unitStatusType, item);
                statusChangeEventBible.Add(item.unitStatusType, null);
            }

            //상태처리
            var unitStateTypeList = lLcroweUtil.GetEnumDefineData<UnitStateType>();
            //unitState.Clear();
            foreach (var item in unitStateTypeList)
            {
                unitState.Add(item, false);
            }

            //타이머처리
            healthPointRateTimer.SetTimer(1.0f);


            //자체적인 이벤트
            //넣고 싶은거 집어넣기
            AddStatusChangeEvent(UnitStatusType.HealthPoint, (float point) => { LimitPoint(unitStatus, point, UnitStatusType.HealthMaxPoint); });

        }

        private void Update()
        {
            BuffManager.UpdateBuffBible(ref unitStatus, ref unitState, ref buffBible);
            if (!healthPointRateTimer.CheckTimer())
            {
                return;
            }

            //체력 관련
            RateTargetPoint(unitStatus, UnitStatusType.HealthPoint, UnitStatusType.HealthRatePoint, UnitStatusType.HealthMaxPoint);
        }

        /// <summary>
        /// 유닛스탯 값을 세팅함수
        /// </summary>
        /// <param name="unitStatusType">찾을 스탯정의 타입</param>
        /// <param name="value">값</param>
        public void SetUnitStatusValue(UnitStatusType unitStatusType, float value)
        {
            if (!unitStatus.TryGetValue(unitStatusType, out var data))
            {
                return;
            }

            data.SetValue(value);
            unitStatus[unitStatusType] = data;
            CallChangeEvent(unitStatusType, data.value);
        }

        ///// <summary>
        ///// 유닛스탯값을 찾아오는 함수//후에 존재하는지 체크하는걸로 바꿔야됨
        ///// </summary>
        ///// <param name="unitStatusType">찾을 스탯타입</param>
        ///// <returns>값</returns>
        //public float GetUnitStatusValue(UnitStatusType unitStatusType)
        //{
        //    if (!unitStatus.TryGetValue(unitStatusType, out var data))
        //    {
        //        return 0;
        //    }
        //    return data.value;
        //}

        /// <summary>
        /// 유닛스탯값을 찾아오는 함수
        /// </summary>
        /// <param name="unitStatusType">찾을 스탯타입</param>
        /// <param name="value">찾은 값</param>
        /// <returns>찾았는지 여부</returns>
        public bool GetUnitStatusValue(UnitStatusType unitStatusType, out float value)
        {
            value = 0;
            if (!unitStatus.TryGetValue(unitStatusType, out var data))
            {
                return false;
            }
            value = data.value;
            return true;
        }

        /// <summary>
        /// 특정 유닛스탯값이 변경되면 각 스탯정의에 맞게 등록되있는 이벤트가 호출
        /// </summary>
        /// <param name="changeUnitStatusType"></param>
        /// <param name="value"></param>
        private void CallChangeEvent(UnitStatusType changeUnitStatusType, float value)
        {
            if (!statusChangeEventBible.TryGetValue(changeUnitStatusType, out var action))
            {
                return;
            }
            
            //존재하면 작동
            action?.Invoke(value);
        }

        /// <summary>
        /// 유닛스탯타입에 맞게 함수를 등록하는 함수
        /// </summary>
        /// <param name="targetUnitStatusType">타겟팅할 유닛스탯타입</param>
        /// <param name="func">등록할 함수</param>
        public void AddStatusChangeEvent(UnitStatusType targetUnitStatusType, System.Action<float> func)
        {
            if (!statusChangeEventBible.ContainsKey(targetUnitStatusType))
            {
                return;
            }
            statusChangeEventBible[targetUnitStatusType] += func;
        }

        /// <summary>
        /// 유닛스탯타입에 맞게 함수를 삭제하는 함수
        /// </summary>
        /// <param name="targetUnitStatusType">타겟팅할 유닛스탯타입</param>
        /// <param name="func">삭제할 함수</param>
        public void RemoveStatusChangeEvent(UnitStatusType targetUnitStatusType, System.Action<float> func)
        {
            if (!statusChangeEventBible.ContainsKey(targetUnitStatusType))
            {
                return;
            }
            statusChangeEventBible[targetUnitStatusType] -= func;
        }




        //----------------------------------------------------------------------------------------
        //자체적인 변동이벤트
        //----------------------------------------------------------------------------------------



        /// <summary>
        /// 유닛스탯에서 해당되는 재생포인트를 이용하여 현재포인트를 재생시켜주는 함수(정수형)
        /// </summary>
        /// <param name="unitStatus">유닛스탯</param>
        /// <param name="curPointType">현재포인트 타입</param>
        /// <param name="ratePointType">재생포인트 타입</param>
        /// <param name="maxPointType">최대포인트 타입</param>
        public static void RateTargetPoint(UnitStatusBible unitStatus, UnitStatusType curPointType, UnitStatusType ratePointType, UnitStatusType maxPointType)
        {
            //int랑 float체크하는것도 괜찮아보임//근데 미미할듯

            //현재포인트 체크
            if (!unitStatus.TryGetValue(curPointType, out var curPoint))
            {
                return;
            }

            //최대포인트 체크
            if (!unitStatus.TryGetValue(maxPointType, out var maxPoint))
            {
                return;
            }

            //레이트포인트 체크
            if (!unitStatus.TryGetValue(ratePointType, out var ratePoint))
            {
                return;
            }

            curPoint.value += ratePoint.value;
            //curPoint.value = Mathf.Clamp(curPoint.value, 0, maxPoint.value);
            curPoint.value = LimitPoint(unitStatus, curPoint.value, maxPointType);


            //if (curPoint.value < maxPoint.value)
            //{
            //    curPoint.value += ratePoint.value;
            //    //최대 최소치 체크
            //    if (curPoint.GetIntValue() > maxPoint.GetIntValue())
            //    {
            //        curPoint.value = maxPoint.GetIntValue();
            //    }
            //    else if(curPoint.value < 0)
            //    {
            //        curPoint.value = 0;
            //    }
            //}
            unitStatus[curPointType] = curPoint;
        }

        //최대치 체크이벤트
        public static float LimitPoint(UnitStatusBible unitStatus, float pointValue, UnitStatusType maxPointType)
        {
            //최대포인트 체크
            if (!unitStatus.TryGetValue(maxPointType, out var maxPoint))
            {
                return pointValue;
            }

            pointValue = Mathf.Clamp(pointValue, 0, maxPoint.value);
            return pointValue;
        }

        //욕구시스템도 통합. 어차피 이리저리 통합할려고 만드는거다
        

        



    }
}
