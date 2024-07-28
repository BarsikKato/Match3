using Core.Abstractions;
using UnityEngine;

namespace Core.Tiles
{
    public sealed class GameTile : IGameTile
    {
        private readonly BoardPosition _boardPosition;
        private readonly Vector2 _worldPosition;

        public IGameItem CurrentItem { get; private set; }

        public bool IsFree => CurrentItem is null;

        public int Row => _boardPosition.Row;

        public int Column => _boardPosition.Column;

        public GameTile(BoardPosition boardPosition, Vector2 worldPosition)
        {
            _boardPosition = boardPosition;
            _worldPosition = worldPosition;
        }

        public void SetItem(IGameItem item)
        {
            CurrentItem = item;
        }
    }
}
