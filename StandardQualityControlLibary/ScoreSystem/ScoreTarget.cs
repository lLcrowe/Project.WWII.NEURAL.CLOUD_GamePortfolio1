using UnityEngine;

namespace lLCroweTool.ScoreSystem
{
    public class ScoreTarget : MonoBehaviour
    {
        //점수를 올리고 기록할때 사용하는 컴포넌트
        //유닛오브젝트에 부착
        //히트와 어택에서 업데이트시켜줌

        //20230612
        //스탯시스템을 고쳐났더니 여기도 귀찬네
        //제작완료
        [SerializeField]private ScoreBible scoreBible = new ScoreBible();

        public ScoreBible ScoreBible { get => scoreBible; }

        /// <summary>
        /// 점수타입들을 기록하여 시작할때 초기화하는 용도에 사용됨
        /// </summary>
        private static ScoreType[] scoreTypeList_static = lLcroweUtil.GetEnumDefineData<ScoreType>().ToArray();

        private void Awake()
        {
            for (int i = 0; i < scoreTypeList_static.Length; i++)
            {
                scoreBible.Add(scoreTypeList_static[i], 0);
            }
        }

        /// <summary>
        /// 등록된 점수를 가져오는 함수
        /// </summary>
        /// <param name="scoreType">가져올 점수타입</param>
        /// <returns>가져온 점수</returns>
        public int GetScore(ScoreType scoreType)
        {
            //20230612
            //이거 0나왔나?
            ScoreBible.TryGetValue(scoreType, out int scoreValue);
            return scoreValue;
        }
    }
}