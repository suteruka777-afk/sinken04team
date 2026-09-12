using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Sinkei.Skills
{
    /// <summary>
    /// すべての妨害スキルの基底クラス。
    /// クールダウン管理、UI連携（アイコン、クールダウンゲージ、暗転表示）、発動処理の基本フローを提供します。
    /// </summary>
    public abstract class SkillBase : MonoBehaviour
    {
        [Header("スキル基本設定")]
        [Tooltip("スキル名")]
        [SerializeField] private string skillName = "Skill";

        [Tooltip("クールダウン時間（秒）。0以下の場合はクールダウンなし")]
        [SerializeField] private float cooldownTime = 10f;

        [Tooltip("1ラウンドにつき1回のみ使用可能かどうか（シャッフル等）")]
        [SerializeField] private bool isOncePerRound = false;

        [Header("UI連携（任意）")]
        [Tooltip("スキルのアイコンImage")]
        [SerializeField] protected Image skillIconImage;

        [Tooltip("クールダウンを表すFillタイプのImage（ある場合）")]
        [SerializeField] protected Image cooldownFillImage;

        [Tooltip("スキル選択時のハイライト枠/オブジェクト")]
        [SerializeField] protected GameObject highlightObject;

        [Tooltip("使用不可時（1ラウンド1回使用後など）の暗転カラー")]
        [SerializeField] protected Color disabledColor = new Color(0.3f, 0.3f, 0.3f, 0.7f);

        // 内部状態
        protected float currentCooldown = 0f;
        protected bool isUsedInCurrentRound = false;
        protected bool isSelected = false;
        protected Color originalIconColor = Color.white;

        // プロパティ
        public string SkillName => skillName;
        public float CooldownTime => cooldownTime;
        public float CurrentCooldown => currentCooldown;
        public bool IsOnCooldown => currentCooldown > 0f;
        public bool IsSelected => isSelected;
        public bool IsUsedInCurrentRound => isUsedInCurrentRound;

        protected virtual void Awake()
        {
            if (skillIconImage != null)
            {
                originalIconColor = skillIconImage.color;
            }
            UpdateUI();
        }

        protected virtual void Update()
        {
            // クールダウンタイマーの更新
            if (currentCooldown > 0f)
            {
                currentCooldown -= Time.deltaTime;
                if (currentCooldown <= 0f)
                {
                    currentCooldown = 0f;
                    OnCooldownEnd();
                }
                UpdateCooldownUI();
            }
        }

        /// <summary>
        /// スキルが現在使用可能かどうかを判定します。
        /// </summary>
        public virtual bool CanUse()
        {
            // クールダウン中は使用不可
            if (IsOnCooldown) return false;

            // 1ラウンド1回制限で使用済みの場合は使用不可
            if (isOncePerRound && isUsedInCurrentRound) return false;

            return true;
        }

        /// <summary>
        /// スキルの発動を試みます。
        /// </summary>
        /// <param name="keyboardCursor">キーボード側カーソルのTransform</param>
        /// <returns>発動に成功した場合はtrue</returns>
        public bool TryExecute(Transform keyboardCursor)
        {
            if (!CanUse())
            {
                return false;
            }

            // 個別スキルの実行処理
            bool success = ExecuteSkill(keyboardCursor);

            if (success)
            {
                if (isOncePerRound)
                {
                    isUsedInCurrentRound = true;
                }

                if (cooldownTime > 0f)
                {
                    StartCooldown(cooldownTime);
                }

                UpdateUI();
            }

            return success;
        }

        /// <summary>
        /// 各スキルの具体的な効果を実装する抽象メソッド。
        /// </summary>
        /// <param name="keyboardCursor">キーボード側カーソルのTransform</param>
        /// <returns>発動成功時はtrue</returns>
        protected abstract bool ExecuteSkill(Transform keyboardCursor);

        /// <summary>
        /// クールダウンを開始します。
        /// </summary>
        public virtual void StartCooldown(float duration)
        {
            currentCooldown = duration;
            UpdateCooldownUI();
        }

        /// <summary>
        /// クールダウン終了時の処理。
        /// </summary>
        protected virtual void OnCooldownEnd()
        {
            UpdateUI();
        }

        /// <summary>
        /// ハイライト（選択状態）の切り替え。
        /// </summary>
        public virtual void SetSelected(bool selected)
        {
            isSelected = selected;
            if (highlightObject != null)
            {
                highlightObject.SetActive(selected);
            }
        }

        /// <summary>
        /// ラウンド交代時などにスキル状態をリセットします。
        /// </summary>
        public virtual void ResetSkill()
        {
            currentCooldown = 0f;
            isUsedInCurrentRound = false;
            UpdateUI();
        }

        /// <summary>
        /// UI全体の表示状態を更新します。
        /// </summary>
        public virtual void UpdateUI()
        {
            if (skillIconImage != null)
            {
                if (isOncePerRound && isUsedInCurrentRound)
                {
                    // 1回限定スキル使用後は暗転
                    skillIconImage.color = disabledColor;
                }
                else
                {
                    skillIconImage.color = originalIconColor;
                }
            }

            UpdateCooldownUI();
        }

        /// <summary>
        /// クールダウンゲージUIを更新します。
        /// </summary>
        protected virtual void UpdateCooldownUI()
        {
            if (cooldownFillImage != null)
            {
                if (cooldownTime > 0f)
                {
                    cooldownFillImage.fillAmount = currentCooldown / cooldownTime;
                }
                else
                {
                    cooldownFillImage.fillAmount = 0f;
                }
            }
        }
    }
}
