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
        [SerializeField] private Canvas canvas;

        public void OnPointerDown(PointerEventData eventData)
        {
            PointerDown?.Invoke(GetWorldPosition(eventData.position));
        }

        public void OnDrag(PointerEventData eventData)
        {
            PointerDrag?.Invoke(GetWorldPosition(eventData.position));
        }

        private Vector2 GetWorldPosition(Vector2 canvasPosition)
        {
            return camera.ScreenToWorldPoint(canvasPosition * canvas.scaleFactor);
        }
    }
}
