using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SkillSelector : MonoBehaviour
{
    [SerializeField] private Transform[] skillImages;//スキル画像を入れる
    [SerializeField] private Transform selectionFrame;//強調用のフレーム

    private int currentIndex = 0;

    private void Start()
    {
        UpdateSelection();
    }

    private void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            currentIndex--;

            if (currentIndex < 0)
                currentIndex = skillImages.Length - 1;

            UpdateSelection();
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
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