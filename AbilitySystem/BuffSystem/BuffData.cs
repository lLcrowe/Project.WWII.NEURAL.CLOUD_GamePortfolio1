using UnityEngine;
using lLCroweTool.ClassObjectPool;

namespace lLCroweTool.BuffSystem
{
    [System.Serializable]
    public class BuffData : CustomClassPoolTarget
    {
        //버프는 버프정와 스탯데이터를 사용하여 최종값을 얻은 다음 
        //최종값을 더하고 빼서 버프를 적용하거나 뺄때 사용.


        //나중에 버프를 스트럭트로 바꿔야 될삘 현재는 그냥 사용
        //20190906

        //20210509체크
        //버프는 현재 적용된 버프데이터와 최종값을 가지고 있는매체임



        //전 시스템에 쓰던 UI를 위해서 적용시키던 데이터
        //나중에 UI관련처리할때 체크해보기
        //buffInfo.IsChange = true;
        //buffInfo.IsAdd = false;


        //private BuffInfo buffInfo;
        private float buffDurationTime;
        private float buffStartTime;


        //스택이 쌓이지 않는 버프이면 기본스택은 1로 고정이 된다.
        //그래야지 연산이 됨
        private int curStackAmount;

        ///// <summary>
        ///// 이벤트말고 상속으로 해야될듯하다//과연 ?? 
        ///// 추가 버프를 버프매니저에서 미리 만들어놓고 사용할떄 가져와서 
        ///// 활성한다음 사용 없앨때는 해당되는 버프이팩트를 삭제
        ///// 버프데이터쪽에서 확인
        ///// </summary>
        //private GameObject buffEffectObject;
        private UnitObject_Base unitObject;


        //최종적으로 연산해줄 데이터
        [Header("최종 스탯(더해준 값을 저장함)")]
        private BuffUnitStatusBible calResultBuffUnitStatusBible = new BuffUnitStatusBible();

        [Header("최종 상태(유닛의 현재상태를 저장함)")]
        private BuffUnitStateBible calResultBuffUnitStateBible = new BuffUnitStateBible();

        public BuffUnitStateBible CalResultBuffUnitStateBible { get => calResultBuffUnitStateBible;  }
        public BuffUnitStatusBible CalResultBuffUnitStatusBible { get => calResultBuffUnitStatusBible; }
        public int CurStackAmount { get => curStackAmount; set => curStackAmount = value; }


        /// <summary>
        /// 버프데이터 초기화
        /// </summary>
        /// <param name="buffInfo">버프정보</param>
        /// <param name="unitObject_Base">유닛오브젝트</param>
        public void InitBuffData(float durationTime, UnitObject_Base unitObject_Base)
        {
            buffDurationTime = durationTime;
            unitObject = unitObject_Base;
            curStackAmount = 1;
        }


        /// <summary>
        /// 버프데이터 리셋
        /// </summary>
        public void ResetBuffData()
        {
            unitObject = null;
            ResetBuffState();
            ResetBuffStatus();
            curStackAmount = 1;
        }

        /// <summary>
        /// 상태버프 리셋
        /// </summary>
        public void ResetBuffState()
        {
            //상태리셋
            foreach (var item in CalResultBuffUnitStateBible)
            {
                var key = item.Key;
                CalResultBuffUnitStateBible[key].value = false;
            }
        }

        /// <summary>
        /// 스탯버프 리셋
        /// </summary>
        public void ResetBuffStatus()
        {
            //스탯리셋
            foreach (var item in CalResultBuffUnitStatusBible)
            {
                var buffStatus = item.Value;
                buffStatus.unitStatusValue.value = 0;
                //CalResultBuffUnitStatusBible[item.Key] = buffStatus;
            }
        }

        /// <summary>
        /// 사용중인 버프여부를 세팅함수
        /// </summary>
        /// <param name="_isUseBuff">사용중인 버프여부</param>
        public void SetIsUseBuff(bool _isUseBuff)
        {   
            SetIsUse(_isUseBuff);
        }

        /// <summary>
        /// 사용중인 버프여부를 가져오는 함수
        /// </summary>
        /// <returns>사용중인 버프여부</returns>
        public bool GetIsUseBuff()
        {
            return GetIsUse();
        }

        /// <summary>
        /// 버프시간이 다 됫는지 체크해주는곳
        /// </summary>
        /// <returns>사라질 여부</returns>
        public bool BuffTimerChecker()
        {
            bool isDone = false;
            if (Time.time > buffStartTime + buffDurationTime)
            { 
                isDone = true;
            }
            return isDone;
        }

        /// <summary>
        /// 버프가 시작된 시간을 가져오는 함수
        /// </summary>
        /// <returns>시작된 시간</returns>
        public float GetBuffStartTime()
        {
            return buffStartTime;
        }

        /// <summary>
        /// 버프타이머를 리셋시켜서 기간연장해주는 함수
        /// </summary>
        public void ResetBuffTime()
        {
            buffStartTime = Time.time;
        }

        //public void SetBuffEffectObject(GameObject _buffEffectObject)
        //{
        //    buffEffectObject = _buffEffectObject;
        //}

        //public GameObject GetBuffEffectObject()
        //{
        //    return buffEffectObject;
        //}

        public UnitObject_Base GetActionObject()
        {
            return unitObject;
        }
    }
}
