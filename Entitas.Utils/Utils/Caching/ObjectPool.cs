using System;
using System.Collections.Generic;

namespace Entitas.Utils {

    public class ObjectPool<T> {

        readonly Func<T> _factoryMethod;
        readonly Action<T> _resetMethod;
        readonly Stack<T> _objectPool;

        public ObjectPool(Func<T> factoryMethod, Action<T> resetMethod = null) {
            _factoryMethod = factoryMethod;
            _resetMethod = resetMethod;
            _objectPool = new Stack<T>();
        }

        public T Get() {
            return _objectPool.Count == 0
                ? _factoryMethod()
                : _objectPool.Pop();
        }

        public void Push(T obj) {
            if(_resetMethod != null) {
                _resetMethod(obj);
            }
            _objectPool.Push(obj);
        }
    }
}
