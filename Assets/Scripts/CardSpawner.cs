using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    public GameObject cardPrefab;

    public int rows = 4;
    public int cols = 7;

    public float spacingX = 2.0f;
    public float spacingY = 2.5f;

    void Start()
    {
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                Vector3 pos = new Vector3(
                    (x - 3) * spacingX,
                    (1.5f - y) * spacingY,
                    0
                );

                GameObject card = Instantiate(
                    cardPrefab,
                    pos,
                    Quaternion.identity
                );
            }
        }
    }
}