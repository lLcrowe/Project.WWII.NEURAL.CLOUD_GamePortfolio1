using System.Collections.Generic;
using UnityEngine;

namespace lLCroweTool.ClassObjectPool
{
    /// <summary>
    /// 클래스폴 타겟으로 지정할 클래스
    /// </summary>
    public abstract class CustomClassPoolTarget
    {
        //상속받아서 사용할떄 켜주고 안쓸때 꺼주기
        private bool isUse = false;

        public bool GetIsUse() => isUse;
        public void SetIsUse(bool value)
        {
            isUse = value;
        }
    }

    /// <summary>
    /// 커스텀클래스폴. 유니티오브젝트폴과 다른 클래스로만 작동되는 폴
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CustomClassPool<T> where T : CustomClassPoolTarget, new()
    {
        [Header("타겟팅될 프리팹")]
        [SerializeField] protected T objectPrefab = null;//타겟팅될 프리팹
        [SerializeField] private List<T> objectPoolList = new List<T>();
        [SerializeField] private Dictionary<T, bool> searchBible = new();//검색용//원래는 합칠려고했는데 힘들듯하다
        [SerializeField] protected int size = 50;


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
        private static T RequestPrefab(CustomClassPool<T> customObjectPool)
        {
            //초기화
            bool isFind = false;
            T targetObject = null;

            //로직작동
            var objectPoolList = customObjectPool.objectPoolList;
            for (int i = 0; i < objectPoolList.Count; i++)
            {
                if (!objectPoolList[i].GetIsUse())
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
                targetObject = new T();
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
        private static void ClearCustomObjectPool(CustomClassPool<T> customObjectPool)
        {
            var objectPoolList = customObjectPool.objectPoolList;
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
                if (objectPoolList[i].GetIsUse() != isActive)
                {
                    objectPoolList[i].SetIsUse(isActive);
                }
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
                if (objectPoolList[i].GetIsUse())
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
        public static void LoadToDecreasePrefab(CustomClassPool<T> customObjectPool)
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
                if (!customObjectPool.objectPoolList[i].GetIsUse())
                {
                    //버그날것같음 //20220704
                    T t = customObjectPool.objectPoolList[i];
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
