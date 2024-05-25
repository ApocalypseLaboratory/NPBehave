using NPBehave;

namespace NPSerialization
{
    public interface IBehavioral
    {
        void LoadBehaviorNodes();
        void UpdateBlackBoard();
        void StartBehavior();
        void StopBehavior();
        Root GetRoot();
    }
}