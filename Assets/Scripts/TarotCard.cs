using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Takes a TarotCardData object and applies its properties
// to the card/board as required.

public class TarotCard : MonoBehaviour
{
    public float rotationSpeed;
    public MeshRenderer cardFront;
    public TarotCardData cardData;
    public float fadeSpeed = 1.0f;
    public bool isReversed;
    // Start is called before the first frame update
    public void Init(TarotCardData cd, bool reversed)
    {
        cardData = cd;
        cardFront.material.mainTexture = cardData.cardPicture;
        isReversed = reversed;
        if (isReversed) {
            transform.eulerAngles = new Vector3(
                transform.rotation.x,
                transform.rotation.y,
                180f
            );
        }
    }

    public IEnumerator FadeOut() {
        float t = 0;
        MeshRenderer mr = GetComponentInChildren<MeshRenderer>();
        while (t < 1.0) {
            t += Time.deltaTime / fadeSpeed;
            mr.material.color = new Color(mr.material.color.r,mr.material.color.g,mr.material.color.b, 1-t);
            yield return null;
        }
        Destroy(mr);
    }

}
