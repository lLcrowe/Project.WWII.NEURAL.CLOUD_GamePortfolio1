using UnityEngine;
using lLCroweTool.Singleton;

namespace lLCroweTool
{
    public class MousePointer : MonoBehaviourSingleton<MousePointer>
    {
        //포인터만 사용
        public Vector3 mouseWorldPosition;
        public float mouseScreenDistance;
        public Vector3 mouseScreenPosition;
        public Ray mouseRay;
        private Transform tr;
#pragma warning disable CS0108 // 멤버가 상속된 멤버를 숨깁니다. new 키워드가 없습니다.
        private Camera camera;

        public Vector3 CameraPos { get => camera.transform.position; }
#pragma warning restore CS0108 // 멤버가 상속된 멤버를 숨깁니다. new 키워드가 없습니다.

        protected override void Awake()
        {   
            tr = transform;
            gameObject.name = "MousePointer";
            camera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            //인풋마우스로부터 시작함//다른걸 스크린2뭐시갱이 정류에 넣지말기
            mouseScreenPosition = Input.mousePosition;
            mouseRay = camera.ScreenPointToRay(mouseScreenPosition);
            mouseScreenPosition.z = mouseScreenDistance;
            mouseWorldPosition = camera.ScreenToWorldPoint(mouseScreenPosition);            
            tr.position = mouseWorldPosition;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            tr = null;
            camera = null;
        }
    }
}
