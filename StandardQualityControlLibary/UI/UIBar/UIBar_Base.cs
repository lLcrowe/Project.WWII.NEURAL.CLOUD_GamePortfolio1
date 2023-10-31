using UnityEngine.UI;
using UnityEngine;
using TMPro;
namespace lLCroweTool.UI.Bar
{   
    public abstract class UIBar_Base : MonoBehaviour
    {
        //구조
        //게임오브젝트
        //슬라이더//None//None
        //이미지//백그라운드이미지
        //자식오브젝트1//전체//채우는 오브젝트        
        //자식오브젝트2//전체//테두리 오브젝트

        //UIBar
        //쓰던 에너지바가 2020버전부터 작동이 안되는 문제가 있어서
        //의존성줄일려고 직접 만들어버림
      
        public Gradient gradient = new Gradient();//색깔지정

        public Image fill;//채우는 이미지
        public Image border;//테두리 이미지

        public TextMeshProUGUI label;//텍스트로 보여주는 용도

        /// <summary>
        /// {cur} = 현재값, {max} = 최대값
        /// </summary>
        public string labelFormat = "{cur}/{max}";//포맷

        private string labelCache;
        

        //이미하나 쓸때마다 배치가 한개씩 늘어버리네 체크하기//메터리얼을 같은거 쓰면 줄던거 같기도한데
        //메쉬하나로?//나중에 보기//0~1 사이로 만드는 Math 함수있던거 같은데 가물

        /// <summary>
        /// UIBar 초기화
        /// </summary>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        /// <param name="value">현재값</param>
        public virtual void InitUIBar(float min, float max, float value) 
        {
            //맨위로 올리기
            if (label == null)
            {
                return;
            }

            if (label.transform.parent == transform)
            {
                label.transform.SetAsLastSibling();
            }
        }


        /// <summary>
        /// 현재값을 가져오는 함수
        /// </summary>
        /// <returns>현재값</returns>
        public abstract float GetCurValue();

        /// <summary>
        /// 현재값을 세팅하는 함수
        /// </summary>
        /// <param name="value">세팅할 값</param>
        protected abstract void SetCurBarValue(float value);

        /// <summary>
        /// 최대값을 가져오는 함수
        /// </summary>
        /// <returns>최대값</returns>
        public abstract float GetMaxValue();

        /// <summary>
        /// 최대값을 세팅하는 함수
        /// </summary>
        /// <param name="value">세팅할값</param>
        public abstract void SetMaxValue(float value);

        /// <summary>
        /// 최소값을 가져오는 함수
        /// </summary>
        /// <returns>최소값</returns>
        public abstract float GetMinValue();

        /// <summary>
        /// 최소값을 세팅하는 함수
        /// </summary>
        /// <param name="value">세팅할 값</param>
        public abstract void SetMinValue(float value);


        /// <summary>
        /// 현재값을 세팅하는 함수
        /// </summary>
        /// <param name="value">세팅할 값</param>
        public void SetCurValue(float value)
        {
            SetCurBarValue(value);
            fill.color = gradient.Evaluate(Mathf.Clamp01(fill.fillAmount));//색깔지정

            if (label == null)
            {
                return;
            }
            //세팅
            label.text = LabelFormatResolve(labelFormat, ref labelCache, GetMinValue(), GetMaxValue(), GetCurValue());
        }

        /// <summary>
        /// UI바의 컬러 알파값을 조정하는 함수(값을 준다음 변경해야됨)
        /// </summary>
        /// <param name="a">알파값</param>
        public void SetColorAlpha(float a)
        {
            Color color = fill.color;
            color.a = a;
            fill.color = color;
        }

        /// <summary>
        /// 지정된 포맷으로 보여줄
        /// </summary>
        /// <param name="format">포맷방식</param>
        /// <param name="labelCach">캐싱될 라벨(비교용)</param>
        /// <param name="minValue">최소값</param>
        /// <param name="maxValue">최대값</param>
        /// <param name="curValue">현재값</param>
        /// <returns>컨텐츠</returns>
        protected static string LabelFormatResolve(string format, ref string labelCach, float minValue, float maxValue, float curValue)
        {
            if (labelCach == format)
            {
                return labelCach;
            }


            format = format.Replace("{cur}", "" + curValue);
            format = format.Replace("{min}", "" + minValue);
            format = format.Replace("{max}", "" + maxValue);
            format = format.Replace("{cur%}", string.Format("{0:00}", curValue * 100));
            format = format.Replace("{cur2%}", string.Format("{0:00.0}", curValue * 100));
            try
            {
                format = string.Format(format, curValue, minValue,
                    maxValue, curValue, curValue * 100);
            }
            catch (System.Exception)
            {
                // ignore
            }
            labelCach = format;
            return format;
        }
    }
}