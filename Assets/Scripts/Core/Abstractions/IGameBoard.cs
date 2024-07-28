using UnityEngine;

namespace Core.Abstractions
{
    public interface IGameBoard
    {
        public int RowCount { get; }

        public int ColumnCount { get; }

        IGameTile GetTile(int row, int column);
        
        Vector2 GetWorldPosition(int row, int column);

        void Fill();

        void SetFillStrategy(IFillStrategy fillStrategy);

        void SwapTiles(IGameTile tileA, IGameTile tileB);

        void SetMatchStrategy(IMatchStrategy matchStrategy);
    }
}
