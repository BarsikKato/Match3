using UnityEngine;

namespace Core.Abstractions
{
    public static class GameBoardExtensions
    {
        public static IGameTile GetTile(this IGameBoard board, BoardPosition boardPosition)
        {
            return board.GetTile(boardPosition.Row, boardPosition.Column);
        }

        public static Vector2 GetWorldPosition(this IGameBoard board, BoardPosition boardPosition)
        {
            return board.GetWorldPosition(boardPosition.Row, boardPosition.Column);
        }
    }
}