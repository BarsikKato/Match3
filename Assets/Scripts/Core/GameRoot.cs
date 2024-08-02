using Core.Abstractions;
using Core.Board;
using Core.Fill;
using Core.Game;
using Core.Input;
using Core.Items;
using Core.Match;
using Core.Tiles.Factory;
using DependencyResolving;
using UnityEngine;

namespace Core
{
    public sealed class GameRoot : MonoBehaviour
    {
        [SerializeField] private GameBoard board;
        [SerializeField] private GameItemPool gameItemPool;
        [SerializeField] private LinearMatchStrategy linearMatchStrategy;
        [SerializeField] private CanvasInput canvasInput;

        private readonly IDependencyResolver _resolver = DependencyResolver.Get();
        private GameController _gameController;

        private void Start()
        {
            AddDependencies();
            Initialize();
        }

        private void OnDestroy()
        {
            _resolver.Dispose();
        }

        private void AddDependencies()
        {
            _resolver.Add<IGameBoard, GameBoard>(board);
            _resolver.Add<IGameItemPool, GameItemPool>(gameItemPool);
            _resolver.Add<ITileFactory, TileFactory>(new TileFactory());
            _resolver.Add<IFillStrategy, ItemFallFillStrategy>(new ItemFallFillStrategy());
            _resolver.Add<IInput, CanvasInput>(canvasInput);
        }

        private void Initialize()
        {
            gameItemPool.Initialize(_resolver);
            linearMatchStrategy.Initialize(_resolver);
            board.Construct(_resolver);
            InitializeController();
        }

        private void InitializeController()
        {
            _gameController = new(_resolver);
            _gameController.SetPlayMode();
        }
    }
}

