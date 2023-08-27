using lLCroweTool.UI.InfinityScroll.Scroll;
using System.Collections.Generic;
using UnityEngine;

namespace lLCroweTool.Achievement
{
    public class AchievementScroll : MonoBehaviour
    {
        //스크롤에 데이터를 주입시키기 위한 종류
        private void Start()
        {
            if (TryGetComponent(out CustomInfinityScroll scroll))
            {
                //임시데이터처리//외부에서 처리하는것            
                var list = AchievementManager.Instance.achievementBible;
                List<AchievementData> valueList = new List<AchievementData>(list.Values);
                scroll.Init(valueList);
            }
        }
    }
}