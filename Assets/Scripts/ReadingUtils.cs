using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class ReadingUtils
{

    public static void HideAllCharacters(TextMeshProUGUI text) {
        text.maxVisibleCharacters = 0;
    }

    public static IEnumerator ReadText(TextMeshProUGUI text, float totalReadingTime) {
        int count = 0;
        bool reading = true;
        while (reading) {
            int visibleCount = count % (text.textInfo.characterCount + 1);
            text.maxVisibleCharacters = visibleCount;
            count += 1;
            if (visibleCount >= text.textInfo.characterCount) {
                reading = false;
            }
            yield return new WaitForSeconds(totalReadingTime / text.textInfo.characterCount);
        }
    }
}
