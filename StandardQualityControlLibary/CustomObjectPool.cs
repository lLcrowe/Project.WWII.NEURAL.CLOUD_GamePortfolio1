using lLCroweTool.DestroyManger;
using System.Collections.Generic;
using UnityEngine;

namespace lLCroweTool.ObjectPool
{
    /// <summary>
    /// 커스텀오브젝트폴
    /// </summary>
    /// <typeparam name="T">모노비헤이비어 상속한 타입</typeparam>
    //[System.Serializable]
    public abstract class CustomObjectPool<T> where T : Component
    {
        [Header("타겟팅될 프리팹")]
        [SerializeField] protected T objectPrefab = null;//타겟팅될 프리팹
        [SerializeField] private List<T> objectPoolList = new List<T>();
        [SerializeField] private Dictionary<T, bool> searchBible = new();//검색용//원래는 합칠려고했는데 힘들듯하다
        [SerializeField] protected int size = 50;

        /// <summary>
        /// 커스텀오브젝트폴 생성자. 프리팹을 세팅할것
        /// </summary>
        /// <param name="targetPrefab">세팅할 프리팹</param>
        //public CustomObjectPool(T targetPrefab)
        //{
        //    SetPrefab(targetPrefab);
        //}

        /// <summary>
        /// 프리팹세팅해주는 함수//생성자 만들때 해줘야됨
        /// </summary>
        /// <param name="targetPrefab">타겟팅할 프리팹</param>
        public void SetPrefab(T targetPrefab)
        {
            objectPrefab = targetPrefab;
        }

        /// <summary>
        /// 적용한 프리팹을 가져오는 함수
        /// </summary>
        /// <returns>적용한 프리팹</returns>
        public T GetPrefab()
        {
            return objectPrefab;
        }

        /// <summary>
        /// 프리팹을 요청하는 함수
        /// </summary>
        /// <returns>프리팹</returns>
        public T RequestPrefab()
        {
            return RequestPrefab(this);
        }

        /// <summary>
        /// 프리팹을 요청하는 함수
        /// </summary>
        /// <returns>프리팹</returns>
        private static T RequestPrefab(CustomObjectPool<T> customObjectPool)
        {
            //초기화
            bool isFind = false;
            T targetObject = null;

            //로직작동
            var objectPoolList = customObjectPool.objectPoolList;
            for (int i = 0; i < objectPoolList.Count; i++)
            {
                if (!objectPoolList[i].gameObject.activeSelf)
                {
                    isFind = true;
                    targetObject = objectPoolList[i];
                    //리셋구간
                    break;
                }
            }
            //찾은게 없다면 오브젝트 하나를 만들어준다.
            if (!isFind)
            {
                targetObject = Object.Instantiate(customObjectPool.objectPrefab);
                targetObject.name = customObjectPool.objectPrefab.name;
                //리셋구간
                customObjectPool.AddObjectPool(targetObject);
                //targetVisualObject.gameObject.SetActive(false);
            }
            return targetObject;
        }

        /// <summary>
        /// 커스텀오브젝트폴을 클리어해주는 함수
        /// </summary>
        public void ClearCustomObjectPool()
        {
            ClearCustomObjectPool(this);
        }

        /// <summary>
        /// 커스텀오브젝트폴을 클리어해주는 함수
        /// </summary>
        /// <param name="customObjectPool">대상이 될 커스텀폴</param>
        private static void ClearCustomObjectPool(CustomObjectPool<T> customObjectPool)
        {
            var objectPoolList = customObjectPool.objectPoolList;
            for (int i = 0; i < objectPoolList.Count; i++)
            {
                var temp = objectPoolList[i];
                if (temp == null)
                {
                    continue;
                }

                DestroyManager.Instance.AddDestoryGameObject(temp.gameObject);
            }
            objectPoolList.Clear();
            customObjectPool.SyncSearchBible();
        }

        /// <summary>
        /// 폴에 있는 모든오브젝트를 비활성화시킵니다.
        /// </summary>
        /// <param name="isActive">활성화여부</param>
        /// <param name="parent">부모객체</param>
        public void AllObjectDeActive(bool isActive = false, Transform parent = null)
        {
            for (int i = 0; i < objectPoolList.Count; i++)
            {
                if (objectPoolList[i].gameObject.activeSelf != isActive)
                {
                    objectPoolList[i].gameObject.SetActive(isActive);
                }
                objectPoolList[i].transform.SetParent(parent);
            }
        }

        /// <summary>
        /// 작동중인 오브젝트들을 카운트하는 함수
        /// </summary>
        /// <returns>작동되는 수량</returns>
        public int GetActiveObjectCount()
        {
            int activeCount = 0;
            for (int i = 0; i < objectPoolList.Count; i++)
            {
                if (objectPoolList[i].gameObject.activeSelf)
                {
                    activeCount++;
                }
            }
            return activeCount;
        }

        /// <summary>
        /// 로딩창에 들어갔을시 사이즈에 맞게 프리팹을 처리하는 함수
        /// </summary>
        /// <param name="customObjectPool">타겟팅될 커스텀오브젝트폴</param>
        public static void LoadToDecreasePrefab(CustomObjectPool<T> customObjectPool)
        {
            //사이즈처리를 로딩창에서만 처리하게 제작
            int curSize = customObjectPool.objectPoolList.Count;//현재 사이즈
            int targetSize = customObjectPool.size;//해당 사이즈까지 내려가야됨

            for (int i = 0; i < customObjectPool.objectPoolList.Count; i++)
            {
                //타겟사이즈 이하 될때까지 파괴
                if (targetSize >= curSize)
                {
                    //멈춤
                    break;  
                }

                //오브젝트가 비활성화시 파괴
                if (!customObjectPool.objectPoolList[i].gameObject.activeSelf)
                {
                    //버그날것같음 //20220704
                    T t = customObjectPool.objectPoolList[i];
                    DestroyManager.Instance.AddDestoryGameObject(customObjectPool.objectPoolList[i].gameObject);
                    customObjectPool.objectPoolList.Remove(t);
                    i--;//<=확인하기
                }
            }
            customObjectPool.SyncSearchBible();
        }

        //-----------------------------------------------------------------------------
        //한번래핑(감싼) 구역
        //-----------------------------------------------------------------------------


        public void AddObjectPool(T target)
        {
            objectPoolList.Add(target);
            searchBible.Add(target, false);
        }


        public bool ContainObjectPool(T target)
        {
            return searchBible.ContainsKey(target);
        }

        public void RemoveObjectPool(T target) 
        {
            objectPoolList.Remove(target);
            searchBible.Remove(target);
        }

        public void SyncSearchBible()
        {
            searchBible.Clear();
            for (int i = 0; i < objectPoolList.Count; i++)
            {
                int index = i;
                searchBible.Add(objectPoolList[index], false);
            }
        }
    }
}
