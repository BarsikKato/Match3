using Core.Abstractions;
using Core.Board;
using Core.Fill;
using Core.Items;
using Core.Match;
using Core.Tiles.Factory;
using DependencyResolving;
using UnityEngine;

namespace Core
{
    public sealed class GameInitializer : MonoBehaviour
    {
        [SerializeField] private GameBoard board;
        [SerializeField] private GameItemPool gameItemPool;
        [SerializeField] private LinearMatchStrategy linearMatchStrategy;

        private void Start()
        {
            using (IDependencyResolver resolver = DependencyResolver.Get())
            {
                AddDependencies(resolver);
                Initialize(resolver);
            }
        }

        private void AddDependencies(IDependencyResolver resolver)
        {
            resolver.Add<IGameBoard, GameBoard>(board);
            resolver.Add<IGameItemPool, GameItemPool>(gameItemPool);
            resolver.Add<ITileFactory, TileFactory>(new TileFactory());
            resolver.Add<IFillStrategy, ItemFallFillStrategy>(new ItemFallFillStrategy());
        }

        private void Initialize(IDependencyResolver resolver)
        {
            gameItemPool.Initialize(resolver);
            linearMatchStrategy.Initialize(resolver);
            board.Construct(resolver);
        }
    }
}

