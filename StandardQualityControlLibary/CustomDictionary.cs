using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace lLCroweTool.Dictionary
{
    //[System.Serializable]
    //public struct KeyValueStruct<T1, T2>
    //{
    //    [SerializeField] private T1 key;
    //    [SerializeField] private T2 value;

    //    public KeyValueStruct(T1 key, T2 value)
    //    {
    //        this.key = key;
    //        this.value = value;
    //    }
    //    public KeyValueStruct(KeyValuePair<T1, T2> value)
    //    {
    //        this.key = value.Key;
    //        this.value = value.Value;
    //    }

    //    public T1 Key
    //    {
    //        get => key;
    //    }

    //    public T2 Value
    //    {
    //        get => value;
    //        set => this.value = value;
    //    }
        
    //}

    /// <summary>
    /// 커스텀딕셔너리, 시리얼 라이징가능하게 제작됨
    /// </summary>
    /// <typeparam name="T1">키 타입</typeparam>
    /// <typeparam name="T2">밸류 타입</typeparam>
    [System.Serializable]//이게있어야 프로퍼티드로우를 할수 있음//시리얼라이징되버림
    public class CustomDictionary<T1, T2> : Dictionary<T1, T2>, ISerializationCallbackReceiver
    {
        //#if UNITY_EDITOR
        //        public KeyValueStruct<T1, T2> editorKeyValue;
        //#endif

        //[SerializeField] public List<KeyValueStruct<T1,T2>> keyValueList = new List<KeyValueStruct<T1, T2>> ();

        //이거 변경//그냥이게 제일났다//프로퍼티드로우버려//시도 꽤해봣는데 안나옴//20230607
        [SerializeField] private List<T1> keyList = new List<T1>();//키값
        [SerializeField] private List<T2> valueList = new List<T2>();//밸류값

        //ISerializationCallbackReceiver는
        //OnBeforeSerialize는 데이터를 저장할 때 호출
        //OnAfterDeserialize는 데이터를 불러올 때 호출
        /// <summary>
        /// 데이터를 저장할떄 호출하는 함수
        /// ISerializationCallbackReceiver 인터페이스함수
        /// </summary>
        public void OnBeforeSerialize()
        {
            keyList.Clear();
            valueList.Clear();

            //new가 싫다면 밑에걸로
            //keyList = new List<T1>(Keys);
            //valueList = new List<T2>(Values);

            foreach (var item in this) 
            {
                keyList.Add(item.Key);
                valueList.Add(item.Value);
            }

            //keyValueList.Clear();
            //foreach (var item in this)
            //{
            //    var data = new KeyValueStruct<T1, T2>(item);
            //    keyValueList.Add(data);
            //}
        }

        /// <summary>
        /// 데이터를 불려올때 호출하는 함수
        /// ISerializationCallbackReceiver 인터페이스함수
        /// </summary>
        public void OnAfterDeserialize()
        {
            Clear();

            for (int i = 0; i < keyList.Count; i++)
            {
                Add(keyList[i], valueList[i]);
            }

            //for (int i = 0; i < keyValueList.Count; i++)
            //{
            //    var data = keyValueList[i];
            //    if (ContainsKey(data.Key))
            //    {
            //        Debug.Log("동일한키값이 존재합니다");
            //        continue;
            //    }
            //    Add(data.Key, data.Value);
            //}
        }

        /// <summary>
        /// 딕셔너리자료들을 인스펙터 리스트에 맞춤
        /// </summary>
        public void SyncDictionaryToList()
        {
            //리스트 초기화
            OnBeforeSerialize();
        }

        /// <summary>
        /// 인스펙터 리스트들을 딕셔너리에 맞춤
        /// </summary>
        public void SyncListToDictionary()
        {
            //딕셔너리 초기화
            Clear();

            for (int i = 0; i < keyList.Count; i++)
            {
                var data = keyList[i];
                //중복된 키가 있다면 에러 출력
                if (ContainsKey(data))
                {
                    Debug.LogError("중복된 키가 있습니다.");
                    //넘어감
                    continue;
                }
                Add(data, valueList[i]);
            }
        }

        //리스트관련
        public List<T1> GetKeyList()
        {
            return keyList;
        }

        public List<T2> GetValueList()
        {
            return valueList;
        }
    }

    //사용법
    //여기서 [System.Serializable] 안붙히면 작동안됨 그리고 필드에다도 똑같이 SerializeField 붙여야지 작동됨
    //[System.Serializable]
    //public class TestBible : CustomDictionary<string, string> { }
    //TestBible testBible = new TestBible();
}
