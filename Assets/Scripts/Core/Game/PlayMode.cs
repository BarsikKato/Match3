using Match3.Abstractions;
using Match3.Core.Input;
using UnityEngine;

namespace Match3.Core.Game
{
    public sealed class PlayMode : IGameMode
    {
        private readonly IGameBoard _gameBoard;
        private readonly IInput _input;

        private IGameTile _lastSelectedTile;

        public PlayMode(IGameBoard gameBoard, IInput input)
        {
            _gameBoard = gameBoard;
            _input = input;
        }

        public void Enter()
        {
            _input.PointerDown += Input_PointerClick;
            _input.PointerDrag += Input_PointerDrag;
        }

        public void Exit()
        {
            _input.PointerDown -= Input_PointerClick;
            _input.PointerDrag -= Input_PointerDrag;
        }

        private void Input_PointerClick(Vector2 position)
        {
            if (_gameBoard.TryGetBoardPosition(position, out BoardPosition boardPosition))
            {
                _lastSelectedTile = _gameBoard.GetTile(boardPosition);
            }
        }

        private void Input_PointerDrag(Vector2 position)
        {
            if (_lastSelectedTile == null)
                return;

            if (_gameBoard.TryGetBoardPosition(position, out BoardPosition boardPosition))
            {
                IGameTile tile = _gameBoard.GetTile(boardPosition);
                if (tile != null && _lastSelectedTile != tile)
                {
                    _gameBoard.SwapTiles(_lastSelectedTile, tile);
                    _lastSelectedTile = null;
                }
            }
        }
    }
}
