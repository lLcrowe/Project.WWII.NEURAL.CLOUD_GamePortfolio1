using UnityEngine;

namespace lLCroweTool.UI.Scene
{
    [CreateAssetMenu(fileName = "New TipDefaultData", menuName = "lLcroweTool/New GameSceneTipData")]
    public class GameSceneTipObjectScript : ScriptableObject
    {
        /// <summary>
        /// 게임씬이 진행할시 팁들에 대한 내용
        /// </summary>
        [Header("게임씬에 뜰 팁에 대해 적어놓는곳.")]
        public string[] tips;
    }
}