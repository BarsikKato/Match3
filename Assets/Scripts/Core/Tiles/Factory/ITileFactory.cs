using Match3.Abstractions;

namespace Match3.Core.Tiles.Factory
{
    public interface ITileFactory
    {
        IGameTile Create(BoardPosition position);
    }
}
