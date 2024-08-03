using Core.Abstractions;
using Core.Items;
using DependencyResolving;
using System.Collections.Generic;
using System.Linq;
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
            List<IGameTile> verticalTiles = new List<IGameTile>() { tile };
            List<IGameTile> horizontalTiles = new List<IGameTile>();
            foreach (var direction in _directions)
            {
                List<IGameTile> matchingList = 
                    direction.Equals(BoardPosition.Up) || direction.Equals(BoardPosition.Down)
                    ? verticalTiles 
                    : horizontalTiles;

                    FindMatchingTilesInDirection(tile, direction, matchingList);
            }

            return verticalTiles
                .Concat(horizontalTiles)
                .ToList();
        }

        private void FindMatchingTilesInDirection(
            IGameTile tile, BoardPosition direction, List<IGameTile> matchingTiles)
        {
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

