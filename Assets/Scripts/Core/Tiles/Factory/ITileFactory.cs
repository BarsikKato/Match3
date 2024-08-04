using Core.Abstractions;

namespace Core.Tiles.Factory
{
    public interface ITileFactory
    {
        IGameTile Create(BoardPosition position);
    }
}
