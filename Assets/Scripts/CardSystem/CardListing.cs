using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;

public class CardListing : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform content;
    [SerializeField]
    private CardItem _cardListing;//control UI //3 //public GameObject object

    private Deck deck;
    private List<CardItem> myDeck;
    private List<Card> normalDeck;

    public static CardItem selectedCard;
    // Start is called before the first frame update
    void Start()
    {
        myDeck = new List<CardItem>();
        deck = new Deck();
        normalDeck = deck.getDeck();
        selectedCard = null;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)//playermessage object[] {13, 14, 15};
    {
        if (!targetPlayer.IsLocal || !changedProps.ContainsKey("playerStartDeck")) return;
        object[] cards = (object[])changedProps["playerStartDeck"];//5
        
        foreach (int id in cards)
        {
            int index = myDeck.FindIndex(x => x.cardId == id);
            if (index != -1) continue;
            
            CardItem newCard = Instantiate(_cardListing,content);//object
            
            if (newCard != null)
            {
                newCard.SetCardInfo(normalDeck[id].id, normalDeck[id].image, normalDeck[id].type);
                myDeck.Add(newCard);
            }
        }
        selectedCard = null;
    }


    public void removeSelectedCardFromHand()
    {
        if (selectedCard == null) return;
        Debug.Log($"selectedCardId: {selectedCard.cardId}");
        int index = myDeck.FindIndex(x => x.cardId == selectedCard.cardId);
        if (index != -1)
        {
            Destroy(myDeck[index].gameObject);
            myDeck.RemoveAt(index);
        }

        updateHandCards();
    }

    public void removeSelectedCardFromHand(int cardId)
    {
        int index = myDeck.FindIndex(x => x.cardId == cardId);
        if (index != -1)
        {
            Destroy(myDeck[index].gameObject);
            myDeck.RemoveAt(index);
        }

        updateHandCards();
    }

    private void updateHandCards()
    {
        Hashtable table = PhotonNetwork.LocalPlayer.CustomProperties;//
        if (table == null) table = new Hashtable();

        object[] newDeck = new object[myDeck.Count];
        for (int i = 0; i < newDeck.Length; i++)
        {
            newDeck[i] = myDeck[i].cardId;
        }

        if (table.ContainsKey("playerStartDeck")) table["playerStartDeck"] = newDeck;
        else table.Add("playerStartDeck", newDeck);
        PhotonNetwork.LocalPlayer.SetCustomProperties(table);
    }

    public void removeAllCards()//????
    {
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }
        myDeck.Clear();
        Hashtable table = PhotonNetwork.LocalPlayer.CustomProperties;//
        if (table == null) table = new Hashtable();
        if (table.ContainsKey("playerStartDeck")) table["playerStartDeck"] = new object[] { };
        else table.Add("playerStartDeck", new object[] { });
        PhotonNetwork.LocalPlayer.SetCustomProperties(table);
    }


}
