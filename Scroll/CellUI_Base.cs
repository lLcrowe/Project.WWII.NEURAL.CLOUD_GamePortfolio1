using Doozy.Engine.UI;
using UnityEngine;
using UnityEngine.UI;

namespace lLCroweTool.UI.InfinityScroll
{
    /// <summary>
    /// 무한스크롤에서 돌아갈 셀 베이스
    /// </summary>
    public class CellUI_Base : MonoBehaviour
    {
        //무슨데이터를 받을것인가에 따라 모양이 달라질것이다
        //그럼 여기는 틀만 가지고 데이터는 분리해야됨
        public int index;

        public Image backGroundImage;
        private RectTransform rect;

        public Button button;//여기서 초기화하지않고 상속받은 클래스로부터 초기화하고 등록시킴

        private readonly static Vector2 targetPivot = new Vector2(0f, 1f);//CellUI용 피봇

        protected virtual void Awake()
        {
            backGroundImage = GetComponent<Image>();            
            
            rect = backGroundImage.rectTransform;
            rect.pivot = targetPivot;//안해나도 자동으로 되게하자
            rect.anchorMin = targetPivot;
            rect.anchorMax = targetPivot;
        }

        /// <summary>
        /// CellUI데이터 세팅
        /// </summary>
        /// <param name="cellData"></param>
        public virtual void SetData<T>(T cellData) where T : CellData
        {
            index = cellData.index;
            //button.SetLabelText($"{index}");
        }

        /// <summary>
        ///버튼을 누룰시 인덱스에 따라 작동될 기능//Awake에서 등록하기
        /// </summary>
        /// <param name="index">인덱스</param>
        protected virtual void ActionButtonEvent(int index)
        {
            //print($"{index}");
        }

        /// <summary>
        /// CellUI 크기와 위치초기화
        /// </summary>
        /// <param name="height">높이</param>
        /// <param name="width">넓이</param>
        /// <param name="pos">위치</param>
        public void InitCellUI(float height, float width, Vector2 pos)
        {   
            //다른거 체크
            if (rect.sizeDelta.y != height || rect.sizeDelta.x != width)
            {
                rect.sizeDelta = new Vector2(width, height);//크기 정하기
            }
            rect.anchoredPosition = pos;
        }

        /// <summary>
        /// 높이가져오기
        /// </summary>
        /// <returns>높이</returns>
        public float GetHeight() 
        {
            return rect.sizeDelta.y;
        }

        /// <summary>
        /// 넓이가져오기
        /// </summary>
        /// <returns>넓이</returns>
        public float GetWidth()
        {
            return rect.sizeDelta.x;
        }
    }
}