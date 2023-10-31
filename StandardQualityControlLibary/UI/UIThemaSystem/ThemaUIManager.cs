using lLCroweTool.DataBase;
using lLCroweTool.Dictionary;
using lLCroweTool.LogSystem;
using lLCroweTool.Singleton;
using System.Collections.Generic;
using UnityEngine;

namespace lLCroweTool.UI.UIThema
{
    public class ThemaUIManager : MonoBehaviourSingleton<ThemaUIManager>
    {
        //현재타겟팅된 테마정보
        [SerializeField] 
        private UIThemaInfo currentUIThemaInfo;

        //현재타겟팅된 아이콘정보
        [SerializeField]
        private IconPresetInfo currentTargetIconPresetInfo;

        [System.Serializable]
        public class UIThemaBible : CustomDictionary<string, UIThemaInfo> { }
        public UIThemaBible uIThemaInfoBible = new UIThemaBible();

        //테마키//아이콘키,아이콘
        [System.Serializable]
        public class IconBible : CustomDictionary<string, Dictionary<string, Sprite>> { }
        public IconBible iconBible = new IconBible();

        public static string logKey = "UIThemaKey";

        protected override void Awake()
        {
            base.Awake();
            //업적 임포트
            InitThemaUIManager(DataBaseManager.Instance.dataBaseInfo);
        }

        /// <summary>
        /// UI테마매니저 초기화
        /// </summary>
        /// <param name="dataBaseInfo">데이터베이스 정보</param>
        public void InitThemaUIManager(DataBaseInfo dataBaseInfo)
        {
            //테마들 등록후 첫번쨰 테마가 기본테마로 처리
            //그다음 아이콘바이블을 초기화처리

            //UI테마등록
            uIThemaInfoBible.Clear();
            foreach (var item in dataBaseInfo.uIThemaInfoList)
            {
                uIThemaInfoBible.TryAdd(item.labelID, item);
            }

            //아이콘프리셋등록
            iconBible.Clear();
            foreach (var item in dataBaseInfo.iconPresetInfoList)
            {
                var dataList = item.iconDataList;
                var bible = new Dictionary<string, Sprite>();

                foreach (var data in dataList)
                {
                    bible.TryAdd(data.iconName,data.iconSprite);
                }

                iconBible.TryAdd(item.labelID, bible);
            }
        }

        /// <summary>
        /// 모든 UI테마를 초기화하는 함수
        /// </summary>
        /// <param name="uIThemaInfo"></param>
        public void InitAllThemaUI(UIThemaInfo uIThemaInfo = null)
        {
            if (uIThemaInfo == null)
            {
                uIThemaInfo = currentUIThemaInfo;
            }

            var array = FindObjectsOfType<ThemaUI>();


            for (int i = 0; i < array.Length; i++)
            {
                if (!LogManager.CheckRegister(logKey))
                {
                    LogManager.Register(logKey, "UIThema",true, true);
                }
                LogManager.Log(logKey, $"{i} {array[i].name}");
                array[i].InitThemaUI(uIThemaInfo);
            }
        }

        /// <summary>
        /// 모든 아이콘테마을 초기화 하는 함수
        /// </summary>
        /// <param name="iconPresetInfo">아이콘프리셋정보</param>
        public void InitAllThemaIcon(IconPresetInfo iconPresetInfo)
        {
            if (iconPresetInfo == null)
            {
                iconPresetInfo = currentTargetIconPresetInfo;
            }

            var array = FindObjectsOfType<ThemaIcon>();


            for (int i = 0; i < array.Length; i++)
            {
                if (!LogManager.CheckRegister(logKey))
                {
                    LogManager.Register(logKey, "ThemaIcon", true, true);
                }
                LogManager.Log(logKey, $"{i} {array[i].name}");

                var temp = array[i];
                string id =  array[i].IconID;
                var sprite = RequestIcon(id);
                temp.SetImage(sprite);
            }
        }

        /// <summary>
        /// UI테마에 맞는 아이콘을 요청하는 함수
        /// </summary>
        /// <param name="iconID">아이콘ID</param>
        /// <returns>아이콘 스프라이트</returns>
        public Sprite RequestIcon(string iconID)
        {
            //UI테마의 아이디에 종속되있음
            iconBible.TryGetValue(currentTargetIconPresetInfo.labelID, out var data);
            data.TryGetValue(iconID, out var sprite);
            return sprite;
        }

        /// <summary>
        /// 타겟팅할 테마UI를 초기화하는 함수
        /// </summary>
        /// <param name="themaUI">UI테마</param>
        public void InitTargetThemaUI(ThemaUI themaUI)
        {
            themaUI.InitThemaUI(currentUIThemaInfo);
        }
    }
}


