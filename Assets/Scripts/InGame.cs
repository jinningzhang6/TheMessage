using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class InGame : ServerCommand, IOnEventCallback
{
    public GameObject[] playerUIs, newPlayerUIS;
    public GameObject[] passingCardUIs, newPassingCardUIs;
    public GameObject[] playerReceiveCardUIs;
    public static GameObject[] newPlayerReceiveCardUIs;
    public GameObject turnPrompt, sendMessageButton, endTurnButton, incomingManipulation, useSpellButton, cancelSpellButton, MainSceneObjects, cardDeck;
    public Text playerTurnUI, cardsLeftUI, reminderText;

    public GameObject openCard, assignCardObject, burnCardWindow;
    //private InGameCommand inGameCommand;
    public CardListing cardListing;
    private BurnCardListing burnCardListing;
    public Player currentTurnPlayer;
    public Vector3 passingCardPosition, assigningCardPosition, assignCardStartingPosition;

    public bool shouldAnimatePassingCard,shouldAnimateAssigningCard;
    public bool usingSpell;
    private int subTurnCount;
    private int currentCardId;

    private const int DrawCardEventCode = 0;
    private const int TurnStartEventCode = 1;
    private const int SendCardEventCode = 2;
    private const int SpellCardEventCode = 3;
    private const int ToEndTurnEventCode = 4;
    private const int CardsLeftEventCode = 5;

    protected override void Awake()
    {
        base.Awake();
        subTurnCount = 0;
        currentCardId = -1;
        usingSpell = false;
        currentTurnPlayer = null;
        shouldAnimatePassingCard = false;
        shouldAnimateAssigningCard = false;
        incomingManipulation.SetActive(false);
        sendMessageButton.SetActive(false);
        endTurnButton.SetActive(false);
        cancelSpellButton.SetActive(false);
        reminderText.gameObject.SetActive(false);
        //inGameCommand = MainSceneObjects.GetComponent<InGameCommand>();
        cardListing = MainSceneObjects.GetComponent<CardListing>();
        burnCardListing = MainSceneObjects.GetComponent<BurnCardListing>();
        assignCardObject.GetComponent<Image>().sprite = new CardAssets().getCardBackground()[1];
    }

    void Start() {
        populateEachPlayerUI(playersCount);
        assignPlayerPosition(newPlayerUIS);
        turnStartDistrubuteCards();
        assignCardStartingPosition = cardDeck.transform.position;
        if (PhotonNetwork.IsMasterClient) startGame();
    }

    private void populateEachPlayerUI(int count)
    {
        newPlayerUIS = new GameObject[count];
        newPassingCardUIs = new GameObject[count];
        newPlayerReceiveCardUIs = new GameObject[count];
        HashSet<int> set = new HashSet<int>();
        if (count < 7)
        {
            Destroy(playerUIs[2].gameObject); Destroy(playerUIs[6].gameObject);
            Destroy(passingCardUIs[2].gameObject); Destroy(passingCardUIs[6].gameObject);
            Destroy(playerReceiveCardUIs[2].gameObject); Destroy(playerReceiveCardUIs[6].gameObject);
            set.Add(2); set.Add(6);
        }
        if (count < 5)
        {
            Destroy(playerUIs[3].gameObject); Destroy(playerUIs[5].gameObject);
            Destroy(passingCardUIs[3].gameObject); Destroy(passingCardUIs[5].gameObject);
            Destroy(playerReceiveCardUIs[3].gameObject); Destroy(playerReceiveCardUIs[5].gameObject);
            set.Add(3); set.Add(5);
        }
        if (count == 5 || count == 7 || count == 3)
        {
            set.Add(4);
            Destroy(playerUIs[4].gameObject); Destroy(passingCardUIs[4].gameObject); Destroy(playerReceiveCardUIs[4].gameObject);
        }
        for(int i=0,j=0; i<playerUIs.Length;i++)
        {
            if (!set.Contains(i))
            {
                newPlayerUIS[j] = playerUIs[i];
                newPassingCardUIs[j] = passingCardUIs[i];
                newPlayerReceiveCardUIs[j] = playerReceiveCardUIs[i];
                
                j++;
            }
        }
        hideAllReceivingCardSection();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("playerStartDeck"))
        {
            object[] cards = (object[])changedProps["playerStartDeck"];
            newPlayerUIS[(int)playerPositions[targetPlayer]].GetComponentsInChildren<Text>()[0].text = $"{cards.Length}";
        }
        if (changedProps.ContainsKey("playerBlueMessage"))
        {
            int msgs = (int)changedProps["playerBlueMessage"];
            newPlayerUIS[(int)playerPositions[targetPlayer]].GetComponentsInChildren<Text>()[1].text = $"{msgs}";
        }
        if (changedProps.ContainsKey("playerRedMessage"))
        {
            int msgs = (int)changedProps["playerRedMessage"];
            newPlayerUIS[(int)playerPositions[targetPlayer]].GetComponentsInChildren<Text>()[2].text = $"{msgs}";
        }
        if (changedProps.ContainsKey("playerBlackMessage"))
        {
            int msgs = (int)changedProps["playerBlackMessage"];
            newPlayerUIS[(int)playerPositions[targetPlayer]].GetComponentsInChildren<Text>()[3].text = $"{msgs}";
        }
        if (changedProps.ContainsKey("msgStack"))
        {
            object[] playerMsgs = (object[])changedProps["msgStack"];
            int newcardId = (int)playerMsgs[playerMsgs.Length - 1];
            openCard.SetActive(true);
            openCard.GetComponentsInChildren<Image>()[1].sprite = Deck[newcardId].image;
        }
    }

    public void OnEvent(EventData photonEvent)//system triggers
    {
        byte eventCode = photonEvent.Code;

        if(eventCode == DrawCardEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            Player drawCardPlayer = (Player)playerSequences[$"{(int)data[0]}"];
            turnPrompt.GetComponent<Text>().text = $"玩家[{drawCardPlayer.NickName}]抽了一张牌";
            StopAllCoroutines();
            StartCoroutine(executeCodeAfterSeconds(2, drawCardPlayer));
            if (!PhotonNetwork.IsMasterClient) return;
            int requestedCards = (int)data[1];
            DrawCardsForPlayer(drawCardPlayer,requestedCards);
        }
        else if (eventCode == TurnStartEventCode)
        {
            openCard.SetActive(false);
            object[] data = (object[])photonEvent.CustomData;// turn?
            turnCount = (int)data[0] % playersCount;
            subTurnCount = turnCount;
            currentTurnPlayer = (Player)playerSequences[$"{turnCount}"];// 0 -> nick, 1 -> bill, 2 -> Eugene
            playerTurnUI.text = currentTurnPlayer.NickName;
            turnPrompt.GetComponent<Text>().text = $"玩家[{currentTurnPlayer.NickName}]的回合";
            if (currentTurnPlayer.IsLocal) sendMessageButton.SetActive(true);//show / hide
            else sendMessageButton.SetActive(false);
            passingCardPosition = newPassingCardUIs[(int)playerPositions[currentTurnPlayer]].transform.position;
            shouldAnimatePassingCard = false;
        }
        else if (eventCode == SendCardEventCode)
        {
            shouldAnimatePassingCard = true;
            object[] data = (object[])photonEvent.CustomData;
            subTurnCount = (int)data[0] % playersCount;
            currentCardId = (int)data[1];
            Player playerToStart = (Player)playerSequences[$"{subTurnCount}"];
            turnPrompt.GetComponent<Text>().text = $"等待玩家[{playerToStart.NickName}]的回复";
            if (playerToStart.IsLocal) incomingManipulation.SetActive(true);
            else incomingManipulation.SetActive(false);
            passingCardPosition = newPassingCardUIs[(int)playerPositions[playerToStart]].transform.position;
        }
        else if(eventCode == ToEndTurnEventCode)
        {
            Player playerToStart = (Player)playerSequences[$"{turnCount}"];
            turnPrompt.GetComponent<Text>().text = $"玩家[{((Player)playerSequences[$"{subTurnCount}"]).NickName}]收到了情报";
            if (playerToStart!=null && playerToStart.IsLocal) endTurnButton.SetActive(true);
            currentCardId = -1;
            shouldAnimatePassingCard = false;
        }
        else if(eventCode == CardsLeftEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            int cardsLeft = (int)data[0];
            cardsLeftUI.text = $"{cardsLeft}";
        }
    }

    /* Command Section*/
    public void sendCard()
    {
        if (CardListing.selectedCard == null && currentCardId==-1) return;//selectedCard->I sent, currentCardId->OtherPlayer sent
        if (currentCardId==-1) currentCardId = CardListing.selectedCard.cardId;
        raiseCertainEvent(SendCardEventCode, new object[] { ++subTurnCount, currentCardId });
        cardListing.removeSelectedCardFromHand();
        incomingManipulation.SetActive(false);
        sendMessageButton.SetActive(false);
        CardListing.selectedCard = null;
    }

    //aimingbot -> ingame.useSpellCard
    public void useSpellCard(int cardId, int cardType, string playerNickName)
    {
        if (!isCastedPlayerAllowed(cardType, (int)playerSequencesByName[playerNickName], subTurnCount)) return;
        object[] content = new object[] {(int)playerSequencesByName[PhotonNetwork.LocalPlayer.NickName],cardId, (int)playerSequencesByName[playerNickName], cardType};
        cardListing.removeSelectedCardFromHand();
        raiseCertainEvent(SpellCardEventCode, content);
        CardListing.selectedCard = null;
    }

    public void acceptCard()
    {
        if (currentCardId == -1) return;
        assignMessage(PhotonNetwork.LocalPlayer,currentCardId);
        incomingManipulation.SetActive(false);
        raiseCertainEvent(ToEndTurnEventCode, new object[] { currentCardId });
    }

    public void endTurn()
    {
        resetUserDebuff();
        ++turnCount;
        startGame();
        endTurnButton.SetActive(false);
    }

    //点击了 'use' button
    //spell 0->锁定 1->调虎离山 2->增援 3->转移
    public void useSpell()
    {
        if (CardListing.selectedCard == null) return;
        int type = CardListing.selectedCard.cardType;// lock? redirect?
        if (!isPlayerCastAllowed(type, subTurnCount, currentCardId))// 不允许操作 -》在别人turn里面 使用了 锁定/增援
        {
            reminderText.text = $"当前技能 [{spellCardsName[CardListing.selectedCard.cardType]}] 无法使用";
            reminderText.gameObject.SetActive(true);
            return;
        }
        if (type == 2 || type == 5)//直接触发条件 -> 增援 可直接触发 不需要点击任何头像
        {
            useSpellCard(CardListing.selectedCard.cardId, CardListing.selectedCard.cardType, PhotonNetwork.LocalPlayer.NickName);
            return;
        }

        //type==0, 1, 3, waiting for user to choose target
        reminderText.gameObject.SetActive(false);
        useSpellButton.SetActive(false); 
        cancelSpellButton.SetActive(true);
        usingSpell = true;
    }

    public void cancelSpell()
    {
        cancelSpellButton.SetActive(false);
        reminderText.gameObject.SetActive(false);
        useSpellButton.SetActive(true);
        usingSpell = false;
    }

    public int getSubTurn() { return subTurnCount; }

    public int getCurrentCardId() { return currentCardId; }

    public void showBurnCardWindow(string playerName)
    {
        int sequence = (int)playerSequencesByName[playerName];
        burnCardListing.AddMsgCard((Player)playerSequences[$"{sequence}"]);
        burnCardWindow.SetActive(true);
    }

    public void hideBurnCardWindow()
    {
        burnCardListing.ResetBurnCardListing();
        burnCardWindow.SetActive(false);
    }

    public static void showAllReceivingCardSection()
    {
        foreach(GameObject gameObject in newPlayerReceiveCardUIs) gameObject.SetActive(true);
    }

    public static void hideAllReceivingCardSection()
    {
        foreach (GameObject gameObject in newPlayerReceiveCardUIs) gameObject.SetActive(false);
    }

    IEnumerator executeCodeAfterSeconds(int secs, Player player)
    {
        shouldAnimateAssigningCard = false;
        yield return new WaitForSeconds(0.1f);
        shouldAnimateAssigningCard = true;
        assigningCardPosition = newPassingCardUIs[(int)playerPositions[player]].transform.position;
        yield return new WaitForSeconds(secs);
        shouldAnimateAssigningCard = false;
    }
}
