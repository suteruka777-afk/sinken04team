using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sinkei.Skills
{
    /// <summary>
    /// <シャッフル> スキル
    /// フィールド上に残っているカード（回収エリアに移動していないカード）のみをランダムにシャッフルして再配置します。
    /// 1ラウンドにつき1回のみ使用可能（使用後はアイコン暗転、Space無効）。
    /// </summary>
    public class ShuffleSkill : SkillBase
    {
        [Header("フィールドスロット設定")]
        [Tooltip("カード配置用スロット（空オブジェクト）の親オブジェクト。指定すると子要素を自動でスロットとして取得します")]
        [SerializeField] private Transform slotsParent;

        [Tooltip("個別に指定する場合のスロット一覧（slotsParentが未指定の場合に使用）")]
        [SerializeField] private List<Transform> fieldSlots = new List<Transform>();

        [Header("カード検出設定")]
        [Tooltip("カードオブジェクトが属する親オブジェクト（全カードの親）。指定すると未回収カードの判定がスムーズになります")]
        [SerializeField] private Transform cardsParent;

        [Tooltip("カードのTag（指定した場合、このTagを持つオブジェクトのみを対象とします）")]
        [SerializeField] private string cardTag = "";

        [Tooltip("スロット座標との一致判定許容距離（単位: Unity単位）")]
        [SerializeField] private float slotSnapThreshold = 0.5f;

        [Header("演出設定")]
        [Tooltip("シャッフル時のカード移動をスムーズにするか（falseの場合は瞬時移動）")]
        [SerializeField] private bool animateShuffle = true;

        [Tooltip("カードが新しい位置へ移動する時間（秒）")]
        [SerializeField] private float moveDuration = 0.3f;

        protected override void Awake()
        {
            base.Awake();

            // 親オブジェクトからスロット一覧を自動収集
            if (slotsParent != null && fieldSlots.Count == 0)
            {
                foreach (Transform child in slotsParent)
                {
                    fieldSlots.Add(child);
                }
            }
        }

        protected override bool ExecuteSkill(Transform keyboardCursor)
        {
            // スロット一覧の再確認（親が設定されている場合）
            CollectSlotsIfNeeded();

            if (fieldSlots == null || fieldSlots.Count == 0)
            {
                Debug.LogWarning("[ShuffleSkill] フィールドスロットが設定されていません。");
                return false;
            }

            // フィールド内のスロット上にある有効なカードと、そのスロットを特定
            List<Transform> activeCards = new List<Transform>();
            List<Transform> occupiedSlots = new List<Transform>();

            FindActiveCardsAndSlots(out activeCards, out occupiedSlots);

            if (activeCards.Count <= 1)
            {
                Debug.Log("[ShuffleSkill] シャッフル対象のカードが1枚以下のため、シャッフルをスキップしました。");
                return true;
            }

            // スロット座標のリストを作成してシャッフル
            List<Vector3> targetPositions = new List<Vector3>();
            foreach (var slot in occupiedSlots)
            {
                targetPositions.Add(slot.position);
            }

            // Fisher-Yates シャッフルアルゴリズム
            ShuffleList(targetPositions);

            // 各カードを新しいスロット座標へ移動
            if (animateShuffle && moveDuration > 0f)
            {
                StartCoroutine(AnimateCardsRoutine(activeCards, targetPositions, occupiedSlots));
            }
            else
            {
                for (int i = 0; i < activeCards.Count; i++)
                {
                    activeCards[i].position = targetPositions[i];

                    // スロットの子オブジェクト構造を採用している場合は親子付けも更新
                    if (activeCards[i].parent == occupiedSlots[i])
                    {
                        // 新しい位置に対応するスロットを探して親子関係更新
                        Transform newSlot = occupiedSlots.Find(s => Vector3.Distance(s.position, targetPositions[i]) < 0.1f);
                        if (newSlot != null)
                        {
                            activeCards[i].SetParent(newSlot);
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 現在フィールド上に存在する未回収カードと対応スロットを検出します。
        /// </summary>
        private void FindActiveCardsAndSlots(out List<Transform> cards, out List<Transform> slots)
        {
            cards = new List<Transform>();
            slots = new List<Transform>();

            // パターン1: スロットの子オブジェクトとしてカードが存在する場合
            bool hasChildCards = false;
            foreach (var slot in fieldSlots)
            {
                if (slot == null) continue;
                if (slot.childCount > 0)
                {
                    Transform child = slot.GetChild(0);
                    if (string.IsNullOrEmpty(cardTag) || child.CompareTag(cardTag))
                    {
                        cards.Add(child);
                        slots.Add(slot);
                        hasChildCards = true;
                    }
                }
            }

            if (hasChildCards) return;

            // パターン2: カードが独立しており、スロット座標上に配置されている場合
            List<Transform> allCandidates = new List<Transform>();
            if (cardsParent != null)
            {
                foreach (Transform child in cardsParent)
                {
                    if (string.IsNullOrEmpty(cardTag) || child.CompareTag(cardTag))
                    {
                        allCandidates.Add(child);
                    }
                }
            }
            else
            {
                // cardsParent未指定時はタグまたは全GameObjectから検索
                if (!string.IsNullOrEmpty(cardTag))
                {
                    GameObject[] tagged = GameObject.FindGameObjectsWithTag(cardTag);
                    foreach (var go in tagged)
                    {
                        allCandidates.Add(go.transform);
                    }
                }
            }

            // スロットの座標と重なっているカードのみを抽出
            foreach (var slot in fieldSlots)
            {
                if (slot == null) continue;

                Transform closestCard = null;
                float minDistance = slotSnapThreshold;

                foreach (var card in allCandidates)
                {
                    float dist = Vector2.Distance(slot.position, card.position);
                    if (dist < minDistance && !cards.Contains(card))
                    {
                        minDistance = dist;
                        closestCard = card;
                    }
                }

                if (closestCard != null)
                {
                    cards.Add(closestCard);
                    slots.Add(slot);
                }
            }
        }

        /// <summary>
        /// リストをランダムにシャッフル（Fisher-Yates）
        /// </summary>
        private void ShuffleList<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                T temp = list[i];
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }

        /// <summary>
        /// カード移動のアニメーションコルーチン
        /// </summary>
        private IEnumerator AnimateCardsRoutine(List<Transform> cards, List<Vector3> targets, List<Transform> originalSlots)
        {
            List<Vector3> startPositions = new List<Vector3>();
            for (int i = 0; i < cards.Count; i++)
            {
                startPositions.Add(cards[i].position);
            }

            float elapsed = 0f;
            while (elapsed < moveDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / moveDuration);

                for (int i = 0; i < cards.Count; i++)
                {
                    if (cards[i] != null)
                    {
                        cards[i].position = Vector3.Lerp(startPositions[i], targets[i], t);
                    }
                }

                yield return null;
            }

            // 最終座標を確定＆親子関係の整合性更新
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i] != null)
                {
                    cards[i].position = targets[i];

                    if (originalSlots != null && i < originalSlots.Count && cards[i].parent == originalSlots[i])
                    {
                        Transform newSlot = fieldSlots.Find(s => s != null && Vector3.Distance(s.position, targets[i]) < 0.1f);
                        if (newSlot != null)
                        {
                            cards[i].SetParent(newSlot);
                        }
                    }
                }
            }
        }

        private void CollectSlotsIfNeeded()
        {
            if (slotsParent != null && fieldSlots.Count == 0)
            {
                fieldSlots.Clear();
                foreach (Transform child in slotsParent)
                {
                    fieldSlots.Add(child);
                }
            }
        }
    }
}
