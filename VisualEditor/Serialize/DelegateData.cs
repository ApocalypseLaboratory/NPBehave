using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;
using static NPBehave.Action;

namespace NPSerialization
{
    public class DelegateData
    {
        [JsonIgnore]
        public Action m_action;
        [JsonIgnore]
        public Func<bool> m_singleFrameFunc;
        [JsonIgnore]
        public Func<bool, Result> m_multiFrameFunc;
        [JsonIgnore]
        public Func<Request, Result> m_multiFrameFunc2;

        [JsonIgnore]
        private string m_actionName = string.Empty;
        [JsonIgnore]
        private string m_singleFrameFuncName = string.Empty;
        [JsonIgnore]
        private string m_multiFrameFuncName = string.Empty;
        [JsonIgnore]
        private string m_multiFrameFunc2Name = string.Empty;

        public void SetDelegate(Action action)
        {
            string delegateString = GetSerializeString(action);
            m_actionName = delegateString;
        }

        public void SetDelegate(Func<bool> func)
        {
            string delegateString = GetSerializeString(func);
            m_singleFrameFuncName = delegateString;
        }

        public void SetDelegate(Func<bool, Result> func)
        {
            string delegateString = GetSerializeString(func);
            m_multiFrameFuncName = delegateString;
        }

        public void SetDelegate(Func<Request, Result> func)
        {
            string delegateString = GetSerializeString(func);
            m_multiFrameFunc2Name = delegateString;
        }

        public string ActionString
        {
            get
            {
                return m_actionName;
            }
            set
            {
                m_actionName = value;
            }
        }

        public string SingleFrameFuncString
        {
            get
            {
                return m_singleFrameFuncName;
            }
            set
            {
                m_singleFrameFuncName = value;
            }
        }

        public string MultiFrameFuncString
        {
            get
            {
                return m_multiFrameFuncName;
            }
            set
            {
                m_multiFrameFuncName = value;
            }
        }

        public string MultiFrameFunc2String
        {
            get
            {
                return m_multiFrameFunc2Name;
            }
            set
            {

                m_multiFrameFunc2Name = value;
            }
        }

        private string GetSerializeString<T>(T delegateFunction) where T : System.Delegate
        {
            if (delegateFunction == null)
                return null;

            if (delegateFunction.Method.ReflectedType == null)
                return null;

            if (delegateFunction.Method.IsStatic)
            {
                return $"{delegateFunction.Method.ReflectedType.FullName}|{delegateFunction.Method.Name}";
            }
            else
            {
                object instance = delegateFunction.Target;
                if (instance == null)
                    return null;

                Type type = instance.GetType();
                long IDObject = 0;
                object[] attributes = type.GetCustomAttributes(typeof(SerializationIDAttribute), false);

                foreach (var attribute in attributes)
                {
                    if (attribute is SerializationIDAttribute serializationID)
                    {
                        IDObject = serializationID.ID;
                        break;
                    }
                }

                return $"{instance.GetType().FullName}|{IDObject}|{delegateFunction.Method.Name}";
            }
        }

        public void CreateDelegate()
        {
            if (!string.IsNullOrEmpty(m_actionName))
            {
                m_action = GetDelegate<Action>(m_actionName);
            }
            else if (!string.IsNullOrEmpty(m_singleFrameFuncName))
            {
                m_singleFrameFunc = GetDelegate<Func<bool>>(m_singleFrameFuncName);
            }
            else if (!string.IsNullOrEmpty(m_multiFrameFuncName))
            {
                m_multiFrameFunc = GetDelegate<Func<bool, Result>>(m_multiFrameFuncName);
            }
            else if (!string.IsNullOrEmpty(m_multiFrameFunc2Name))
            {
                m_multiFrameFunc2 = GetDelegate<Func<Request, Result>>(m_multiFrameFunc2Name);
            }
        }

        private T GetDelegate<T>(string value) where T : System.Delegate
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            else
            {
                var parts = value.Split('|');
                string typeName = null;
                long ID = 0;
                string methodName = null;

                if (parts.Length == 2)
                {
                    typeName = parts[0];
                    methodName = parts[1];
                }
                else if (parts.Length == 3)
                {
                    typeName = parts[0];
                    ID = Convert.ToInt64(parts[1]);
                    methodName = parts[2];
                }

                var type = typeName != null ? Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.FullName == typeName) : null;

                if (type == null)
                {
                    throw new ArgumentException($"Type {typeName} not found.");
                }

                if (methodName == null)
                {
                    return null;
                }

                var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance) ?? throw new ArgumentException($"Static method {methodName} not found in type {typeName}.");
                if (!method.IsStatic)
                {
                    object instance;
                    if (ID == 0)
                    {
                        return null;
                    }

                    if (!InstanceContext.Instance.TryGetReference(type, ID, out instance))
                    {
                        if (typeof(T) == typeof(Action))
                        {
                            MethodInfo fakeMethodInfo = typeof(DelegateData).GetMethod(nameof(CheckAction));
                            return (T)Delegate.CreateDelegate(typeof(T), this, fakeMethodInfo);
                        }
                        else if (typeof(T) == typeof(Func<bool>))
                        {
                            MethodInfo fakeMethodInfo = typeof(DelegateData).GetMethod(nameof(CheckSingleFrameFunc));
                            return (T)Delegate.CreateDelegate(typeof(T), this, fakeMethodInfo);
                        }
                        else if (typeof(T) == typeof(Func<bool, Result>))
                        {
                            MethodInfo fakeMethodInfo = typeof(DelegateData).GetMethod(nameof(CheckMultiFrameFunc));
                            return (T)Delegate.CreateDelegate(typeof(T), this, fakeMethodInfo);
                        }
                        else if (typeof(T) == typeof(Func<Request, Result>))
                        {
                            MethodInfo fakeMethodInfo = typeof(DelegateData).GetMethod(nameof(CheckMultiFrameFunc2));
                            return (T)Delegate.CreateDelegate(typeof(T), this, fakeMethodInfo);
                        }
                    }

                    return (T)Delegate.CreateDelegate(typeof(T), instance, method);
                }

                return (T)Delegate.CreateDelegate(typeof(T), method);
            }
        }
    
        private void CheckAction()
        {
            if (m_action == null)
            {
                if (TryGetInstance(m_actionName, out object instance, out var method))
                {
                    m_action = (Action)Delegate.CreateDelegate(typeof(Action), instance, method);
                    m_action.Invoke();
                }
            }
            else
            {
                m_action.Invoke();
            }
        }

        private bool CheckSingleFrameFunc()
        {
            if (m_singleFrameFunc == null)
            {
                if (TryGetInstance(m_singleFrameFuncName, out object instance, out var method))
                {
                    m_singleFrameFunc = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), instance, method);
                    return m_singleFrameFunc.Invoke();
                }
                return false;
            }
            else
            {
                return m_singleFrameFunc.Invoke();
            }
        }

        private Result CheckMultiFrameFunc(bool arg)
        {
            if (m_multiFrameFunc == null)
            {
                if (TryGetInstance(m_multiFrameFuncName, out object instance, out var method))
                {
                    m_multiFrameFunc = (Func<bool, Result>)Delegate.CreateDelegate(typeof(Func<bool, Result>), instance, method);
                    return m_multiFrameFunc.Invoke(arg);
                }
                return Result.FAILED;
            }
            else
            {
                return m_multiFrameFunc.Invoke(arg);
            }
        }

        private Result CheckMultiFrameFunc2(Request arg)
        {
            if (m_multiFrameFunc2 == null)
            {
                if (TryGetInstance(m_multiFrameFunc2Name, out object instance, out var method))
                {
                    m_multiFrameFunc2 = (Func<Request, Result>)Delegate.CreateDelegate(typeof(Func<Request, Result>), instance, method);
                    return m_multiFrameFunc2.Invoke(arg);
                }
                return Result.FAILED;
            }
            else
            {
                return m_multiFrameFunc2.Invoke(arg);
            }
        }

        private static bool TryGetInstance(string delegateString, out object instance, out MethodInfo method)
        {
            var parts = delegateString.Split('|');

            long ID = Convert.ToInt64(parts[1]);
            string methodName = parts[2];
            string typeName = parts[0];
            var type = typeName != null ? Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.FullName == typeName) : null;
            method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance) ?? throw new ArgumentException($"Static method {methodName} not found in type {typeName}.");

            return InstanceContext.Instance.TryGetReference(type, ID, out instance);
        }

        public string GetMethodName()
        {
            if (!string.IsNullOrEmpty(m_actionName))
            {
                return m_actionName;
            }
            else if (!string.IsNullOrEmpty(m_singleFrameFuncName))
            {
                return m_singleFrameFuncName;
            }
            else if (!string.IsNullOrEmpty(m_multiFrameFuncName))
            {
                return m_multiFrameFuncName;
            }
            else if (!string.IsNullOrEmpty(m_multiFrameFunc2Name))
            {
                return m_multiFrameFunc2Name;
            }

            return string.Empty;
        }
    
        public bool ResetMethod(string methodString)
        {
            string[] parts = methodString.Split('|');
            if (parts.Length != 3 && parts.Length != 2)
                return false;

            string typeName;
            string methodName;
            MethodInfo methodInfo = null;
            if (parts.Length == 2)
            {
                typeName = parts[0];
                methodName = parts[1];
            }
            else
            {
                typeName = parts[0];
                methodName = parts[2];
            }

            Type type = Type.GetType(typeName);
            if (type == null)
                return false;

            methodInfo = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (methodInfo == null)
                return false;

            Type[] parameterTypes = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();
            if (methodInfo.ReturnType == typeof(void))
            {
                if (parameterTypes.Length == 0)
                {
                    Clear();
                    m_actionName = methodString;
                    return true;
                }
            }
            else if (methodInfo.ReturnType == typeof(bool))
            {
                if (parameterTypes.Length == 0)
                {
                    Clear();
                    m_singleFrameFuncName = methodString;
                    return true;
                }
            }
            else if (methodInfo.ReturnType == typeof(Result))
            {
                if (parameterTypes.Length == 1 && parameterTypes[0] == typeof(bool))
                {
                    Clear();
                    m_multiFrameFuncName = methodString;
                    return true;
                }
                else if (parameterTypes.Length == 1 && parameterTypes[0] == typeof(Request))
                {
                    Clear();
                    m_multiFrameFunc2Name = methodString;
                    return true;
                }
            }

            return false;
        }

        public void Clear()
        {
            m_action = null;
            m_singleFrameFunc = null;
            m_multiFrameFunc = null;
            m_multiFrameFunc2 = null;

            m_actionName = string.Empty;
            m_singleFrameFuncName = string.Empty;
            m_multiFrameFuncName = string.Empty;
            m_multiFrameFunc2Name = string.Empty;
        }

        public bool IsDelegateCreated()
        {
            return m_action != null || m_singleFrameFunc != null || m_multiFrameFunc != null || m_multiFrameFunc2 != null;
        }
    }
}
