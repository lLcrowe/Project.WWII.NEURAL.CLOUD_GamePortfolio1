using UnityEngine;
using lLCroweTool.Sound;
using lLCroweTool.DataBase;
#if UNITY_EDITOR
using UnityEditor;
#pragma warning disable 0618

namespace lLCroweTool.QC.EditorOnly
{
    [CustomEditor(typeof(CreateSound))]
    public class CreateSoundInspectorEditor : Editor
    {
        //나중에 구조변경하고 사운드플레이가능할수 있게 에디터를 제작해야겠다 일단은 그냥 제작
        //리스너가 CreatSound에 있고 사운드플레이어가 주변에 여러개 배치할수 있게 처리


        private CreateSound targetObject;
        private AudioSource audioSource;
        private static SoundInfo soundInfo;
        

        private void OnEnable()
        {
            targetObject = (CreateSound)target;
            targetObject.audioSource = targetObject.GetAddComponent<AudioSource>();
            audioSource = targetObject.audioSource;

            //기본적인 초기화
            if (targetObject.audioListener == null)
            {
                GameObject go = new GameObject("Listener", typeof(AudioListener));
                go.transform.InitTrObjPrefab(targetObject.transform, targetObject.transform);
                targetObject.audioListener = go.GetAddComponent<AudioListener>();
            }
        }

        private void OnSceneGUI()
        {
            //리스너위치 이동가능하게
            //자기자신위치를 체크
            lLcroweUtilEditor.DrawSphere(targetObject.audioListener.transform.position);
            lLcroweUtilEditor.TransformHandle(targetObject.audioListener.transform);
        }

        public override void OnInspectorGUI()
        {
            if (soundInfo == null)
            {
                soundInfo = new SoundInfo();
                soundInfo.name = "NewSound";
                return;
            }


            //base.OnInspectorGUI();
            lLcroweUtilEditor.ObjectFieldAndNullButton("사운드 정보", ref soundInfo, false);
            lLcroweUtilEditor.ObjectFieldAndNullButton("사운드 클립", ref soundInfo.soundData.audioClip, false);

            //사운드 정보들 체크//길이//
            string content = soundInfo.soundData.audioClip == null ? "사운드길이 : 0" : $"사운드길이 : {soundInfo.soundData.audioClip.length}";
            EditorGUILayout.LabelField(content);   
            

            lLcroweUtilEditor.EditorGUILayoutHorizontal(() =>
            {
                //소리재생
                lLcroweUtilEditor.Button("소리재생", () =>
                {
                    audioSource.clip = soundInfo.soundData.audioClip;
                    audioSource.Play();
                });

                //소리중지
                lLcroweUtilEditor.Button("소리중지", () =>
                {
                    audioSource.Stop();
                });
                //반복재생하게
                lLcroweUtilEditor.Button(audioSource.loop ? "반복중" : "반복X", () =>
                {
                    audioSource.loop = !audioSource.loop;
                });
            });

            //사운드 정보생성//여러 설정처리

            //지금은 바쁘니 이건넘기고 생성만 제작
            lLcroweUtilEditor.LabelBaseDataShow("이름",soundInfo);
            soundInfo.soundData.isPlayOneShot = EditorGUILayout.Toggle("한번만 작동되기",soundInfo.soundData.isPlayOneShot);//이건 좀 바꿔서 말해야할듯한데//설명이 직관적이지않음
            

            lLcroweUtilEditor.EditorButton("사운드정보생성",()=>
            {
                var newSoundInfo = Instantiate(soundInfo);
                lLcroweUtilEditor.CreateDataObject(ref newSoundInfo, newSoundInfo.labelID, "SoundInfo", "SoundInfo");
            });

            //리스너위치 초기화//바로옆에//필요할까?
        }

        //public static void PlayClip(AudioClip clip, int startSample = 0, bool loop = false)
        //{
        //    System.Reflection.Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        //    System.Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        //    System.Reflection.MethodInfo method = audioUtilClass.GetMethod(
        //        "PlayClip",
        //        System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public,
        //        null,
        //        new System.Type[] { typeof(AudioClip), typeof(int), typeof(bool) },
        //        null
        //    );
        //    method.Invoke(
        //        null,
        //        new object[] { clip, startSample, loop }
        //    );
        //}

        //public static void PlayClip(AudioClip clip)
        //{
        //    Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        //    Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        //    MethodInfo method = audioUtilClass.GetMethod(
        //        "PlayClip",
        //        BindingFlags.Static | BindingFlags.Public,
        //        null,
        //        new System.Type[] {
        //typeof(AudioClip)
        //    },
        //    null
        //    );
        //    method.Invoke(
        //        null,
        //        new object[] {
        //clip
        //    }
        //    );
        //}

        //public static void StopAllClips()
        //{
        //    Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        //    Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        //    MethodInfo method = audioUtilClass.GetMethod(
        //        "StopAllClips",
        //        BindingFlags.Static | BindingFlags.Public,
        //        null,
        //        new System.Type[] { },
        //        null
        //    );
        //    method.Invoke(
        //        null,
        //        new object[] { }
        //    );
        //}
    }
}
#endif