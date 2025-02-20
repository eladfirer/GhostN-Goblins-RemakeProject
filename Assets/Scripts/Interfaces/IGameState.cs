namespace Interfaces
{
    public interface IGameState
    {
        public void EnterState(GameManager gameManager);
        public IGameState OnUpdate(GameManager gameManager);
        public void ExitState();
    }
}