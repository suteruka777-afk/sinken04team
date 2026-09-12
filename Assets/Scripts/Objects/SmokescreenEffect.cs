using System.Collections;
using UnityEngine;

namespace Sinkei.Skills
{
    /// <summary>
    /// 煙幕エフェクトオブジェクトを制御するスクリプト。
    /// 5秒間の持続時間、表示レイヤー（カード < 煙 < カーソル）、フェードイン/アウト処理を管理します。
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class SmokescreenEffect : MonoBehaviour
    {
        [Header("持続時間設定")]
        [Tooltip("煙幕が持続する時間（秒）")]
        [SerializeField] private float duration = 5f;

        [Tooltip("終了時のフェードアウト時間（秒）")]
        [SerializeField] private float fadeOutDuration = 0.5f;

        [Header("描画レイヤー設定")]
        [Tooltip("カードより手前、カーソルより奥になるSortingLayer名（空欄の場合はOrderのみ変更）")]
        [SerializeField] private string sortingLayerName = "";

        [Tooltip("Order in Layer（カードより大きく、カーソルより小さく設定。例: カード0, 煙幕5, カーソル10）")]
        [SerializeField] private int orderInLayer = 5;

        private SpriteRenderer spriteRenderer;
        private Color initialColor;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                initialColor = spriteRenderer.color;

                if (!string.IsNullOrEmpty(sortingLayerName))
                {
                    spriteRenderer.sortingLayerName = sortingLayerName;
                }
                spriteRenderer.sortingOrder = orderInLayer;
            }
        }

        private void Start()
        {
            StartCoroutine(LifetimeRoutine());
        }

        private IEnumerator LifetimeRoutine()
        {
            float activeDuration = Mathf.Max(0f, duration - fadeOutDuration);
            yield return new WaitForSeconds(activeDuration);

            // フェードアウト
            if (spriteRenderer != null && fadeOutDuration > 0f)
            {
                float elapsed = 0f;
                while (elapsed < fadeOutDuration)
                {
                    elapsed += Time.deltaTime;
                    float alpha = Mathf.Lerp(initialColor.a, 0f, elapsed / fadeOutDuration);
                    Color col = spriteRenderer.color;
                    col.a = alpha;
                    spriteRenderer.color = col;
                    yield return null;
                }
            }

            Destroy(gameObject);
        }
    }
}
