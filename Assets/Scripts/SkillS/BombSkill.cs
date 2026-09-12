using UnityEngine;

namespace Sinkei.Skills
{
    /// <summary>
    /// <爆弾> スキル
    /// キーボード側カーソル位置に爆弾を設置します。3秒後に爆発し、巻き込まれたマウスカーソルを3秒間フリーズさせます。
    /// ダミースキル発動中は使用できません。
    /// クールダウン: 15秒
    /// </summary>
    public class BombSkill : SkillBase
    {
        [Header("爆弾プレハブ")]
        [Tooltip("設置する爆弾プレハブ（BombObjectコンポーネント付き推奨）")]
        [SerializeField] private GameObject bombPrefab;

        [Header("ダミースキル参照（未指定時は自動検索）")]
        [Tooltip("ダミースキルの参照（発動中チェック用）")]
        [SerializeField] private DummySkill dummySkill;

        [Tooltip("爆弾の親オブジェクト（未指定時はルートに生成）")]
        [SerializeField] private Transform spawnParent;

        protected override void Awake()
        {
            base.Awake();
            if (dummySkill == null)
            {
                dummySkill = FindFirstObjectByType<DummySkill>();
            }
        }

        public override bool CanUse()
        {
            // ダミースキルが有効な間は爆弾を使用できない
            if (dummySkill != null && dummySkill.IsDummyActive)
            {
                return false;
            }

            // SkillManager経由でも検索チェック
            if (SkillManager.Instance != null)
            {
                DummySkill managerDummy = SkillManager.Instance.GetSkill<DummySkill>();
                if (managerDummy != null && managerDummy.IsDummyActive)
                {
                    return false;
                }
            }

            return base.CanUse();
        }

        protected override bool ExecuteSkill(Transform keyboardCursor)
        {
            if (bombPrefab == null)
            {
                Debug.LogWarning("[BombSkill] 爆弾プレハブが設定されていません。");
                return false;
            }

            Vector3 spawnPos = keyboardCursor != null ? keyboardCursor.position : Vector3.zero;
            spawnPos.z = 0f;

            if (spawnParent != null)
            {
                Instantiate(bombPrefab, spawnPos, Quaternion.identity, spawnParent);
            }
            else
            {
                Instantiate(bombPrefab, spawnPos, Quaternion.identity);
            }

            return true;
        }
    }
}
