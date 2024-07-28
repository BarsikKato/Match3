using Core.Abstractions;
using UnityEngine;

namespace Core.Tiles.Factory
{
    public sealed class TileFactory : ITileFactory
    {
        public IGameTile Create(BoardPosition boardPosition, Vector2 worldPosition)
        {
            return new GameTile(boardPosition, worldPosition);
        }
    }
}
