using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCardsListing : MonoBehaviour
{
    [SerializeField]
    private Transform content;
    [SerializeField]
    private CardItem _cardListing;

    private Deck deck;
    private List<GameObject> listing;
    private List<Card> normalDeck;

    // Start is called before the first frame update
    void Start()
    {
        deck = new Deck();
        listing = new List<GameObject>();
        normalDeck = deck.getDeck();
    }

    public void AddSpellCard(int id)
    {
        CardItem newCard = Instantiate(_cardListing, content);
        newCard.SetCardInfo(normalDeck[id].id, normalDeck[id].image, normalDeck[id].type);
        listing.Add(newCard.gameObject);
    }

    public void ResetSpellCardListing()
    {
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }
        listing.Clear();
    }

}
