using UnityEngine;

namespace Sinkei.Controllers
{
    /// <summary>
    /// キーボード側プレイヤーのカーソルを制御するスクリプト。
    /// スプライトの切り替え（ダミースキル用）、描画レイヤー管理、WASD移動（必要に応じて有効化）を提供します。
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class KeyboardCursorController : MonoBehaviour
    {
        [Header("スプライト設定")]
        [Tooltip("キーボードカーソルの通常時のスプライト（未設定の場合は初期スプライトを使用）")]
        [SerializeField] private Sprite normalSprite;

        [Header("描画レイヤー設定")]
        [Tooltip("カーソルのSortingLayer名")]
        [SerializeField] private string sortingLayerName = "";

        [Tooltip("Order in Layer（カードや煙幕よりも最前面にするため大きめの値。例: 10）")]
        [SerializeField] private int orderInLayer = 10;

        [Header("移動設定（既存の移動処理がある場合はオフに設定可能）")]
        [Tooltip("このスクリプト側でWASD移動を処理するかどうか")]
        [SerializeField] private bool enableMovement = false;

        [Tooltip("移動速度")]
        [SerializeField] private float moveSpeed = 8f;

        [Tooltip("移動可能範囲の制限（X最小, X最大, Y最小, Y最大）")]
        [SerializeField] private Vector4 movementBounds = new Vector4(-8f, 8f, -4.5f, 4.5f);

        private SpriteRenderer spriteRenderer;
        private Sprite defaultSprite;

        public SpriteRenderer SpriteRenderer => spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                defaultSprite = normalSprite != null ? normalSprite : spriteRenderer.sprite;
                spriteRenderer.sprite = defaultSprite;

                if (!string.IsNullOrEmpty(sortingLayerName))
                {
                    spriteRenderer.sortingLayerName = sortingLayerName;
                }
                spriteRenderer.sortingOrder = orderInLayer;
            }
        }

        private void Update()
        {
            if (enableMovement)
            {
                HandleMovement();
            }
        }

        /// <summary>
        /// WASD移動処理
        /// </summary>
        private void HandleMovement()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 movement = new Vector3(horizontal, vertical, 0f).normalized;
            Vector3 newPosition = transform.position + movement * (moveSpeed * Time.deltaTime);

            // 移動範囲制限
            newPosition.x = Mathf.Clamp(newPosition.x, movementBounds.x, movementBounds.y);
            newPosition.y = Mathf.Clamp(newPosition.z, movementBounds.z, movementBounds.w);

            transform.position = newPosition;
        }

        /// <summary>
        /// カーソルのスプライトを指定のスプライトに変更します（ダミースキル用）。
        /// </summary>
        /// <param name="newSprite">変更先のスプライト</param>
        public void ChangeSprite(Sprite newSprite)
        {
            if (spriteRenderer != null && newSprite != null)
            {
                spriteRenderer.sprite = newSprite;
            }
        }

        /// <summary>
        /// カーソルのスプライトを通常時のスプライトに戻します。
        /// </summary>
        public void RestoreSprite()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = defaultSprite;
            }
        }
    }
}
