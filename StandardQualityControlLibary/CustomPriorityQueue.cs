using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace lLCroweTool.PriorityQueue
{
    /// <summary>
    /// 우선순위 큐 Heap으로 제작 (Log2n)
    /// 큐에서 집어넣을때와 뺄때 정렬됨
    /// </summary>
    /// <typeparam name="T">데이터 타입</typeparam>
    public class CustomPriorityQueue<T>
    {
        //안에 있는걸 수정하지않은 상태로 
        //신규데이터를 집어넣은다음 안에 있는걸 수정하면
        //당연히 정렬이 안된다.


        private List<T> itemList;
        private IComparer<T> comparer;
        

        /// <summary>
        /// 커스텀 우선순위 큐 생성자(heap사용)
        /// </summary>
        /// <param name="comparer">정렬 방식</param>
        public CustomPriorityQueue(IComparer<T> comparer, int capacity = 10)
        {   
            this.comparer = comparer;
            itemList = new(capacity);
        }

        /// <summary>
        /// 인큐
        /// </summary>
        /// <param name="item">타입에 따른 데이터아이템</param>
        public void Enqueue(T item)
        {
            itemList.Add(item);
            int childIndex = itemList.Count - 1;

            while (childIndex > 0)
            {
                int parentIndex = (childIndex - 1) / 2;

                if (comparer.Compare(itemList[childIndex], itemList[parentIndex]) >= 0)
                    break;

                T tmp = itemList[childIndex];
                itemList[childIndex] = itemList[parentIndex];
                itemList[parentIndex] = tmp;

                childIndex = parentIndex;
            }
        }

        /// <summary>
        /// 디큐
        /// </summary>
        /// <returns>타입에 따른 데이터아이템</returns>
        public T Dequeue()
        {
            int lastIndex = itemList.Count - 1;
            T frontItem = itemList[0];
            itemList[0] = itemList[lastIndex];
            itemList.RemoveAt(lastIndex);

            lastIndex--;

            int parentIndex = 0;

            while (true)
            {
                int leftChildIndex = parentIndex * 2 + 1;
                int rightChildIndex = parentIndex * 2 + 2;

                if (leftChildIndex > lastIndex)
                    break;

                int minIndex = leftChildIndex;

                if (rightChildIndex <= lastIndex && comparer.Compare(itemList[rightChildIndex], itemList[leftChildIndex]) < 0)
                    minIndex = rightChildIndex;

                if (comparer.Compare(itemList[parentIndex], itemList[minIndex]) <= 0)
                    break;

                T tmp = itemList[parentIndex];
                itemList[parentIndex] = itemList[minIndex];
                itemList[minIndex] = tmp;

                parentIndex = minIndex;
            }

            return frontItem;
        }

        /// <summary>
        /// 우선순위 큐에 쌓인 데이터수량을 가져옴
        /// </summary>
        public int Count => itemList.Count;

        /// <summary>
        /// 우선순위 큐를 비워버림
        /// </summary>
        public void Clear()
        {
            itemList.Clear();
        }

        /// <summary>
        /// 아이템이 존재하는지 체크하는 함수
        /// </summary>
        /// <param name="item">찾을 아이템</param>
        /// <returns>찾았는지 여부</returns>
        public bool Contains(T item)
        {   
            return itemList.Contains(item);
        }

        public override string ToString()
        {
            string content ="-=우선순위큐=-\n";
            for (int i = 0; i < itemList.Count; i++)
            {
                content = lLcroweUtil.GetCombineString(content, $"{itemList[i]} \n");
            }
            content = lLcroweUtil.GetCombineString(content, $"{itemList.Count}개");

            return content;
        }

        public void Sort()
        {
            itemList.Sort(comparer);
        }

        public List<T> GetList()
        {
            return itemList;
        }
    }
}
