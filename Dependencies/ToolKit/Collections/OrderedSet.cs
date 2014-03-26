using System.Collections;
using System.Collections.Generic;

namespace ToolKit {
    public class OrderedSet<T> : ICollection<T>, IEnumerable<T> 
    #if NET_4_0
    , ISet<T>
    #endif
    {
        readonly HashSet<T> _hashSet;
        LinkedList<T> _list;

        public OrderedSet() {
            _hashSet = new HashSet<T>();
            _list = new LinkedList<T>();
        }

        public OrderedSet(IEnumerable<T> collection) {
            _hashSet = new HashSet<T>(collection);
            _list = new LinkedList<T>(_hashSet);
        }

        #region ISet implementation

        public bool Add(T item) {
            var added = _hashSet.Add(item);
            if (added)
                _list.AddLast(item);

            return added;
        }

        public void ExceptWith(IEnumerable<T> other) {
            _hashSet.ExceptWith(other);
            _list = new LinkedList<T>(_hashSet);
        }

        public void IntersectWith(IEnumerable<T> other) {
            _hashSet.IntersectWith(other);
            _list = new LinkedList<T>(_hashSet);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other) {
            return _hashSet.IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other) {
            return _hashSet.IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<T> other) {
            return _hashSet.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other) {
            return _hashSet.IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other) {
            return _hashSet.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other) {
            return _hashSet.SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<T> other) {
            _hashSet.SymmetricExceptWith(other);
            _list = new LinkedList<T>(_hashSet);
        }

        public void UnionWith(IEnumerable<T> other) {
            _hashSet.UnionWith(other);
            _list = new LinkedList<T>(_hashSet);
        }

        #endregion

        #region ICollection implementation

        void ICollection<T>.Add(T item) {
            if (_hashSet.Add(item))
                _list.AddLast(item);
        }

        public void Clear() {
            _hashSet.Clear();
            _list.Clear();
        }

        public bool Contains(T item) {
            return _hashSet.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item) {
            var removed = _hashSet.Remove(item);
            if (removed)
                _list.Remove(item);

            return removed;
        }

        public int Count { get { return _hashSet.Count; } }

        public bool IsReadOnly { get { return false; } }

        #endregion

        #region IEnumerable implementation

        public IEnumerator<T> GetEnumerator() {
            return _list.GetEnumerator();
        }

        #endregion

        #region IEnumerable implementation

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion

        public T First() {
            return _list.First.Value;
        }

        public T[] ToArray() {
            var array = new T[Count];
            CopyTo(array, 0);
            return array;
        }
    }
}
