using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NPSerialization
{ 
    public static class NodeDataUtils
    {
        public static void AddChild(NodeData parent, NodeData child)
        {
            if (parent == null || child == null)
                return;

            if (!parent.m_linkedNodeIDs.Contains(child.m_ID))
            {
                parent.m_linkedNodeIDs.Add(child.m_ID);
            }
            child.m_parentID = parent.m_ID;
        }

        public static void AddChildren(NodeData parent, params NodeData[] children)
        {
            if (parent == null)
                return;

            foreach (NodeData child in children)
            {
                if (!parent.m_linkedNodeIDs.Contains(child.m_ID))
                {
                    parent.m_linkedNodeIDs.Add(child.m_ID);
                }
                child.m_parentID = parent.m_ID;
            }
        }

        public static void RemoveChild(NodeData parent, NodeData child)
        {
            if (parent == null || child == null)
                return;

            if (parent.m_linkedNodeIDs.Contains(child.m_ID))
            {
                parent.m_linkedNodeIDs.Remove(child.m_ID);
            }
            child.m_parentID = 0;
        }

        public static void RemoveChildren(NodeData parent, params NodeData[] children)
        {
            if (parent == null)
                return;

            foreach (NodeData child in children)
            {
                if (parent.m_linkedNodeIDs.Contains(child.m_ID))
                {
                    parent.m_linkedNodeIDs.Remove(child.m_ID);
                }
                child.m_parentID = 0;
            }
        }

        public static string GetSubTitle(NodeData nodeData)
        {
            var subTitle = string.Empty;
            switch(nodeData.m_nodeType)
            {
                case NodeType.Composite:
                    if (typeof(SelectorData).FullName == nodeData.TYPE_NAME_FOR_SERIALIZATION)
                    {
                        subTitle = "Run children sequentially until one succeeds and succeed (succeeds if one of the children succeeds).";
                    }
                    else if (typeof(SequenceData).FullName == nodeData.TYPE_NAME_FOR_SERIALIZATION)
                    {
                        subTitle = "Run children sequentially until one fails and fail (succeeds if none of the children fails)";
                    }
                    break;
                case NodeType.Decorator:
                    if (typeof(BlackboardConditionData).FullName == nodeData.TYPE_NAME_FOR_SERIALIZATION)
                    {
                        var blackboardConditionData = nodeData as BlackboardConditionData;
                        subTitle = blackboardConditionData.m_blackboardData.m_key + "," + blackboardConditionData.m_operator.ToString() + "," + blackboardConditionData.m_blackboardData.GetValue().ToString() + "," + blackboardConditionData.m_stopsOnChange.ToString();
                    }
                    else if (typeof(ServiceData).FullName == nodeData.TYPE_NAME_FOR_SERIALIZATION)
                    {
                        var serviceData = nodeData as ServiceData;
                        subTitle = serviceData.m_interval.ToString() + "," + serviceData.m_delegateData.GetMethodName();
                    }
                    break;
                case NodeType.Task:
                    if (typeof(ActionData).FullName == nodeData.TYPE_NAME_FOR_SERIALIZATION)
                    {
                        var actionData = nodeData as ActionData;
                        subTitle = actionData.m_actionData.GetMethodName();
                    }
                    else
                    {
                        subTitle = nodeData.TYPE_NAME_FOR_SERIALIZATION;
                    }
                    break;
            }

            return subTitle;
        }

        public static Dictionary<NodeType, List<Type>> GetNodeDataTypeMap()
        {
            var nodeDataTypeMap = new Dictionary<NodeType, List<Type>>();

            var nodeDataTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsSubclassOf(typeof(NodeData)));

            foreach (var type in nodeDataTypes)
            {
                var instance = Activator.CreateInstance(type) as NodeData;
                NodeType nodeType = instance.m_nodeType;
                if (nodeDataTypeMap.ContainsKey(nodeType))
                {
                    nodeDataTypeMap[nodeType].Add(type);
                }
                else
                {
                    nodeDataTypeMap.Add(nodeType, new List<Type>() { type });
                }
            }

            return nodeDataTypeMap;
        }

        public static bool CheckSerializationID(Type type)
        {
            object[] attributes = type.GetCustomAttributes(typeof(SerializationIDAttribute), false);

            return attributes.Length > 0;
        }

        public static List<MethodInfo> GetNPWaitFuncMethods(Type type, BindingFlags flags)
        {
            List<MethodInfo> taskMethods = new List<MethodInfo>();

            // Func<bool>
            taskMethods.AddRange(GetMethodsWithSignature(type, flags, typeof(float), null));

            return taskMethods;
        }

        public static List<MethodInfo> GetNPTaskMethods(Type type, BindingFlags flags)
        {
            List<MethodInfo> taskMethods = new List<MethodInfo>();

            // Action
            taskMethods.AddRange(GetMethodsWithSignature(type, flags, typeof(void), null));

            // Func<bool>
            taskMethods.AddRange(GetMethodsWithSignature(type, flags, typeof(bool), null));

            // Func<bool, Result>
            taskMethods.AddRange(GetMethodsWithSignature(type, flags, typeof(NPBehave.Action.Result), typeof(bool)));

            // Func<Request, Result>
            taskMethods.AddRange(GetMethodsWithSignature(type, flags, typeof(NPBehave.Action.Result), typeof(NPBehave.Action.Request)));

            return taskMethods;
        }

        public static List<MethodInfo> GetMethodsWithSignature(Type type, BindingFlags flags, Type returnType, params Type[] parameterTypes)
        {
            MethodInfo[] methods = type.GetMethods(flags);

            List<MethodInfo> filteredMethods = methods.Where(method =>
                method.ReturnType == returnType &&
                (parameterTypes == null && method.GetParameters().Length == 0) || 
                (parameterTypes != null && method.GetParameters().Select(p => p.ParameterType).SequenceEqual(parameterTypes))
            ).ToList();

            return filteredMethods;
        }

        public static string GetSerializeString(MethodInfo methodInfo)
        {
            if (methodInfo == null)
                return null;

            if (methodInfo.ReflectedType == null)
                return null;

            if (methodInfo.IsStatic)
            {
                return $"{methodInfo.ReflectedType.FullName}|{methodInfo.Name}";
            }
            else
            {
                Type type = methodInfo.ReflectedType;

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

                if (IDObject == 0)
                {
                    return null;
                }

                return $"{type.FullName}|{IDObject}|{methodInfo.Name}";
            }
        }
    }
}