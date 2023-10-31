using lLCroweTool.DataBase;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace lLCroweTool.UI.PullCard
{
    public class PullCardBoardUI : MonoBehaviour
    {
        //카드를 뽑은 후 그것에 대한 내용을 보여주기 위한 패널
        public List<UnitObjectInfo> objectInfoList = new List<UnitObjectInfo>();

        public Transform cardPos;
        public PullCardUI pullCardUIPrefab;
        public Button confirmButton;

        public void Show(UnitObjectInfo unitObjectInfo, System.Action confirmAction)
        {
            this.SetActive(true);
            var targetObject = ObjectPoolManager.Instance.RequestDynamicComponentObject(pullCardUIPrefab);
            targetObject.InitPullCardUI(unitObjectInfo);
            targetObject.InitTrObjPrefab(cardPos, cardPos);

            confirmButton.onClick.AddListener(()=> 
            {
                confirmAction?.Invoke();
                targetObject.SetActive(false);
                this.SetActive(false);
                confirmButton.onClick.RemoveAllListeners();
            });
        }
    }
}