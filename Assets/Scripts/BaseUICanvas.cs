using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BaseUICanvas : MonoBehaviour
{
    public bool fading = false;
    public float fadeSpeed;
    public CanvasGroup canvasGroup;

    public IEnumerator FadeIn() {
        fading = true;
        float t = 0;
        while (t < 1.0) {
            t += Time.deltaTime / fadeSpeed;
            canvasGroup.alpha = t;
            yield return null;
        }
        fading = false;
    }

    public IEnumerator FadeOut() {
        fading = true;
        float t = 0;
        while (t < 1.0) {
            t += Time.deltaTime / fadeSpeed;
            canvasGroup.alpha = 1 - t;
            yield return null;
        }
        fading = false;
    }
}