using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores a few cards and "shuffles" them for a visual effect.
// Basically just visual sugar.

public enum GameState {
    Shuffling,
    Finishing_Shuffling,
    ReadyToDeal,
    Dealing,
    ReadyToFlip,
    Flipping,
    ReadyToRead,
    Reading,
    ReadyToFadeOutReading,
    FadingOutCard,
    ReadyToBeginGenerativePhase,
    BeginningGenerativePhase,
    ReadyToDoGenerativePhase,
    GenerativePhase,
    Done
}

public class Deck : MonoBehaviour
{
    public GameObject deckCardPrefab;
    public GameObject tarotCardPrefab;
    public DeckCard[] cards;
    public int numberOfCards;

    public float shuffleSpeed;
    public float timeBetweenShuffles;
    public float shuffleDistanceFromDeck;
    public float timeBetweenCards;

    public float cardDealSpeed;
    public float cardFlipSpeed;
    public float oddsOfReversedCard = .25f;
    public GameState gameState;
    public Transform cardDealLocation;
    public Transform[] dealtCardLocations;

    public Canvas readingCanvas;
    public Canvas generativeCanvas;

    public Object[] cardsData;
    private List<int> cardsAlreadyDealt;
    private TarotCard currentCard;
    private List<TarotCard> dealtCards;
    private int numCardsAlreadyRead = 0;


    // Start is called before the first frame update
    void Start()
    {
        cardsAlreadyDealt = new List<int>();
        dealtCards = new List<TarotCard>();
        cardsData = Resources.LoadAll("CardData", typeof(TarotCardData));
        gameState = GameState.Shuffling;
        Vector3[] shuffleTargets = getShuffleTargets();
        cards = new DeckCard[numberOfCards];
        for (int i = 0; i < numberOfCards; i++) {
            cards[i] = Instantiate(deckCardPrefab).GetComponent<DeckCard>();
            cards[i].transform.position = new Vector3(
                transform.position.x,
                transform.position.y,
                transform.position.z - (i * 1f)
            );
            PopulateCardProps(cards[i], i, shuffleTargets);
        }
    }

    // Runs every frame.
    // Used in this class to handle user input.
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            switch (gameState) {
                case GameState.Shuffling:
                    gameState = GameState.ReadyToDeal; // stop shuffling
                    break;
                case GameState.ReadyToDeal:
                    gameState = GameState.Dealing; // start dealing
                    DealCard();
                    break;
                case GameState.ReadyToFlip:
                    gameState = GameState.Flipping; // start dealing
                    StartCoroutine(FlipCard());
                    break;
                case GameState.ReadyToRead:
                    gameState = GameState.Reading;
                    StartCoroutine(ReadCard());
                    break;
                case GameState.ReadyToFadeOutReading:
                    gameState = GameState.FadingOutCard;
                    StartCoroutine(FadeOutReading());
                    break;
                case GameState.ReadyToBeginGenerativePhase:
                    gameState = GameState.BeginningGenerativePhase;
                    StartCoroutine(BeginGenerativePhase());
                    break;
                case GameState.ReadyToDoGenerativePhase:
                    gameState = GameState.GenerativePhase;
                    StartCoroutine(DoGenerativePhase());
                    break;
                default:
                    break;
            }
        }
    }
    // set props on cards used for shuffling.
    // it's kind of tacky to do it this way (vs having these props be defined on the cards)
    // but the cards exist exclusively for the benefit of the deck
    // and should be subject to its whims

    void PopulateCardProps(DeckCard card, int cardIndex, Vector3[] shuffleTargets) {
            card.transform.parent = transform;
            card.shuffleTargets = shuffleTargets;
            card.shuffleSpeed = shuffleSpeed;
            card.timeBetweenShuffles = timeBetweenShuffles;
            card.timeBetweenCards = timeBetweenCards;
            card.deck = this;
            card.Init(
                (cardIndex - numberOfCards / 2) > 0 ? 1 : 0,
                Mathf.RoundToInt(Mathf.Repeat(cardIndex, numberOfCards / 2))
            );

    }

    // shuffling order:
    // - move to the bottom of the deck
    // - move to the right of the deck
    // - move up
    // - move back to the top of the deck
    Vector3[] getShuffleTargets() {
        return new Vector3[] {
            // bottom of the deck
            new Vector3 (
                transform.position.x,
                transform.position.y,
                transform.position.z - (numberOfCards * 5f)
            ),
            // to the right of the deck, still on the bottom
            new Vector3 (
                transform.position.x + shuffleDistanceFromDeck,
                transform.position.y,
                transform.position.z - (numberOfCards * 5f)
            ),
            // to the right of the deck, moved to the top
            new Vector3 (
                transform.position.x + shuffleDistanceFromDeck,
                transform.position.y,
                transform.position.z + 5f
            ),
            // top of the deck
            new Vector3 (
                transform.position.x,
                transform.position.y,
                transform.position.z + 5f
            )
        };
    }

    // instantiate card
    // populate it with random card data
    void DealCard() {
        TarotCard card = Instantiate(tarotCardPrefab).GetComponent<TarotCard>();
        dealtCards.Add(card);
        if (cardsAlreadyDealt.Count < cardsData.Length) {
            int randomCardDataIndex = Random.Range(0, cardsData.Length);
            while (cardsAlreadyDealt.Contains(randomCardDataIndex)) {
                randomCardDataIndex = Random.Range(0, cardsData.Length);
            }
            cardsAlreadyDealt.Add(randomCardDataIndex);
            bool isReversed = Random.Range(0f, 1f) < oddsOfReversedCard;
            card.Init(cardsData[randomCardDataIndex] as TarotCardData, isReversed);
            StartCoroutine(DoCardMovement(card));
        } else {
            Debug.LogError("More cards dealt than exist in data set. This shouldn't ever happen.");
        }
    }

    void FadeOutDeck() {
        Debug.Log("fading out deck...");
        foreach (DeckCard card in cards) {
            StartCoroutine(card.FadeOut());
        }
    }

    IEnumerator DoCardMovement(TarotCard dealtCard) {
        float t = 0;
        while (t < 1) {
            t += Time.deltaTime / cardDealSpeed;
            dealtCard.transform.position = Vector3.Slerp(
                transform.position,         // location of deck
                dealtCardLocations[cardsAlreadyDealt.Count - 1].position,  // next card deal location
                t
            );
            yield return null;
        }
        gameState = GameState.ReadyToDeal;
         if (cardsAlreadyDealt.Count == dealtCardLocations.Length) {
            gameState = GameState.ReadyToFlip;
        }
    }

    IEnumerator FlipCard() {
        if (numCardsAlreadyRead == 0) {
            FadeOutDeck();
        }
        float t = 0;
        float yRot = 0;
        TarotCard card = dealtCards[numCardsAlreadyRead];
        while (t < 1) {
            t += Time.deltaTime / cardFlipSpeed;
            yRot = Mathf.Lerp (
                0f,
                -180f,
                t
            );
            card.transform.eulerAngles = new Vector3(
                card.transform.eulerAngles.x,
                yRot,
                card.transform.eulerAngles.z
            );
            yield return null;
        }
        gameState = GameState.ReadyToRead;
    }

    IEnumerator ReadCard() {
        CardReadingUI readingHandler = readingCanvas.GetComponent<CardReadingUI>();
        readingHandler.Init(dealtCards[numCardsAlreadyRead]);
        yield return StartCoroutine(readingHandler.FadeIn());
        while (readingHandler.fading) {
            yield return null;
        }
        yield return StartCoroutine(readingHandler.ReadCard());
        while (readingHandler.reading) {
            yield return null;
        }
        numCardsAlreadyRead++;
        gameState = GameState.ReadyToFadeOutReading;
    }

    IEnumerator FadeOutReading() {
        CardReadingUI readingHandler = readingCanvas.GetComponent<CardReadingUI>();
        yield return StartCoroutine(readingHandler.FadeOut());
        while (readingHandler.fading) {
            yield return null;
        }
        if (numCardsAlreadyRead < dealtCards.Count) {
            gameState = GameState.ReadyToFlip;
        } else {
            gameState = GameState.ReadyToBeginGenerativePhase;
        }
    }

    IEnumerator BeginGenerativePhase() {
        foreach(TarotCard card in dealtCards) {
            StartCoroutine(card.FadeOut());
        }
        GenerativeUI generativeUI = generativeCanvas.GetComponent<GenerativeUI>();
        yield return StartCoroutine(generativeUI.FadeIn());
        while (generativeUI.fading) {
            yield return null;
        }
        yield return StartCoroutine(generativeUI.ReadText());
        gameState = GameState.ReadyToDoGenerativePhase;
    }


    IEnumerator DoGenerativePhase() {
        GenerativeUI generativeUI = generativeCanvas.GetComponent<GenerativeUI>();
        yield return StartCoroutine(generativeUI.DoGeneration(dealtCards));
    }
}
