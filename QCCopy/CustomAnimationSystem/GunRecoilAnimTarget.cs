using UnityEngine;

namespace lLCroweTool.Anim.GunRecoil.Muzzle
{
    /// <summary>
    /// 건리코일애님이 될 타겟
    /// </summary>
    public class GunRecoilAnimTarget : MonoBehaviour
    {
        //건리코일 애님의 리코일대상은 움직이기만 하고
        //건에 대한 머즐위치는 별개이니 그걸 대응하기 위해 제작됨

        [Header("머즐(총구)위치")]
        //머즐위치
        [SerializeField] private Transform muzzlePos;

        public Transform GetMuzzleTr()
        {
            return muzzlePos;
        }
    }
}