using Core.Abstractions;
using System.Collections;
using UnityEngine;

namespace Core.Items
{
    public sealed class GameItem : MonoBehaviour, IGameItem
    {
        public const int UnmatchableType = -1;
        private const float SwapTime = 0.25f;

        [SerializeField] private SpriteRenderer spriteRenderer;

        private int _type;

        public int Type => _type;

        public IEnumerator SetPositionTo(Vector2 position)
        {
            float time = 0;
            Vector2 startPosition = transform.position;
            while (time <= 1)
            {
                transform.position = Vector2.Lerp(startPosition, position, time);
                time += Time.deltaTime / SwapTime;
                yield return null;
            }

            transform.position = position;
        }

        public void SetVisible(bool value)
        {
            spriteRenderer.enabled = value;
            if (value == false)
            {
                SetType(UnmatchableType, null);
            }
        }

        public void SetType(int type, Sprite sprite)
        {
            _type = type;
            spriteRenderer.sprite = sprite;
        }
    }
}