using UnityEngine;

namespace lLCroweTool.Singleton
{
    public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T instance;
        public static T Instance
        {
            get
            {
                if (ReferenceEquals(instance, null))
                {
                    instance = FindObjectOfType<T>();
                    if (ReferenceEquals(instance, null))
                    {
                        //내가 가진 형태에서는 이형태로 할시 문제가 발생함
                        //GameObject tmp = new GameObject("-=" + typeof(T).Name + "=-", typeof(T));
                        //tmp.TryGetComponent(out T instance);

                        GameObject tmp = new GameObject();
                        instance = tmp.AddComponent<T>();
                        tmp.name = "-=" + typeof(T).Name + "=-";
                    }
                }
                return instance;
            }
        }
        
        //목적에 따라 씬마다 유지되지 않는 싱글톤이 있다.//고로 제작
        [Header("파괴가능한 싱글톤인지")]
        public bool isDestroySingleton = false;

        private static bool isOverLapSingleton = false;

        protected virtual void Awake()
        {
            isOverLapSingleton = false;
            if (instance != null)
            {
                //비어있지않고
                if (instance != this)
                {
                    //인스턴스가 자기자신이 아니면 자기자신을 없앰
                    //이미 지정되있으니까//새로생기는건 필요없음
                    Destroy(gameObject);
                    isOverLapSingleton = true;
                    return;
                }
            }

            //인스턴스 지정
            //instance = transform.GetComponent<T>();                        
            instance = this as T;

            if (isDestroySingleton)
            {
                return;
            }

            //루트오브젝트가 돈디스트로이가 걸린다면 그자식오브젝트는 안먹는다            
            DontDestroyOnLoad(gameObject);//루트 오브젝트일시에만 작동됨//루트오브젝트가 아닐시 따로 작동은 안됨
            //instance = FindObjectOfType<T>();
        }

        protected virtual void OnDestroy()
        {
            instance = null;
        }

        public bool CheckOverLapSingleTon()
        {
            return isOverLapSingleton;
        }

        /// <summary>
        /// 특정싱글톤이 씬상에 존재하는지 체크하는 함수
        /// </summary>
        /// <returns>존재여부</returns>
        public static bool CheckExistScene()
        {
            if (ReferenceEquals(instance, null))
            {
                return false; 
            }

            if (instance.gameObject == null)
            {
                instance = null;
                return false;
            }

            return true;
        }
    }
}