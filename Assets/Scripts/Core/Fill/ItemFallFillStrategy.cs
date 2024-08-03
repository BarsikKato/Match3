using Core.Abstractions;
using Core.Items;
using Core.Tiles;
using DependencyResolving;
using Extensions;
using System.Collections;
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
                for (int row = _maxBoardPosition.Row - 1; row >= 0; row--)
                {
                    BoardPosition generatorPosition = new BoardPosition(-1, column);
                    BoardPosition boardPosition = new BoardPosition(row, column);

                    IGameTile tile = _gameBoard.GetTile(boardPosition);
                    if (!tile.IsFree)
                    {
                        IGameItem currentItem = tile.CurrentItem;
                        if (currentItem.Type == GameItem.UnmatchableType)
                        {
                            tile.SetItem(null);
                            _gameItemPool.AddItem(currentItem);
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (IsAnyItemUp(boardPosition, out IGameTile tileWithItem))
                    {
                        IGameItem upItem = tileWithItem.CurrentItem;
                        tileWithItem.SetItem(null);

                        tile.SetItem(upItem);
                        MoveItemDown(upItem, tile);
                        continue;
                    }

                    IGameItem item = _gameItemPool.GetItem();
                    item.SetPositionTo(
                        _gameBoard.GetWorldPosition(generatorPosition));

                    tile.SetItem(item);
                    Vector2 worldPosition = _gameBoard.GetWorldPosition(boardPosition);
                    item.SetPositionTo(worldPosition).FullyIterate();
                }
            }
        }

        private void MoveItemDown(IGameItem item, IGameTile downTile)
        {
            Vector2 nextPosition = _gameBoard.GetWorldPosition(downTile.GetBoardPosition());
            IEnumerator fallingRoutine = item.SetPositionTo(nextPosition);
            (_gameBoard as MonoBehaviour).StartCoroutine(fallingRoutine);
        }

        private bool IsAnyItemUp(BoardPosition boardPosition, out IGameTile tileWithItem)
        {
            do
            {
                BoardPosition upPosition = boardPosition.UpPosition;
                tileWithItem = _gameBoard.GetTile(upPosition);
                if (tileWithItem != null 
                    && !tileWithItem.IsFree 
                    && tileWithItem.CurrentItem.Type != GameItem.UnmatchableType)
                    return true;

                boardPosition = upPosition;
            }
            while (boardPosition.Row > 1);

            return false;
        }
    }
}