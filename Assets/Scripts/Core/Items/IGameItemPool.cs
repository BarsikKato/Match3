using Match3.Abstractions;

namespace Match3.Core.Items
{
    public interface IGameItemPool
    {
        void AddItem(IGameItem item);

        IGameItem GetItem();
    }
}
