using System.Collections;
using UnityEngine;

namespace Sinkei.Controllers
{
    /// <summary>
    /// マウス側プレイヤーのカーソルを制御するスクリプト。
    /// マウス座標への追従と、爆弾スキルに巻き込まれた際の座標固定（3秒間フリーズ）を処理します。
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class MouseCursorController : MonoBehaviour
    {
        public static MouseCursorController Instance { get; private set; }

        [Header("カメラ参照")]
        [Tooltip("メインカメラ（未指定時はCamera.mainを使用）")]
        [SerializeField] private Camera targetCamera;

        [Header("フリーズ設定")]
        [Tooltip("フリーズ時のカーソルカラー（色を変えたい場合）")]
        [SerializeField] private Color frozenColor = new Color(0.6f, 0.8f, 1f, 0.8f);

        [Tooltip("フリーズ解除時の通常カラー")]
        [SerializeField] private Color normalColor = Color.white;

        [Header("描画レイヤー設定")]
        [Tooltip("カーソルのSortingLayer名")]
        [SerializeField] private string sortingLayerName = "";

        [Tooltip("Order in Layer（最前面表示用。例: 10）")]
        [SerializeField] private int orderInLayer = 10;

        private SpriteRenderer spriteRenderer;
        private bool isFrozen = false;
        private Vector3 frozenPosition;
        private Coroutine freezeCoroutine;

        public bool IsFrozen => isFrozen;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }

            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                if (!string.IsNullOrEmpty(sortingLayerName))
                {
                    spriteRenderer.sortingLayerName = sortingLayerName;
                }
                spriteRenderer.sortingOrder = orderInLayer;
            }
        }

        private void Update()
        {
            if (isFrozen)
            {
                // フリーズ中は固定座標を維持
                transform.position = frozenPosition;
                return;
            }

            // 通常時はマウス座標に追従
            FollowMousePosition();
        }

        /// <summary>
        /// マウス座標を取得してワールド座標に反映
        /// </summary>
        private void FollowMousePosition()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
                if (targetCamera == null) return;
            }

            Vector3 mouseScreenPos = Input.mousePosition;
            // 2Dカメラからの距離を設定（カメラがZ=-10の場合など）
            mouseScreenPos.z = -targetCamera.transform.position.z;

            Vector3 worldPos = targetCamera.ScreenToWorldPoint(mouseScreenPos);
            worldPos.z = 0f;

            transform.position = worldPos;
        }

        /// <summary>
        /// 指定された秒数間、マウスカーソルの座標を固定（操作不能に）します。
        /// </summary>
        /// <param name="duration">固定する時間（秒）</param>
        public void Freeze(float duration)
        {
            if (freezeCoroutine != null)
            {
                StopCoroutine(freezeCoroutine);
            }
            freezeCoroutine = StartCoroutine(FreezeRoutine(duration));
        }

        private IEnumerator FreezeRoutine(float duration)
        {
            isFrozen = true;
            frozenPosition = transform.position;

            if (spriteRenderer != null)
            {
                spriteRenderer.color = frozenColor;
            }

            yield return new WaitForSeconds(duration);

            isFrozen = false;
            if (spriteRenderer != null)
            {
                spriteRenderer.color = normalColor;
            }

            freezeCoroutine = null;
        }

        /// <summary>
        /// 強制的にフリーズを解除します。
        /// </summary>
        public void Unfreeze()
        {
            if (freezeCoroutine != null)
            {
                StopCoroutine(freezeCoroutine);
                freezeCoroutine = null;
            }

            isFrozen = false;
            if (spriteRenderer != null)
            {
                spriteRenderer.color = normalColor;
            }
        }
    }
}
