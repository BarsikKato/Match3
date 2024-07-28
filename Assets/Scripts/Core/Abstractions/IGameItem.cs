using UnityEngine;

namespace Core.Abstractions
{
    public interface IGameItem
    {
        int Type { get; }

        void SetPositionTo(Vector2 position);

        void SetVisible(bool value);

        void SetType(int type, Sprite sprite);
    }
}
