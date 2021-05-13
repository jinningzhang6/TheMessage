using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using ExitGames.Client.Photon;

public class DropCardItem : MonoBehaviourPunCallbacks, IDropHandler
{
    private InGame inGame;
    public GameObject game_object;
    private List<int> droppedCards;

    public GameObject[] gameObjects;

    private const int DropCardEventCode = 7;

    void Start()
    {
        gameObjects = new GameObject[5];
        inGame = game_object.GetComponent<InGame>();
        droppedCards = new List<int>();
        PhotonNetwork.AddCallbackTarget(this);
    }

    public void OnDrop(PointerEventData eventData)
    {
        CardItem cardItem = eventData.selectedObject.GetComponent<CardItem>();
        inGame.raiseCertainEvent(DropCardEventCode, new object[] { cardItem.cardId });
    }

    public void OnEvent(EventData photonEvent)//system triggers
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == DropCardEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            int card = (int)data[0];
            addDropCards(card);
        }
    }

    public void addDropCards(int cardId)
    {
        droppedCards.Add(cardId);
    }

    public void removeCards(int cardId)
    {
        int index = droppedCards.FindIndex(x => x == cardId);
        if (index != -1) droppedCards.RemoveAt(index);
        manipulateDeckUI();
    }

    public int getDroppedCards()
    {
        return droppedCards.Count;
    }

    private void manipulateDeckUI()
    {
        int count = droppedCards.Count;

        foreach(GameObject object1 in gameObjects)
        {
            object1.SetActive(true);
        }

        if (count < 100) gameObjects[5].SetActive(false);
        if (count < 50) gameObjects[4].SetActive(false);
        if (count < 10) gameObjects[3].SetActive(false);
        if (count < 2) gameObjects[2].SetActive(false);
    }

    void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}
