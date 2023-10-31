using System.Collections;
using UnityEngine;

namespace lLCroweTool.NoticeDisplay.Extend
{
    /// <summary>
    /// 알람디스플레이에서 확장기능을 사용하고자할떄 쓰는 클래스(기초)
    /// </summary>
    public abstract class NoticeDisplayUIExtend_Base : MonoBehaviour
    {
        /// <summary>
        /// 디큐해서 나온 메세지를 받는 함수
        /// </summary>
        /// <param name="content">메세지콘텐츠</param>
        /// <returns>알람UI를 소환하는 여부 </returns>
        public abstract bool SendContentMessage(string content);
    }
}