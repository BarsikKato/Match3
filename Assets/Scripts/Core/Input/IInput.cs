using System;
using UnityEngine;

namespace Match3.Core.Input
{
    public interface IInput
    {
        event Action<Vector2> PointerDown;

        event Action<Vector2> PointerDrag;
    }
}