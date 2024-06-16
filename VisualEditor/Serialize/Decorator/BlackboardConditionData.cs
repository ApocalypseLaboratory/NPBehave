using Newtonsoft.Json;
using NPBehave;

namespace NPSerialization
{
    public class BlackboardConditionData : NodeData
    {
        public override string TYPE_NAME_FOR_SERIALIZATION { get { return typeof(BlackboardConditionData).FullName; } }

        public Operator m_operator;
        public Stops m_stopsOnChange;
        public BlackboardKVData m_blackboardData = new();

        [JsonIgnore]
        private BlackboardCondition m_blackboardCondition;

        public BlackboardConditionData() : base()
        {
            m_nodeType = NodeType.Decorator;
        }

        public BlackboardConditionData(long id) : base(id)
        {
            m_nodeType = NodeType.Decorator;
        }

        public override Decorator CreateDecorator(Node node)
        {
            switch (m_blackboardData.m_compareType)
            {
                case CompareType.TString:
                    m_blackboardCondition = new BlackboardCondition(m_blackboardData.m_key, m_operator, m_blackboardData.m_theStringValue,m_stopsOnChange, node);
                    break;
                case CompareType.TBoolean:
                    m_blackboardCondition = new BlackboardCondition(m_blackboardData.m_key, m_operator, m_blackboardData.m_theBoolValue, m_stopsOnChange, node);
                    break;
                case CompareType.TInt:
                    m_blackboardCondition = new BlackboardCondition(m_blackboardData.m_key, m_operator, m_blackboardData.m_theIntValue, m_stopsOnChange, node);
                    break;
                case CompareType.TFloat:
                    m_blackboardCondition = new BlackboardCondition(m_blackboardData.m_key, m_operator, m_blackboardData.m_theFloatValue, m_stopsOnChange, node);
                    break;
            }

            return m_blackboardCondition;
        }

        public override Node GetNode()
        {
            return m_blackboardCondition;
        }
    }
}
