using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class BurnCardListing : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform content;
    [SerializeField]
    private CardItem _cardListing;

    private Deck deck;
    private List<CardItem> listing;
    private List<Card> normalDeck;

    public static CardItem selectedBurnCard;

    // Start is called before the first frame update
    void Start()
    {
        deck = new Deck();
        listing = new List<CardItem>();
        normalDeck = deck.getDeck();
    }

    public void AddMsgCard(Player player)
    {
        Hashtable table = getPlayerHashTable(player);
        if (!table.ContainsKey("msgStack")) return;
        object[] receivedMsgs = (object[])table["msgStack"];

        foreach(object each in receivedMsgs)
        {
            int id = (int)each;
            int index = listing.FindIndex(x => x.cardId == id);
            if (index != -1) continue;
            CardItem newCard = Instantiate(_cardListing, content);
            newCard.SetCardInfo(normalDeck[id].id, normalDeck[id].image, normalDeck[id].type);
            listing.Add(newCard);
        }
    }

    private Hashtable getPlayerHashTable(Player player)
    {
        Hashtable table = player.CustomProperties;
        if (table == null) table = new Hashtable();
        return table;
    }

    public void ResetBurnCardListing()
    {
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }
        listing.Clear();
    }
}
