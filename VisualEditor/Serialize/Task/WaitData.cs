using Newtonsoft.Json;
using NPBehave;
using System;
using System.Linq;
using System.Reflection;

namespace NPSerialization
{
    public class WaitData : NodeData
    {
        public override string TYPE_NAME_FOR_SERIALIZATION { get { return typeof(WaitData).FullName; } }

        [JsonIgnore]
        private Wait m_wait;
        public float m_seconds = -1;
        public string m_blackboardKey = null;
        public float m_randomVariance = float.NaN;

        public WaitFunctionData m_waitFunctionData = new();

        public WaitData() : base()
        {
            m_nodeType = NodeType.Task;
        }

        public WaitData(long id) : base(id)
        {
            m_nodeType = NodeType.Task;
        }

        public override Task CreateTask()
        {
            if (!string.IsNullOrEmpty(m_blackboardKey))
            {
                if (m_randomVariance != float.NaN)
                {
                    m_wait = new Wait(m_blackboardKey, 0);
                }
               else
                {
                    m_wait = new Wait(m_blackboardKey, m_randomVariance);
                }
            }
            else if (!string.IsNullOrEmpty(m_waitFunctionData.WaitFuncString))
            {
                if (!m_waitFunctionData.IsDelegateCreated())
                    m_waitFunctionData.CreateDelegate();

                m_wait = new Wait(m_waitFunctionData.m_waitFunc, m_seconds);
            }
            else
            {
                m_wait = new Wait(m_seconds);
            }
            
            return m_wait;
        }

        public override Node GetNode()
        {
            return m_wait;
        }

        public class WaitFunctionData
        {
            [JsonIgnore]
            private string m_waitFuncName = string.Empty;
            [JsonIgnore]
            public Func<float> m_waitFunc;

            public string WaitFuncString
            {
                get { return m_waitFuncName; }
                set { m_waitFuncName = value; }
            }

            private string GetSerializeString<T>(T delegateFunction) where T : System.Delegate
            {
                return DelegateData.GetSerializeString(delegateFunction);
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
                            if (typeof(T) == typeof(Func<float>))
                            {
                                MethodInfo fakeMethodInfo = typeof(DelegateData).GetMethod(nameof(CheckSingleFrameFunc));
                                return (T)Delegate.CreateDelegate(typeof(T), this, fakeMethodInfo);
                            }
                        }

                        return (T)Delegate.CreateDelegate(typeof(T), instance, method);
                    }

                    return (T)Delegate.CreateDelegate(typeof(T), method);
                }
            }

            private float CheckSingleFrameFunc()
            {
                if (m_waitFunc == null)
                {
                    if (DelegateData.TryGetInstance(m_waitFuncName, out object instance, out var method))
                    {
                        m_waitFunc = (Func<float>)Delegate.CreateDelegate(typeof(Func<float>), instance, method);
                        return m_waitFunc.Invoke();
                    }
                    return 0.0f;
                }
                else
                {
                    return m_waitFunc.Invoke();
                }
            }
        
            public bool IsDelegateCreated()
            {
                return m_waitFunc != null;
            }

            public void CreateDelegate()
            {
                if (!string.IsNullOrEmpty(m_waitFuncName))
                {
                    m_waitFunc = GetDelegate<Func<float>>(m_waitFuncName);
                }
            }

            public string GetMethodName()
            {
                if (!string.IsNullOrEmpty(m_waitFuncName))
                {
                    return m_waitFuncName;
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

                if (methodInfo.ReturnType == typeof(float))
                {
                    if (parameterTypes.Length == 0)
                    {
                        m_waitFuncName = methodString;
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
