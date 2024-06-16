using Newtonsoft.Json;
using NPBehave;

namespace NPSerialization
{
    public class CooldownData : NodeData
    {
        public override string TYPE_NAME_FOR_SERIALIZATION { get { return typeof(CooldownData).FullName; } }

        public bool m_startAfterDecoratee = false;
        public bool m_resetOnFailiure = false;
        public bool m_failOnCooldown = false;
        public float m_cooldownTime = 0.0f;
        public float m_randomVariation = 0.05f;
        public bool m_isReady = true;

        [JsonIgnore]
        private Cooldown m_cooldown;

        public CooldownData() : base() 
        {
            m_nodeType = NodeType.Decorator;
        }

        public CooldownData(long id) : base(id) 
        {
            m_nodeType = NodeType.Decorator;
        }

        public override Decorator CreateDecorator(Node node)
        {
            m_cooldown = new Cooldown(m_cooldownTime, m_randomVariation, m_startAfterDecoratee, m_resetOnFailiure, m_failOnCooldown, node);
            return m_cooldown;
        }

        public override Node GetNode()
        {
            return m_cooldown;
        }
    }
}