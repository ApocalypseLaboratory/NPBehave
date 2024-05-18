using NPBehave;

namespace NPSerialization
{
    public interface IBehavoiral
    {
        void LoadBehavoirNodes();
        void UpdateBlackBoard();
        void StartBehavoir();
        void StopBehavoir();
        Root GetRoot();
    }
}