using lLCroweTool.Dictionary;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace lLCroweTool.ScoreSystem.UI
{   
    public class ScoreBoard : MonoBehaviour
    {
        //점수판
        //유닛들의 점수를 보여줄 보드
        //점수들을 기록한후 정렬시켜주는 역할을 가짐

        //여러점수들을 여기서 가지고 스코어에다 뿌려줘야됨
        //그래야지 최대점수를 체크할수 있다.
        //그러면 스코어들을체크할때 마다 각 스코어타입들에 대한 값들을 가지고 있어야됨
        public UnitScoreUI unitScoreUIPrefab;//프리팹
        private List<UnitScoreUI> unitScoreUIList = new();
        public Transform targetContentTr;//소환될위치        

        //특정점수 정렬버튼들(대미지 히트 등등)
        public Button damageSortButton;
        public Button hitSortButton;
        public Button hillSortButton;

        public Button closeButton;



        //점수데이터를 캐싱하는게 필요      
        //묶은상태에서 정렬시켜버리는게 좋아보임

        //스코어타입//스코어정보(유닛, 스코어타입의 점수(정렬용), 점수타겟)//최종연산때는 정렬        
        public class ScoreInfoBible : CustomDictionary<ScoreType, List<ScoreInfo>> { }
        public ScoreInfoBible scoreInfoBible = new();
        //최대점수
        public int maxScoreValue;

        /// <summary>
        /// 커스텀딕셔너리와 같이 사용하는 점수를 저장하는 구역
        /// </summary>
        public struct ScoreInfo
        {
            //유닛
            public UnitObject_Base unitObject;

            //해당타입의 스코어//비교하기 위해존재
            public int score;

            //점수
            public ScoreTarget scoreTarget;
        }

        private void Awake()
        {
            //버튼들에 이벤트추가
            damageSortButton.onClick.AddListener(() => SortScoreType(ScoreType.GiveDamage));
            hitSortButton.onClick.AddListener(() => SortScoreType(ScoreType.TakenDamage));
            hillSortButton.onClick.AddListener(() => SortScoreType(ScoreType.Hill));

            closeButton.onClick.AddListener(() => this.SetActive(false));
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
            foreach (var item in scoreBible)
            {
                if (!scoreInfoBible.ContainsKey(item.Key))
                {
                    var scoreInfoList = new List<ScoreInfo>();

                    scoreInfoBible.Add(item.Key, scoreInfoList);
                }

                var score = item.Value;

                //어차피 다 초기화되니까 그냥 넣어도됨
                ScoreInfo scoreInfo = new ScoreInfo();
                scoreInfo.unitObject = unitObject;
                scoreInfo.score = score;
                scoreInfo.scoreTarget = scoreTarget;
                scoreInfoBible[item.Key].Add(scoreInfo);

                //최대수치 갱신
                if (maxScoreValue < score)
                {
                    maxScoreValue = score;
                }
            }
        }

        /// <summary>
        /// 점수판 리셋//등록된 점수타겟들을 리셋
        /// </summary>
        public void ResetScoreBoard()
        {
            ResetUnitScoreUI();
            scoreInfoBible.Clear();
            maxScoreValue = 0;
        }

        /// <summary>
        /// UI초기화
        /// </summary>
        private void ResetUnitScoreUI()
        {
            //UI초기화
            for (int i = 0; i < unitScoreUIList.Count; i++)
            {
                unitScoreUIList[i].SetActive(false);
            }
            unitScoreUIList.Clear();
        }

        /// <summary>
        /// 등록된 점수들을 정렬해주는 함수(DESC)
        /// </summary>
        /// <param name="sortScoreType">정렬할 점수타입</param>
        public void SortScoreType(ScoreType sortScoreType)
        {
            ResetUnitScoreUI();

            //정렬
            if (scoreInfoBible.ContainsKey(sortScoreType))
            {
                scoreInfoBible[sortScoreType].Sort(new ValueSort(false));
            }
            
            foreach (var item in scoreInfoBible[sortScoreType])
            {
                UnitScoreUI unitScoreUI = ObjectPoolManager.Instance.RequestDynamicComponentObject(unitScoreUIPrefab);
                unitScoreUI.InitTrObjPrefab(targetContentTr, targetContentTr);


                var unitData = item.unitObject.unitObjectInfo;

                unitScoreUI.InitUnitScoreUI(unitData.classIcon, unitData.icon, maxScoreValue);
                unitScoreUI.damageBar.SetCurValue(item.scoreTarget.GetScore(ScoreType.GiveDamage));
                unitScoreUI.takenDamageBar.SetCurValue(item.scoreTarget.GetScore(ScoreType.TakenDamage));
                unitScoreUI.hillBar.SetCurValue(item.scoreTarget.GetScore(ScoreType.Hill));

                float mainA = 1f;
                float sizeA = 0.3f;


                //색깔 강조처리
                switch (sortScoreType)
                {
                    case ScoreType.KillCount:
                        break;
                    case ScoreType.GiveDamage:
                        unitScoreUI.damageBar.SetColorAlpha(mainA);
                        unitScoreUI.takenDamageBar.SetColorAlpha(sizeA);
                        unitScoreUI.hillBar.SetColorAlpha(sizeA);
                        break;
                    case ScoreType.TakenDamage:
                        unitScoreUI.damageBar.SetColorAlpha(sizeA);
                        unitScoreUI.takenDamageBar.SetColorAlpha(mainA);
                        unitScoreUI.hillBar.SetColorAlpha(sizeA);
                        break;
                    case ScoreType.HitCount:
                        break;
                    case ScoreType.AttackCount:
                        break;
                    case ScoreType.Hill:
                        unitScoreUI.damageBar.SetColorAlpha(sizeA);
                        unitScoreUI.takenDamageBar.SetColorAlpha(sizeA);
                        unitScoreUI.hillBar.SetColorAlpha(mainA);
                        break;
                }
                unitScoreUIList.Add(unitScoreUI);
            }
        }
    }
}