using Core.Abstractions;
using DependencyResolving;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Items
{
    public sealed class GameItemPool : MonoBehaviour, IGameItemPool
    {
        [SerializeField] private GameItem gameItemPrefab;
        [SerializeField] private GameItemRepository gameItemRepository;

        private readonly Queue<IGameItem> _itemsPool = new Queue<IGameItem>();

        public void Initialize(IDependencyResolver resolver)
        {
            IGameBoard gameBoard = resolver.Resolve<IGameBoard>();
            CreateItemPool(gameBoard.RowCount * gameBoard.ColumnCount);
        }

        private void CreateItemPool(int poolSize)
        {
            for (int i = 0; i < poolSize; i++)
            {
                IGameItem item = Instantiate(gameItemPrefab);
                _itemsPool.Enqueue(item);
            }
        }

        public void AddItem(IGameItem item)
        {
            _itemsPool.Enqueue(item);
        }

        public IGameItem GetItem()
        {
            IGameItem item = _itemsPool.Dequeue();
            RandomizeItem(item);
            return item;
        }

        private void RandomizeItem(IGameItem item)
        {
            IReadOnlyList<Sprite> items = gameItemRepository.Items;
            int index = Random.Range(0, items.Count);
            item.SetType(index, items[index]);
        }
    }
}
