using NPSerialization;
using UnityEngine;
using static NPBehave.Action;

namespace NPVisualEditor_Example
{
    [SerializationID(1001)]
    public partial class Boss
    {
        public Boss(NPCModel model)
        {
            m_npc = model;
        }

        public void Idle()
        {
            m_npc.Power += POWER_RECOVERY;
            m_npc.AnimState = AnimState.IDLE;
        }

        protected void Attack()
        {
            m_npc.Power -= POWER_CONSUMPTION;
            m_npc.AnimState = AnimState.ATTACK;
        }

        private void RevertAttack()
        {
            m_npc.Power -= POWER_CONSUMPTION;
            m_npc.AnimState = AnimState.ATTACK;
        }

        private void TackDamage()
        {
            m_npc.Power -= 2 * POWER_CONSUMPTION;
            m_npc.AnimState = AnimState.TACK_DAMAGE;
        }

        private void RotateAttack()
        {
            m_npc.Power -= POWER_CONSUMPTION;
            m_npc.AnimState = AnimState.ATTACK;
        }

        public static void PrintIdle()
        {
            Debug.Log("Idle");
        }

        protected static void PrintAttack()
        {
            Debug.Log("Attack");
        }

        private static void PrintRevertAttack()
        {
            Debug.Log("RevertAttack");
        }

        public void Walk()
        {
            m_npc.Power -= POWER_CONSUMPTION;
            m_npc.AnimState = AnimState.WALK;
        }

        public bool Defend()
        {
            System.Random random = new();
            int num = random.Next(9);
            if (num > 7)
            {
                m_npc.HP -= HP_CONSUMPTION;
                m_npc.Power -= DEFEND_POWER_CONSUMPTION;
                return false;
            }
            return true;
        }

        public bool IsInCombat()
        {
            float curTime = Time.time;
            float deltaTime = curTime - m_blackboard.Get<float>("F_LAST_FOUND_CHARACTER");
            bool bNearCharacter = m_blackboard.Get<bool>("B_NEAR_CHARACTER");
            if (deltaTime > 5 && !bNearCharacter)
            {
                return false;
            }
            return true;
        }

        private NPCModel m_npc;
        private const int POWER_CONSUMPTION = 1;
        private const int DEFEND_POWER_CONSUMPTION = 5;
        private const int POWER_RECOVERY = 4;
        private const int HP_CONSUMPTION = 4;
    }
}