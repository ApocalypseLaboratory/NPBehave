using Newtonsoft.Json;
using NPBehave;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace NPSerialization
{
    /// <summary>
    /// Base node data
    /// </summary>
    /// <note>
    /// Maybe I should use Schema, but Super Base Class is enough
    /// </note>
    [JsonConverter(typeof(NodeDataConverter))]
    [KnownType(typeof(SelectorData))]
    [KnownType(typeof(SequenceData))]
    [KnownType(typeof(BlackboardConditionData))]
    [KnownType(typeof(ServiceData))]
    [KnownType(typeof(RootData))]
    [KnownType(typeof(ActionData))]
    [KnownType(typeof(WaitData))]
    [KnownType(typeof(WaitUtilStoppedData))]
    public class NodeData
    {
        // Must be override
        public virtual string TYPE_NAME_FOR_SERIALIZATION { get { return typeof(NodeData).FullName; } }

        public long m_ID;
        public NodeType m_nodeType;
        public List<long> m_linkedNodeIDs = new List<long>();
        public long m_parentID;
        public string m_description;
        
        public NodeData()
        {
        }

        public NodeData(long id) 
        {
            m_ID = id;
        }

        public Vector2 m_position;

        public virtual Node GetNode() => null;

        public virtual Task CreateTask() => null;

        public virtual Decorator CreateDecorator(Node node) => null;

        public virtual Composite CreateComposite(Node[] nodes) => null;
    }
}

