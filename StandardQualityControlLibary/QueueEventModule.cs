using System.Collections.Generic;
using UnityEngine;

namespace lLCroweTool
{
    /// <summary>
    /// 큐 이벤트 모듈
    /// </summary>
    /// <typeparam name="T">특정타입</typeparam>    
    public class QueueEventModule<T>: Queue<T>, ISerializationCallbackReceiver
    {
        //현모듈은 큐에 특정타입을 집어넣은 후에 특정시간이 지난뒤에 이벤트를 작동하게 만드는 기능이 존재
        [SerializeField] private List<T> queueList = new List<T>();//키값
        [SerializeField] private float timer = 0.1f;//타이머
        [SerializeField] private float time;//타이머 체크용

        //ISerializationCallbackReceiver는
        //OnBeforeSerialize는 데이터를 저장할 때 호출
        //OnAfterDeserialize는 데이터를 불러올 때 호출
        /// <summary>
        /// 데이터를 저장할떄 호출하는 함수
        /// ISerializationCallbackReceiver 인터페이스함수
        /// </summary>
        public void OnBeforeSerialize()
        {
            queueList.Clear();
            
            foreach (var target in this)
            {
                queueList.Add(target);                
            }
        }

        /// <summary>
        /// 데이터를 불려올때 호출하는 함수
        /// ISerializationCallbackReceiver 인터페이스함수
        /// </summary>
        public void OnAfterDeserialize()
        {
            Clear();

            for (int i = 0; i < queueList.Count; i++)
            {
                Enqueue(queueList[i]);
            }
        }


        ///// <summary>
        ///// 큐 엥큐
        ///// </summary>
        ///// <param name="value">집어넣을 타입or값</param>
        //public void Enqueue(T value)
        //{
        //    Enqueue(value);           
        //}

        ///// <summary>
        ///// 큐 데큐
        ///// </summary>
        ///// <returns>현재 나올 특정타입or값</returns>
        //public T Dequeue()
        //{
        //    TimeReset();            
        //    return Dequeue();
        //}

        ///// <summary>
        ///// 큐 피크//값이 안사라지고 무엇인지 체크
        ///// </summary>
        ///// <returns>현재 나올 특정타입or값</returns>
        //public new T Peek()
        //{   
        //    return Peek();
        //}

        /// <summary>
        /// 작동시간 확인 함수
        /// </summary>
        /// <returns>작동시간 여부</returns>
        public bool CheckActionTime()
        {
           return Time.time > time + timer && Count != 0;
        }

        /// <summary>
        /// 작동시간 확인 함수(오토리셋)
        /// </summary>
        /// <returns>작동시간 여부</returns>
        public bool CheckActionTimeAutoReset()
        {
            bool check = Time.time > time + timer && Count != 0;
            if (check)
            {
                TimeReset();
            }
            return check;
        }

        /// <summary>
        /// 작동 타이머 설정
        /// </summary>
        /// <param name="value">값</param>
        public void SetTimer(float value)
        {
            timer = value;
        }

        /// <summary>
        /// 타임 리셋
        /// </summary>
        public void TimeReset()
        {
            time = Time.time;
        }
    }
}
