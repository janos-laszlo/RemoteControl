using System;
using System.Collections.Generic;

namespace Domain.Common.DataStructures
{
    class Heap<T>
    {
        const int ARRAY_DEFAULT_LENGTH = 100;
        readonly Func<T, T, bool> swapPredicate;
        T[] itemsArray;
        int itemCount;

        public Heap(Func<T, T, bool> swapPredicate) :
            this(ARRAY_DEFAULT_LENGTH, swapPredicate)
        { }
        public Heap(int arrayLength, Func<T, T, bool> swapPredicate)
        {
            itemsArray = new T[arrayLength];
            this.swapPredicate = swapPredicate;
            itemCount = 0;
        }

        public int Count
        {
            get
            {
                return itemCount;
            }
        }
        public void Add(T value)
        {
            if (itemCount >= itemsArray.Length)
            {
                GrowBackingArray();
            }
            itemsArray[itemCount] = value;
            FloatUp(itemCount++);
        }
        public bool TryPeek(out T top)
        {
            top = default;
            if (itemCount > 0)
            {
                top = itemsArray[0];
                return true;
            }
            return false;
        }
        public bool TryExtractTop(out T top)
        {
            top = RemoveAt(0);
            return !EqualityComparer<T>.Default.Equals(top, default);
        }
        public void Clear()
        {
            itemCount = 0;
            itemsArray = new T[itemsArray.Length];
        }

        private void FloatUp(int index)
        {
            while (index > 0 && swapPredicate(itemsArray[index], itemsArray[Parent(index)]))
            {
                Swap(index, Parent(index));
                index = Parent(index);
            }
        }
        private void FloatDown(int index)
        {
            while (index < itemCount)
            {
                // Get the left and right child indexes.
                int left = Left(index);
                int right = Right(index);
                // Make sure we are still within the heap.
                if (left >= itemCount)
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
                if (swapPredicate(itemsArray[index], itemsArray[extremeChildIndex]))
                {
                    // The current item is larger/smaller than its children (heap property is satisfied).
                    break;
                }
                Swap(index, extremeChildIndex);
                index = extremeChildIndex;
            }
        }
        private T RemoveAt(int index)
        {
            if (index < 0 || index >= itemCount) return default;
            T result = itemsArray[index];
            itemsArray[index] = itemsArray[itemCount - 1];
            --itemCount;
            FloatDown(index);
            return result;
        }
        private int Parent(int index)
        {
            return (index - 1) / 2;
        }
        private int Left(int index)
        {
            return 2 * index + 1;
        }
        private int Right(int index)
        {
            return 2 * index + 2;
        }
        private int ExtremeChildIndex(int left, int right)
        {
            // Find the index of the child with the largest/smallest value.
            if (right >= itemCount)
            {
                // No right child.
                return left;
            }
            return swapPredicate(itemsArray[left], itemsArray[right]) ? left : right;
        }
        private void GrowBackingArray()
        {
            T[] newItems = new T[itemsArray.Length * 2];
            for (int i = 0; i < itemsArray.Length; ++i)
            {
                newItems[i] = itemsArray[i];
            }
            itemsArray = newItems;
        }
        private void Swap(int left, int right)
        {
            T temp = itemsArray[left];
            itemsArray[left] = itemsArray[right];
            itemsArray[right] = temp;
        }
    }
}
