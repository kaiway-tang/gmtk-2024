using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField] Transform HPScaler;
    [SerializeField] SpriteRenderer[] rend;
    static Color col;
    static Vector3 vect3;
    public void SetHPPercentage(float percentage, bool showBar = true)
    {
        fadeTimer = 100;

        vect3 = HPScaler.localScale;
        vect3.x = percentage;
        HPScaler.localScale = vect3;

        SetAlphas(1);
    }

    void SetAlphas(float alpha)
    {
        col = rend[0].color;
        col.a = alpha;
        rend[0].color = col;

        col = rend[1].color;
        col.a = alpha;
        rend[1].color = col;
    }

    private void Start()
    {
        SetAlphas(0);
    }

    int fadeTimer;
    private void FixedUpdate()
    {
        if (fadeTimer > 0)
        {
            fadeTimer--;
            SetAlphas(fadeTimer/100f);
        }
    }
}
