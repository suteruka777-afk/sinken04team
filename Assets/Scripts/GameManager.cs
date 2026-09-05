using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private CardFlip firstCard;
    private CardFlip secondCard;

    private bool checking = false;

    void Awake()
    {
        Instance = this;
    }

    public bool CanSelectCard()
    {
        // すでに判定中なら選択できない
        if (checking)
        {
            return false;
        }

        // 2枚選ばれていたら選択できない
        if (firstCard != null && secondCard != null)
        {
            return false;
        }

        return true;
    }

    public void SelectCard(CardFlip card)
    {
        if (firstCard == null)
        {
            firstCard = card;
        }
        else if (secondCard == null)
        {
            secondCard = card;

            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        checking = true;

        // 少し待つ
        yield return new WaitForSeconds(0.5f);

        // カードIDを比較
        if (firstCard.cardID == secondCard.cardID)
        {
            // 同じカード
            Destroy(firstCard.gameObject);
            Destroy(secondCard.gameObject);
        }
        else
        {
            // 違うカード
            firstCard.FlipCard();
            secondCard.FlipCard();
        }

        firstCard = null;
        secondCard = null;

        checking = false;
    }
}