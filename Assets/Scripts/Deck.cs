using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {
    ReadyToDeal,
    Dealing,
    DealingDone,
    FlippingCard,
    FlippingCardDone,
    ReadingCard,
    ReadingCardDone,
    FadingOutCard,
    FadingOutCardDone,
    ShowingGenerativeUI,
    ShowingGenerativeUIDone,
    GenerativePhase,
    Done
}

public class Deck : MonoBehaviour
{
    public GameObject deckCardPrefab;
    public GameObject tarotCardPrefab;
    public DeckCard[] cards;
    public int numberOfCards;

    public float cardDealSpeed;
    public float cardFlipSpeed;
    public float oddsOfReversedCard = .25f;
    public GameState gameState;
    public Transform cardDealLocation;
    public Transform[] dealtCardLocations;

    public Canvas readingCanvas;
    public Canvas generativeCanvas;

    private CardReadingUI readingUI;
    private GenerativeUI generativeUI;


    public Object[] cardsData;
    private List<int> cardsAlreadyDealt;
    private List<TarotCard> dealtCards;
    private int numCardsAlreadyRead = 0;


    // Start is called before the first frame update
    void Start()
    {
        cardsData = Resources.LoadAll("CardData", typeof(TarotCardData));
        readingUI = readingCanvas.GetComponent<CardReadingUI>();
        generativeUI = generativeCanvas.GetComponent<GenerativeUI>();
        ResetGameState();
    }

    void ClearGameState() {
        StopAllCoroutines();
        foreach (TarotCard tcard in dealtCards) {
            tcard.StopAllCoroutines();
            Destroy(tcard.gameObject);
        }
        foreach (DeckCard dcard in cards) {
            dcard.StopAllCoroutines();
            Destroy(dcard.gameObject);
        }
        numCardsAlreadyRead = 0;
        readingUI.StopAllCoroutines();
        StartCoroutine(readingUI.FadeOut());
        generativeUI.StopAllCoroutines();
        StartCoroutine(generativeUI.FadeOut());
        generativeUI.Reset();
    }

    void ResetGameState() {
        cardsAlreadyDealt = new List<int>();
        dealtCards = new List<TarotCard>();
        gameState = GameState.ReadyToDeal;
        cards = new DeckCard[numberOfCards];
        for (int i = 0; i < numberOfCards; i++) {
            cards[i] = Instantiate(deckCardPrefab).GetComponent<DeckCard>();
            cards[i].transform.position = new Vector3(
                transform.position.x,
                transform.position.y,
                transform.position.z - (i * 1f)
            );
        }
    }

    // Runs every frame.
    // Used in this class to handle user input.
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            switch (gameState) {
                case GameState.ReadyToDeal:
                    gameState = GameState.Dealing; // start dealing
                    DealCard();
                    break;
                case GameState.DealingDone:
                    gameState = GameState.FlippingCard;
                    StartCoroutine(FlipCard());
                    break;
                case GameState.FadingOutCardDone:
                    gameState = GameState.FlippingCard;
                    StartCoroutine(FlipCard());
                    break;
                case GameState.FlippingCardDone:
                    gameState = GameState.ReadingCard;
                    StartCoroutine(ReadCard());
                    break;
                case GameState.ReadingCardDone:
                    gameState = GameState.FadingOutCard;
                    StartCoroutine(FadeOutReading());
                    break;
                case GameState.ShowingGenerativeUIDone:
                    gameState = GameState.GenerativePhase;
                    DoGenerativePhase();
                    break;
                default:
                    break;
            }
        }
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
            gameState = GameState.DealingDone;
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
        gameState = GameState.FlippingCardDone;
    }

    IEnumerator ReadCard() {
        readingUI.Init(dealtCards[numCardsAlreadyRead]);
        yield return StartCoroutine(readingUI.FadeIn());
        yield return StartCoroutine(readingUI.ReadCard());
        while (readingUI.reading) {
            yield return null;
        }
        numCardsAlreadyRead++;
        gameState = GameState.ReadingCardDone;
    }

    IEnumerator FadeOutReading() {
        CardReadingUI readingUI = readingCanvas.GetComponent<CardReadingUI>();
        yield return StartCoroutine(readingUI.FadeOut());
        if (numCardsAlreadyRead < dealtCards.Count) {
            gameState = GameState.FadingOutCardDone;
        } else {
            gameState = GameState.ShowingGenerativeUI;
            yield return StartCoroutine(BeginGenerativePhase());
        }
    }

    IEnumerator BeginGenerativePhase() {
        foreach(TarotCard card in dealtCards) {
            StartCoroutine(card.FadeOut());
        }
        yield return StartCoroutine(generativeUI.FadeIn());
        yield return StartCoroutine(generativeUI.ReadText());
        gameState = GameState.ShowingGenerativeUIDone;
    }

    void DoGenerativePhase() {
        GenerativeUI generativeUI = generativeCanvas.GetComponent<GenerativeUI>();
        StartCoroutine(generativeUI.DoGeneration(dealtCards));
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void StartOver() {
        ClearGameState();
        ResetGameState();
    }
}
