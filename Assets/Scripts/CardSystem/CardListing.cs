using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CardListing : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform content;
    [SerializeField]
    private CardItem _cardListing;//control UI //3

    private Deck deck;
    private List<CardItem> myDeck;
    private List<Card> normalDeck;

    public const byte TurnStartEventCode = 1;
    public const byte DrawCardEventCode = 2;
    public const byte SendCardEventCode = 3;
    // Start is called before the first frame update
    void Start()
    {
        myDeck = new List<CardItem>();
        deck = new Deck();
        normalDeck = deck.getDeck();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (!targetPlayer.IsLocal) return;
        object[] cards = (object[])changedProps["playerStartDeck"];//5
        
        foreach (int id in cards)
        {
            Debug.Log($"[CardToHand OnPlayerPropertiesUpdate]: My Card id: {id}");
            CardItem newCard = Instantiate(_cardListing,content);//object
            if (newCard != null)
            {
                newCard.SetCardInfo(normalDeck[id].id, normalDeck[id].color);
                myDeck.Add(newCard);
            }
        }
    }

    private void clearHandCards()
    {

    }

}
