using lLCroweTool.Dictionary;
using lLCroweTool.ObjectPool;
using UnityEngine;

namespace lLCroweTool.PoolBible
{
    /// <summary>
    /// 컴포넌트 폴
    /// </summary>
    [System.Serializable]
    public class CustomPool : CustomObjectPool<Component> { }

    /// <summary>
    /// 컴포넌트 폴 바이블
    /// </summary>유니티컴포넌트를 상속한 타입</typeparam>
    /// [System.Serializable]
    public class CustomPoolBible<T> : CustomDictionary<string, CustomPool> where T : Component
    {
        public T RequestPrefab(T component)
        {
            T target = null;
            string id = component.name;
            if (ContainsKey(id))
            {
                target = this[id].RequestPrefab() as T;
            }
            else
            {
                CustomPool pool = new CustomPool();
                pool.SetPrefab(component);
                Add(id, pool);
                target = this[id].RequestPrefab() as T;
            }

            return target;
        }

        public void ReturnPrefab(T component)
        {
            string id = component.name;
            if (ContainsKey(id))
            {
                //등록된 폴이 있으면 안쪽에 해당컴포넌트가 있는지체크                
                if (!this[id].ContainObjectPool(component))//좀더 빨리할수 있으면..딕셔너리 써야할듯한데//리스트에서 내부에 딕셔너리추가후 한번래핑함
                {
                    //없으면 등록
                    T newTarget = Object.Instantiate(component);
                    newTarget.name = component.name;
                    newTarget.gameObject.SetActive(false);
                    this[id].AddObjectPool(newTarget);
                }

            }
            else
            {
                //등록된 폴이 없으면 새로생성해서 등록후 추가
                CustomPool pool = new CustomPool();
                pool.SetPrefab(component);
                T newTarget = Object.Instantiate(component);
                newTarget.name = component.name;
                newTarget.gameObject.SetActive(false);
                pool.AddObjectPool(newTarget);
                Add(id, pool);
            }
            component.gameObject.SetActive(false);
        }

        /// <summary>
        /// 등록된 폴을 리셋해주는 함수
        /// </summary>
        public void ResetCustomPoolBible()
        {
            foreach (var item in this)
            {
                var customPool = item.Value;
                customPool.ClearCustomObjectPool();
            }
        } 
        
        /// <summary>
        /// 폴에 있는 모든오브젝트를 비활성화시킵니다.
        /// </summary>
        /// <param name="isActive">활성화여부</param>
        /// <param name="parent">부모객체</param>
        public void AllObjectDeActive(bool isActive = false, Transform parent = null)
        {
            var objectPoolList = GetValueList();

            for (int i = 0; i < objectPoolList.Count; i++)
            {
                objectPoolList[i].AllObjectDeActive(isActive, parent);
            }
        }
    }


    /// <summary>
    /// 컴포넌트 폴(해쉬코드)
    /// </summary>
    [System.Serializable]
    public class TestCustomPool : CustomObjectPool<Component> { }

    /// <summary>
    /// 컴포넌트 폴 바이블
    /// </summary>유니티컴포넌트를 상속한 타입</typeparam>
    /// [System.Serializable]
    
    //일관성있게 처리할려면 유니티오브젝트의 이름을 동기화시켜줘야됨    
    public class TestCustomPoolBible<T> : CustomDictionary<int, CustomPool> where T : Component
    {
        public T RequestPrefab(T component)
        {
            T target = null;
            int id = component.name.GetHashCode();//현재로서는 이게 더빠름
            if (ContainsKey(id))
            {
                target = this[id].RequestPrefab() as T;
            }
            else
            {
                CustomPool pool = new CustomPool();
                pool.SetPrefab(component);
                Add(id, pool);
                target = this[id].RequestPrefab() as T;
            }

            return target;
        }

        //쪼매 위험하다//왜냐하면 에셋의 프리팹으로 되있는 게임오브젝트가 아닌 씬상에 배치되있는애가 선택된 다음 그친구를 오브젝트폴을 사용하다가
        //그 배치되있는애가 사라져버리면 그대로 문제가발생된다.//그러면 하나복사해서 비활성화후 붙여넣어버리자..!//이러면 원래있던애가 사라져도 괜찮찮아
        public void ReturnPrefab(T component)
        {
            int id = component.name.GetHashCode();//현재로서는 이게 더빠름
            if (ContainsKey(id))
            {
                //등록된 폴이 있으면 안쪽에 해당컴포넌트가 있는지체크                
                if (!this[id].ContainObjectPool(component))//좀더 빨리할수 있으면..딕셔너리 써야할듯한데//리스트에서 내부에 딕셔너리추가후 한번래핑함
                {
                    //없으면 등록
                    T newTarget = Object.Instantiate(component);
                    newTarget.name = component.name;
                    newTarget.gameObject.SetActive(false);
                    this[id].AddObjectPool(newTarget);
                }

            }
            else
            {
                //등록된 폴이 없으면 새로생성해서 등록후 추가
                CustomPool pool = new CustomPool();
                pool.SetPrefab(component);
                T newTarget = Object.Instantiate(component);
                newTarget.name = component.name;
                newTarget.gameObject.SetActive(false);
                pool.AddObjectPool(newTarget);
                Add(id, pool);
            }
            component.gameObject.SetActive(false);
        }


        public void ResetComponentPoolBible()
        {
            foreach (var item in this)
            {
                item.Value.ClearCustomObjectPool();
            }
        }

        /// <summary>
        /// 폴에 있는 모든오브젝트를 비활성화시킵니다.
        /// </summary>
        /// <param name="isActive">활성화여부</param>
        /// <param name="parent">부모객체</param>
        public void AllObjectDeActive(bool isActive = false, Transform parent = null)
        {
            var objectPoolList = GetValueList();

            for (int i = 0; i < objectPoolList.Count; i++)
            {
                objectPoolList[i].AllObjectDeActive(isActive, parent);
            }
        }
    }

    //테스트버전
    //public class TestBible : CustomBasePoolBible<int,Transform>{ }
    //public class CustomBasePoolBible<T1, T2> : CustomDictionary<T1, CustomPool> where T2 : Component where T1 : struct, class//상반된거 안됨//타입형식만됨//지원안함
    //{
    //    public T2 RequestPrefab(T2 component)
    //    {
    //        T2 target = null;
    //        T1 id = component.name.GetHashCode() as T1;//현재로서는 이게 더빠름
    //        if (ContainsKey(id))
    //        {
    //            target = this[id].RequestPrefab() as T;
    //        }
    //        else
    //        {
    //            CustomPool pool = new CustomPool();
    //            pool.SetPrefab(component);
    //            Add(id, pool);
    //            target = this[id].RequestPrefab() as T;
    //        }

    //        return target;
    //    }

    //    //쪼매 위험하다//왜냐하면 에셋의 프리팹으로 되있는 게임오브젝트가 아닌 씬상에 배치되있는애가 선택된 다음 그친구를 오브젝트폴을 사용하다가
    //    //그 배치되있는애가 사라져버리면 그대로 문제가발생된다.//그러면 하나복사해서 비활성화후 붙여넣어버리자..!//이러면 원래있던애가 사라져도 괜찮찮아
    //    public void ReturnPrefab(T component)
    //    {
    //        int id = component.name.GetHashCode();//현재로서는 이게 더빠름
    //        if (ContainsKey(id))
    //        {
    //            //등록된 폴이 있으면 안쪽에 해당컴포넌트가 있는지체크                
    //            if (!this[id].ContainObjectPool(component))//좀더 빨리할수 있으면..딕셔너리 써야할듯한데//리스트에서 내부에 딕셔너리추가후 한번래핑함
    //            {
    //                //없으면 등록
    //                T newTarget = Object.Instantiate(component);
    //                newTarget.name = component.name;
    //                newTarget.gameObject.SetActive(false);
    //                this[id].AddObjectPool(newTarget);
    //            }

    //        }
    //        else
    //        {
    //            //등록된 폴이 없으면 새로생성해서 등록후 추가
    //            CustomPool pool = new CustomPool();
    //            pool.SetPrefab(component);
    //            T newTarget = Object.Instantiate(component);
    //            newTarget.name = component.name;
    //            newTarget.gameObject.SetActive(false);
    //            pool.AddObjectPool(newTarget);
    //            Add(id, pool);
    //        }
    //        component.gameObject.SetActive(false);
    //    }


    //    public void ResetComponentPoolBible()
    //    {
    //        foreach (var item in this)
    //        {
    //            item.Value.ClearCustomObjectPool();
    //        }
    //    }

    //    /// <summary>
    //    /// 폴에 있는 모든오브젝트를 비활성화시킵니다.
    //    /// </summary>
    //    /// <param name="isActive">활성화여부</param>
    //    /// <param name="parent">부모객체</param>
    //    public void AllObjectDeActive(bool isActive = false, Transform parent = null)
    //    {
    //        var objectPoolList = GetValueList();

    //        for (int i = 0; i < objectPoolList.Count; i++)
    //        {
    //            objectPoolList[i].AllObjectDeActive(isActive, parent);
    //        }
    //    }


    //}

    //나중에 리스트말고 Queue랑 Iinterface부분을 사용해서 처리하는것도 좋아봄임
    //문제가 있다 스크립트를 늘리기 싫은 문제가 있음. 그건곧 관리해줘야될게 늘어난다는 소리





    //일반 클래스들을 담아둘 대상

}