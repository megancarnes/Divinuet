using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardReadingUI : BaseUICanvas
{
    // Start is called before the first frame update
    public Image cardImage;
    public TextMeshProUGUI cardText;
    public bool reading;
    public float totalReadingTime = 45.0f;

    public void Start() {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    public void Init(TarotCard card) {
        cardImage.sprite = card.cardData.cardPicture2x;
        ReadingUtils.HideAllCharacters(cardText);
        cardText.text = card.cardData.cardTextMain.ToString() + "\n\n";
        if (card.isReversed) {
            cardImage.transform.eulerAngles = new Vector3(0, 0, 180f);
            cardText.text += card.cardData.cardTextReversed.ToString();
        } else {
            cardImage.transform.eulerAngles = new Vector3(0, 0, 0);
            cardText.text += card.cardData.cardTextUpright.ToString();
        }
    }

    public IEnumerator ReadCard() {
        reading = true;
        yield return ReadingUtils.ReadText(cardText, totalReadingTime);
        reading = false;
        // int count = 0;
        // while (reading) {
        //     int visibleCount = count % (cardText.textInfo.characterCount + 1);
        //     cardText.maxVisibleCharacters = visibleCount;
        //     count += 1;
        //     if (visibleCount >= cardText.textInfo.characterCount) {
        //         reading = false;
        //     }
        //     yield return new WaitForSeconds(secondsBetweenCharacters);
        // }
    }


}
