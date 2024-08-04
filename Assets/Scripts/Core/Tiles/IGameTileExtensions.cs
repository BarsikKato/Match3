using Match3.Abstractions;

namespace Match3.Core.Tiles
{
    public static class IGameTileExtensions
    {
        public static BoardPosition GetBoardPosition(this IGameTile gameTile)
        {
            return new BoardPosition(gameTile.Row, gameTile.Column);
        }
    }
}

