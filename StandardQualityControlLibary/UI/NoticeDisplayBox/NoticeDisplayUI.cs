using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using lLCroweTool.NoticeDisplay.Extend;
using DG.Tweening;

namespace lLCroweTool.NoticeDisplay
{   
    public class NoticeDisplayUI : MonoBehaviour
    {
        //알람이 왔을때 떠버리는거 체크
        //꽤 여러종류가 있다
        //여기는 뭘로 할것인가

        //1.무한스크롤을 이용해서 지금까지 쌓아온 여러가지 보여주는거
        //=>확장으로 만들어버림
        //2.현재들어온거 슝슝보여주기 
        //=>제작완료 + 에디터도 완성

        /// <summary>
        /// 방향타입
        /// </summary>
        public enum EDirectionMoveType
        {
            Left,
            Right, 
            Up, 
            Down,
            Origin,
        }

        /// <summary>
        /// 두트윈세팅
        /// </summary>
        [System.Serializable]
        public class DoSetting
        {
            //로컬기준
            [Header("스케일")]
            public Ease scaleEase;
            public Vector2 startSize = Vector2.one;
            public Vector2 endSize = Vector2.one;
            public float scaleSpeed = 1;

            [Header("배치위치")]
            public Ease moveEase;
            public Vector2 batchPos;
            public float moveSpeed = 1;

            [Header("페이드")]
            public Ease fadeEase;
            public float startFade = 1;
            public float endFade = 1;
            public float fadeSpeed = 1;

        }

        [Header("기본 설정")]        
        public NoticeUI noticeUIPrefab;

        [Space]
        [Header("Show세팅")]
        public DoSetting showSetting = new DoSetting();

        [Space]
        [Header("UI보여질때 세팅")]        
        public EDirectionMoveType showMoveType = EDirectionMoveType.Up;
        public int showMaxCount = 4;//최대로 보여줄대상
        private int curShowCount = 0;

        public bool isOrderTextShow = true;
        public float showTime = 2f;//몇초동안 보여줄건지
        public float showTimeForManyObject = 1;
        private float showTimer;//캐시//체크용

        public float distance = 200f;
        public float moveSpeed = 5f;

        [Space]
        [Header("Hide세팅")]
        public DoSetting hideSetting = new DoSetting();

        [Header("확장")]
        public NoticeDisplayUIExtend_Base extend;

        //나중에 아이콘이나 다른알람 이미지를 사용할때를 위한코드
        //public struct IconTag
        //{
        //    public Sprite icon;
        //    public string id;
        //}

        //public IconTag[] iconTag;
        //public CustomDictionary<string, Sprite> iconBible = new CustomDictionary<string, Sprite>();


        [System.Serializable]
        public class NoticeMessageQueue : QueueEventModule<string> {}
        public NoticeMessageQueue noticeMessageQueue = new NoticeMessageQueue();
      
        private List<NoticeUI> onEnableObjectList = new List<NoticeUI>();        
        private WaitForSeconds showWaitForSeconds;
        private WaitForSeconds hideWaitForSeconds;
        

        protected void Awake()
        {   
            showWaitForSeconds = new WaitForSeconds(showSetting.fadeSpeed);
            hideWaitForSeconds = new WaitForSeconds(hideSetting.fadeSpeed);
        }

        private void OnDisable()
        {
            for (int i = 0; i < onEnableObjectList.Count; i++)
            {
                onEnableObjectList[i].SetActive(false);
            }
            onEnableObjectList.Clear();
            curShowCount = 0;
        }

        private void Update()
        {
            UpdateMessage();
            UpdateAnimationTransform();
        }

        /// <summary>
        /// 들어온 메시지를 갱신해주는 함수
        /// </summary>
        private void UpdateMessage()
        {
            //존재하고 시간됫는지 체크
            if (!noticeMessageQueue.CheckActionTime())
            {
                return;
            }

            //최대치 이상있는지
            if (curShowCount >= showMaxCount)
            {
                return;
            }
                        
            string content = noticeMessageQueue.Dequeue();
            noticeMessageQueue.TimeReset();

            //확장기능체크
            if (!ReferenceEquals(extend , null))
            {
                if (!extend.SendContentMessage(content))
                {
                    return;
                }
            }

            //소환하기
            NoticeUI noticeUIBox = ObjectPoolManager.Instance.RequestDynamicComponentObject(noticeUIPrefab);
            noticeUIBox.transform.SetParent(transform);
            noticeUIBox.transform.SetAsLastSibling();//맨 마지막으로 내려서 제일 먼저 보이게

            //noticeUIBox.transform.InitTrObjPrefab(transform.position + (GetDirectionNorVector(showDirectionMoveType) * distance * onEnableObjectList.Count), Quaternion.identity, showPos);
            noticeUIBox.ShowNoticeUI(content, showSetting,(GetDirectionNorVector(showMoveType) * distance * curShowCount));
            StartCoroutine(ShowNoticeUI(noticeUIBox));
        }

        /// <summary>
        /// 많은 메세지가 존재하면 보여줄 시간변경
        /// </summary>
        /// <returns>보여줄 시간</returns>
        private float GetShowTime()
        {
            return noticeMessageQueue.Count > showMaxCount ? showTimeForManyObject : showTime;
        }

        /// <summary>
        /// 들어온 알람UI들의 이동애니메이션을 담당하는 함수
        /// </summary>
        private void UpdateAnimationTransform()
        {
            Vector3 targetPos = transform.position;
            for (int i = 0; i < onEnableObjectList.Count; i++)
            {
                int index = i;
                NoticeUI noticeUIBox = onEnableObjectList[index];
                if (noticeUIBox == null)
                {
                    onEnableObjectList.RemoveAt(index);
                    i--;
                    continue;
                }

                //보여줄시간이 지났는지 체크
                bool check = false;
                if (isOrderTextShow) 
                {
                    check = Time.time > showTimer + GetShowTime() + showSetting.fadeSpeed;//일정시간동안
                }
                else
                {
                    check = Time.time > noticeUIBox.GetTime() + GetShowTime() + showSetting.fadeSpeed;//텍스트가 들어온 시간에 처리
                }

                if (check)
                {
                    //없애기
                    noticeUIBox.transform.SetAsFirstSibling();//맨 첫번째로 올려서 안보이게 하기
                    onEnableObjectList.RemoveAt(index);
                    showTimer = Time.time;
                    StartCoroutine(HideNoticeUI(noticeUIBox));
                    continue;
                }

                //if (curShowCount >= showMaxCount)
                //{
                //    return;
                //}

                //기준점에서 위로
                noticeUIBox.transform.position = Vector2.Lerp(noticeUIBox.transform.position, targetPos + (GetDirectionNorVector(showMoveType) * distance * index), Time.deltaTime * moveSpeed);
            }
        }

        /// <summary>
        /// 알람UI Show 코루틴
        /// </summary>
        /// <param name="noticeUIBox"></param>
        /// <returns></returns>
        private IEnumerator ShowNoticeUI(NoticeUI noticeUIBox)
        {
            curShowCount++;
            yield return showWaitForSeconds;
            onEnableObjectList.Add(noticeUIBox);
        }

        /// <summary>
        /// 알람UI Hide 코루틴
        /// </summary>
        /// <param name="noticeUIBox">정보UI</param>
        /// <returns></returns>
        private IEnumerator HideNoticeUI(NoticeUI noticeUIBox)
        {
            //Vector3 localPos = noticeUIBox.transform.localPosition;
            //localPos += GetDirectionNorVector(hideMoveType) * 500;
            
            noticeUIBox.HideNoticeUI(hideSetting);
            yield return hideWaitForSeconds;
            curShowCount--;
            noticeUIBox.SetActive(false);
        }

        /// <summary>
        /// 방향 타입에 따른 노말벡터를 가져오는 함수
        /// </summary>
        /// <param name="moveType">방향타입</param>
        /// <returns>노말벡터</returns>
        public static Vector3 GetDirectionNorVector(EDirectionMoveType moveType)
        {
            switch (moveType)
            {
                case EDirectionMoveType.Left:
                    return Vector2.left;                    
                case EDirectionMoveType.Right:
                    return Vector2.right;
                case EDirectionMoveType.Up:
                    return Vector2.up;
                case EDirectionMoveType.Down:
                    return Vector2.down;
                case EDirectionMoveType.Origin:
                    return Vector2.zero;
            }
            return Vector2.zero;
        }

        public static EDirectionMoveType ReverseMoveType(EDirectionMoveType moveType)
        {
            switch (moveType)
            {
                case EDirectionMoveType.Left:
                    return EDirectionMoveType.Right;                    
                case EDirectionMoveType.Right:
                    return EDirectionMoveType.Left;                    
                case EDirectionMoveType.Up:
                    return EDirectionMoveType.Down;
                case EDirectionMoveType.Down:
                    return EDirectionMoveType.Up;
            }
            return moveType;
        }

        /// <summary>
        /// 알람박스에 메세지를 집어넣는 함수
        /// </summary>
        /// <param name="content">메세지컨텐츠</param>
        public void AddNewMessage(string content)
        {
            content = lLcroweUtil.GetCombineString(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"), "\n", content);

            if (noticeMessageQueue.Count == 0)
            {
                showTimer = Time.time;
            }
            noticeMessageQueue.TimeReset();
            noticeMessageQueue.Enqueue(content);
        }
    }
}

