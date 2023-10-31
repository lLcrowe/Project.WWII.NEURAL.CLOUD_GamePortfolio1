using UnityEngine;

namespace lLCroweTool.DataBase
{
    /// <summary>
    /// UI테마에 대한 아이콘프리셋정보
    /// </summary>
    [CreateAssetMenu(fileName = "New SoundInfo", menuName = "lLcroweTool/New SoundInfo")]
    public class SoundInfo : LabelBase
    {    
        public SoundData soundData = new SoundData();


        public override LabelBaseDataType GetLabelDataType()
        {
            return LabelBaseDataType.Nothing;
        }
    }


    [System.Serializable]
    public class SoundData
    {
        public AudioClip audioClip;
        public bool isPlayOneShot;
        public bool isUseDirectionSound;//방향에 따른 사운드처리//아직 제작안함

        public float volume;
        public float pitch;
        public float roll;
    }
}