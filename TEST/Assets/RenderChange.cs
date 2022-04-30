using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderChange : MonoBehaviour
{
    public GameObject renderScreen;
    RawImage rawImage;
    [Range(0.0f, 1.0f)]
    public float fadeAlpha = 0;

    private void Start()
    {
        rawImage = renderScreen.GetComponent<RawImage>();
    }

    private void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            FadeToForest();
        }
        if (Input.GetKeyDown("o"))
        {
            FadeToOffice();
        }

        //SetFadeValue(fadeAlpha);
    }

    public void FadeToForest()
    {
        renderScreen.GetComponent<Animation>().Play("FadeToForest");
    }

    public void FadeToOffice()
    {
        renderScreen.GetComponent<Animation>().Play("FadeToOffice");
    }

    public void SetFadeValue(float alpha)
    {
        rawImage.color = new Color(1, 1, 1, alpha);
    }
}
