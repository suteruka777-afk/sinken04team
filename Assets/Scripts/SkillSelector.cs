using UnityEngine;
using UnityEngine.UI;

public class SkillSelector : MonoBehaviour
{
    [SerializeField] private RectTransform[] skillImages;//スキル画像を入れる
    [SerializeField] private RectTransform selectionFrame;//強調用のフレーム

    private int currentIndex = 0;

    private void Start()
    {
        UpdateSelection();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentIndex--;

            if (currentIndex < 0)
                currentIndex = skillImages.Length - 1;

            UpdateSelection();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            currentIndex++;

            if (currentIndex >= skillImages.Length)
                currentIndex = 0;

            UpdateSelection();
        }
    }

    private void UpdateSelection()
    {
        selectionFrame.position = skillImages[currentIndex].position;
    }

    public int GetSelectedIndex()
    {
        return currentIndex;
    }
}