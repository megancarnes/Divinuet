using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ornamental card used for deck-shuffling

public class DeckCard : MonoBehaviour
{
    // how long it takes to move the card from each spot to the next (units/second)
    public float shuffleSpeed;
    public float timeBetweenShuffles;
    public float timeBetweenCards;
    public Vector3[] shuffleTargets; // should physically move towards each target in sequence
    // Start is called before the first frame update
    public int nextShuffleTargetDestination;
    public Deck deck;
    private int indexInGroup;
    public float fadeSpeed = 1.0f;

    public void Init(int shuffleGroup, int idx)
    {
        indexInGroup = idx;
        StartCoroutine(Shuffle(shuffleGroup * timeBetweenShuffles));
    }

    IEnumerator Shuffle(float initialWaitTime) {
        yield return new WaitForSeconds(initialWaitTime);
        Vector3 origin = transform.position;
        float t;
        while (deck.gameState == GameState.Shuffling) {
            yield return new WaitForSeconds(timeBetweenShuffles + indexInGroup * timeBetweenCards);
            nextShuffleTargetDestination = 0;
            while (nextShuffleTargetDestination < shuffleTargets.Length) {
                t = 0;
                while (t < 1) {
                    t += Time.deltaTime / shuffleSpeed;
                    transform.position = Vector3.Slerp(
                        origin,
                        shuffleTargets[nextShuffleTargetDestination],
                        t
                    );
                    yield return null;
                }
                nextShuffleTargetDestination++;
                origin = transform.position;
            }
        }
    }
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
