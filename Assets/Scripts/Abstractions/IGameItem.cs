using System.Collections;
using UnityEngine;

namespace Match3.Abstractions
{
    public interface IGameItem
    {
        int Type { get; }

        IEnumerator SetPositionTo(Vector2 position);

        void SetVisible(bool value);

        void SetType(int type, Sprite sprite);
    }
}
