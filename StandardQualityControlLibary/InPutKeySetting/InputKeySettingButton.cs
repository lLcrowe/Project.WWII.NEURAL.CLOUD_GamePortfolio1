using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace lLCroweTool 
{
    /// <summary>
    /// 인풋키세팅을 위한 UI카드.
    ///InputSettingUI에서 사용함
    /// </summary>
    public class InputKeySettingButton : MonoBehaviour
    {
        //구조
        //Text(textContent)
        //  Button(button)
        //      Text(buttonText )
        [SerializeField] private TextMeshProUGUI textContent;   //어떠한 내용을 키인지 보여주는 텍스트
        [SerializeField] private Button button;                 //클릭할 버튼
        [SerializeField] private TextMeshProUGUI buttonText;    //버튼아래의 텍스트

        /// <summary>
        /// 인풋UI카드세팅을 위한 함수
        /// </summary>
        /// <param name="keyContent">키에 대한 내용</param>
        /// <param name="keyCode">지정된 키코드</param>
        /// <param name="buttonAction">버튼눌렸을시 이벤트</param>
        /// <param name="targetColor">버튼눌렸을시 변경색깔</param>
        public void InitInputSettingUICard(string keyContent, KeyCode keyCode ,UnityAction buttonAction, Color targetColor)
        {
            //현지화구역
            textContent.text = keyContent;
            ChangeButtonText(keyCode.ToString());
            button.onClick.AddListener(buttonAction);
            button.onClick.AddListener(delegate { ChangeButtonColor(targetColor); });
        }

        /// <summary>
        /// 버튼색깔 변경함수
        /// </summary>
        /// <param name="targetColor">변경할 색깔</param>
        public void ChangeButtonColor(Color targetColor)
        {
            button.image.color = targetColor;
        }

        /// <summary>
        /// 버튼 텍스트내용을 바꿔주는 함수
        /// </summary>
        /// <param name="textContent">바꿀 텍스트 내용</param>
        public void ChangeButtonText(string textContent)
        {
            //현지화구역
            buttonText.text = textContent;
        }
    }
}

