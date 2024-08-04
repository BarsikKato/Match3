namespace Core.Abstractions
{
    public interface IFillStrategy
    {
        bool IsBoardFilled { get; }

        void FillBoard();
    }
}
