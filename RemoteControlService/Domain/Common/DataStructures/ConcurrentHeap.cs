using System;
using System.Threading;

namespace Domain.Common.DataStructures
{
    class ConcurrentHeap<T>
    {
        const int DEFAULT_LENGTH = 2;
        readonly ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();
        readonly Heap<T> heap;

        public ConcurrentHeap(Func<T, T, bool> swapPredicate) :
            this(DEFAULT_LENGTH, swapPredicate)
        { }
        public ConcurrentHeap(int length, Func<T, T, bool> swapPredicate)
        {
            heap = new Heap<T>(length, swapPredicate);
        }

        public int Count
        {
            get
            {
                readerWriterLock.EnterReadLock();
                try
                {
                    return heap.Count;
                }
                finally
                {
                    readerWriterLock.ExitReadLock();
                }
            }
        }
        public void Add(T value)
        {
            readerWriterLock.EnterWriteLock();
            try
            {
                heap.Add(value);
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
        }
        public bool TryPeek(out T top)
        {
            top = default;
            readerWriterLock.EnterReadLock();
            try
            {
                return heap.TryPeek(out top);
            }
            finally
            {
                readerWriterLock.ExitReadLock();
            }
        }
        public bool TryExtractTop(out T top)
        {
            top = default;
            readerWriterLock.EnterWriteLock();
            try
            {
                return heap.TryExtractTop(out top);
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
        }
        public void Clear()
        {
            readerWriterLock.EnterWriteLock();
            try
            {
                heap.Clear();
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
        }
    }
}
