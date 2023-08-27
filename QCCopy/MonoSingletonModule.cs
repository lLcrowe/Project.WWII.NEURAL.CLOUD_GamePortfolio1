using UnityEngine;

namespace lLCroweTool.SingletonModule
{
    public class MonoSingletonModule<T> where T : MonoBehaviour
    {
        //싱글톤을 모듈형식으로 만드는 기능을 가지는 함수


        protected static T instance;
        public static T Instance
        {
            get
            {
                if (ReferenceEquals(instance, null))
                {
                    instance = Object.FindObjectOfType<T>();
                    if (ReferenceEquals(instance, null))
                    {
                        GameObject tmp = new GameObject();
                        instance = tmp.AddComponent<T>();
                        tmp.name = "-=" + typeof(T).Name + "SingletonModule=-";
                    }
                }
                return instance;
            }
        }



        public static void Init(T target)
        {
            instance = target;
            if (instance != null)
            {
                if (instance != target)
                {
                    Object.Destroy(instance.gameObject);
                }
            }
            //Application.isPlaying//종료할때도 작동되있음//20230326
            //instance = transform.GetComponent<T>();                        

            //루트오브젝트가 돈디스트로이가 걸린다면 그자식오브젝트는 안먹는다            
            Object.DontDestroyOnLoad(instance.gameObject);//루트 오브젝트일시에만 작동됨//루트오브젝트가 아닐시 따로 작동은 안됨
            //instance = FindObjectOfType<T>();
        }



    }
}
