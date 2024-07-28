using Core.Abstractions;
using UnityEngine;

namespace Core.Tiles.Factory
{
    public interface ITileFactory
    {
        IGameTile Create(BoardPosition position, Vector2 worldPosition);
    }
}
