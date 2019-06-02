using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GenerativeUI : BaseUICanvas
{
    public CanvasGroup textCanvasGroup;
    public TextMeshProUGUI text;    public bool reading = false;
    public float totalReadingTime = 5.0f;
    public float backgroundFadeSpeed = 1.0f;
    public ParticleSystem ps;

    public void Start() {
        ps.gameObject.SetActive(false);
        ReadingUtils.HideAllCharacters(text);
    }
    public IEnumerator ReadText() {
        reading = true;
        yield return ReadingUtils.ReadText(text, totalReadingTime);
        reading = false;
    }

    public IEnumerator DoGeneration(List<TarotCard> cards) {
        yield return StartCoroutine(FadeOut());
        Color particleColor = new Color32();
        particleColor.r = GetColorFromCardOrder(cards[0].cardData.order);
        particleColor.g = GetColorFromCardOrder(cards[1].cardData.order);
        particleColor.b = GetColorFromCardOrder(cards[2].cardData.order);
        particleColor.a = 255;
        ParticleSystem.MainModule ma = ps.main;
        ma.startColor = particleColor;
        ps.gameObject.SetActive(true);
    }

    public void Reset() {
        // textCanvasGroup.alpha = 0;
        text.maxVisibleCharacters = 0;
        ps.gameObject.SetActive(false);
    }

    float GetColorFromCardOrder(int order) {
        return order < 7 ? order * 3 : (order + 78) * 3;
    }
}