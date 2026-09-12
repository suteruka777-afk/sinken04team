using UnityEngine;

namespace Sinkei.Skills
{
    /// <summary>
    /// <煙幕> スキル
    /// キーボード側カーソルを中心に周囲のカードを覆う煙幕を5秒間発生させます。
    /// クールダウン: 10秒
    /// </summary>
    public class SmokescreenSkill : SkillBase
    {
        [Header("煙幕プレハブ")]
        [Tooltip("発生させる煙幕プレハブ（SmokescreenEffectコンポーネント付き推奨）")]
        [SerializeField] private GameObject smokescreenPrefab;

        [Tooltip("煙幕の親オブジェクト（未指定の場合はルートに生成）")]
        [SerializeField] private Transform spawnParent;

        protected override bool ExecuteSkill(Transform keyboardCursor)
        {
            if (smokescreenPrefab == null)
            {
                Debug.LogWarning("[SmokescreenSkill] 煙幕プレハブが設定されていません。");
                return false;
            }

            Vector3 spawnPos = keyboardCursor != null ? keyboardCursor.position : Vector3.zero;
            // 2D用にZ座標を適切な値（例: 0）に合わせる
            spawnPos.z = 0f;

            if (spawnParent != null)
            {
                Instantiate(smokescreenPrefab, spawnPos, Quaternion.identity, spawnParent);
            }
            else
            {
                Instantiate(smokescreenPrefab, spawnPos, Quaternion.identity);
            }

            return true;
        }
    }
}
