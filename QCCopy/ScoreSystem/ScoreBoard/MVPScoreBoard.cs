using lLCroweTool.Dictionary;
using lLCroweTool.ScoreSystem;
using lLCroweTool.ScoreSystem.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace lLCroweTool.ScoreSystem.UI
{
    public class MVPScoreBoard : MonoBehaviour
    {
        //게임마다 있던 MVP 점수 시스템을 제작

        //1. 각각 기여도 점수를 전부 합산
        //2. 총 점수가 가장 높은 사람을 MVP로 선정.
        //3. 기여한 점수중 높은 점수 3가지 보여준다

        //점수에 대한 보너스점수를 지정//커스텀인스팩터로 처리
        //이거 CSV로 처리//스코어보너스 점수
        [System.Serializable]
        public class ScoreBonusBible : CustomDictionary<ScoreType, int> { }        
        [SerializeField]private ScoreBonusBible scoreBonusBible = new ScoreBonusBible();
        
        public ScoreBonusBible ScoreBonusInfoBible { get => scoreBonusBible; }

        //최고점수를 얻었을때 
        //무엇을 보여줄건지 체크
        //참고게임//배필5//히오스//뉴럴 클라우드

        //여기이름 바꿔야 할듯

        //캐릭터이름
        //MVP타이틀이름

        //이것도 나중에 처리
        //엑셀파일 만들어야됨
        public struct ScoreTypeBonusInfo
        {
            public Color colorMVPBackGroundColor;//MVP별칭뒤의 백그라운드 컬러//시리얼라이징해서 보니까 RGBA로 보여짐
            public int bunusScale;//점수에 대한 보너스 점수
            public string scoreMVPAlias;//이 점수로 MVP일시 별명
            public string scoreMVPDescription;//이 점수로 MVP일시 별명 사유
        }       
        

        public TextMeshProUGUI mvpAliasText;//MVP 별명
        public TextMeshProUGUI mvpAliasReasonText;//MVP 별명에 대한 이유
        public TextMeshProUGUI mvpUnitNameText;//MVP유닛이름

        public Button scoreShowButton;//피해현황보여주기
        public Transform rewordSlotPos;//보상슬롯 위치


        //정렬시킬 점수리스트
        private List<MVPScoreData> mVPScoreInfoList = new List<MVPScoreData>();

        /// <summary>
        /// 커스텀딕셔너리와 같이 사용하는 점수를 저장하는 구조체
        /// </summary>
        public struct MVPScoreData
        {
            //유닛
            public UnitObject_Base unitObject;

            //해당타입의 종합스코어//비교하기 위해존재
            public int totalScore;

            //해당유닛에 대한 점수를 비교하기 위해존재//점수타겟에 있는 모든점수를 집어넣음
            public List<ScoreTargetData> scoreTargetDataList;
        }

        /// <summary>
        /// 점수타입끼리 비교를 위한 구조체
        /// </summary>
        public struct ScoreTargetData
        {
            public ScoreType scoreType;
            public int scoreTypeForScore;
        }


        /// <summary>
        /// 점수데이터들을 등록하는 함수
        /// </summary>
        /// <param name="unitObject">유닛오브젝트</param>
        /// <param name="scoreTarget">스코어타겟</param>
        public void RegisterScoreTarget(UnitObject_Base unitObject, ScoreTarget scoreTarget)
        {
            var scoreBible = scoreTarget.ScoreBible;

            //등록
            int totalScore = 0;
            MVPScoreData mVPScoreInfo = new MVPScoreData();
            mVPScoreInfo.unitObject = unitObject;
            mVPScoreInfo.scoreTargetDataList = new List<ScoreTargetData>();

            foreach (var item in scoreBible)
            {
                ScoreTargetData scoreTargetInfo = new ScoreTargetData();
                var scoreType = item.Key;
                var score = item.Value;

                //여기점수는 그대로 들어가서 기록해서 보여주는 용도
                scoreTargetInfo.scoreType = scoreType;
                scoreTargetInfo.scoreTypeForScore = score;
                mVPScoreInfo.scoreTargetDataList.Add(scoreTargetInfo);

                //여기점수는 보너스점수를 주어 전체적인 점수내용을 주는 용도
                totalScore += CalBonusScore(scoreType, score);
            }

            //여기서 스코어타겟 점수는 정렬시켜줌
            mVPScoreInfo.scoreTargetDataList.Sort(new ValueSort());
            mVPScoreInfo.totalScore = totalScore;
            mVPScoreInfoList.Add(mVPScoreInfo);//등록
        }

        /// <summary>
        /// MVP를 보여주는 함수
        /// </summary>
        /// <returns>MVP가 된 유닛오브젝트</returns>
        public UnitObject_Base ShowMVP()
        {
            //총점을 정렬
            mVPScoreInfoList.Sort(new ValueSort());

            //UI에 정보배치
            //MVP 배치


            //어떤정보를 보여줘야될지 체크해보기

            foreach (var item in mVPScoreInfoList)
            {
                var unit = item.unitObject;
                
            }



            var highScoreUnit = mVPScoreInfoList[0].unitObject;
            //mvpAliasText.text = $"MVP {}";//MVP명칭
            //mvpAliasReasonText//별명에 대한 이유
            mvpUnitNameText.text = $"{highScoreUnit.unitObjectInfo.labelNameOrTitle}";//유닛이름
            

            return highScoreUnit;
        }

        /// <summary>
        /// 점수판 리셋//등록된 점수타겟들을 리셋
        /// </summary>
        public void ResetScoreBoard()
        {
            mVPScoreInfoList.Clear();
        }

        /// <summary>
        /// 들어온 점수들을 지정된 보너스값과 곱하여 돌려주는 함수
        /// </summary>
        /// <param name="scoreType">점수타입</param>
        /// <param name="score">점수</param>
        /// <returns>계산될 보너스점수</returns>
        private int CalBonusScore(ScoreType scoreType, int score)
        {
            scoreBonusBible.TryGetValue(scoreType, out int bonusScoreScale);
            score *= bonusScoreScale;

            //print($"{scoreType} Value:{score} = {score / bonusScoreScale}x{bonusScoreScale}");
            return score;
        }

    }

}


#if UNITY_EDITOR

namespace lLCroweTool.QC.EditorOnly
{
    [UnityEditor.CustomEditor(typeof(MVPScoreBoard))]
    public class MVPScoreBoardEditor : UnityEditor.Editor
    {
        private MVPScoreBoard targetObject;
        private ScoreType[] scoreTypeArray = lLcroweUtil.GetEnumDefineData<ScoreType>().ToArray();

        private void OnEnable()
        {
            targetObject = target as MVPScoreBoard;
            var bible = targetObject.ScoreBonusInfoBible;
            if (bible.Count == scoreTypeArray.Length)
            {
                return;
            }

            //동기화//기존거 가진체로 위치만 변경하여 주기
            Dictionary<ScoreType, int> newBible = new Dictionary<ScoreType, int>(bible);
            bible.Clear();


            for (int i = 0; i < scoreTypeArray.Length; i++)
            {
                var scoreType = scoreTypeArray[i];
                if (newBible.ContainsKey(scoreType))
                {
                    bible.Add(scoreType, newBible[scoreType]);
                    continue;
                }
                //기본값은 1
                bible.Add(scoreType, 1);
            }
        }
    }
}

#endif


