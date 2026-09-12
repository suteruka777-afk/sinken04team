using System.Collections;
using UnityEngine;
using Sinkei.Controllers;

namespace Sinkei.Skills
{
    /// <summary>
    /// <ダミー> スキル
    /// 10秒間、キーボード側カーソルの見た目をマウス側のカーソルと同じにします。
    /// 効果発動中は <爆弾> スキルの使用が禁止されます。
    /// クールダウン: 10秒
    /// </summary>
    public class DummySkill : SkillBase
    {
        [Header("ダミー設定")]
        [Tooltip("偽装するマウスカーソルのスプライト")]
        [SerializeField] private Sprite mouseCursorSprite;

        [Tooltip("ダミー効果の持続時間（秒）")]
        [SerializeField] private float duration = 10f;

        [Tooltip("キーボードカーソル（未指定時は発動元のTransformから取得）")]
        [SerializeField] private KeyboardCursorController keyboardCursor;

        // ダミー効果発動中フラグ（爆弾スキルの発動制限に使用）
        private bool isDummyActive = false;
        public bool IsDummyActive => isDummyActive;

        private Coroutine dummyCoroutine;

        public override bool CanUse()
        {
            // 既にダミー効果が発動中の場合は重複発動不可
            if (isDummyActive) return false;

            return base.CanUse();
        }

        protected override bool ExecuteSkill(Transform keyboardCursorTransform)
        {
            KeyboardCursorController controller = keyboardCursor;
            if (controller == null && keyboardCursorTransform != null)
            {
                controller = keyboardCursorTransform.GetComponent<KeyboardCursorController>();
            }

            if (controller == null && keyboardCursorTransform != null)
            {
                // KeyboardCursorControllerが直接ない場合、SpriteRendererを直接操作
                var sr = keyboardCursorTransform.GetComponent<SpriteRenderer>();
                if (sr != null && mouseCursorSprite != null)
                {
                    if (dummyCoroutine != null) StopCoroutine(dummyCoroutine);
                    dummyCoroutine = StartCoroutine(DirectSpriteRoutine(sr));
                    return true;
                }
            }

            if (controller == null)
            {
                Debug.LogWarning("[DummySkill] KeyboardCursorControllerまたはSpriteRendererが見つかりません。");
                return false;
            }

            if (mouseCursorSprite == null)
            {
                Debug.LogWarning("[DummySkill] マウスカーソルのスプライト(mouseCursorSprite)が設定されていません。");
                return false;
            }

            if (dummyCoroutine != null)
            {
                StopCoroutine(dummyCoroutine);
            }
            dummyCoroutine = StartCoroutine(DummyRoutine(controller));

            return true;
        }

        private IEnumerator DummyRoutine(KeyboardCursorController controller)
        {
            isDummyActive = true;
            controller.ChangeSprite(mouseCursorSprite);

            yield return new WaitForSeconds(duration);

            controller.RestoreSprite();
            isDummyActive = false;
            dummyCoroutine = null;
        }

        private IEnumerator DirectSpriteRoutine(SpriteRenderer sr)
        {
            isDummyActive = true;
            Sprite original = sr.sprite;
            sr.sprite = mouseCursorSprite;

            yield return new WaitForSeconds(duration);

            sr.sprite = original;
            isDummyActive = false;
            dummyCoroutine = null;
        }

        public override void ResetSkill()
        {
            if (dummyCoroutine != null)
            {
                StopCoroutine(dummyCoroutine);
                dummyCoroutine = null;
            }

            if (isDummyActive)
            {
                if (keyboardCursor != null)
                {
                    keyboardCursor.RestoreSprite();
                }
                isDummyActive = false;
            }

            base.ResetSkill();
        }
    }
}
