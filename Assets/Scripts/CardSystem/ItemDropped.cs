using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemDropped : MonoBehaviour, IDropHandler
{
    private InGame inGame;
    public GameObject game_object;
    public void OnDrop(PointerEventData eventData)
    {
        //Debug.Log($"Dropped on this object {gameObject.name}"); 
        //Text[] texts = gameObject.GetComponentsInChildren<Text>();
        //if (texts.Length < 5) return;
        //inGame = game_object.GetComponent<InGame>();
        //if (!inGame.currentTurnPlayer.IsLocal) return;
        //CardItem cardItem = eventData.selectedObject.GetComponent<CardItem>();
        //inGame.useSpellCard(cardItem.cardId, texts[5].text);
    }
}
