using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    private List<Card> deck;
    private Color[] colors;

    public Deck()
    {
        deck = new List<Card>();
        colors = new Color[] { Color.blue, Color.red, Color.black };
        if (!PhotonNetwork.IsConnected) return;
        resetCards();
    }

    private void resetCards()
    {
        int id = 0;
        deck = new List<Card>();
        foreach (Color color in colors)
        {
            for (int x = 0; x < 4; x++, id++)
            {
                deck.Add(new Card(color, id));
            }
        }
    }

    public List<Card> shuffleCards()
    {
        for (int n = deck.Count - 1; n > 0; --n)
        {
            int k = Random.Range(0,n + 1);
            Card temp = deck[n];
            deck[n] = deck[k];
            deck[k] = temp;
        }

        return deck;
    }

    public List<Card> getDeck()
    {
        return this.deck;
    }

    /*
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        Debug.Log($"[Deck | RoomPropertiesUpdate in Deck.cs]: serverDeck count: {deck.Count}");
        if (deck.Count < 100)
        {
            cardInDeck5.SetActive(false);
        }
        if (deck.Count < 50)
        {
            cardInDeck4.SetActive(false);
        }
        if (deck.Count < 10)
        {
            cardInDeck3.SetActive(false);
        }
        if (deck.Count < 2)
        {
            cardInDeck2.SetActive(false);
        }
        if (deck.Count < 1)
        {
            cardInDeck1.SetActive(false);
        }
    }*/
}
