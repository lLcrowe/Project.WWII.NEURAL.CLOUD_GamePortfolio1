using UnityEngine;

namespace lLCroweTool.QC.Curve
{
    /// <summary>
    /// 레벨커브데이터(커브, 고정형데이터)
    /// </summary>
    [System.Serializable]
    public class LevelCurveData 
    {
        //인트변수를 커브로 표현하기 위한 클래스
        //스킬레벨이라든가 스탯등에서 사용하기 편리를 위해 제작

        //중간값을 디테일하게 알 필요없을시

        //상당히 유용한데 잘이용하면 각 스탯에 커브를 적용
        //밸런스를 잡을때 사용할때 정말 좋은거 같다
        //전에 체크해보니 커브는 크게 두종류를 사용한다.
        //Linear 커브(일정하게 난이도 상승)과 EaseIn (가면갈수록 난이도가 상승)

        //이거 어차피 소수점으로 바꾸면 더 연산 많이드니 그냥 정수로 하는게 맞아보이는데        
        //time = maxLv(최대레벨)
        //value = 100000(maximum 경험치)
        //이거 에디터에서 처리하기

        [Header("최대수치관련")]
        public int maxLevel = 10;//최대레벨
        public int maxExp = 1000;//최대 경험치//레벨에 따른 수치

        [Header("최대수치관련")]
        [Min(1)] public int minLevel = 1;
        [Min(0)] public int minExp = 0;

        //레벨커브
        public AnimationCurve levelCurve = new AnimationCurve(new Keyframe(1, 0), new Keyframe(10, value: 1000));

        public int MaxLevel { get => maxLevel; }
        public int MaxExp { get => maxExp; }
        public int MinLevel { get => minLevel; }
        public int MinExp { get => minExp; }



        /// <summary>
        /// 현재 레벨에 따른 커브값을 가져오는 함수(Float)
        /// </summary>
        /// <param name="curLevel">현재 레벨</param>
        /// <returns>값</returns>
        public float GetLevelCurveFloatValue(int curLevel)
        {
            return levelCurve.Evaluate(curLevel);
        }

        /// <summary>
        /// 현재 값에 따른 커브값을 가져오는 함수(Float)
        /// </summary>
        /// <param name="curValue">현재값</param>
        /// <returns>값</returns>
        public float GetLevelCurveFloatValue(float curValue)
        {
            return levelCurve.Evaluate(curValue);
        }

        /// <summary>
        /// 현재 레벨에 따른 커브값을 가져오는 함수(Int)
        /// </summary>
        /// <param name="curLevel">현재 레벨</param>
        /// <returns>값</returns>
        public int GetLevelCurveIntValue(int curLevel)
        {
            return (int)levelCurve.Evaluate(curLevel);
        }
    }
}
