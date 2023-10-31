using UnityEngine;
using UnityEngine.Events;

namespace lLCroweTool.LevelSystem
{
    /// <summary>
    /// 레벨모듈 요소
    /// </summary>
    public class LevelModule_Element
    {
        //레벨정보
        public LevelInfo levelInfo;

        [Min(1)]
        public int curLv;//현재레벨
        public int totalExp;//현재 총합경험치//이걸올려야됨        

        //BarUI를 위한
        private int prevExperience;
        private int nextExperience;
        private int remainingExperience;//레벨업까지 남은 EXP

        //레벨업시 이벤트처리
        public UnityEvent levelUpEvent = new UnityEvent();
        public UnityEvent levelDownEvent = new UnityEvent();
               
        public int CurLv { get => curLv; }
        public int TotalExp { get => totalExp; }
        public int PrevExperience { get => prevExperience; }
        public int NextExperience { get => nextExperience; }
        public int RemainingExperience { get => remainingExperience; }

        /// <summary>
        /// 경험치량을 추가하는 함수
        /// </summary>
        /// <param name="addExp">추가할 경험치량</param>
        public void AddExp(int addExp)
        {
            totalExp += addExp;
            UpdateLevelModule(this);
        }

        /// <summary>
        /// 레벨업함수
        /// </summary>
        public void AddLevelUp()
        {
            int exp = GetNextLvForExp();
            AddExp(exp);
        }

        /// <summary>
        /// 레벨업에 필요한 니드데이터들을 가져오는 함수
        /// </summary>
        /// <returns>니드데이터</returns>
        public NeedDataStruct[] GetLevelUpNeedDataArray()
        {
            return levelInfo.GetAllLevelUpNeedData(curLv);
        }

        //이건 여기용도가 아닌거 같은데//이 함수는 다른 개체에서 레벨모듈을 레벨업할때 사용할때 필요함
        /// <summary>
        /// 레벨업이 가능한지 체크하는 함수
        /// </summary>
        /// <param name="labelBase">라벨데이터</param>
        /// <param name="amount">수량</param>
        /// <returns>가능한 여부</returns>
        public bool CheckLevelUp(NeedDataStruct[] needDataStructArray)
        {
            var allNeedDataArray = levelInfo.GetAllLevelUpNeedData(curLv);
            bool[] checkArray = new bool[allNeedDataArray.Length];

            //필요데이터들을 차례차례체크
            for (int i = 0; i < allNeedDataArray.Length; i++)
            {
                var needData = allNeedDataArray[i];

                //체크할데이터들
                for (int j = 0; j < needDataStructArray.Length; j++)
                {
                    var checkNeedData = needDataStructArray[j];

                    //많거나 동일하면 체크
                    if(needData.CheckMoreThanEqual(checkNeedData))
                    {
                        checkArray[i] = true;
                        break;
                    }
                }

                if (!checkArray[i])
                {
                    //하나라도 false면 니드한게 없으니 패스
                    return false;
                }

                //쿼리
                //var list = needDataStructArray.Where(x => x.CheckMoreThanEqual(needDataGroup) == false).Select(y => y).Count();
                //var query = from check in needDataStructArray
                //            where !needDataGroup.CheckMoreThanEqual(check)
                //            select check;
            }
            return true;
        }
        
        /// <summary>
        /// 다음레벨까지의 경험치를 가져오는 함수
        /// </summary>
        /// <returns>다음레벨까지의 경험치</returns>
        public int GetNextLvForExp()
        {
            return nextExperience - prevExperience;
        }

        /// <summary>
        /// 레벨모듈 업데이트
        /// </summary>
        /// <param name="levelModule">레벨모듈</param>
        private static void UpdateLevelModule(LevelModule_Element levelModule)
        {
            //레벨업체크
            if (levelModule.TotalExp - levelModule.prevExperience < 0)
            {
                levelModule.curLv--;
                levelModule.levelDownEvent.Invoke();
            }
            if (levelModule.nextExperience - levelModule.TotalExp < 0)
            {
                levelModule.curLv++;
                levelModule.levelUpEvent.Invoke();
            }            

            //최소치체크
            levelModule.curLv = levelModule.curLv < 0 ? 0 : levelModule.curLv;
            levelModule.totalExp = levelModule.TotalExp < 0 ? 0 : levelModule.TotalExp;
            levelModule.curLv = levelModule.curLv > levelModule.levelInfo.curveData.MaxLevel ? levelModule.levelInfo.curveData.MaxLevel : levelModule.curLv;

            //그래프사용하여 체크
            levelModule.prevExperience = levelModule.levelInfo.GetLevelCurveIntValue(levelModule.curLv);
            levelModule.nextExperience = levelModule.levelInfo.GetLevelCurveIntValue(levelModule.curLv + 1);
            levelModule.remainingExperience = (levelModule.nextExperience - levelModule.TotalExp);
        }


        /// <summary>
        /// 현재 레벨에 따른 커브값을 가져오는 함수(Float)
        /// </summary>
        /// <param name="curLevel">현재 레벨</param>
        /// <returns>값</returns>
        public float GetCurLevelCurveFloatValue()
        {
            return levelInfo.GetLevelCurveFloatValue(curLv);
        }

        /// <summary>
        /// 현재 레벨에 따른 커브값을 가져오는 함수(Int)
        /// </summary>
        /// <param name="curLevel">현재 레벨</param>
        /// <returns>값</returns>
        public int GetCurLevelCurveIntValue()
        {
            return levelInfo.GetLevelCurveIntValue(curLv);
        }

    }
}