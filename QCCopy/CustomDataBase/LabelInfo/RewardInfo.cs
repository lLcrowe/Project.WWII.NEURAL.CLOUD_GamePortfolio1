using UnityEngine;

namespace lLCroweTool.DataBase
{
    //다른곳에서 연동시켜서 가져와서 나누어 주는 구역
    //가변데이터는 아니지만 그렇다고 따로 들고다니기에는 작은 데이터라 스크립터블은 체크해봐야됨

    //csv에서 뒤에서 계속 열거해서 만듬
    //아이템이름을 특정룰에 따라 계속만듬

    //최종적으로는 CSV에서는 아이템 수량 아이템 수량 식으로 넘어옴 

    [CreateAssetMenu(fileName = "New RewardInfo", menuName = "lLcroweTool/New RewardInfo")]    
    public class RewardInfo : LabelBase
    {        
        public string[] itemNameArray = new string[0];//아이디
        public int[] amountArray = new int[0];

        public override LabelBaseDataType GetLabelDataType()
        {
            return LabelBaseDataType.RewardData;
        }
    }
}
