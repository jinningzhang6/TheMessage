using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Realtime;
using Photon.Pun;


public class ItemDropped : MonoBehaviourPunCallbacks, IDropHandler
{
    private InGame inGame;
    public GameObject game_object;
    public void OnDrop(PointerEventData eventData)
    {
        Text[] texts = gameObject.GetComponentsInChildren<Text>();
        if (texts.Length < 5) return;
        inGame = game_object.GetComponent<InGame>();
        CardItem cardItem = eventData.selectedObject.GetComponent<CardItem>();
        int playerSequel = (int)inGame.playerSequencesByName[texts[5].text];
        inGame.cardListing.removeSelectedCardFromHand(cardItem.cardId);//update no this card
        inGame.assignMessage((Player)inGame.playerSequences[$"{playerSequel}"], cardItem.cardId);//no card
        inGame.raiseCertainEvent(6, new object[] { playerSequel, cardItem.cardId });
    }
}
