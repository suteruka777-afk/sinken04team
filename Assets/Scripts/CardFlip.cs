using UnityEngine;

public class CardFlip : MonoBehaviour
{
    public GameObject frontSprite;
    public GameObject backSprite;

    // カードの種類
    public int cardID;

    private bool isFront = false;

    private void Start()
    {
        frontSprite.SetActive(false);
        backSprite.SetActive(true);
    }

    public void FlipCard()
    {
        isFront = !isFront;

        frontSprite.SetActive(isFront);
        backSprite.SetActive(!isFront);
    }

    public void OnClickCard()
    {
        if (!isFront)
        {
            // 2枚選択済みならクリックできない
            if (GameManager.Instance != null)
            {
                if (!GameManager.Instance.CanSelectCard())
                {
                    return;
                }

                FlipCard();
                GameManager.Instance.SelectCard(this);
            }
            else
            {
                FlipCard();
            }
        }
    }
}