using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sinkei.Skills
{
    /// <summary>
    /// 妨害スキル全体の統括マネージャー。
    /// スキルの選択状態の同期、Spaceキーによるスキル発動、相互依存条件のチェック、ラウンドリセットを管理します。
    /// </summary>
    public class SkillManager : MonoBehaviour
    {
        public static SkillManager Instance { get; private set; }

        [Header("キーボード側カーソル参照")]
        [Tooltip("キーボード側カーソルのTransform（スキルの出現位置基準）")]
        [SerializeField] private Transform keyboardCursorTransform;

        [Header("登録スキル一覧（左から順に並べて登録）")]
        [Tooltip("スキルコンポーネント（SmokescreenSkill, DummySkill, BombSkill, ShuffleSkill等）")]
        [SerializeField] private SkillBase[] skills;

        [Header("操作設定")]
        [Tooltip("スキル発動キー（デフォルト: Space）")]
        [SerializeField] private KeyCode activateKey = KeyCode.Space;

        [Tooltip("本スクリプト側でもQ/Eキーでの選択移動を処理するかどうか（既存スクリプトで制御済みの場合はOFFでも可）")]
        [SerializeField] private bool handleSelectInput = true;

        [Tooltip("スキル選択（左）キー")]
        [SerializeField] private KeyCode selectPrevKey = KeyCode.Q;

        [Tooltip("スキル選択（右）キー")]
        [SerializeField] private KeyCode selectNextKey = KeyCode.E;

        // 現在選択中のスキルインデックス
        private int currentSelectedIndex = 0;

        // イベント
        public event Action<int> OnSkillSelectionChanged;
        public event Action<SkillBase> OnSkillExecuted;
        public event Action<SkillBase> OnSkillFailed;

        // プロパティ
        public int CurrentSelectedIndex => currentSelectedIndex;
        public SkillBase CurrentSelectedSkill => (skills != null && currentSelectedIndex >= 0 && currentSelectedIndex < skills.Length) ? skills[currentSelectedIndex] : null;
        public Transform KeyboardCursorTransform => keyboardCursorTransform;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            // 初期選択状態の反映
            if (skills != null && skills.Length > 0)
            {
                SelectSkill(0);
            }
        }

        private void Update()
        {
            // Q/Eキーでのスキル選択操作（オプション）
            if (handleSelectInput && skills != null && skills.Length > 0)
            {
                if (Input.GetKeyDown(selectPrevKey))
                {
                    SelectPreviousSkill();
                }
                else if (Input.GetKeyDown(selectNextKey))
                {
                    SelectNextSkill();
                }
            }

            // Spaceキーでのスキル発動
            if (Input.GetKeyDown(activateKey))
            {
                ExecuteCurrentSkill();
            }
        }

        /// <summary>
        /// 指定インデックスのスキルを選択します（Q/Eキー操作側から直接呼ぶことも可能）。
        /// </summary>
        /// <param name="index">スキルのインデックス</param>
        public void SelectSkill(int index)
        {
            if (skills == null || skills.Length == 0) return;

            currentSelectedIndex = Mathf.Clamp(index, 0, skills.Length - 1);

            for (int i = 0; i < skills.Length; i++)
            {
                if (skills[i] != null)
                {
                    skills[i].SetSelected(i == currentSelectedIndex);
                }
            }

            OnSkillSelectionChanged?.Invoke(currentSelectedIndex);
        }

        /// <summary>
        /// 前（左）のスキルを選択します。
        /// </summary>
        public void SelectPreviousSkill()
        {
            if (skills == null || skills.Length == 0) return;
            int newIndex = currentSelectedIndex - 1;
            if (newIndex < 0) newIndex = skills.Length - 1;
            SelectSkill(newIndex);
        }

        /// <summary>
        /// 次（右）のスキルを選択します。
        /// </summary>
        public void SelectNextSkill()
        {
            if (skills == null || skills.Length == 0) return;
            int newIndex = (currentSelectedIndex + 1) % skills.Length;
            SelectSkill(newIndex);
        }

        /// <summary>
        /// 現在選択されているスキルを発動します。
        /// </summary>
        public bool ExecuteCurrentSkill()
        {
            SkillBase activeSkill = CurrentSelectedSkill;
            if (activeSkill == null) return false;

            Transform targetTransform = keyboardCursorTransform != null ? keyboardCursorTransform : this.transform;

            bool success = activeSkill.TryExecute(targetTransform);

            if (success)
            {
                OnSkillExecuted?.Invoke(activeSkill);
            }
            else
            {
                OnSkillFailed?.Invoke(activeSkill);
            }

            return success;
        }

        /// <summary>
        /// 特定の型のスキルを取得します。
        /// </summary>
        public T GetSkill<T>() where T : SkillBase
        {
            if (skills == null) return null;
            foreach (var skill in skills)
            {
                if (skill is T targetSkill)
                {
                    return targetSkill;
                }
            }
            return null;
        }

        /// <summary>
        /// ラウンド交代時などに全スキルの状態（クールダウン、使用フラグ）をリセットします。
        /// </summary>
        public void ResetAllSkills()
        {
            if (skills == null) return;
            foreach (var skill in skills)
            {
                if (skill != null)
                {
                    skill.ResetSkill();
                }
            }
        }
    }
}
