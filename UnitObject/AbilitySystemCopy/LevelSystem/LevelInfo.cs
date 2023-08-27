using lLCroweTool.Dictionary;
using lLCroweTool.QC.Curve;
using System.Collections.Generic;

namespace lLCroweTool.LevelSystem
{
    /// <summary>
    /// 레벨 정보데이터
    /// </summary>
    public class LevelInfo : LabelBase
    {
        //작동순서가 위에서 아래로 아래가 있으면 덮어쓰기 진행//두번째부터 메뉴얼
        public LabelBase needExpTargetData;//타겟데이터//레벨업시 필요한
        public LevelCurveData curveData = new LevelCurveData();

        //결국 필요데이터//수치//중복안되야되면//레벨//덮어쓸데이터
        [System.Serializable]
        public class AdditionalLvUpBible : CustomDictionary<int, AdditionalLevelUpData> { }
        public AdditionalLvUpBible additionalLvUpBible = new AdditionalLvUpBible();

        //포맷관련
        public string levelText = "LV";
        public string maxLevelText = "LV";

        //최대랩과 경험치를 위에 쓰지않고 애니메이션커브만 사용하기 위한 코드

        //public int MaxLevel => (int)levelCurve.keys[levelCurve.keys.Length - 1].time;
        //public int MaxExp => (int)levelCurve.keys[levelCurve.keys.Length - 1].value;

        private static List<NeedDataStruct> levelUpNeedDataStructList = new List<NeedDataStruct>(10);

        /// <summary>
        /// 모든 레벨업 필요데이터를 가져오는 함수
        /// </summary>
        /// <param name="level">타겟팅할 레벨</param>
        /// <returns>레벨업데이터</returns>
        public NeedDataStruct[] GetAllLevelUpNeedData(int level)
        {
            levelUpNeedDataStructList.Clear();
            //현재 레벨에 대한 추가적인수치가 있는지 체크
            if (additionalLvUpBible.TryGetValue(level, out AdditionalLevelUpData additionalLevelUpData))
            {
                for (int i = 0; i < additionalLevelUpData.needDataGroup.needDataArray.Length; i++)
                {
                    var needData = additionalLevelUpData.needDataGroup;
                    NeedDataStruct tempLevelUpNeedDataStruct = new NeedDataStruct();
                    tempLevelUpNeedDataStruct.needTargetData = needData.needDataArray[i];
                    tempLevelUpNeedDataStruct.needAmount = needData.needAmountArray[i];
                    levelUpNeedDataStructList.Add(tempLevelUpNeedDataStruct);
                }

                if (additionalLevelUpData.isOverLap)
                {
                    return levelUpNeedDataStructList.ToArray();
                }
            }

            //기존 니드레벨
            NeedDataStruct levelUpNeedDataStruct = new NeedDataStruct();
            levelUpNeedDataStruct.needTargetData = needExpTargetData;
            levelUpNeedDataStruct.needAmount = GetLevelCurveIntValue(level);
            levelUpNeedDataStructList.Add(levelUpNeedDataStruct);

            return levelUpNeedDataStructList.ToArray();
        }

        /// <summary>
        /// 현재 레벨에 따른 커브값을 가져오는 함수(Float)
        /// </summary>
        /// <param name="curLevel">현재 레벨</param>
        /// <returns>값</returns>
        public float GetLevelCurveFloatValue(int curLevel)
        {
            return curveData.GetLevelCurveFloatValue(curLevel);
        }

        /// <summary>
        /// 현재 레벨에 따른 커브값을 가져오는 함수(Int)
        /// </summary>
        /// <param name="curLevel">현재 레벨</param>
        /// <returns>값</returns>
        public int GetLevelCurveIntValue(int curLevel)
        {
            return curveData.GetLevelCurveIntValue(curLevel);
        }

        public override LabelBaseDataType GetLabelDataType()
        {
            return LabelBaseDataType.LevelData;
        }
    }

    /// <summary>
    /// 레벨업시 필요데이터//분리시키기
    /// </summary>
    [System.Serializable]
    public class AdditionalLevelUpData
    {
        public int targetOverLapLevel;//덮어쓸 레벨
        public bool isOverLap;//덮어쓸건지//추가로 요구할건지 체크

        //여기다 커브를 집어넣는것도 하나의 방식//=>int[] float[]일시 커브를 빌려서 채울수 있게해주는 형태를 지원
        public NeedDataGroup needDataGroup = new NeedDataGroup();
    }
}
