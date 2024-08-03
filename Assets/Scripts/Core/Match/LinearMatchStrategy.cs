using Core.Abstractions;
using Core.Items;
using DependencyResolving;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Match
{
    public sealed class LinearMatchStrategy : MonoBehaviour, IMatchStrategy
    {
        [SerializeField] private int requiredCount;

        private IGameBoard _board;

        private readonly BoardPosition[] _directions = new BoardPosition[]
        {
            BoardPosition.Up,
            BoardPosition.Down,
            BoardPosition.Left,
            BoardPosition.Right
        };

        public void Initialize(IDependencyResolver resolver)
        {
            _board = resolver.Resolve<IGameBoard>();
            _board.SetMatchStrategy(this);
        }

        public bool TryMatch(
            IGameTile tileA, 
            IGameTile tileB, 
            out IReadOnlyCollection<IGameTile> matchA, 
            out IReadOnlyCollection<IGameTile> matchB)
        {
            matchA = FindMatchingTiles(tileA);
            matchB = FindMatchingTiles(tileB);
            return matchA.Count > 0 || matchB.Count > 0;
        }

        private IReadOnlyCollection<IGameTile> FindMatchingTiles(IGameTile tile)
        {
            List<IGameTile> matchingTiles = new List<IGameTile>();
            foreach (var direction in _directions)
            {
                IReadOnlyCollection<IGameTile> directionTiles = 
                    FindMatchingTilesInDirection(tile, direction);

                if (directionTiles.Count >= requiredCount)
                    matchingTiles.AddRange(directionTiles);
            }

            return matchingTiles;
        }

        private IReadOnlyCollection<IGameTile> FindMatchingTilesInDirection(
            IGameTile tile, BoardPosition direction)
        {
            List<IGameTile> matchingTiles = new() { tile };
            BoardPosition currentPosition = new BoardPosition(tile.Row, tile.Column);
            while (true)
            {
                BoardPosition nextPosition = currentPosition + direction;
                IGameTile nextTile = _board.GetTile(nextPosition);
                if (nextTile == null || !IsTilesMatching(tile, nextTile))
                    break;

                matchingTiles.Add(nextTile);
                currentPosition = nextPosition;
            }

            return matchingTiles;
        }

        private bool IsTilesMatching(IGameTile tileA, IGameTile tileB) 
        {
            int itemAType = tileA.CurrentItem.Type;
            int itemBType = tileB.CurrentItem.Type;
            return itemAType == itemBType
                && itemAType != GameItem.UnmatchableType
                && itemBType != GameItem.UnmatchableType;
        }
    }
}

