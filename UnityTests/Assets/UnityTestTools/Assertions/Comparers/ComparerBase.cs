using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

namespace UnityTest
{
    public abstract class ComparerBase : ActionBase
    {
        public enum CompareToType
        {
            CompareToObject,
            CompareToConstantValue,
            CompareToNull
        }

        public CompareToType compareToType = CompareToType.CompareToObject;

        public GameObject other;
        protected object m_ObjOtherVal;
        public string otherPropertyPath = "";
        private MemberResolver m_MemberResolverB;

        protected abstract bool Compare(object a, object b);

        protected override bool Compare(object objValue)
        {
            if (compareToType == CompareToType.CompareToConstantValue)
            {
                m_ObjOtherVal = ConstValue;
            }
            else if (compareToType == CompareToType.CompareToNull)
            {
                m_ObjOtherVal = null;
            }
            else
            {
                if (other == null)
                    m_ObjOtherVal = null;
                else
                {
                    if (m_MemberResolverB == null)
                        m_MemberResolverB = new MemberResolver(other, otherPropertyPath);
                    m_ObjOtherVal = m_MemberResolverB.GetValue(UseCache);
                }
            }
            return Compare(objValue, m_ObjOtherVal);
        }

        public virtual Type[] GetAccepatbleTypesForB()
        {
            return null;
        }

        #region Const value

        public virtual object ConstValue { get; set; }
        public virtual object GetDefaultConstValue()
        {
            throw new NotImplementedException();
        }

        #endregion

        public override string GetFailureMessage()
        {
            var message = GetType().Name + " assertion failed.\n" + go.name + "." + thisPropertyPath + " " + compareToType;
            switch (compareToType)
            {
                case CompareToType.CompareToObject:
                    message += " (" + other + ")." + otherPropertyPath + " failed.";
                    break;
                case CompareToType.CompareToConstantValue:
                    message += " " + ConstValue + " failed.";
                    break;
                case CompareToType.CompareToNull:
                    message += " failed.";
                    break;
            }
            message += " Expected: " + m_ObjOtherVal + " Actual: " + m_ObjVal;
            return message;
        }
    }

    [Serializable]
    public abstract class ComparerBaseGeneric<T> : ComparerBaseGeneric<T, T>
    {
    }

    [Serializable]
    public abstract class ComparerBaseGeneric<T1, T2> : ComparerBase
    {
        public T2 constantValueGeneric = default(T2);

        public override Object ConstValue
        {
            get
            {
                return constantValueGeneric;
            }
            set
            {
                constantValueGeneric = (T2)value;
            }
        }

        public override Object GetDefaultConstValue()
        {
            return default(T2);
        }

        static bool IsValueType(Type type)
        {
#if !UNITY_METRO
            return type.IsValueType;
#else
            return false;
#endif
        }

        protected override bool Compare(object a, object b)
        {
            var type = typeof(T2);
            if (b == null && IsValueType(type))
            {
                throw new ArgumentException("Null was passed to a value-type argument");
            }
            return Compare((T1)a, (T2)b);
        }

        protected abstract bool Compare(T1 a, T2 b);

        public override Type[] GetAccepatbleTypesForA()
        {
            return new[] { typeof(T1) };
        }

        public override Type[] GetAccepatbleTypesForB()
        {
            return new[] {typeof(T2)};
        }

        protected override bool UseCache { get { return true; } }
    }
}
