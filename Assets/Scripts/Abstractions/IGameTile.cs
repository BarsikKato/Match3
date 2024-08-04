namespace Match3.Abstractions
{
    public interface IGameTile
    {
        int Row { get; }

        int Column { get; }

        bool IsFree { get; }

        IGameItem? CurrentItem { get; }

        void SetItem(IGameItem item);
    }
}
