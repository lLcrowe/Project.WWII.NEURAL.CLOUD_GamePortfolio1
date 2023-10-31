using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Doozy.Engine.UI;

namespace lLCroweTool.ToolTipSystem
{
    /// <summary>
    /// 툴팁자동 생성 스크립트
    /// 툴팁을 자동으로 세팅해주고 원하는 스크립트에다 붙여넣어 작동시키는 방식
    /// 현 클래스는 백그라운드에
    /// </summary>
    
    public class ToolTipUiView : MonoBehaviour
    {
        /// <summary>
        /// ContentSizeFitter
        /// 가로크기와 세로크기를 설정하는 옵션을 가지고 있음.
        /// 3종류의 옵선이 있다.
        /// 1. 해당방향을 UI크기를 직접 맞추거나 고정된 크기를 사용해야하는 경우 사용하는 옵션
        /// 2. 레이아웃 요소의 최소크기를 기준으로 UI크기를 맞추는 옵션인데 이옵셥은 단독으로 사용되는 경우는 별로 없고 주로 레이어 그룹계열의 컴포넌트와 함꼐 사용됨 
        /// 3. 기본적인 콘텐츠의 크기에 따라 UI의 크기를 맞춘 옵션.
        /// 
        /// 피벗의 중심으로 작동됨
        /// </summary>

        //가로로 사용할지 세로로 사용할지
        //public enum CteateToolTipType
        //{
        //    Vertical,
        //    Horizon,
        //}

        //가로와 세로등으로 이용하고 싶으면 텍스트와이미지가 자식으로 있는 오브젝트에 
        //ver&hor Layout Group Component 를 추가하여 작업할것

        //작동방식(함수)
        //1. MoveToolTip
        //2. ClearText
        //3. ShowText

        //텍스트작업할떄 메뉴얼
        //http://digitalnativestudios.com/textmeshpro/docs/rich-text/
        //많이 사용하는거 따로 빼온것
        //<align="right">Right
        //<align="center">Center
        //<align="left"> Left        
        //<color="red">Red 
        //<color=#005500>Dark Green 
        //<#0000FF>Blue 
        //<color=#FF000088>Semitransparent Red
        //<alpha=#FF>FF 
        //<alpha=#CC>CC 
        //<alpha=#AA>AA 
        //<alpha=#88>88 
        //<alpha=#66>66 
        //<alpha=#44>44 
        //<alpha=#22>22 
        //<alpha=#00>00
        //<size=100%>Echo 
        //<size=80%>Echo 
        //<size=60%>Echo 
        //<size=40%>Echo 
        //<size=20%>Echo

        //사용할시 예시
        //voyageMap.spaceVoyageMapToolTip.ShowText("<align=\"center\">알림", "갈수있습니다.", null, "항해맵이정표에 등록되었습니다.", true);


        [Header("움직이는 툴팁인가")]
        public bool isUseMovingToolTip = false;
        [Header("오프셋")]
        public Vector3 offsetPos;//오프셋 위치
        [Space]
        [Header("컴포넌트")]
        public Image toolTipImage;//툴팁 이이콘 이미지//자식에 있음
        public TextMeshProUGUI toolTipText;//툴팁창안에 있는 텍스트//자식에 있음
        public UIView toolTipView;//보여줄 툴팁창//자기자신한테 있음
        //UIView의 Show Hide는 카테고리와 이름을 통해 해당되는 모든 uIView를 작동시킨다.
        //해당 설정을 어덯게 바꾸는지를 모르니  Custom에서 ToolTip으로 바꾸는걸 권장
        [Space]
        public LayoutElement layoutElement;//content size fitter, layout group
        public int charLimit = 500;
        [Space]
        [Header("폰트사이즈")]        
        [SerializeField] private float fontSize = 25f;//오토사이즈는 꺼야됨


        //[SerializeField] private RectTransform canvasRect;//해상도의 최대높이와 넓이를 확인하기위한것 

        protected virtual void Awake()
        {
            OffText();
            //앨리먼트 체크
            int textLength = toolTipText.text.Length;
            layoutElement.enabled = (textLength > 500) ? true : false;

            //레이캐스트타겟 끄기
            toolTipText.raycastTarget = false;
            toolTipImage.raycastTarget = false;
            if (TryGetComponent(out Image image))
            {
                image.raycastTarget = false;
            }

            if (isUseMovingToolTip)
            {
                Vector3 anchorPos = MousePointer.Instance.mouseScreenPosition / toolTipView.RectTransform.localScale.x + offsetPos;
                CheckSidePos(anchorPos);
            }
        }

        private void Update()
        {   
            //움직이는 툴팁일시
            if (!isUseMovingToolTip)
            {
                return;
            }
            Vector2 anchorPos = MousePointer.Instance.mouseScreenPosition / toolTipView.RectTransform.localScale.x + offsetPos;
            CheckSidePos(anchorPos);

            //폰트사이즈
            if (toolTipText.fontSize != fontSize)
            {
                toolTipText.fontSize = fontSize;
            }
        }

        /// <summary>
        /// 텍스트를 입력하고 보여주는 함수
        /// </summary>
        /// <param name="titleText">제목(한칸띄기)</ Oparam>
        /// <param name="contentText">내용(한칸띄기 O)</param>
        /// <param name="sprite">아이콘 및 이미지(192~256px)</param>
        /// <param name="extendContent">추가적인 내용(한칸띄기 X)</param>
        /// <param name="useDoublePeriod">마침표로 ".." 을 후반부에 집어넣겠는가</param>
        public void ShowText(string titleText, string contentText, Sprite sprite = null, string extendContent = "", bool useDoublePeriod = false)
        {
            transform.SetAsLastSibling();
            if (isUseMovingToolTip)
            {
                Vector2 anchorPos = MousePointer.Instance.mouseScreenPosition / toolTipView.RectTransform.localScale.x + offsetPos;
                CheckSidePos(anchorPos);
            }
            toolTipView.Show();

            //이미지를 사용
            toolTipImage.sprite = sprite == null ? null : sprite;

            //추가적인
            toolTipText.text += titleText + "\n";
            toolTipText.text += contentText + "\n";
            toolTipText.text += extendContent;

            //..을 추가시키겠는가
            if (useDoublePeriod)
            {
                toolTipText.text += "..";
            }

            //앨리먼트 체크
            int textLength = toolTipText.text.Length;
            layoutElement.enabled = (textLength > 500) ? true : false;
        }

        /// <summary>
        /// 툴팁에 지정된 내용들 지우는 함수
        /// </summary>
        public void ClearText()
        {
            toolTipImage.sprite = null;
            toolTipText.text = "";
        }

        /// <summary>
        /// 틀팁 비활성화
        /// </summary>
        public void OffText()
        {
            toolTipView.Hide();
            //if (toolTipView.IsShowing)
            //{
               
            //}
        }

        /// <summary>
        /// 툴팁이 보이는 위치 옮기는 함수
        /// </summary>
        /// <param name="_pos">타겟</param>
        public void MoveToolTip(Transform _pos)
        {
            if (_pos != null)
            {
                toolTipView.RectTransform.position = _pos.position + offsetPos;
            }
            CheckSidePos(toolTipView.RectTransform.position);
            //Debug.Log("월드 포지션" + _pos.position);
            //Debug.Log("로컬 포지션" + _pos.localPosition);
        }

        /// <summary>
        /// 외곽 라인을 체크하여 안쪽으로만 표시하게 해줄시 있게하는 함수
        /// </summary>
        /// <param name="_anchorPos">타겟 위치</param>
        private void CheckSidePos(Vector2 _anchorPos)
        {
            //외곽체크
            //스크린 오른쪽 사이드
            if (_anchorPos.x + toolTipView.RectTransform.rect.width > Screen.width)
            {
                _anchorPos.x = Screen.width - toolTipView.RectTransform.rect.width;
            }
            //스크린 왼쪽 사이드
            else if (_anchorPos.x < 0)
            {
                _anchorPos.x = 0;
            }

            //스크린 위 사이드
            if (_anchorPos.y + toolTipView.RectTransform.rect.height > Screen.height)
            {
                _anchorPos.y = Screen.height - toolTipView.RectTransform.rect.height;
            }
            //스크린 아래 사이드
            else if (_anchorPos.y < 0)
            {
                _anchorPos.y = 0;
            }
            toolTipView.RectTransform.position = _anchorPos;
        }
    }
}