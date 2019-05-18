using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores individual card data.
// Create more of these to add new cards to the deck!

public class CardGeneratedEffects : ScriptableObject {
    public Color color;
}

[CreateAssetMenu(fileName = "New TarotCardData", menuName = "Tarot Card Data", order = 51)] // adds menu option to create new card data
public class TarotCardData : ScriptableObject
{
    public string cardName;
    public int order;
    public Texture2D cardPicture;
    public Sprite cardPicture2x;
    [TextArea]
    public string cardTextMain;
    [TextArea]
    public string cardTextUpright;
    [TextArea]
    public string cardTextReversed;
}
