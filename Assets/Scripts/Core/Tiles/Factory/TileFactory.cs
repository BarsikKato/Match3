using Match3.Abstractions;

namespace Match3.Core.Tiles.Factory
{
    public sealed class TileFactory : ITileFactory
    {
        public IGameTile Create(BoardPosition boardPosition)
        {
            return new GameTile(boardPosition);
        }
    }
}
