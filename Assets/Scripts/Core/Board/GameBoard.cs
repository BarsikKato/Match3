using Core.Abstractions;
using Core.Fill;
using Core.Tiles.Factory;
using DependencyResolving;
using System.Collections.Generic;
using System.Threading.Tasks;
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

            SwapTiles(_grid[new BoardPosition(0, 0)], _grid[new BoardPosition(0, 1)]);
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
            return _tileOrigin + new Vector2(row * _tileSize.x, column * -_tileSize.y);
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
            SwapTest(tileA, tileB);
        }

        private async void SwapTest(IGameTile tileA, IGameTile tileB)
        {
            await Task.Delay(5000);

            IGameItem itemA = tileA.CurrentItem;
            IGameItem itemB = tileB.CurrentItem;
            tileA.SetItem(itemB);
            itemB.SetPositionTo(GetWorldPosition(tileA.Row, tileA.Column));
            tileB.SetItem(itemA);
            itemA.SetPositionTo(GetWorldPosition(tileB.Row, tileB.Column));

            bool isMatching = _matchStrategy.TryMatch(
                tileA, tileB, out var matchA, out var matchB);

            if (isMatching)
            {
                ClearMatchedTiles(matchA);
                ClearMatchedTiles(matchB);
            }
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
    }
}