using Match3.Abstractions;
using UnityEngine;

namespace Match3.Core
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

        public static bool TryGetBoardPosition(this IGameBoard board, Vector2 worldPosition, out BoardPosition boardPosition)
        {
            bool isSuccess = board.TryGetBoardPosition(worldPosition, out int row, out int column);
            boardPosition = new BoardPosition(row, column);
            return isSuccess;
        }
    }
}