using Match3.Abstractions;
using Match3.Core.Fill;
using Match3.Core.Tiles;
using Match3.Core.Tiles.Factory;
using Match3.DependencyResolving;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Match3.Core.Board
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
            fillStrategy.Initialize(dependencyResolver, this);
            SetFillStrategy(fillStrategy);
            Fill();
        }

        private void Fill()
        {
            _fillStrategy.FillBoard();
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
                    CreateBoardTile(tileFactory, boardPosition);
                }
            }
        }

        private void CreateBoardTile(
            ITileFactory factory, BoardPosition boardPosition)
        {
            IGameTile gameTile = factory.Create(boardPosition);
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

            if (_matchStrategy.TryFindMatchingTiles(tileA, out var matchA) |
                _matchStrategy.TryFindMatchingTiles(tileB, out var matchB))
            {
                List<IGameTile> allMatches = matchA.Concat(matchB).ToList();
                ClearTiles(allMatches);

                bool anyMoreMatchings;
                do
                {
                    anyMoreMatchings = false;
                    Fill();
                    WaitUntil waitUntilFilled = new WaitUntil(() => _fillStrategy.IsBoardFilled);
                    yield return waitUntilFilled;
                    anyMoreMatchings = HasAdditionalMatchings(out var presentedMatches);
                    ClearTiles(presentedMatches);
                }
                while (anyMoreMatchings);
            }
            else
            {
                // Reverse movement
                yield return SwapTilesRoutine(tileA, tileB);
            }

            _isTilesSwapping = false;
        }

        /// <summary>
        /// Checks if any more matchings present on the board.
        /// </summary>
        /// <returns>Is matchings present</returns>
        private bool HasAdditionalMatchings(out IReadOnlyCollection<IGameTile> presentedMatches)
        {
            bool hasAdditionalMatchings = false;
            List<IGameTile> allMatches = new List<IGameTile>();
            foreach (var tile in _grid.Values)
            {
                if (_matchStrategy.TryFindMatchingTiles(tile, out var matches))
                {
                    allMatches.AddRange(matches);
                    hasAdditionalMatchings = true;
                }
            }

            presentedMatches = allMatches;
            return hasAdditionalMatchings;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClearTiles(IReadOnlyCollection<IGameTile> tiles)
        {
            foreach (var tile in tiles)
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
            return _grid.ContainsKey(new BoardPosition(row, column));
        }
    }
}