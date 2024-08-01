using System;
using UnityEngine;

namespace Core.Input
{
    public interface IInput
    {
        event Action<Vector2> PointerClick;

        event Action<Vector2> PointerDrag;
    }
}