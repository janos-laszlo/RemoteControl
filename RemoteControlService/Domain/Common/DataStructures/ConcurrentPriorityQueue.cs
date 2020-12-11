using System;

namespace Domain.Common.DataStructures
{
    class ConcurrentPriorityQueue<T>
    {
        readonly ConcurrentHeap<T> heap;

        public ConcurrentPriorityQueue(Func<T, T, bool> swapPredicate)
        {
            heap = new ConcurrentHeap<T>(2, swapPredicate);
        }

        public void Enqueue(T value)
        {
            heap.Add(value);
        }

        public bool TryDequeue(out T top)
        {
            return heap.TryExtractTop(out top);
        }

        public bool TryPeek(out T topTask)
        {
            return heap.TryPeek(out topTask);
        }

        public void Clear()
        {
            heap.Clear();
        }

        public int Count
        {
            get
            {
                return heap.Count;
            }
        }
    }
}
