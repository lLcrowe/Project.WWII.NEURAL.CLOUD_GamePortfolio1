using lLCroweTool;
using UnityEngine;

namespace Assets.A0
{
    public class CatmullRomSplineTest : MonoBehaviour
    {
        //캣멀룸 스플라인//기존의 베지어곡선과 살짝 다르다
        //이건 Handle이 맨앞과 맨뒤에 있는 형태
        

        //https://ko.wikipedia.org/wiki/%EC%BA%A3%EB%A9%80%EB%A1%AC_%EC%8A%A4%ED%94%8C%EB%9D%BC%EC%9D%B8
        //테스트바람//20230709
        //작동 잘함//20230824
        public Transform startHandleTr;
        public Transform startTr;
        public Transform endTr;
        public Transform endHandleTr;

        public int pointAmount = 20;
        public SplineType splineType;
               

        public enum SplineType
        {
            Uniform,
            Centripetal,
            Chordal,
        }

        public AxisDirectionType axisDirectionType;

      


        private void Update()
        {
            Vector3 _p0 = startHandleTr.position;
            Vector3 _p1 = startTr.position;
            Vector3 _p2 = endTr.position;
            Vector3 _p3 = endHandleTr.position;            
            GetCatmullRomSplinePoint(_p0, _p1, _p2, _p3, 0);
        }


        /// <summary>
        /// 캣멀룸스플라인 포인트를 가져오는 함수
        /// </summary>
        /// <param name="_p0">시작핸들</param>
        /// <param name="_p1">시작</param>
        /// <param name="_p2">끝</param>
        /// <param name="_p3">끝핸들</param>
        /// <param name="interpolationValue">보간값//0: 곡선의 시작점 / 1: 곡선의 끝점</param>
        /// <returns></returns>
        public Vector3 GetCatmullRomSplinePoint(Vector3 _p0, Vector3 _p1, Vector3 _p2, Vector3 _p3, float interpolationValue)
        {
            float t0 = 0;
            float t1 = GetNextT(t0, _p0, _p1);
            float t2 = GetNextT(t1, _p1, _p2);
            float t3 = GetNextT(t2, _p2, _p3);

            float t = Mathf.Lerp(t1, t2, interpolationValue);

            Vector3 A1 = (t1 - t) / (t1 - t0) * _p0 + (t - t0) / (t1 - t0) * _p1;
            Vector3 A2 = (t2 - t) / (t2 - t1) * _p1 + (t - t1) / (t2 - t1) * _p2;
            Vector3 A3 = (t3 - t) / (t3 - t2) * _p2 + (t - t2) / (t3 - t2) * _p3;
            Vector3 B1 = (t2 - t) / (t2 - t0) * A1 + (t - t0) / (t2 - t0) * A2;
            Vector3 B2 = (t3 - t) / (t3 - t1) * A2 + (t - t1) / (t3 - t1) * A3;
            Vector3 C = (t2 - t) / (t2 - t1) * B1 + (t - t1) / (t2 - t1) * B2;

            return C;
        }

        private float GetNextT(float t, Vector3 p0, Vector3 p1)
        {
            return Mathf.Pow(Vector3.SqrMagnitude(p1 - p0), 0.5f * GetAlpha(splineType)) + t;
        }

        private float GetAlpha(SplineType type)
        {
            switch (type)
            {
                case SplineType.Uniform:
                    return 0f;
                case SplineType.Centripetal:
                    return 0.5f;
                case SplineType.Chordal:
                    return 1f;
            }
            return 0;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector3 _p0 = startHandleTr.position;
            Vector3 _p1 = startTr.position;
            Vector3 _p2 = endTr.position;
            Vector3 _p3 = endHandleTr.position;

            
            Vector3 prevP = GetCatmullRomSplinePoint(_p0, _p1, _p2, _p3, 0);
            for (int i = 1; i < pointAmount; i++)
            {
                Vector3 p = GetCatmullRomSplinePoint(_p0, _p1, _p2, _p3, (float)i / pointAmount);
                Gizmos.DrawLine(prevP, p);
                prevP = p;
            }
        }
    }
}