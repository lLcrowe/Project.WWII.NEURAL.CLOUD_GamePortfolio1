using System.Collections.Generic;
using UnityEngine;
using lLCroweTool.Singleton;
using System.Collections;

namespace lLCroweTool.DestroyManger
{
    [DefaultExecutionOrder(1000)]
    public class DestroyManager : MonoBehaviourSingleton<DestroyManager>
    {
        //오브젝트를 파괴시키는 매니저
        //파괴시킬오브젝트를 현 매니저에 집어넣어 
        //일정시간마다 일정한 오브젝트들을 삭제하게 만듬

        //private static DestroyManager instance;//인스턴스 생성
        //public static DestroyManager Instance
        //{
        //    get
        //    {
        //        if (ReferenceEquals(instance, null))
        //        {
        //            instance = FindObjectOfType<DestroyManager>();

        //            if (ReferenceEquals(instance, null))
        //            {
        //                GameObject tmp = new GameObject();
        //                instance = tmp.AddComponent<DestroyManager>();
        //                tmp.name = "-=DestroyManager=-";
        //            }
                    
        //        }
        //        return instance;
        //    }
        //}
        
        public List<GameObject> destoryGameObjectList = new List<GameObject>();//파괴할 오브젝트 리스트

        /// <summary> 삭제할 오브젝트 수 (기본값 5개) </summary>
        public int destroyCount = 2;
        //{
        //    get
        //    {
        //        return destroyCount;
        //    }
        //    set
        //    {
        //        if (value <= 0 )
        //        {
        //            value = 1;
        //        }
        //        destroyCount = value;
        //    }
        //}
        /// <summary>업데이트할 타임세팅용(기본값 10초)</summary>
        public float timer = 2f;
        //{
        //    get
        //    {
        //        return timer;
        //    }
        //    set
        //    {
        //        if (value <= 0)
        //        {
        //            value = 5;
        //        }
        //        timer = value;
        //    }
        //}

        //캐싱변수 구역
        private GameObject _targetDestroy;//파괴할 타겟오브젝트//캐싱용
        //private IDestoryTarget _destoryTarget;//파괴할 타겟오브젝트의 인터페이스 //캐싱용
        private int tempCount = 0;//캐싱용
        private Transform tr;//캐싱용
                                    //destrory만 메모리제거가 안되고
                                    //해당 오브젝트의 참조카운트를 리셋시켜서 GC가 볼수 있게 해줘야함
                                    //REF 로 파괴.? 좀더 체크해볼것
                                    //파괴를 해도 참조를 하므로 GC가 체크를 못함. 계층에서만 삭제됨.
                                    //참조하는걸 다 비게 만들것->>나중에 시간날떄 해야함
        private WaitForSeconds waitForSeconds;

        protected override void Awake()
        {
            base.Awake();
            waitForSeconds = new WaitForSeconds(timer);
            StartCoroutine(UpdateDestroyManager());
            tr = transform;
            lLcroweUtil.DontDestroyTargetObject(gameObject);
        }

        /// <summary> 업데이트 모듈에 집어넣는것</summary>
        private IEnumerator UpdateDestroyManager()
        {
            do
            {
                tempCount = 0;
                for (int i = 0; i < destoryGameObjectList.Count; i++)
                {
                    if (tempCount >= destroyCount)
                    {   
                        break;
                    }

                    _targetDestroy = destoryGameObjectList[i];
                    if (!ReferenceEquals(_targetDestroy, null))
                    {
                        destoryGameObjectList.Remove(_targetDestroy);

                        //118번줄 참고할것
                        //if (_targetDestroy.TryGetComponent(out _destoryTarget))
                        //{
                        //    _destoryTarget.DestroyConnectRef();
                        //}

                        Destroy(_targetDestroy, GetRandomDestroyTime());

                        _targetDestroy = null;
                        //_destoryTarget = null;
                        tempCount++;
                    }
                }
                yield return waitForSeconds;
            } while (true);
        }

        /// <summary>
        /// 안쓰는 오브젝트들의 매모리를 할당해제 시키는 함수 
        /// 사용처 => 로딩스크린창에서 로딩이 다끝난후 사용
        /// </summary>
        public static void AllocateForMemory()
        {
            //안쓰는 오브젝트들의 매모리를 할당해제시켜버림
            Resources.UnloadUnusedAssets();//<==로딩할떄 사용할것 그때용도에 알맞다
            System.GC.Collect();
        }

        /// <summary>
        ///파괴시킬오브젝트를 추가시키는 함수 
        /// </summary>
        /// <param name="_targetObject">파괴할 오브젝트</param>
        public void AddDestoryGameObject(GameObject _targetObject)
        {
            if (ReferenceEquals(_targetObject, null))
            {
                return;
            }

            if (!destoryGameObjectList.Contains(_targetObject))
            {
                _targetObject.transform.SetParent(tr);
                destoryGameObjectList.Add(_targetObject);
                _targetObject.SetActive(false);
            }
        }

        /// <summary>
        /// 넣자마자 비활성화가 아닌 넣고나서 랜덤한 시간에 비활성화후 파괴시킴
        /// </summary>
        /// <param name="_targetObject"></param>
        public void AddWaitDestoryGameObject(GameObject _targetObject)
        {
            if (!destoryGameObjectList.Contains(_targetObject))
            {
                _targetObject.SetActive(false);
                _targetObject.transform.SetParent(tr);
                StartCoroutine(WaitDeactiveGameObject(_targetObject));
            }
        }
        private IEnumerator WaitDeactiveGameObject(GameObject _gameObject)
        {
            //float tempIndex = Random.Range(1f, 10f);
            //yield return Timing.WaitForSeconds(tempIndex / (Mathf.Round(TimerModuleManager.Instance.GetUpdateTimerScale() * 100) / 100));//느려보임
            //yield return new WaitForSeconds((tempIndex * (Time.timeScale * 100f)) * 0.01f);//신규
            //(5*(1*100))*0.01f = 5sec
            yield return waitForSeconds;
            destoryGameObjectList.Add(_gameObject);
        }


        /// <summary>
        /// 랜덤값을 가져와서 준다.
        /// (참조하는건 최대 부수는 카운트)
        /// </summary>
        /// <returns></returns>
        private float GetRandomDestroyTime()
        {    
            float _time = Random.Range(0, destroyCount);
            return _time;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            destoryGameObjectList.Clear();
            _targetDestroy = null;
            tr = null;
            StopAllCoroutines();
        }

        /// <summary>
        /// IDestoryTarget을 상속했을경우
        /// 연결되있는 참조값들을 Null값으로 해주는 행위를 해야함
        /// 그래야지만 가비지컬렉터에서 필요없는 메모리를 더욱 잘 회수함
        /// 118번줄 체크할것
        /// </summary>
        //public interface IDestoryTarget
        //{   
        //    /// <summary>
        //    /// 해당 유닛과 연결되있는 모든 참조하는 값들을  Null 값으로 만들어줘야함
        //    /// 그외에는 밸류값들을 Default로 해주는 방법이 있음
        //    /// </summary>
        //    void DestroyConnectRef();
        //    //코드 예시
        //    //
        //}
    }
}

