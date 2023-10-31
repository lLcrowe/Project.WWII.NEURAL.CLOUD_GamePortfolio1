using System.Collections;
using UnityEngine;

namespace lLCroweTool.UI.Line
{
    public class ImageLine : MonoBehaviour
    {
        //유니티에서 사용할 이미지로 만드는 Line
        //현재위치와 상대위치를 세팅하여 라인을 그려주는 클래스
        
        public float lineWidth = 1.0f;//라인폭
        public Vector3 point;//타겟포인트

        //내부컴포넌트
        private RectTransform imageRect;
        public RectTransform ImageRect { get => imageRect; }

        private void Awake()
        {
            imageRect = GetComponent<RectTransform>();
        }

        /// <summary>
        /// 포인트지정
        /// </summary>
        /// <param name="aPoint">시작포인트</param>
        /// <param name="bPoint">끝포인튼</param>
        public void SetPoint(Vector3 aPoint, Vector3 bPoint)
        {
            transform.position = aPoint;
            Vector3 originPos = transform.position;
            Vector3 dir =(originPos + bPoint) - originPos;

            imageRect.sizeDelta = new Vector2(dir.magnitude, lineWidth);//width는 길이//height는 라인의 폭
            imageRect.pivot = new Vector2(0, 0.5f);//피봇설정

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;//기울기처리
            imageRect.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}