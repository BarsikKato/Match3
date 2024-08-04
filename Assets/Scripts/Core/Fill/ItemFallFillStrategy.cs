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

        private MonoBehaviour _coroutineRunner;

        public bool IsBoardFilled { get; private set; } = false;

        public void Initialize(IDependencyResolver resolver, MonoBehaviour coroutineRunner)
        {
            _gameBoard = resolver.Resolve<IGameBoard>();
            _gameItemPool = resolver.Resolve<IGameItemPool>();
            _maxBoardPosition = new BoardPosition(_gameBoard.RowCount, _gameBoard.ColumnCount);
            _coroutineRunner = coroutineRunner;
        }

        public void FillBoard()
        {
            IsBoardFilled = false;
            for (int column = 0; column < _maxBoardPosition.Column; column++)
            {
                for (int row = _maxBoardPosition.Row - 1; row >= 0; row--)
                {
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
                        DropItemDown(tile, tileWithItem);
                    }
                    else
                    {
                        GenerateNewItem(tile, boardPosition);
                    }
                }
            }

            IsBoardFilled = true;
        }

        private void DropItemDown(IGameTile tile, IGameTile upTileWithItem)
        {
            IGameItem upItem = upTileWithItem.CurrentItem;
            upTileWithItem.SetItem(null);

            tile.SetItem(upItem);
            MoveItemDown(upItem, tile);
        }

        private void GenerateNewItem(IGameTile tile, BoardPosition boardPosition)
        {
            BoardPosition generatorPosition = new BoardPosition(-1, tile.Column);
            IGameItem item = _gameItemPool.GetItem();
            item.SetPositionTo(
                _gameBoard.GetWorldPosition(generatorPosition)).FullyIterate();

            tile.SetItem(item);
            Vector2 worldPosition = _gameBoard.GetWorldPosition(boardPosition);
            _coroutineRunner.StartCoroutine(item.SetPositionTo(worldPosition));
        }

        private void MoveItemDown(IGameItem item, IGameTile downTile)
        {
            Vector2 nextPosition = _gameBoard.GetWorldPosition(downTile.GetBoardPosition());
            IEnumerator fallingRoutine = item.SetPositionTo(nextPosition);
            _coroutineRunner.StartCoroutine(fallingRoutine);
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
            while (boardPosition.Row > 0);

            return false;
        }
    }
}