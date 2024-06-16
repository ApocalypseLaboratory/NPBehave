using Newtonsoft.Json;
using NPBehave;

namespace NPSerialization
{
    public class SucceederData : NodeData
    {
        public override string TYPE_NAME_FOR_SERIALIZATION { get { return typeof(SucceederData).FullName; } }

        [JsonIgnore]
        private Succeeder m_succeeder;

        public SucceederData() : base() 
        {
            m_nodeType = NodeType.Decorator;
        }

        public SucceederData(long id) : base(id)
        {
            m_nodeType = NodeType.Decorator;
        }

        public override Decorator CreateDecorator(Node node)
        {
            m_succeeder = new Succeeder(node);
            return m_succeeder;
        }

        public override Node GetNode()
        {
            return m_succeeder;
        }
    }
}