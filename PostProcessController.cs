//using lLCroweTool.Singleton;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using UnityEngine;
//using UnityEngine.Rendering.PostProcessing;

//namespace Assets.A0
//{
//    internal class PostProcessController : MonoBehaviour
//    {
//        private DepthOfField depthOfField;
//        private ChromaticAberration chromatic;
//        private PostProcessVolume volume;


//        //에디터상에 있는건 시각적으로 보여주는거고
//        //볼륨 프로파일에 있음
//        //정보를 담고 있는 파일



//        //포스트프로세싱

//        //UI모드 => Overlay => UI에 대한 랜더링이 제일 마지막에 됨




//        private void Awake()
//        {
            
//        }


//        public void Initialize()
//        {
//            volume = GetComponent<PostProcessVolume>();
//            volume.profile.TryGetSettings(out depthOfField);
//            volume.profile.TryGetSettings(out chromatic);
//        }

//        public IEnumerator ExecuteChromatic(float start, float end, float speed)
//        {
//            chromatic.active = true;

//            //오버라이드 함수를 사용해서 값을 변경하면 체크박스가 커지게 됩니다.
//            chromatic.intensity.Override(start);

//            //아래처럼 사용하면 값만 변경됨
//            //depthOfField.focusDistance.value = speed;
//            float temp = 0;
//            while (true)
//            {
//                temp += Time.deltaTime / speed;
//                float delta = Mathf.Lerp(start, end, temp);
//                chromatic.intensity.value = delta;
//                if (temp > 1.0f)
//                {
//                    break;
//                }
//                yield return null;
//            }

//            chromatic.active = false;
//            chromatic.intensity.Override(start);
//        }

//        public IEnumerator ExecuteDepthOfField(float start = 3.5f, float end = 0.1f, float speed = 1)
//        {
//            depthOfField.active = true;

//            //오버라이드 함수를 사용해서 값을 변경하면 체크박스가 커지게 됩니다.
//            depthOfField.focusDistance.Override(start);

//            //아래처럼 사용하면 값만 변경됨
//            //depthOfField.focusDistance.value = speed;
//            float temp = 0;
//            while (true)
//            {
//                temp += Time.deltaTime / speed;
//                float delta = Mathf.Lerp(start, end, temp);
//                depthOfField.focusDistance.value = delta;
//                if (temp > 1.0f)
//                {
//                    yield break;
//                }
//                yield return null;
//            }
//        }



//        //여기쪽 이해된게 맞나

//        public Material material;

//        public float value;

//        /// <summary>
//        /// 카메라에 있어야됨
//        /// </summary>
//        /// <param name="source">현재 연산이 완료된 픽셀</param>
//        /// <param name="destination">화면상에 출력된 픽셀</param>
//        private void OnRenderImage(RenderTexture source, RenderTexture destination)
//        {
//            if (material == null)
//            {
//                return;
//            }



//            material.SetFloat("Value", value);

//            //현재 연산된 픽셀을 메터리얼로 한번 걸려내는 거다



//            Graphics.Blit(source, destination, material);
                    
//        }

//    }
//}
