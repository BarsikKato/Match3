using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Input
{
    public sealed class CanvasInput : MonoBehaviour, IInput, IPointerDownHandler, IDragHandler
    {
        public event Action<Vector2> PointerDown;
        public event Action<Vector2> PointerDrag;

        [SerializeField] private new Camera camera;

        public void OnPointerDown(PointerEventData eventData)
        {
            Vector2 position = GetWorldPosition(eventData.position);
            PointerDown?.Invoke(position);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 position = GetWorldPosition(eventData.position);
            PointerDrag?.Invoke(position);
        }

        private Vector2 GetWorldPosition(Vector2 canvasPosition)
        {
            return camera.ScreenToWorldPoint(canvasPosition);
        }
    }
}
