using System;
using System.Threading;

namespace Domain.Common.DataStructures
{
    public class ConcurrentHeap<T>
    {
        const int DEFAULT_LENGTH = 100;
        readonly ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();
        readonly Func<T, T, bool> swapPredicate;
        T[] _items;
        int _count;
        public ConcurrentHeap(Func<T, T, bool> swapPredicate) : this(DEFAULT_LENGTH, swapPredicate) { }
        public ConcurrentHeap(int length, Func<T, T, bool> swapPredicate)
        {
            this.swapPredicate = swapPredicate;
            _items = new T[length];
            _count = 0;
        }
        public int Count
        {
            get
            {
                readerWriterLock.EnterReadLock();
                try
                {
                    return _count;
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
                if (_count >= _items.Length)
                {
                    GrowBackingArray();
                }
                _items[_count] = value;
                int index = _count;
                while (index > 0 && swapPredicate(_items[index], _items[Parent(index)]))
                {
                    Swap(index, Parent(index));
                    index = Parent(index);
                }
                ++_count;
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
                if (_count > 0)
                {
                    top = _items[0];
                    return true;
                }
                return false;
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
                if (_count <= 0)
                {
                    return false;
                }
                top = _items[0];
                _items[0] = _items[_count - 1];
                --_count;
                int index = 0;
                while (index < _count)
                {
                    // Get the left and right child indexes.
                    int left = 2 * index + 1;
                    int right = 2 * index + 2;
                    // Make sure we are still within the heap.
                    if (left >= _count)
                    {
                        break;
                    }
                    // To avoid having to swap twice, we swap with the largest/smallest value.
                    // E.g.,
                    // 5
                    // 6 8
                    // If we swapped with 6 first we'd have
                    // 6
                    // 5 8
                    // and we'd require another swap to get the desired tree.
                    // 8
                    // 6 5
                    // So we find the largest/smallest child and just do the right thing at the start.
                    int extremeChildIndex = ExtremeChildIndex(left, right);
                    if (swapPredicate(_items[index], _items[extremeChildIndex]))
                    {
                        // The current item is larger/smaller than its children (heap property is satisfied).
                        break;
                    }
                    Swap(index, extremeChildIndex);
                    index = extremeChildIndex;
                }
                return true;
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
                _count = 0;
                _items = new T[DEFAULT_LENGTH];
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
        }

        private int ExtremeChildIndex(int left, int right)
        {
            // Find the index of the child with the largest/smallest value.
            if (right >= _count)
            {
                // No right child.
                return left;
            }
            else
            {
                if (swapPredicate(_items[left], _items[right]))
                {
                    return left;
                }
                else
                {
                    return right;
                }
            }
        }
        private void GrowBackingArray()
        {
            T[] newItems = new T[_items.Length * 2];
            for (int i = 0; i < _items.Length; ++i)
            {
                newItems[i] = _items[i];
            }
            _items = newItems;
        }
        private int Parent(int index)
        {
            return (index - 1) / 2;
        }
        private void Swap(int left, int right)
        {
            T temp = _items[left];
            _items[left] = _items[right];
            _items[right] = temp;
        }
    }
}
