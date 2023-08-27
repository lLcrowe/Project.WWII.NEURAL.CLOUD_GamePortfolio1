
using lLCroweTool.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;


namespace lLCroweTool
{
    public interface ISaveTarget
    {
        /// <summary>
        /// 등록하기
        /// </summary>
        /// <param name="saveTarget"></param>
        static void JoinSaveTarget(ISaveTarget saveTarget)
        {
            SaveSystem.SaveSystem.Instance.JoinISaveTarget(saveTarget);
        }


    }
}


namespace lLCroweTool.SaveSystem
{
    public class SaveSystem : MonoBehaviourSingleton<SaveSystem>
    {
        //https://devruby7777.tistory.com/entry/Unity-%EC%9C%A0%EB%8B%88%ED%8B%B0%EC%9D%98-%EB%8D%B0%EC%9D%B4%ED%84%B0-%EC%A0%80%EC%9E%A5-%EB%B0%A9%EB%B2%95%EB%93%A4%EA%B3%BC-%EA%B7%B8-%EA%B2%BD%EB%A1%9C
        //플레이어 프리팹을 사용해보자  //좀더 쉬운방법없나//이거 생노가다인데//대용량데이터에는 적합하지않다고함//레지스트리를 더럽혀서
        //Json?   //제이슨으로 체크하기    //좀 체크해보자


        //public enum ESaveType
        //{
        //    Float,
        //    Int,
        //    String,
        //    Vector3,
        //    Vector3Int3,

        //}



        public string path;

        public Dictionary<ISaveTarget, bool> saveTargetBible = new Dictionary<ISaveTarget, bool>();

     
        //제이슨 생각보다 간단하네//왜쓰는지알겠다

      

        public void JoinISaveTarget(ISaveTarget saveTarget)
        {
            saveTargetBible.TryAdd(saveTarget, false);
        }

        
     



        public static void SetSave<OriginClass>(ISaveTarget saveTarget) where OriginClass : class
        {
            OriginClass target = saveTarget as OriginClass;            
            string saveString = JsonUtility.ToJson(target);

            //이제 경로에 저장


        }

       

        public void SaveVector3(string key, Vector3 targetVec)
        {
            //레지스트리용//제이슨=>문자열로 파싱 직렬화//클래스마다 양식을 만들어야될 번거로움

            PlayerPrefs.SetFloat($"{key}-VectorX", targetVec.x);
            PlayerPrefs.SetFloat($"{key}-VectorY", targetVec.y);
            PlayerPrefs.SetFloat($"{key}-VectorZ", targetVec.z);
        }

        public Vector3 LoadVector3(string key)
        {
            return new Vector3(PlayerPrefs.GetFloat($"{key}-VectorX"), PlayerPrefs.GetFloat($"{key}-VectorY"), PlayerPrefs.GetFloat($"{key}-VectorZ"));
        }

        
    }
}