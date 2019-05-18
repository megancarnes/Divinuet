using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GenerativeUI : BaseUICanvas
{
    public CanvasGroup textCanvasGroup;
    public TextMeshProUGUI text;
    public Image backgroundImage;
    public bool reading = false;
    public float totalReadingTime = 5.0f;
    public float backgroundFadeSpeed = 1.0f;
    public Sprite[] backgroundSprites;
    public ParticleSystem ps;

    public void Start() {
        ps.gameObject.SetActive(false);
        ReadingUtils.HideAllCharacters(text);
        backgroundImage.color = new Color(1,1,1,0);
    }
    public IEnumerator ReadText() {
        reading = true;
        yield return ReadingUtils.ReadText(text, totalReadingTime);
        reading = false;
    }

    public IEnumerator DoGeneration(List<TarotCard> cards) {
        float t = 0;
        while (t < 1.0) {
            t += Time.deltaTime / fadeSpeed;
            textCanvasGroup.alpha = 1 - t;
            yield return null;
        }
        backgroundImage.sprite = backgroundSprites[Random.Range(0, backgroundSprites.Length - 1)];
        t = 0;
        while (t < 1.0) {
            t += Time.deltaTime / backgroundFadeSpeed;
            backgroundImage.color = new Color(1,1,1, t);
            yield return null;
        }
        Color particleColor = new Color32();
        particleColor.r = GetColorFromCardOrder(cards[0].cardData.order);
        particleColor.g = GetColorFromCardOrder(cards[1].cardData.order);
        particleColor.b = GetColorFromCardOrder(cards[2].cardData.order);
        particleColor.a = 255;
        ParticleSystem.MainModule ma = ps.main;
        ma.startColor = particleColor;
        ps.gameObject.SetActive(true);
    }

    float GetColorFromCardOrder(int order) {
        return order < 7 ? order * 3 : (order + 78) * 3;
    }
}