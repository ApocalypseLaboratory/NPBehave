using Newtonsoft.Json;
using NPBehave;

namespace NPSerialization
{
    public class ServiceData : NodeData
    {
        public override string TYPE_NAME_FOR_SERIALIZATION { get { return typeof(ServiceData).FullName; } }

        [JsonIgnore]
        private Service m_service;

        public float m_interval = -1.0f;
        public DelegateData m_delegateData = new();

        public ServiceData() : base()
        {
            m_nodeType = NodeType.Decorator;
        }

        public ServiceData(long id) : base(id)
        {
            m_nodeType = NodeType.Decorator;
        }

        public override Decorator CreateDecorator(Node node)
        {
            if (!m_delegateData.IsDelegateCreated())
                m_delegateData.CreateDelegate();

            if (m_delegateData.m_action == null)
                throw new Exception("No Service Method!");

            m_service = new Service(m_interval, m_delegateData.m_action, node);

            return m_service;
        }

        public override Node GetNode()
        {
            return m_service;  
        }
    }
}
