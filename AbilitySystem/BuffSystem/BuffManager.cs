using UnityEngine;
using lLCroweTool.Singleton;
using lLCroweTool.ClassObjectPool;
using lLCroweTool.Ability;

namespace lLCroweTool.BuffSystem
{
    public class BuffManager : MonoBehaviourSingleton<BuffManager>
    {
        //버프 매니저는 버프오브젝트를 가지고 있으며 데이터는 안가지고 있다
        //하지만 요청하면 해당 버프오브젝트를 버프이팩터에 제공해주면서 
        //버프데이터를 포함하여 제공해주는 중간 제공체 역할을 해줌
        //결국은 오브젝트폴링을 사용하여 중간에서 일정수를 제공해줌
        //파이터매니저에 있는 기능을 가져와서 업그레이드시킴

        //프리팹으로 가지고 있으면 편할거 같다.
        //기존텍스트매니저에서 하는거 따라 만들어야될듯
        //public int buffNumber;//버프를 몇개 만들것인지
        //public int buffCount = 0;//현재 작동되는 버프번호
        //public BuffData prefabBuff;//버프프리팹

        //20230601 버프매니저를 최신화된 유틸로 새롭게 갱신
        //컴포넌트가 아닌 클래스용 오브젝트폴을 사용하여 처리
        //신규 어빌리티 기능에 맞게 버프시스템 구조를 변경
        //


        [System.Serializable]
        public class BuffObjectPool : CustomClassPool<BuffData> { }

        public BuffObjectPool buffObjectPool = new BuffObjectPool();

        
        /// <summary>
        /// 버프데이터를 요청하는 함수
        /// </summary>
        /// <param name="buffInfo">버프정보</param>
        /// <returns>버프데이터</returns>
        private BuffData RequestBuffData(BuffInfo buffInfo)
        {
            BuffData targetBuffObject = buffObjectPool.RequestPrefab();
            return targetBuffObject;
        }

        //20220818
        //기존로직 => 버프와 유닛데이터와 분리안되있음
        //바뀐 로직 =>
        //버프를 먼저 확인후 그다음 캐릭터유닛의 상태를 확인한후 적용
        //클래스에 있던 버프정보들을 따로 클래스를 제작하여 집어넣음

        /// <summary>
        /// 해당유닛의 버프사전에 버프를 추가하고 스탯,상태에 적용시키는 함수
        /// </summary>
        /// <param name="unitStatus">계산시킬 대상스탯</param>
        /// <param name="unitState">계산시킬 대상상태</param>
        /// <param name="unitBuffBible">유닛의 버프사전</param>
        /// <param name="buffInfo">추가해줄 버프정보</param>
        /// <param name="infoState">적용시킬수 잇는 상태정보</param>
        /// <param name="actionObject">버프영향을 받을 유닛오브젝트</param>
        public void AddBuff(ref UnitStatusBible unitStatus, ref UnitStateBible unitState, ref BuffBible unitBuffBible, BuffInfo buffInfo, UnitStateBible infoState, UnitObject_Base actionObject)
        {
            //뭔가 문제있을가능성이 다분히 보이는데 나중에체크

            //버프를 적용할수 있는 데이터들을 체크
            if (buffInfo.isUseApplyNeedCondition)
            {
                if (!buffInfo.applyNeedBible.CheckNeedCost(unitStatus, unitState))
                {
                    return;
                }
            }

            //적용되고 있는 버프정보인지를 체크
            if (!unitBuffBible.TryGetValue(buffInfo, out BuffData buffData))
            {
                //없으면
                //요청후 집어넣기                
                buffData = RequestBuffData(buffInfo);
                buffData.InitBuffData(buffInfo.buffDurationTime, actionObject);
                buffData.ResetBuffData();
                if (buffInfo.isStackBuff)
                {
                    buffData.CurStackAmount = 0;
                }
                unitBuffBible.Add(buffInfo, buffData);
            }

            //타임을 사용할 경우 타임리셋
            if (buffInfo.isUseTimeBuff)
            {
                buffData.ResetBuffTime();
            }
            
            //중첩스택여부
            if (buffInfo.isStackBuff)
            {
                //기존적용된 스탯을 되돌려주는 작업을 해주고 스택을 올려주는 구역

                //스택쌓인게 1이상일시 기존걸 원복
                if (buffData.CurStackAmount > 0)
                {   
                    //기존버프데이터를 사용해 전에 적용되던 스탯빼기
                    foreach (var item in buffData.CalResultBuffUnitStatusBible)
                    {
                        var resultBuffStatusData = item.Value;
                        var statusValueType = item.Key;

                        //해당 스탯이 있는지 체크
                        if (!unitStatus.ContainsKey(statusValueType))
                        {
                            //없으면 넘김
                            continue;
                        }

                        //있으면 유닛스탯에 적용
                        var target = unitStatus[statusValueType];
                        target.value -= resultBuffStatusData.unitStatusValue.GetFloatValue();
                        unitStatus[statusValueType] = target;
                    }

                    //스탯부분만 리셋
                    buffData.ResetBuffStatus();
                }

                //스택올리기//최대스택체크
                if (buffInfo.maxStack > buffData.CurStackAmount)
                {
                    buffData.CurStackAmount++;
                }
            }

            //버프카테고리에 따라 적용
            switch (buffInfo.buffKategorie)
            {
                case BuffKategorie.AddStat:
                    //기본스택이 1이면 처음 적용되는 버프이므로//상태쪽 처리
                    if (buffData.CurStackAmount == 1)
                    {
                        //전에 있던 최종변경값 버프데이터 초기화
                        buffData.ResetBuffData();

                        //상태 최종버프연산값
                        CalResultStateBuff(ref buffData, infoState);

                        //상태처리//20230607//어덯게 만드냐에 따라 메커니즘이 상당히 달라질듯 이건 생각해봐야될것
                        //기존상태 저장은 위에서했고 여기는 버프상태정보를 매칭시켜주는 곳
                        foreach (var item in buffInfo.buffStateBible)
                        {
                            var resultBuffStateData = item.Value;
                            var stateType = item.Key;

                            //해당 상태가 있는지 체크
                            if (!unitState.ContainsKey(stateType))
                            {
                                //없으면 넘기고
                                continue;
                            }

                            //있으면 유닛상태에 적용
                            unitState[stateType] = resultBuffStateData.value;
                        }
                    }

                    //스탯 최종버프연산값
                    CalResultStatusBuff(ref unitStatus, ref buffData, ref buffInfo);

                    //스탯처리
                    foreach (var item in buffData.CalResultBuffUnitStatusBible)
                    {
                        var resultBuffStatusData = item.Value;
                        var statusValueType = item.Key;

                        //해당 스탯이 있는지 체크
                        if (!unitStatus.ContainsKey(statusValueType))
                        {
                            //없으면 넘김
                            continue;
                        }

                        //있으면 유닛스탯에 적용
                        var target = unitStatus[statusValueType];
                        target.value += resultBuffStatusData.unitStatusValue.GetFloatValue();
                        unitStatus[statusValueType] = target;
                    }
                    break;
                case BuffKategorie.AddEffect:
                    break;
                case BuffKategorie.Complex:
                    break;
            }
            buffData.SetIsUseBuff(true);
            //Debug.Log("버프를 추가되었습니다.");
        }

        /// <summary>
        /// 해당유닛의 버프사전에서 버프를 삭제하고 스탯,상태에 적용시키는 함수
        /// </summary>
        /// <param name="unitStatus">계산시킬 대상스탯</param>
        /// <param name="unitState">계산시킬 대상상태</param>
        /// <param name="unitBuffBible">유닛의 버프사전</param>
        /// <param name="buffInfo">삭제해줄 버프정보</param>
        public static void RemoveBuff(ref UnitStatusBible unitStatus, ref UnitStateBible unitState, ref BuffBible unitBuffBible, BuffInfo buffInfo)
        {
            //존재하지않으면 넘기기
            if (!unitBuffBible.TryGetValue(buffInfo,out BuffData buffData))
            {
                return;
            }

            //시간초기화
            if (buffInfo.isUseTimeBuff)
            {
                buffData.ResetBuffTime();
            }

            
            int stackAmount = 0;
            //카테고리마다 다르게 처리
            switch (buffInfo.buffKategorie)
            {
                case BuffKategorie.AddStat:

                    //스탯처리//기존거 빼버리기
                    foreach (var item in buffData.CalResultBuffUnitStatusBible)
                    {
                        var resultBuffStatusData = item.Value;
                        var statusValueType = item.Key;

                        //해당 스탯이 있는지 체크
                        if (!unitStatus.ContainsKey(statusValueType))
                        {
                            //없으면 넘김
                            continue;
                        }

                        //있으면 유닛스탯에 적용//반대로
                        var target = unitStatus[statusValueType];
                        target.value -= resultBuffStatusData.unitStatusValue.GetFloatValue();
                        unitStatus[statusValueType] = target;
                    }

                    //다음스탯 최종계산
                    stackAmount = --buffData.CurStackAmount;
                    CalResultStatusBuff(ref unitStatus, ref buffData, ref buffInfo);

                    //스택//뺄때는 구지 상태를 체크할 필요는 없어보임
                    //스택이 0이하면//없애야할 버프이니
                    if (stackAmount <= 0)
                    {
                        //상태처리
                        foreach (var item in buffData.CalResultBuffUnitStateBible)
                        {
                            var resultBuffStateData = item.Value;
                            var stateValueType = item.Key;

                            //해당 상태가 있는지 체크
                            if (!unitState.ContainsKey(stateValueType))
                            {
                                //없으면 넘기고
                                continue;
                            }

                            //있으면 유닛상태에 적용
                            unitState[stateValueType] = resultBuffStateData.value;
                        }
                        buffData.ResetBuffStatus();
                    }
                    else
                    {
                        //기존스택을 하나줄이고 1이상일시
                        //해당스택에 맞게 연산을 새롭게 처리

                        //기존버프데이터를 사용해 전에 적용되던 스탯더하기
                        foreach (var item in buffData.CalResultBuffUnitStatusBible)
                        {
                            var resultBuffStatusData = item.Value;
                            var statusValueType = item.Key;

                            //해당 스탯이 있는지 체크
                            if (!unitStatus.ContainsKey(statusValueType))
                            {
                                //없으면 넘김
                                continue;
                            }

                            //있으면 유닛스탯에 적용
                            var target = unitStatus[statusValueType];
                            target.value += resultBuffStatusData.unitStatusValue.GetFloatValue();
                            unitStatus[statusValueType] = target;
                        }
                    }
                    break;
                case BuffKategorie.AddEffect:
                    break;
                case BuffKategorie.Complex:
                    break;
            }

            //스택체크후
            if (stackAmount <= 0)
            {
                //스택이 0이면
                //유닛버프사전에서 지우기//버프데이터사용중지로 바꾸기
                unitBuffBible.Remove(buffInfo);
                buffData.SetIsUseBuff(false);
            }
            //아니면 그냥 넘기기
        }

        /// <summary>
        /// 모든버프를 삭제
        /// </summary>
        /// <param name="unitStatus">계산시킬 대상스탯</param>
        /// <param name="unitState">계산시킬 대상상태</param>
        /// <param name="unitBuffBible">유닛의 버프사전</param>
        public void AllRemoveBuff(ref UnitStatusBible unitStatus, ref UnitStateBible unitState, ref BuffBible unitBuffBible)
        {
            BuffInfo[] buffInfo = new BuffInfo[unitBuffBible.Count];

            int index = 0;
            foreach (var item in unitBuffBible)
            {
                buffInfo[index] = item.Key;
                index++;
            }

            for (int i = 0; i < buffInfo.Length; i++)
            {
                RemoveBuff(ref unitStatus, ref unitState, ref unitBuffBible, buffInfo[i]);
            }

            Debug.Log("모든 버프를 삭제했습니다.");
        }

        /// <summary>
        /// 버프정보를 통해 상태만 버프데이터를 세팅해주는 함수
        /// </summary>
        /// <param name="buffData">타겟팅할 버프데이터</param>
        /// <param name="unitStateBible"></param>
        private void CalResultStateBuff(ref BuffData buffData, UnitStateBible unitStateBible)
        {
            //이름이 뭔가 안맞지만 그다른처리를 하는 함수임
            //상태이상 버프들은 현재상태를 가져와서 저장하는게 맞음
            //전에도 그렇게처리함
            //그다음에 버프정보를 적용시키는 원리
            //반대로 만들면 문제가 발생하는게 있음

            
            //상태먼저 최종값 계산
            //버프가 리셋되서 들어옴
            //버프데이터에 기존 상태들을 저장
            var buffDataStateBible = buffData.CalResultBuffUnitStateBible;
            foreach (var item in unitStateBible)
            {
                //적용시킬버프
                bool stateValue = item.Value;
                UnitStateType StateType = item.Key;

                //적용할수 있는 존재인지 체크
                if (!buffDataStateBible.ContainsKey(StateType))
                {
                    //적용할수 없는 상태이면 추가
                    var data = new BuffUnitStateValue();
                    data.unitStateType = StateType;
                    buffDataStateBible.Add(StateType, data);
                }

                //기존상태를 저장
                buffDataStateBible[StateType].value = stateValue;
            }
        }

        /// <summary>
        /// 버프 정보를 통해 스탯만 버프데이터를 세팅해주는 함수
        /// </summary>
        /// <param name="unitStatusBible"></param>
        /// <param name="buffData"></param>
        private static void CalResultStatusBuff(ref UnitStatusBible unitStatusBible, ref BuffData buffData, ref BuffInfo buffInfo)
        {
            //스택에 따른 연산//스탯만 연관됨
            if (buffData.CurStackAmount <= 0)
            {
                return;
            }
            
            //더하기//스택량에 따라 곱하기
            //곱하기//스택량에 따라 곱하기
            //퍼센트//스탹량애 따라 퍼센트값곱하기

            //스탯 최종값계산
            var resultBuffStatusBible = buffData.CalResultBuffUnitStatusBible;
            int stackAmount = buffData.CurStackAmount;

            foreach (var item in buffInfo.buffStatusBible)
            {
                BuffUnitStatusValue applyBuffStatus = item.Value;
                var applyStatusValueType = item.Key;

                //해당버프스택이 존재하는지 체크
                if (!resultBuffStatusBible.ContainsKey(applyStatusValueType))
                {
                    //없으면 제작
                    BuffUnitStatusValue temp = new BuffUnitStatusValue();
                    resultBuffStatusBible.Add(applyStatusValueType, temp);
                }

                //계산될 방식 체크
                float resultValue = 0;
                switch (applyBuffStatus.buffStatusValueApplyType)
                {
                    case BuffStatusValueApplyType.Add:
                        //더하기 계산
                        resultValue = applyBuffStatus.unitStatusValue.GetFloatValue() * stackAmount;
                        break;
                    case BuffStatusValueApplyType.Multiple:
                        //곱하기 계산
                        //참고 스탯체크
                        if (unitStatusBible.TryGetValue(applyStatusValueType, out var unitMulStatus))
                        {
                            resultValue = unitMulStatus.GetFloatValue() * applyBuffStatus.unitStatusValue.GetFloatValue() * stackAmount;
                        }

                        break;
                    case BuffStatusValueApplyType.Percent:
                        //퍼센트 계산
                        //참고 스탯체크
                        if (unitStatusBible.TryGetValue(applyStatusValueType, out var unitPerStatus))
                        {
                            resultValue = lLcroweUtil.GetPercentForValue(unitPerStatus.GetFloatValue(), applyBuffStatus.unitStatusValue.GetFloatValue() * stackAmount);
                        }
                        break;
                }

                resultBuffStatusBible[applyStatusValueType].unitStatusValue.value = resultValue;
            }
        }

        /// <summary>
        /// 버프사전 업데이트(외부에서 특정타이머로 돌리기//0.2초)
        /// </summary>
        /// <param name="unitStatus">오닛스탯</param>
        /// <param name="unitState">유닛상태</param>
        /// <param name="unitBuffBible">유닛버프사전</param>
        public static void UpdateBuffBible(ref UnitStatusBible unitStatus, ref UnitStateBible unitState, ref BuffBible unitBuffBible)
        {   
            //버프수가 0 이하면 그대로 리턴
            if (unitBuffBible.Count == 0)
            {
                return;
            }

            //버프시간체크후 삭제//점수체크
            foreach (var item in unitBuffBible)
            {
                BuffData buffData = item.Value;
                BuffInfo buffInfo = item.Key;

                //20230601//딜 + 회복 체크=>아직 신규시스템에는 구현이 안됨
                //행위능력데이터로 행해질 예정

                //타이머체크
                if (buffInfo.isUseTimeBuff)
                {
                    //중단 체크

                    //사라지는 조건에 따른 중단
                    if (buffInfo.isUseTimeBuffDisableBuffCondition)
                    {
                        if (buffInfo.disableBuffConditionBible.CheckNeedCost(unitStatus, unitState))
                        {
                            RemoveBuff(ref unitStatus, ref unitState, ref unitBuffBible, buffInfo);
                            break;
                        }
                    }

                    //타이머중단
                    if (buffData.BuffTimerChecker())
                    {
                        RemoveBuff(ref unitStatus, ref unitState, ref unitBuffBible, buffInfo);
                        break;
                    }
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            buffObjectPool.ClearCustomObjectPool();
            buffObjectPool = null;
            //buffObjectList.Clear();
        }
    }
}