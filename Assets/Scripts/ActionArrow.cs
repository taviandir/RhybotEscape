using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionArrow : MonoBehaviour
{
    public Sprite arrowHighlight;

    private Sprite arrowNormal;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        arrowNormal = sr.sprite;
    }

    public void SetHighlight(bool highlight)
    {
        sr.sprite = highlight ? arrowHighlight : arrowNormal;
    }
}
