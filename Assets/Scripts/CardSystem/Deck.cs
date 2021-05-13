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
        for (; id < 6; id++)//Adding blue锁定卡片
        {
            deck.Add(new Card(id, 1, 0, 0, spellCards[0],0));//直达 密电 文本
        }

        for (; id < 9; id++)//Adding blue_black锁定卡片
        {
            deck.Add(new Card(id, 1, 0, 1, spellCards[1],0));
        }

        for (; id < 15; id++)//Adding red锁定卡片
        {
            deck.Add(new Card(id, 0, 1, 0, spellCards[2],0));
        }

        for (; id < 18; id++)//Adding red_black锁定卡片
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
        deck.Add(new Card(id++, 0, 0, 1, spellCards[15], 4));//gamble_black 2
        deck.Add(new Card(id++, 0, 0, 1, spellCards[15], 4));
        deck.Add(new Card(id++, 0, 1, 0, spellCards[16], 5));//intercept_red 2
        deck.Add(new Card(id++, 0, 1, 0, spellCards[16], 5));
        deck.Add(new Card(id++, 0, 1, 1, spellCards[17], 5));//intercept_red_black 1
        deck.Add(new Card(id++, 0, 0, 1, spellCards[18], 5));//intercept_black 5
        deck.Add(new Card(id++, 0, 0, 1, spellCards[18], 5));
        deck.Add(new Card(id++, 0, 0, 1, spellCards[18], 5));
        deck.Add(new Card(id++, 0, 0, 1, spellCards[18], 5));
        deck.Add(new Card(id++, 0, 0, 1, spellCards[18], 5));
        deck.Add(new Card(id++, 1, 0, 0, spellCards[19], 5));//intercept_blue 2
        deck.Add(new Card(id++, 1, 0, 0, spellCards[19], 5));
        deck.Add(new Card(id++, 1, 0, 1, spellCards[20], 5));//intecept_blue_black 1
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
