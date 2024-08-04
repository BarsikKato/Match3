using Core.Abstractions;

namespace Core.Tiles.Factory
{
    public sealed class TileFactory : ITileFactory
    {
        public IGameTile Create(BoardPosition boardPosition)
        {
            return new GameTile(boardPosition);
        }
    }
}
