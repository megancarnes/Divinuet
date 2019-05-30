using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ornamental card used for deck-shuffling

public class DeckCard : MonoBehaviour
{
    public float fadeSpeed = 1.0f;

    public IEnumerator FadeOut() {
        float t = 0;
        MeshRenderer[] mrs = GetComponentsInChildren<MeshRenderer>();
        while (t < 1.0) {
            t += Time.deltaTime / fadeSpeed;
            foreach(MeshRenderer mr in mrs) {
                mr.material.color = new Color(mr.material.color.r,mr.material.color.g,mr.material.color.b, 1-t);
            }
            yield return null;
        }
        foreach(MeshRenderer mr in mrs) {
            Destroy(mr);
        }
    }
}
