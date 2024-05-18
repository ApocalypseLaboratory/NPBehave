using System.Collections.Generic;
using System;

namespace NPSerialization
{
    public class InstanceContext
    {
        static InstanceContext instance;

        public static InstanceContext Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new InstanceContext();
                }
                return instance;
            }
        }

        public bool RegisterReference(object obj, long id)
        {
            if (obj == null)
                return false;

            Type type = obj.GetType();
            if (!m_TypeToWeakReferenceDict.ContainsKey(type))
            {
                m_TypeToWeakReferenceDict[type] = new();
            }
            m_TypeToWeakReferenceDict[type][id] = new WeakReference(obj);

            return true;
        }

        public bool TryGetReference(Type type, long id, out object reference)
        {
            reference = null;
            if (m_TypeToWeakReferenceDict.ContainsKey(type))
            {
                if (m_TypeToWeakReferenceDict[type].ContainsKey(id))
                {
                    WeakReference weakReference = m_TypeToWeakReferenceDict[type][id];
                    if (weakReference.IsAlive)
                    {
                        reference = weakReference.Target;
                        return true;
                    }
                }
            }

            return false;
        }

        private Dictionary<Type, Dictionary<long, WeakReference>> m_TypeToWeakReferenceDict = new();
    }
}