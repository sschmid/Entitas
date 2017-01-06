using System;
using System.Collections.Generic;

namespace Entitas {

    public class ObjectPool<T> {

        Func<T> _factoryMethod;
        Action<T> _resetMethod;
        Stack<T> _context;

        public ObjectPool(Func<T> factoryMethod, Action<T> resetMethod = null) {
            _factoryMethod = factoryMethod;
            _resetMethod = resetMethod;
            _context = new Stack<T>();
        }

        public T Get() {
            return _context.Count == 0
                ? _factoryMethod()
                : _context.Pop();
        }

        public void Push(T obj) {
            if(_resetMethod != null) {
                _resetMethod(obj);
            }
            _context.Push(obj);
        }
    }
}
