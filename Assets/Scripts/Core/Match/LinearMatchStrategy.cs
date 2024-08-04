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

        public bool TryFindMatchingTiles(
            IGameTile tile, 
            out IReadOnlyCollection<IGameTile> match)
        {
            match = FindMatchingTiles(tile);
            return match.Count > 1;
        }

        private IReadOnlyCollection<IGameTile> FindMatchingTiles(IGameTile tile)
        {
            List<IGameTile> verticalMatchings = new List<IGameTile>();
            List<IGameTile> horizontalMatchings = new List<IGameTile>();
            foreach (var direction in _directions)
            {
                List<IGameTile> matchingList = 
                    direction.Equals(BoardPosition.Up) || direction.Equals(BoardPosition.Down)
                    ? verticalMatchings 
                    : horizontalMatchings;

                    FindMatchingTilesInDirection(tile, direction, matchingList);
            }

            return GetMatchingsCollection(tile, verticalMatchings, horizontalMatchings);
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

        private IReadOnlyCollection<IGameTile> GetMatchingsCollection(
            IGameTile tile, List<IGameTile> verticalMatchings, List<IGameTile> horizontalMatchings)
        {
            List<IGameTile> result = new List<IGameTile>();
            int requiredCountInDirection = requiredCount - 1;
            if (verticalMatchings.Count >= requiredCountInDirection)
            {
                result.AddRange(verticalMatchings);
            }

            if (horizontalMatchings.Count >= requiredCountInDirection)
            {
                result.AddRange(horizontalMatchings);
            }

            if (result.Count > 0)
                result.Add(tile);

            return result;
        }
    }
}

