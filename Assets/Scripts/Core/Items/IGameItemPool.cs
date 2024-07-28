using Core.Abstractions;

namespace Core.Items
{
    public interface IGameItemPool
    {
        void AddItem(IGameItem item);

        IGameItem GetItem();
    }
}
