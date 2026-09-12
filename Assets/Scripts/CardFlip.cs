using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CardFlip : MonoBehaviour
{
    public GameObject frontSprite;
    public GameObject backSprite;

    public int cardID;

    private bool isFront = false;

    private RectTransform cardRect;

    private void Start()
    {
        frontSprite.SetActive(false);
        backSprite.SetActive(true);

        // Cardを取得
        Transform card = transform.Find("Card");

        if (card != null)
        {
            cardRect = card.GetComponent<RectTransform>();
        }
        else
        {
            Debug.LogWarning("TrumpCardの中にCardが見つかりません");
        }
    }

    // クリック → 表裏を変更
    public void FlipCard()
    {
        isFront = !isFront;

        frontSprite.SetActive(isFront);
        backSprite.SetActive(!isFront);
    }

    // カードをクリック
    public void OnClickCard()
    {
        FlipCard();

        // Buttonの選択状態を解除
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void Update()
    {
        // Enterキー
        if (Keyboard.current != null &&
            Keyboard.current.enterKey.wasPressedThisFrame)
        {
            MoveCard();
        }
    }

    // Card全体を移動
    private void MoveCard()
    {
        if (cardRect == null)
        {
            Debug.LogWarning("CardのRectTransformがありません");
            return;
        }

        cardRect.anchoredPosition = new Vector2(0f, -800f);

        Debug.Log("Cardを(1000, -250)に移動しました！");
    }
}