
using UnityEngine;
using lLCroweTool.LevelSystem;
using lLCroweTool.DataBase;

namespace lLCroweTool.Ability.Level
{
    //일단 기본 싸이클은 돌아가게 만들고 레벨시스템과 유닛스탯시스템을 통합하여 흔히 보이는 게임캐릭터카드 시스템을 제작해야겠따


    [System.Serializable]
    public class LevelUnitData
    {
        public LevelInfo levelInfo;
        public int curLevel;

        public UnitObjectInfo unitInfo;


    }
}
