using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIFadeIn:MonoBehaviour{
    public CanvasGroup canvasGroup;

    public float fadeSpeed = 1f;

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if(canvasGroup.alpha > 0){
            float newAlpha = Mathf.Lerp(canvasGroup.alpha, 0f, Time.deltaTime * fadeSpeed);
            canvasGroup.alpha = newAlpha;
        }
    }
}