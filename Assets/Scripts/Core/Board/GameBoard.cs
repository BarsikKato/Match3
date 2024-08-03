using Core.Abstractions;
using Core.Fill;
using Core.Tiles;
using Core.Tiles.Factory;
using DependencyResolving;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Core.Board
{
    public sealed class GameBoard : MonoBehaviour, IGameBoard
    {
        [SerializeField] private Transform board;
        [SerializeField] private SpriteRenderer tilePrefab;
        [Space]
        [SerializeField] private int rowCount;
        [SerializeField] private int columnCount;

        private readonly Dictionary<BoardPosition, IGameTile> _grid = new();
        private IFillStrategy _fillStrategy;
        private IMatchStrategy _matchStrategy;

        private Vector2 _tileSize;
        private Vector2 _tileOrigin;

        private bool _isTilesSwapping = false;

        public int RowCount => rowCount;

        public int ColumnCount => columnCount;

        private void Awake()
        {
            _tileSize = tilePrefab.bounds.size;
            _tileOrigin = GetOriginPosition(_tileSize);
        }

        public void Construct(IDependencyResolver dependencyResolver)
        {
            ITileFactory tileFactory = dependencyResolver.Resolve<ITileFactory>();
            CreateBoardTiles(tileFactory);

            ItemFallFillStrategy fillStrategy = new ItemFallFillStrategy();
            fillStrategy.Initialize(dependencyResolver);
            SetFillStrategy(fillStrategy);
            Fill();
        }

        private void CreateBoardTiles(ITileFactory tileFactory)
        {
            for (int row = 0; row < rowCount; row++)
            {
                for (int column = 0; column < columnCount; column++)
                {
                    var tileSprite = Instantiate(tilePrefab, board);
                    tileSprite.transform.position = GetWorldPosition(row, column);

                    BoardPosition boardPosition = new BoardPosition(row, column);
                    CreateBoardTile(tileFactory, boardPosition, tileSprite.transform.position);
                }
            }
        }

        private void CreateBoardTile(
            ITileFactory factory, BoardPosition boardPosition, Vector2 worldPosition)
        {
            IGameTile gameTile = factory.Create(boardPosition, worldPosition);
            _grid.Add(boardPosition, gameTile);
        }

        private Vector2 GetOriginPosition(Vector2 size)
        {
            float x = board.position.x - size.x * Mathf.Floor(columnCount / 2);
            float y = board.position.y + size.y * Mathf.Floor(rowCount / 2);
            return new Vector2(x, y);
        }

        public Vector2 GetWorldPosition(int row, int column)
        {
            return _tileOrigin + new Vector2(column * _tileSize.x, row * -_tileSize.y);
        }

        public IGameTile GetTile(int row, int column)
        {
            var requestedPosition = new BoardPosition(row, column);
            if (!_grid.ContainsKey(requestedPosition))
                return null;

            return _grid[requestedPosition];
        }

        public void Fill()
        {
            _fillStrategy.FillBoard();
        }

        public void SetFillStrategy(IFillStrategy fillStrategy)
        {
            _fillStrategy = fillStrategy;
        }

        public void SwapTiles(IGameTile tileA, IGameTile tileB)
        {
            if (_isTilesSwapping)
                return;

            _isTilesSwapping = true;
            StartCoroutine(SwapAndMatchTilesRoutine(tileA, tileB));
        }

        private IEnumerator SwapAndMatchTilesRoutine(IGameTile tileA, IGameTile tileB)
        {            
            yield return SwapTilesRoutine(tileA, tileB);

            bool isMatching = _matchStrategy.TryMatch(
                tileA, tileB, out var matchA, out var matchB);

            if (isMatching)
            {
                ClearMatchedTiles(matchA);
                ClearMatchedTiles(matchB);
                Fill();
            }
            else
            {
                // Reverse movement
                yield return SwapTilesRoutine(tileA, tileB);
            }

            _isTilesSwapping = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IEnumerator SwapTilesRoutine(IGameTile tileA, IGameTile tileB)
        {
            Coroutine[] tileMovingCoroutines = SwapTilesAndGetMovementCoroutines(tileA, tileB);
            foreach (Coroutine coroutine in tileMovingCoroutines)
            {
                yield return coroutine;
            }
        }

        private Coroutine[] SwapTilesAndGetMovementCoroutines(IGameTile tileA, IGameTile tileB)
        {
            IGameItem itemA = tileA.CurrentItem;
            IGameItem itemB = tileB.CurrentItem;

            tileA.SetItem(itemB);
            tileB.SetItem(itemA);

            Vector2 tileAWorldPosition = this.GetWorldPosition(tileA.GetBoardPosition());
            Vector2 tileBWorldPosition = this.GetWorldPosition(tileB.GetBoardPosition());

            Coroutine moveItemB = StartCoroutine(itemB.SetPositionTo(tileAWorldPosition));
            Coroutine moveItemA = StartCoroutine(itemA.SetPositionTo(tileBWorldPosition));
            return new Coroutine[] { moveItemA, moveItemB };
        }

        private void ClearMatchedTiles(IReadOnlyCollection<IGameTile> matchingTiles)
        {
            foreach (var tile in matchingTiles)
            {
                tile.CurrentItem.SetVisible(false);
            }
        }

        public void SetMatchStrategy(IMatchStrategy matchStrategy)
        {
            _matchStrategy = matchStrategy;
        }

        public bool TryGetBoardPosition(Vector2 worldPosition, out int row, out int column)
        {
            Vector2 halfSize = _tileSize / 2f;
            row = (int)Mathf.Floor(-(worldPosition - _tileOrigin).y + halfSize.y);
            column = (int)Mathf.Floor((worldPosition - _tileOrigin).x + halfSize.x);
            return row >= 0 && column >= 0;
        }
    }
}