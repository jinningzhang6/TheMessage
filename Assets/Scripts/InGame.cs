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
    public GameObject turnPrompt, sendMessageButton, endTurnButton, incomingManipulation, useSpellButton, cancelSpellButton, MainSceneObjects;
    public Text playerTurnUI, cardsLeftUI, reminderText;

    public GameObject openCard;
    private InGameCommand inGameCommand;
    private CardListing cardListing;
    public Player currentTurnPlayer;
    public Vector3 passingCardPosition;

    public bool shouldAnimatePassingCard;
    public bool usingSpell;
    private int subTurnCount;
    private int currentCardId;

    private const int TurnStartEventCode = 1;
    private const int SendCardEventCode = 2;
    private const int SpellCardEventCode = 3;
    private const int ToEndTurnEventCode = 4;

    protected override void Awake()
    {
        base.Awake();
        subTurnCount = 0;
        currentCardId = -1;
        usingSpell = false;
        currentTurnPlayer = null;
        shouldAnimatePassingCard = false;
        incomingManipulation.SetActive(false);
        sendMessageButton.SetActive(false);
        endTurnButton.SetActive(false);
        cancelSpellButton.SetActive(false);
        reminderText.gameObject.SetActive(false);
        inGameCommand = MainSceneObjects.GetComponent<InGameCommand>();
        cardListing = MainSceneObjects.GetComponent<CardListing>();
    }

    void Start() {
        populateEachPlayerUI(playersCount);
        assignPlayerPosition(newPlayerUIS);
        turnStartDistrubuteCards();
        if (PhotonNetwork.IsMasterClient) startGame();
    }

    private void populateEachPlayerUI(int count)
    {
        newPlayerUIS = new GameObject[count];
        newPassingCardUIs = new GameObject[count];
        HashSet<int> set = new HashSet<int>();
        if (count < 7)
        {
            Destroy(playerUIs[2].gameObject); Destroy(playerUIs[6].gameObject);
            Destroy(passingCardUIs[2].gameObject); Destroy(passingCardUIs[6].gameObject);
            set.Add(2); set.Add(6);
        }
        if (count < 5)
        {
            Destroy(playerUIs[3].gameObject); Destroy(playerUIs[5].gameObject);
            Destroy(passingCardUIs[3].gameObject); Destroy(passingCardUIs[5].gameObject);
            set.Add(3); set.Add(5);
        }
        if (count == 5 || count == 7 || count == 3)
        {
            set.Add(4);
            Destroy(playerUIs[4].gameObject); Destroy(passingCardUIs[4].gameObject);
        }
        for(int i=0,j=0; i<playerUIs.Length;i++)
        {
            if (!set.Contains(i))
            {
                newPlayerUIS[j] = playerUIs[i];
                newPassingCardUIs[j++] = passingCardUIs[i];
            }
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("playerStartDeck"))
        {
            object[] cards = (object[])changedProps["playerStartDeck"];
            Debug.Log($"player{targetPlayer.NickName} propertiesupdate: card length: {cards.Length}");
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
    }

    public void OnEvent(EventData photonEvent)//system triggers
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == TurnStartEventCode)
        {
            openCard.SetActive(false);
            object[] data = (object[])photonEvent.CustomData;// turn?
            turnCount = (int)data[0] % playersCount;
            subTurnCount = turnCount;
            currentTurnPlayer = (Player)playerSequences[$"{turnCount}"];// 0 -> nick, 1 -> bill, 2 -> Eugene
            playerTurnUI.text = currentTurnPlayer.NickName;
            turnPrompt.GetComponent<Text>().text = $"{currentTurnPlayer.NickName}的回合, 抽取两张牌";
            if (currentTurnPlayer.IsLocal) sendMessageButton.SetActive(true);//show / hide
            else sendMessageButton.SetActive(false);
            if (PhotonNetwork.IsMasterClient) DrawCardsForPlayer(currentTurnPlayer, 2);
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
            turnPrompt.GetComponent<Text>().text = $"等待 {playerToStart.NickName}的回复";
            if (playerToStart.IsLocal) incomingManipulation.SetActive(true);
            else incomingManipulation.SetActive(false);
            passingCardPosition = newPassingCardUIs[(int)playerPositions[playerToStart]].transform.position;
        }
        else if(eventCode == ToEndTurnEventCode)
        {
            Player playerToStart = (Player)playerSequences[$"{turnCount}"];
            turnPrompt.GetComponent<Text>().text = $"{((Player)playerSequences[$"{subTurnCount}"]).NickName}收到了情报";
            if (playerToStart!=null && playerToStart.IsLocal) endTurnButton.SetActive(true);
            openCard.SetActive(true);
            openCard.GetComponentsInChildren<Image>()[1].sprite = Deck[currentCardId].image;
            currentCardId = -1;
            shouldAnimatePassingCard = false;
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
        raiseCertainEvent(SpellCardEventCode, content);
        cardListing.removeSelectedCardFromHand();
        CardListing.selectedCard = null;
    }

    public void acceptCard()
    {
        if (currentCardId == -1) return;
        int blueColor = Deck[currentCardId].blue, redColor = Deck[currentCardId].red, blackColor = Deck[currentCardId].black;
        if (blueColor!=0) assignMessage(0);
        if (redColor !=0) assignMessage(1);
        if (blackColor!=0)assignMessage(2);
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
        if (!isPlayerCastAllowed(type))// 不允许操作 -》在别人turn里面 使用了 锁定/增援
        {
            reminderText.text = $"You can not use {spellCardsName[CardListing.selectedCard.cardType]} in other players turn";
            reminderText.gameObject.SetActive(true);
            return;
        }
        if (type == 2)//直接触发条件 -> 增援 可直接触发 不需要点击任何头像
        {
            useSpellCard(CardListing.selectedCard.cardId, CardListing.selectedCard.cardType, PhotonNetwork.LocalPlayer.NickName);
            return;
        }

        //type==0, 1, 3
        //既满足条件 而又需要选择 target
        reminderText.gameObject.SetActive(false);// error text 消失
        useSpellButton.SetActive(false); // use button 消失
        cancelSpellButton.SetActive(true);// cancel button 显现
        usingSpell = true; // 提示user相关 animation动态
    }

    public void cancelSpell()
    {
        cancelSpellButton.SetActive(false);
        reminderText.gameObject.SetActive(false);
        useSpellButton.SetActive(true);
        usingSpell = false;
    }
}
