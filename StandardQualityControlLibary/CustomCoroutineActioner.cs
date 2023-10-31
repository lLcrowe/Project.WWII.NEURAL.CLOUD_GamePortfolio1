using System;
using System.Threading.Tasks;
using UnityEngine;

namespace lLCroweTool.QC.Coroutine
{
    public class CustomCoroutineActioner<T>
    {        
        //20230816//제작대기
        //코루틴 래핑해서 필요한 액션 작동되게하기 => 작업중
        //코루친 처리는 불값으로 작동되는지 체크 따로 이벤트등록 작동 되면 기존이벤트 삭제
        //타이밍이 너무 오래걸리면 취소처리
        //멀티스레딩을 위한 처리를 체크
        //외부 스레드에서 계산하고 계산한값을 그대로 가져다쓰는거

        //코루틴대신 태스크로 작업할예정


        public Func<T> action;

        private bool isAction;

        public void Action(Func<T> action)
        {
            if (isAction)
            {
                return;
            }
           
        }

        private async void Run(Func<T> action)
        {
            isAction = true;

            var task1 = Task.Run(() => action());
                        
            // task1이 끝나길 기다렸다가 끝나면 결과치를 sum에 할당
            T result = await task1;
            
        }


    }
}
