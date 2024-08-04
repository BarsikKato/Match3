using UnityEngine;

namespace Core.Abstractions
{
    public interface IGameBoard
    {
        int RowCount { get; }

        int ColumnCount { get; }

        IGameTile GetTile(int row, int column);
        
        Vector2 GetWorldPosition(int row, int column);

        bool TryGetBoardPosition(Vector2 worldPosition, out int row, out int column);

        void SetFillStrategy(IFillStrategy fillStrategy);

        void SwapTiles(IGameTile tileA, IGameTile tileB);

        void SetMatchStrategy(IMatchStrategy matchStrategy);
    }
}
