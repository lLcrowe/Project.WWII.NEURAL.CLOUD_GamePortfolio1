using UnityEngine;

namespace lLCroweTool.Sound
{
    public class CreateSound : MonoBehaviour
    {
        //사운드 정보를 제작하기 위한 컴포넌트//인게임에서는 쓸일이 없음
        public AudioSource audioSource;

        //자식으로 가지고 있음//거리체크용
        public AudioListener audioListener;
    }
}
