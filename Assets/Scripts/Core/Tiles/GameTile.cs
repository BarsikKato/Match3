using Match3.Abstractions;
using UnityEngine;

namespace Match3.Core.Tiles
{
    public sealed class GameTile : IGameTile
    {
        private readonly BoardPosition _boardPosition;

        public IGameItem CurrentItem { get; private set; }

        public bool IsFree => CurrentItem is null;

        public int Row => _boardPosition.Row;

        public int Column => _boardPosition.Column;

        public GameTile(BoardPosition boardPosition)
        {
            _boardPosition = boardPosition;
        }

        public void SetItem(IGameItem item)
        {
            CurrentItem = item;
        }
    }
}
