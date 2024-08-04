using Match3.Abstractions;
using Match3.Core.Items;
using Match3.Core.Tiles;
using Match3.DependencyResolving;
using Match3.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Core.Fill
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
            _coroutineRunner.StartCoroutine(FillBoardRoutine());
        }

        public IEnumerator FillBoardRoutine()
        {
            List<Coroutine> awaitedCoroutines = new List<Coroutine>();
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
                        Coroutine coroutine = _coroutineRunner.StartCoroutine(DropItemDown(tile, tileWithItem));
                        awaitedCoroutines.Add(coroutine);
                    }
                    else
                    {
                        Coroutine coroutine = _coroutineRunner.StartCoroutine(GenerateNewItem(tile, boardPosition));
                        awaitedCoroutines.Add(coroutine);
                    }
                }
            }

            foreach (Coroutine coroutine in awaitedCoroutines)
            {
                yield return coroutine;
            }

            IsBoardFilled = true;
        }

        private IEnumerator DropItemDown(IGameTile tile, IGameTile upTileWithItem)
        {
            IGameItem upItem = upTileWithItem.CurrentItem;
            upTileWithItem.SetItem(null);

            tile.SetItem(upItem);
            yield return MoveItemDown(upItem, tile);
        }

        private IEnumerator GenerateNewItem(IGameTile tile, BoardPosition boardPosition)
        {
            BoardPosition generatorPosition = new BoardPosition(-1, tile.Column);
            IGameItem item;
            while (true)
            {
                item = _gameItemPool.GetItem();
                IGameTile leftTile = _gameBoard.GetTile(boardPosition.LeftPosition);
                IGameTile downTile = _gameBoard.GetTile(boardPosition.DownPosition);
                if (leftTile != null && leftTile.CurrentItem.Type == item.Type ||
                    downTile != null && downTile.CurrentItem.Type == item.Type)
                {
                    _gameItemPool.AddItem(item);
                }
                else
                {
                    break;
                }
            }

            item.SetPositionTo(
                _gameBoard.GetWorldPosition(generatorPosition)).FullyIterate();

            tile.SetItem(item);
            Vector2 worldPosition = _gameBoard.GetWorldPosition(boardPosition);
            yield return _coroutineRunner.StartCoroutine(item.SetPositionTo(worldPosition));
        }

        private IEnumerator MoveItemDown(IGameItem item, IGameTile downTile)
        {
            Vector2 nextPosition = _gameBoard.GetWorldPosition(downTile.GetBoardPosition());
            IEnumerator fallingRoutine = item.SetPositionTo(nextPosition);
            yield return _coroutineRunner.StartCoroutine(fallingRoutine);
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