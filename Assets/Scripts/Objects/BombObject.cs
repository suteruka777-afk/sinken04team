using System.Collections;
using UnityEngine;
using Sinkei.Controllers;

namespace Sinkei.Skills
{
    /// <summary>
    /// 設置された爆弾オブジェクトを制御するスクリプト。
    /// 3秒のカウントダウン後に爆発し、範囲内のマウスカーソルを3秒間フリーズさせます。
    /// </summary>
    public class BombObject : MonoBehaviour
    {
        [Header("爆発タイミング設定")]
        [Tooltip("設置から爆発までの時間（秒）")]
        [SerializeField] private float countdownTime = 3f;

        [Tooltip("爆発によるマウスカーソル固定時間（秒）")]
        [SerializeField] private float freezeDuration = 3f;

        [Header("爆発演出・判定")]
        [Tooltip("別個の爆発エフェクトPrefab（ある場合、爆発時に生成）")]
        [SerializeField] private GameObject explosionEffectPrefab;

        [Tooltip("爆発時のスプライト（Prefabを使わず自身を爆発画像に切り替える場合）")]
        [SerializeField] private Sprite explosionSprite;

        [Tooltip("爆発判定用コライダー（爆発時に有効化）")]
        [SerializeField] private Collider2D explosionCollider;

        [Tooltip("コライダーを使わない場合の爆発判定半径（Physics2Dによる範囲判定）")]
        [SerializeField] private float explosionRadius = 1.5f;

        [Tooltip("爆発エフェクトの表示持続時間（秒）")]
        [SerializeField] private float explosionLifetime = 0.5f;

        [Header("点滅演出（任意）")]
        [Tooltip("カウントダウン中に点滅させるかどうか")]
        [SerializeField] private bool enableBlink = true;

        [Tooltip("点滅カラー")]
        [SerializeField] private Color blinkColor = Color.red;

        private SpriteRenderer spriteRenderer;
        private Color initialColor;
        private bool hasExploded = false;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                initialColor = spriteRenderer.color;
            }

            if (explosionCollider != null)
            {
                explosionCollider.enabled = false;
            }
        }

        private void Start()
        {
            StartCoroutine(CountdownRoutine());
        }

        private IEnumerator CountdownRoutine()
        {
            float elapsed = 0f;

            // 3秒間のカウントダウン（徐々に点滅が速くなる演出）
            while (elapsed < countdownTime)
            {
                elapsed += Time.deltaTime;

                if (enableBlink && spriteRenderer != null)
                {
                    // 残り時間が短くなるほど点滅速度が速くなる
                    float frequency = Mathf.Lerp(2f, 10f, elapsed / countdownTime);
                    float lerpVal = Mathf.PingPong(elapsed * frequency, 1f);
                    spriteRenderer.color = Color.Lerp(initialColor, blinkColor, lerpVal);
                }

                yield return null;
            }

            Explode();
        }

        /// <summary>
        /// 爆発処理を実行
        /// </summary>
        private void Explode()
        {
            if (hasExploded) return;
            hasExploded = true;

            // 1. 別プレハブの爆発エフェクトがある場合は生成
            if (explosionEffectPrefab != null)
            {
                GameObject fx = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
                Destroy(fx, explosionLifetime);
            }

            // 2. 自身を爆発スプライトに切り替え
            if (explosionSprite != null && spriteRenderer != null)
            {
                spriteRenderer.sprite = explosionSprite;
                spriteRenderer.color = Color.white;
            }

            // 3. コライダーによる判定を有効化
            if (explosionCollider != null)
            {
                explosionCollider.enabled = true;
            }

            // 4. OverlapCircleによる確実な範囲判定（コライダー未検知の保険）
            CheckMouseHitInRadius();

            // 一定時間後に自身を破棄
            Destroy(gameObject, explosionLifetime);
        }

        /// <summary>
        /// 半径内のマウスカーソルを検知してフリーズさせる
        /// </summary>
        private void CheckMouseHitInRadius()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            foreach (var hit in hits)
            {
                ApplyFreezeIfMouse(hit.gameObject);
            }

            // シングルトン / インスタンスが存在する場合の距離チェック（保険）
            if (MouseCursorController.Instance != null)
            {
                float dist = Vector2.Distance(transform.position, MouseCursorController.Instance.transform.position);
                if (dist <= explosionRadius)
                {
                    MouseCursorController.Instance.Freeze(freezeDuration);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (hasExploded)
            {
                ApplyFreezeIfMouse(other.gameObject);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (hasExploded)
            {
                ApplyFreezeIfMouse(collision.gameObject);
            }
        }

        /// <summary>
        /// 対象がマウスカーソルの場合にフリーズを付与
        /// </summary>
        private void ApplyFreezeIfMouse(GameObject target)
        {
            MouseCursorController mouseCursor = target.GetComponent<MouseCursorController>();
            if (mouseCursor == null)
            {
                mouseCursor = target.GetComponentInParent<MouseCursorController>();
            }

            if (mouseCursor != null)
            {
                mouseCursor.Freeze(freezeDuration);
            }
        }

        private void OnDrawGizmosSelected()
        {
            // エディタ上での爆発範囲の可視化
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}
