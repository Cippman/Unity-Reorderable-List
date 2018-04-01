using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CippSharp.Reorderable
{

    [Serializable]
    public abstract class ReorderableArray<T> : ICloneable, IList<T>, ICollection<T>, IEnumerable<T>
    {

        [SerializeField]
        protected T[] array = new T[0];

        public ReorderableArray()
            : this(0)
        {
        }

        public ReorderableArray(int length)
        {
            array = new T[length];
        }

        public virtual T this [int index]
        {

            get { return array[index]; }
            set { array[index] = value; }
        }

        public virtual int Length
        {

            get { return array.Length; }
        }

        public virtual bool IsReadOnly
        {

            get { return array.IsReadOnly; }
        }

        public virtual int Count
        {

            get { return array.Length; }
        }

        public virtual object Clone()
        {

            return array.Clone();
        }

        public virtual bool Contains(T value)
        {

            return Array.IndexOf(array, value) >= 0;
        }

        public virtual int IndexOf(T value)
        {

            return Array.IndexOf(array, value);
        }

        public virtual void Insert(int index, T item)
        {

            ((IList<T>)array).Insert(index, item);
        }

        public virtual void RemoveAt(int index)
        {

            ((IList<T>)array).RemoveAt(index);
        }

        public virtual void Add(T item)
        {

            ((IList<T>)array).Add(item);
        }

        public virtual void Clear()
        {

            ((IList<T>)array).Clear();
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {

            ((IList<T>)this.array).CopyTo(array, arrayIndex);
        }

        public virtual bool Remove(T item)
        {

            return ((IList<T>)array).Remove(item);
        }

        public virtual IEnumerator<T> GetEnumerator()
        {

            return ((IList<T>)array).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {

            return ((IList<T>)array).GetEnumerator();
        }

        public static implicit operator Array(ReorderableArray<T> reorderableArray)
        {

            return reorderableArray.array;
        }

        public static implicit operator T[](ReorderableArray<T> reorderableArray)
        {

            return reorderableArray.array;
        }
    }
}