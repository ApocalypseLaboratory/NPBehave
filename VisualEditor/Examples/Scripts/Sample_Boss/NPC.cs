using UnityEngine;

namespace NPVisualEditor_Example
{
    public class NPC : MonoBehaviour, ISampleNPC
    {
        private void Start()
        {
            m_sprite = this.transform.Find("Sprite").gameObject;
            m_animator = m_sprite.GetComponent<Animator>();
        }

        private void Update()
        {
            switch (NPCModel.AnimState)
            {
                case AnimState.IDLE:
                    m_animator.SetBool("bAttack", false);
                    m_animator.SetBool("bWalk", false);
                    m_animator.SetBool("bDefend", false);
                    break;
                case AnimState.WALK:
                    m_animator.SetBool("bWalk", true);
                    NPCModel.Position += NPCModel.Speed * Time.deltaTime;
                    transform.position = NPCModel.Position;
                    break;
                case AnimState.ATTACK:
                    m_animator.SetBool("bAttack", true);
                    break;
                case AnimState.DEFEND:
                    m_animator.SetBool("bDefend", true);
                    break;
            }
        }

        public NPCModel NPCModel
        {
            get; set;
        }

        private GameObject m_sprite;
        private Animator m_animator;
    }
}