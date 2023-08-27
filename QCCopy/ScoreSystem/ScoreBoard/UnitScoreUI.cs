using lLCroweTool.UI.Bar;
using UnityEngine;
using UnityEngine.UI;

namespace lLCroweTool.ScoreSystem.UI
{
    public class UnitScoreUI : MonoBehaviour
    {
        //유닛에 대한 점수를 시각적으로 보여주는 역할을 맏음
        //점수판(ScoreBoard)에서 사용

        //아이콘들
        public Image classIcon;
        public Image unitIcon;

        //UI바
        public UIBar_Base damageBar;
        public UIBar_Base takenDamageBar;
        public UIBar_Base hillBar;

        /// <summary>
        /// 유닛스코어 UI초기화
        /// </summary>
        /// <param name="classIconSprite">클래스아이콘</param>
        /// <param name="unitIconSprite">유닛아이콘</param>
        /// <param name="maxValue">최대수치</param>
        public void InitUnitScoreUI(Sprite classIconSprite, Sprite unitIconSprite, int maxValue)
        {   
            classIcon.sprite = classIconSprite;
            unitIcon.sprite = unitIconSprite;

            damageBar.InitUIBar(0, maxValue, 0);
            takenDamageBar.InitUIBar(0, maxValue, 0);
            hillBar.InitUIBar(0, maxValue, 0);
        }

    }
}