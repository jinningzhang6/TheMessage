using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    private List<Card> deck;
    private Sprite[] spellCards;

    public Deck()
    {
        deck = new List<Card>();
        spellCards = new CardAssets().getCardAssets();
        resetCards();
    }

    private void resetCards()
    {
        int id = 0;
        deck = new List<Card>();
        for (; id < 6; id++)//Adding blueËø¶¨¿¨Æ¬
        {
            deck.Add(new Card(id, 1, 0, 0, spellCards[0],0));
        }

        for (; id < 9; id++)//Adding blue_blackËø¶¨¿¨Æ¬
        {
            deck.Add(new Card(id, 1, 0, 1, spellCards[1],0));
        }

        for (; id < 15; id++)//Adding redËø¶¨¿¨Æ¬
        {
            deck.Add(new Card(id, 0, 1, 0, spellCards[2],0));
        }

        for (; id < 18; id++)//Adding red_blackËø¶¨¿¨Æ¬
        {
            deck.Add(new Card(id, 0, 1, 1, spellCards[3], 0));
        }

        for (; id < 22; id++) deck.Add(new Card(id, 0, 1, 0, spellCards[4], 1));//red       4
        for (; id < 24; id++) deck.Add(new Card(id, 0, 1, 1, spellCards[5], 1));//red_black 2
        for (; id < 26; id++) deck.Add(new Card(id, 0, 0, 1, spellCards[6], 1));//black     2
        for (; id < 30; id++) deck.Add(new Card(id, 1, 0, 0, spellCards[7], 1));//blue      4
        for (; id < 32; id++) deck.Add(new Card(id, 1, 0, 1, spellCards[8], 1));//blue_black 2
        deck.Add(new Card(id++, 0, 1, 0, spellCards[9], 2));//help_red 1
        deck.Add(new Card(id++, 0, 0, 1, spellCards[10], 2));//help_black 2
        deck.Add(new Card(id++, 0, 0, 1, spellCards[10], 2));
        deck.Add(new Card(id++, 1, 0, 0, spellCards[11], 2));//help_blue 1
        deck.Add(new Card(id++, 0, 1, 0, spellCards[12], 3));
        deck.Add(new Card(id++, 0, 1, 0, spellCards[12], 3));//redirect 2
        deck.Add(new Card(id++, 0, 0, 1, spellCards[13], 3));
        deck.Add(new Card(id++, 1, 0, 0, spellCards[14], 3));
        deck.Add(new Card(id++, 1, 0, 0, spellCards[14], 3));
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
