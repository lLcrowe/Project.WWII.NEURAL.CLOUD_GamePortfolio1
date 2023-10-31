namespace lLCroweTool.Ability
{
    [System.Serializable]
    public class StatusData
    {
        //스탯데이터
        //스탯info로 안 만든이유와 유닛정보하고 별개로 두는 이유는
        //다른모듈이라 별개로 둠//분리용
        public UnitStatusValue[] unitStatusArray = new UnitStatusValue[0];
        public UnitStateBible infoUnitStateApplyBible = new UnitStateBible();//해당상태를 적용가능한지 여부//검색하기 빠르게 하기위해 딕셔너리처리
    }
}