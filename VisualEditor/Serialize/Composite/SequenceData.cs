using Newtonsoft.Json;
using NPBehave;

namespace NPSerialization
{
    public class SequenceData : NodeData
    {
        public override string TYPE_NAME_FOR_SERIALIZATION { get { return typeof(SequenceData).FullName; } }

        [JsonIgnore]
        private Sequence m_sequence;

        public SequenceData() : base()
        {
            m_nodeType = NodeType.Composite;
        }

        public SequenceData(long id) : base(id)
        {
            m_nodeType = NodeType.Composite;
        }

        public override Composite CreateComposite(Node[] nodes)
        {
            m_sequence = new Sequence(nodes);
            return m_sequence;
        }

        public override Node GetNode()
        {
            return m_sequence;
        }
    }
}
