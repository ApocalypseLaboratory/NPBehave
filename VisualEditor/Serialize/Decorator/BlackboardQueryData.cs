using Newtonsoft.Json;
using NPBehave;
using System.Collections.Generic;

namespace NPSerialization
{
    public class BlackboardQueryData : NodeData
    {
        public override string TYPE_NAME_FOR_SERIALIZATION { get { return typeof(BlackboardQueryData).FullName; } }
        public List<string> m_keys = new();
        public DelegateData m_queryData = new();
        public Stops m_stopsOnChange;

        [JsonIgnore]
        private BlackboardQuery m_blackboardQuery;

        public BlackboardQueryData() : base()
        {
            m_nodeType = NodeType.Decorator;
        }

        public BlackboardQueryData(long id) : base(id)
        {
            m_nodeType = NodeType.Decorator;
        }

        public override Decorator CreateDecorator(Node node)
        {
            if (m_keys.Count == 0)
                return null;

            if (string.IsNullOrEmpty(m_queryData.SingleFrameFuncString))
                return null;

            if (!m_queryData.IsDelegateCreated())
                m_queryData.CreateDelegate();

            m_blackboardQuery = new BlackboardQuery(m_keys.ToArray(), m_stopsOnChange, m_queryData.m_singleFrameFunc, node);

            return m_blackboardQuery;
        }

        public override Node GetNode()
        {
            return m_blackboardQuery;
        }
    }
}