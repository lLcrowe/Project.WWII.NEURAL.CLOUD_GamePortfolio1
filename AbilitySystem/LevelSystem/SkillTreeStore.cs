using System.Collections;
using UnityEngine;

namespace lLCroweTool.LevelSystem.SkillTreeStore
{
    public class SkillTreeStore : MonoBehaviour
    {
        //레벨업시 올라가는 코스트//아마뉴럴에서는 사용안할수도
        //레벨업모듈하고 같은 게임오브젝트에 할당시킴


        
        public int cost;//스킬트리라든가에 쓸 코스트//이건 모듈분리


        private void Awake()
        {
            //이벤트만 집어넣음
            if (TryGetComponent(out LevelModule_Element levelModule))
            {
                levelModule.levelUpEvent.AddListener(()=>AddCost());
                levelModule.levelDownEvent.AddListener(() => SubCost());
            }
            
        }

        public void AddCost(int addAmount = 1)
        {
            cost += addAmount;
        }

        public void SubCost(int addAmount = 1)
        {
            cost -= addAmount;
        }
    }
}