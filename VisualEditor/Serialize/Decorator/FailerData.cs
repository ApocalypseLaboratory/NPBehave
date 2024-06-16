using Newtonsoft.Json;
using NPBehave;

namespace NPSerialization
{
    public class FailerData : NodeData
    {
        public override string TYPE_NAME_FOR_SERIALIZATION { get { return typeof(FailerData).FullName; } }

        [JsonIgnore]
        private Failer m_failer;

        public FailerData() : base() 
        {
            m_nodeType = NodeType.Decorator;
        }

        public FailerData(long id) : base(id) 
        {
            m_nodeType = NodeType.Decorator;
        }

        public override Decorator CreateDecorator(Node node)
        {
            m_failer = new Failer(node);
            return m_failer;    
        }

        public override Node GetNode()
        {
            return m_failer;
        }
    }
}