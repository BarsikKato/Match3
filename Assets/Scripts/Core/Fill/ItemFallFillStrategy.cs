using Core.Abstractions;
using Core.Items;
using DependencyResolving;
using Extensions;
using UnityEngine;

namespace Core.Fill
{
    public sealed class ItemFallFillStrategy : IFillStrategy
    {
        private IGameBoard _gameBoard;
        private IGameItemPool _gameItemPool;
        private BoardPosition _maxBoardPosition;

        public void Initialize(IDependencyResolver resolver)
        {
            _gameBoard = resolver.Resolve<IGameBoard>();
            _gameItemPool = resolver.Resolve<IGameItemPool>();
            _maxBoardPosition = new BoardPosition(_gameBoard.RowCount, _gameBoard.ColumnCount);
        }

        public void FillBoard()
        {
            for (int column = 0; column < _maxBoardPosition.Column; column++)
            {
                for (int row = 0; row < _maxBoardPosition.Row; row++)
                {
                    BoardPosition generatorPosition = new BoardPosition(-1, column);
                    BoardPosition boardPosition = new BoardPosition(row, column);

                    IGameTile tile = _gameBoard.GetTile(boardPosition);
                    if (!tile.IsFree)
                        continue;

                    IGameItem item = _gameItemPool.GetItem();
                    item.SetPositionTo(
                        _gameBoard.GetWorldPosition(generatorPosition));

                    tile.SetItem(item);
                    Vector2 worldPosition = _gameBoard.GetWorldPosition(boardPosition);
                    item.SetPositionTo(worldPosition).FullyIterate();
                }
            }
        }

        private bool CanFall(BoardPosition boardPosition, out IGameTile downTile)
        {
            downTile = _gameBoard.GetTile(boardPosition.DownPosition);
            return downTile != null && downTile.IsFree;
        }
    }
}