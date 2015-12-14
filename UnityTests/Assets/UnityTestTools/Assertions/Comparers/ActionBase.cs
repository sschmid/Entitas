using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnityTest
{
    public abstract class ActionBase : ScriptableObject
    {
        public GameObject go;
        protected object m_ObjVal;

        private MemberResolver m_MemberResolver;

        public string thisPropertyPath = "";
        public virtual Type[] GetAccepatbleTypesForA()
        {
            return null;
        }
        public virtual int GetDepthOfSearch() { return 2; }

        public virtual string[] GetExcludedFieldNames()
        {
            return new string[] { };
        }

        public bool Compare()
        {
            if (m_MemberResolver == null)
                m_MemberResolver = new MemberResolver(go, thisPropertyPath);
            m_ObjVal = m_MemberResolver.GetValue(UseCache);
            var result = Compare(m_ObjVal);
            return result;
        }

        protected abstract bool Compare(object objVal);

        virtual protected bool UseCache { get { return false; } }

        public virtual Type GetParameterType() { return typeof(object); }

        public virtual string GetConfigurationDescription()
        {
            string result = "";
#if !UNITY_METRO
            foreach (var prop in GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                     .Where(info => info.FieldType.IsSerializable))
            {
                var value = prop.GetValue(this);
                if (value is double)
                    value = ((double)value).ToString("0.########");
                if (value is float)
                    value = ((float)value).ToString("0.########");
                result += value + " ";
            }
#endif  // if !UNITY_METRO
            return result;
        }

        IEnumerable<FieldInfo> GetFields(Type type)
        {
#if !UNITY_METRO
            return type.GetFields(BindingFlags.Public | BindingFlags.Instance);
#else
            return null;
#endif
        }

        public ActionBase CreateCopy(GameObject oldGameObject, GameObject newGameObject)
        {
#if !UNITY_METRO
            var newObj = CreateInstance(GetType()) as ActionBase;
#else
            var newObj = (ActionBase) this.MemberwiseClone();
#endif
            var fields = GetFields(GetType());
            foreach (var field in fields)
            {
                var value = field.GetValue(this);
                if (value is GameObject)
                {
                    if (value as GameObject == oldGameObject)
                        value = newGameObject;
                }
                field.SetValue(newObj, value);
            }
            return newObj;
        }

        public virtual void Fail(AssertionComponent assertion)
        {
            Debug.LogException(new AssertionException(assertion), assertion.GetFailureReferenceObject());
        }

        public virtual string GetFailureMessage()
        {
            return GetType().Name + " assertion failed.\n(" + go + ")." + thisPropertyPath + " failed. Value: " + m_ObjVal;
        }
    }

    public abstract class ActionBaseGeneric<T> : ActionBase
    {
        protected override bool Compare(object objVal)
        {
            return Compare((T)objVal);
        }
        protected abstract bool Compare(T objVal);

        public override Type[] GetAccepatbleTypesForA()
        {
            return new[] { typeof(T) };
        }

        public override Type GetParameterType()
        {
            return typeof(T);
        }
        protected override bool UseCache { get { return true; } }
    }
}
