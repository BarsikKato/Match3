namespace Match3.Abstractions
{
    public interface IFillStrategy
    {
        bool IsBoardFilled { get; }

        void FillBoard();
    }
}
