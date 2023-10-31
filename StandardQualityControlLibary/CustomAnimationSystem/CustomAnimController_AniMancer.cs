using UnityEngine;
using Animancer;
using lLCroweTool.Dictionary;

namespace lLCroweTool.Anim
{
    /// <summary>
    /// 애니멘서를 한번 래핑한 클래스. 애니메이션클립들을 세팅해서 처리할수 있음
    /// </summary>
    [RequireComponent(typeof(AnimancerComponent))]
    public class  CustomAnimController_AniMancer : MonoBehaviour
    {
        //애니멘서를 사용해서 처리할 예정

        //애니메이션 타입들체크
        //유닛상태 타입
        public enum UnitAnimationStateType
        {
            Idle,
            Move,
            Dead,
            Attack,//공격시 랜덤으로

            Skill2,
            Skill3,

            Victory,//승리시 카메라앵글이 완료되기전까지 애니메이션이 동작하지 않음
        }


        /// <summary>
        /// 커스텀애님클립처리
        /// </summary>
        [System.Serializable]
        public class CustomAnimClip
        {
            //애니멘서의 클립트랜지션은 모노비헤이비어를 상속받지않으니 직접적으로 New처리해줘야됨

            public UnitAnimationStateType unitAnimationStateType;
            public ClipTransition clipTransition = new ClipTransition();


            public ClipTransition GetClipTransition()
            {
                return clipTransition;
            }
        }

        //정보쪽으로 옮길꺼//좀더 생각해보니 아닌듯
        //이거 애니메이션 선택하느냐 작업량이 좀 있따.
        //작업편의성을 위해 그대로 두자//그러면 스트럭트가 맞아보이는데
        public CustomAnimClip[] CustomAnimClipArray = new CustomAnimClip[0];

        [System.Serializable]
        public class AnimClipBible : CustomDictionary<UnitAnimationStateType, CustomAnimClip> { }
        private AnimClipBible animClipBible = new AnimClipBible();
        private AnimancerComponent anim;



        //public UnitAnimationStateType testType;
        //[ButtonMethod]
        //public void Test()
        //{
        //    ActionAnim(testType);
        //}


        private void Awake()
        {
            anim = GetComponent<AnimancerComponent>();

            //중복된게 없을것
            for (int i = 0; i < CustomAnimClipArray.Length; i++)
            {
                var data = CustomAnimClipArray[i];
                animClipBible.TryAdd(data.unitAnimationStateType, data);
            }

            ActionAnim(UnitAnimationStateType.Idle);
        }

        public void ActionAnim(UnitAnimationStateType unitAnimationStateType)
        {
            if (!animClipBible.ContainsKey(unitAnimationStateType))
            {
                return;
            }

            var clip = animClipBible[unitAnimationStateType].GetClipTransition();

            //플레이중인 클립하고 신규로 들어온 클립이랑 동일하고 플레이중이면
            if (anim.IsPlaying(clip.Clip))
            {
                return;
            }

            anim.Play(clip);
        }
    }
}
