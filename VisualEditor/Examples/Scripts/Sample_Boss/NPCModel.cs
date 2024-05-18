using UnityEngine;

namespace NPVisualEditor_Example
{
    public class NPCModel
    {
        public NPCModel(long id, int hp, int power)
        {
            ID = id;
            HP = hp;
            Power = power;
        }

        public int HP { get; set; }
        public int Power { get; set; }
        public long ID { get; set; }
        public AnimState AnimState { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Speed { get; set;}
    }
}