using Match3.Abstractions;
using Match3.Core.Input;
using Match3.DependencyResolving;

namespace Match3.Core.Game
{
    public class GameController
    {
        private readonly IGameMode _playMode;

        private IGameMode _currentMode;

        public GameController(IDependencyResolver resolver)
        {
            IGameBoard gameBoard = resolver.Resolve<IGameBoard>();
            IInput input = resolver.Resolve<IInput>();

            _playMode = new PlayMode(gameBoard, input);
        }

        public void SetPlayMode()
        {
            ChangeMode(_playMode);
        }

        private void ChangeMode(IGameMode mode)
        {
            _currentMode?.Exit();
            _currentMode = mode;
            _currentMode.Enter();
        }
    }
}
