namespace Core
{
    public readonly struct BoardPosition
    {
        public int Row { get; }

        public int Column { get; }

        /// <summary>
        /// BoardPosition(-1, 0)
        /// </summary>
        public static BoardPosition Up => new BoardPosition(-1, 0);

        /// <summary>
        /// BoardPosition(1, 0)
        /// </summary>
        public static BoardPosition Down => new BoardPosition(1, 0);

        /// <summary>
        /// BoardPosition(0, 1)
        /// </summary>
        public static BoardPosition Right => new BoardPosition(0, 1);

        /// <summary>
        /// BoardPosition(0, -1)
        /// </summary>
        public static BoardPosition Left => new BoardPosition(0, -1);

        /// <summary>
        /// Higher position on board.
        /// </summary>
        public BoardPosition UpPosition => this + Up;

        /// <summary>
        /// Lower position on board.
        /// </summary>
        public BoardPosition DownPosition => this + Down;

        /// <summary>
        /// Position on board to the right.
        /// </summary>
        public BoardPosition RightPosition => this + Right;

        /// <summary>
        /// Position on board to the left.
        /// </summary>
        public BoardPosition LeftPosition => this + Left;

        public BoardPosition(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public static BoardPosition operator +(BoardPosition a, BoardPosition b)
        {
            return new BoardPosition(a.Row + b.Row, a.Column + b.Column);
        }

        public static BoardPosition operator -(BoardPosition a, BoardPosition b)
        {
            return new BoardPosition(a.Row - b.Row, a.Column - b.Column);
        }

        public static implicit operator (int row, int column)(BoardPosition boardPosition)
        {
            return (boardPosition.Row, boardPosition.Column);
        }

        public override string ToString()
        {
            return $"({Row}, {Column})";
        }
    }
}
