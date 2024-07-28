using System.Collections.Generic;
using UnityEngine;

namespace Core.Items
{
    [CreateAssetMenu(fileName = "Items Repository", menuName = "Game/Items Repository")]
    public sealed class GameItemRepository : ScriptableObject
    {
        [SerializeField] private List<Sprite> items;

        public IReadOnlyList<Sprite> Items => items;
    }
}