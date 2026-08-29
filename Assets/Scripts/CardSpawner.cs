using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    public GameObject cardPrefab;
    public int rows = 4;
    public int cols = 7;
    public float spacingX = 1.2f;
    public float spacingY = 1.5f;

    void Start()
    {
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                Vector3 pos = new Vector3(x * spacingX, -y * spacingY, 0);
                Instantiate(cardPrefab, pos, Quaternion.identity);
            }
        }
    }
}
