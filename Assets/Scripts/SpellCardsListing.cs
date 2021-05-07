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
    private List<CardItem> listing;
    private List<Card> normalDeck;

    // Start is called before the first frame update
    void Start()
    {
        deck = new Deck();
        listing = new List<CardItem>();
        normalDeck = deck.getDeck();
    }

    public void AddSpellCard(int id)
    {
        CardItem newCard = Instantiate(_cardListing, content);
        if (newCard == null) return;
        newCard.SetCardInfo(normalDeck[id].id, normalDeck[id].image, normalDeck[id].type);
        listing.Add(newCard);
    }

    public void ResetSpellCardListing()
    {
        for(int i = 0; i < listing.Count; i++)
        {
            Destroy(listing[i].gameObject);
        }
    }

}
