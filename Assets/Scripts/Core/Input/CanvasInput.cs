using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Input
{
    public sealed class CanvasInput : MonoBehaviour, IInput, IPointerClickHandler, IDragHandler
    {
        public event Action<Vector2> PointerClick;
        public event Action<Vector2> PointerDrag;

        [SerializeField] private new Camera camera;

        public void OnPointerClick(PointerEventData eventData)
        {
            PointerClick?.Invoke(GetWorldPosition(eventData.position));
        }

        public void OnDrag(PointerEventData eventData)
        {
            PointerDrag?.Invoke(GetWorldPosition(eventData.position));
        }

        private Vector2 GetWorldPosition(Vector2 canvasPosition)
        {
            return camera.ScreenToWorldPoint(canvasPosition);
        }
    }
}
