using lLCroweTool.UI.Bar;
using TMPro;
using UnityEngine;

namespace lLCroweTool.LevelSystem
{
    public class LevelModuleUI : MonoBehaviour
    {
        //레벨모듈UI
        //레벨모듈을 UI로 보여줄 목적으로 만들어짐

        //타겟팅할 레벨모듈
        public LevelModule_Element targetLevelModule;

        //UI컴포넌트
        public UIBar_Base bar_Base;

        public TextMeshProUGUI curLvText;//현재레벨
        public TextMeshProUGUI nextLvText;//다음레벨

        public TextMeshProUGUI curField;
        public TextMeshProUGUI remainingField;

        

        /// <summary>
        /// 레벨UI를 세팅하는 함수
        /// </summary>
        /// <param name="levelModule">타겟팅할 레벨모듈</param>
        public void SetLevelModuleUI(LevelModule_Element levelModule)
        {
            targetLevelModule = levelModule;
        }

        /// <summary>
        /// UI갱신해주는 함수
        /// </summary>
        /// <param name="levelModuleUI">레벨업모듈UI</param>
        public static void UpdateUI(LevelModuleUI levelModuleUI)
        {
            LevelModule_Element levelModule = levelModuleUI.targetLevelModule;
            LevelInfo levelInfo = levelModule.levelInfo;
            string format = levelModule.CurLv >= levelInfo.curveData.MaxLevel ? levelInfo.maxLevelText : levelInfo.levelText;

            //레벨체크
            levelModuleUI.curLvText.text = levelModule.CurLv + levelInfo.levelText;
            levelModuleUI.nextLvText.text = (levelModule.CurLv + 1) + format;

            //바
            levelModuleUI.bar_Base.InitUIBar(levelModule.PrevExperience, levelModule.NextExperience, levelModule.TotalExp);

            levelModuleUI.curField.text = (levelModule.TotalExp - levelModule.PrevExperience).ToString();
            levelModuleUI.remainingField.text = (levelModule.NextExperience - levelModule.TotalExp).ToString();
        }


    }
}