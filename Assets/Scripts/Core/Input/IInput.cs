using System;
using UnityEngine;

namespace Core.Input
{
    public interface IInput
    {
        event Action<Vector2> PointerDown;

        event Action<Vector2> PointerDrag;
    }
}