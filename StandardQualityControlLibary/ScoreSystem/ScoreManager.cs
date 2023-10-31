using lLCroweTool.Dictionary;
using UnityEngine;

namespace lLCroweTool.ScoreSystem
{
    /// <summary>
    /// 점수타입을 정의하는 이넘
    /// </summary>
    public enum ScoreType
    {
        //디폴트
        KillCount,           //킬수
        GiveDamage,     //대미지량//일반공격//스킬공격
        TakenDamage,    //받은 모든 대미지
        HitCount,       //히트수
        AttackCount,    //공격횟수
        Hill,           //힐량
    }

    /// <summary>
    /// 점수 사전//점수타입//점수량
    /// </summary>
    [System.Serializable]
    public class ScoreBible : CustomDictionary<ScoreType, int> { }

    public static class ScoreManager_Extend
    {
        /// <summary>
        /// 스코어타겟의 특정 점수를 올리는 함수
        /// </summary>
        public static void AddScore(this ScoreTarget scoreTarget,ScoreType scoreType, int value)
        {
            var scoreBible = scoreTarget.ScoreBible;
            if (!scoreBible.ContainsKey(scoreType))
            {
                scoreBible.Add(scoreType, 0);
            }
            scoreBible[scoreType] += value;
            scoreBible.SyncDictionaryToList();


            //토탈점수 올림
            ScoreManager.Instance.AddTotalScore(scoreType, value);
        }
    }


    public class ScoreManager : MonoBehaviour
    {
        private static ScoreManager instance;
        public static ScoreManager Instance
        {
            get
            {
                if (ReferenceEquals(instance, null))
                {
                    instance = FindObjectOfType<ScoreManager>();
                    //if (ReferenceEquals(instance, null))
                    if (ReferenceEquals(instance, null))
                    {
                        GameObject gameObject = new GameObject();
                        instance = gameObject.AddComponent<ScoreManager>();
                        gameObject.name = "-=ScoreManager=-";
                    }
                }
                return instance;
            }
        }

        
      
        //전체 플레이어 점수를 뜻함
        [Header("총합점수")]
        public ScoreBible totalScoreBible = new ScoreBible();

        private void Awake()
        {
            instance = this;
        }

        /// <summary>
        /// 총합 플레이어 점수 올리는 함수 
        /// </summary>
        public void AddTotalScore(ScoreType scoreType, int value)
        {
            if (!totalScoreBible.ContainsKey(scoreType))
            {
                totalScoreBible.Add(scoreType, 0);
            }
            totalScoreBible[scoreType] += value;
            totalScoreBible.SyncDictionaryToList();
        }
    }
}