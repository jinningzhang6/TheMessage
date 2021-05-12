using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameAnimation : MonoBehaviourPunCallbacks
{
    public GameObject InGameObjects;
    public GameObject passingCardLive;
    public GameObject assigningCardLive;
    public GameObject cardDeck;

    private InGame inGame;
    private Sprite[] passingCardBck;
    private Vector3 startingDeckPos;

    // Start is called before the first frame update
    void Start()
    {
        inGame = InGameObjects.GetComponent<InGame>();
        startingDeckPos = cardDeck.transform.position;
        passingCardBck = new CardAssets().getCardBackground();
        passingCardLive.SetActive(false);
        assigningCardLive.SetActive(false);
        passingCardLive.GetComponent<Image>().sprite = passingCardBck[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (inGame.shouldAnimatePassingCard)
        {
            passingCardLive.SetActive(true);
            passingCardLive.transform.position = Vector3.Lerp(passingCardLive.transform.position, inGame.passingCardPosition, 5 * Time.deltaTime);
        }
        else
        {
            passingCardLive.SetActive(false);
        }
        if (inGame.shouldAnimateAssigningCard)
        {
            assigningCardLive.SetActive(true);
            assigningCardLive.transform.position = Vector3.Lerp(assigningCardLive.transform.position, inGame.assigningCardPosition, 5 * Time.deltaTime);
        }
        else
        {
            assigningCardLive.SetActive(false);
            assigningCardLive.transform.position = startingDeckPos;
        }
    }

    public void drawCard()//0 -> drawCardEventCode
    {
        object[] content = new object[] { (int)inGame.playerSequencesByName[$"{PhotonNetwork.LocalPlayer.NickName}"],1};
        inGame.raiseCertainEvent(0,content);
    }
}
