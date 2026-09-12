using UnityEngine;
using TMPro;

public class GameManagerUI : MonoBehaviour
{
    // プレイヤーの得点
    private int player1Score = 0;
    private int player2Score = 0;

    // UI
    public TMP_Text player1ScoreText;
    public TMP_Text player2ScoreText;
    public TMP_Text messageText;
    public TMP_Text pointEffectText;

    void Start()
    {
        UpdateScoreUI();

        {
            Debug.LogError("23行目で yourVariable が null になっています！");
        }

        // +1表示を最初は非表示
        pointEffectText.gameObject.SetActive(false);
    }

    // PLAYER 1がペアを取った
    public void Player1GetPair()
    {
        player1Score++;

        UpdateScoreUI();

        messageText.text = "PLAYER 1 +1 POINT!";

        pointEffectText.text = "+1 POINT!";
        pointEffectText.gameObject.SetActive(true);
    }

    // PLAYER 2がペアを取った
    public void Player2GetPair()
    {
        player2Score++;

        UpdateScoreUI();

        messageText.text = "PLAYER 2 +1 POINT!";

        pointEffectText.text = "+1 POINT!";
        pointEffectText.gameObject.SetActive(true);
    }

    // UIのポイントを更新
    void UpdateScoreUI()
    {
        if (player1ScoreText != null)
            player1ScoreText.text = "★ " + player1Score + " POINT";
        if (player2ScoreText != null)
        player2ScoreText.text = "★ " + player2Score + " POINT";
    }
}