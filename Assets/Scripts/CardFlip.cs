using System;
using UnityEngine;

public class CardFlip : MonoBehaviour
{
    public Sprite frontSprite;  // ï\ñ 
    public Sprite backSprite;   // ó†ñ 
    private SpriteRenderer sr;
    private bool isFront = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = backSprite; // èâä˙èÛë‘ÇÕó†ñ 
    }

    void OnMouseDown()
    {
        if (isFront)
        {
            sr.sprite = backSprite;
            isFront = false;
        }
        else
        {
            sr.sprite = frontSprite;
            isFront = true;
        }
    }

    internal void FlipBack()
    {
        throw new NotImplementedException();
    }
}
