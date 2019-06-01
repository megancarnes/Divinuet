using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Stores individual card data.
// Create more of these to add new cards to the deck!

public enum CardSuit { MajorArcana, Cups, Swords, Pentacles, Wands }
public class CardGeneratedEffects : ScriptableObject {
    public Color color;
}

// [CreateAssetMenu(fileName = "New TarotCardData", menuName = "Tarot Card Data", order = 51)] // adds menu option to create new card data
public class TarotCardData : ScriptableObject
{
    public string cardName;
    public int order;
    public CardSuit suit;
    public Texture2D cardPicture;
    public Sprite cardPicture2x;
    [TextArea]
    public string cardTextMain;
    [TextArea]
    public string cardTextUpright;
    [TextArea]
    public string cardTextReversed;

    #if UNITY_EDITOR
    // The following is a helper that adds a menu item to create an TarotCardData Asset
        [MenuItem("Assets/Create/TarotCardData")]
        public static void CreateTarotCardData()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Tarot Card", "New Tarot Card", "Asset", "Save Tarot Card", "Assets/Resources/CardData/");
            if (path == "")
                return;
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<TarotCardData>(), path);
        }
    #endif
}
