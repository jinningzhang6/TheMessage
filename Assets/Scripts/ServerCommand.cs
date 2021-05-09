using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using Photon.Pun;
using System.Linq;

public class ServerCommand : MonoBehaviourPunCallbacks
{
    private string[] playerMessages = new string[] { "playerBlueMessage", "playerRedMessage", "playerBlackMessage" };
    protected string[] spellCardsName = new string[] { "锁定", "调虎离山", "增援", "redirect", "gamble", "intercept" };

    private List<int> serverDeck;
    public List<Card> Deck;
    public Hashtable playerSequences;
    public Hashtable playerSequencesByName;
    public Hashtable playerPositions;

    public int playersCount;
    public int turnCount;

    private const int TurnStartEventCode = 1;

    protected virtual void Awake()
    {
        playersCount = PhotonNetwork.CurrentRoom.PlayerCount;
        turnCount = 0;
        Deck = new Deck().getDeck(); playerSequences = new Hashtable();
        playerSequencesByName = new Hashtable(); playerPositions = new Hashtable();
        PhotonNetwork.AddCallbackTarget(this);//+ listener -> notified
        if(PhotonNetwork.IsMasterClient) serverDeck = new SystemDeck().getDeck();
    }

    protected void startGame() { raiseCertainEvent(TurnStartEventCode, new object[] { turnCount });}

    protected void assignPlayerPosition(GameObject[] newPlayerUIS)
    {
        int sequence = (int)PhotonNetwork.CurrentRoom.CustomProperties["sequence"];
        int pos = -1;
        for (int i = 0; i < playersCount; i++)
        {
            Player player = (Player)PhotonNetwork.CurrentRoom.CustomProperties[$"{sequence + i}"];
            if (player == null) continue;
            if (player.IsLocal) pos = i;
            playerSequences.Add($"{i}", player);// nick 0, bill 1, eugene 2 //table -> (key,value)
            playerSequencesByName.Add(player.NickName, i);
        }
        if (pos == -1) return;
        int originalPos = pos;
        int positionIndex = 0;
        bool passed = false;

        while (positionIndex < playersCount)
        {
            int posIndex = pos % playersCount;
            if (passed && posIndex == originalPos) break;
            Player player = (Player)PhotonNetwork.CurrentRoom.CustomProperties[$"{(posIndex + sequence)}"];
            newPlayerUIS[positionIndex].GetComponentsInChildren<Text>()[5].text = player.NickName;
            playerPositions.Add(player, positionIndex);
            positionIndex++;
            pos++;
            if (!passed) passed = true;
        }
    }

    protected void turnStartDistrubuteCards()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        foreach (Player player in playerSequences.Values) DrawCardsForPlayer(player, 3);
    }

    public void DrawCardsForPlayer(Player player, int cardsNumToDraw)
    {
        Hashtable table = player.CustomProperties;//
        if (table == null) table = new Hashtable();

        object[] existingCards;
        object[] startDeck;
        if (table.ContainsKey("playerStartDeck")) existingCards = (object[])table["playerStartDeck"];
        else existingCards = new object[0];

        startDeck = new object[cardsNumToDraw + existingCards.Length];
        int i = 0;
        for (; i < existingCards.Length; i++) startDeck[i] = existingCards[i];
        for (; i < startDeck.Length; i++)
        {
            if (serverDeck.Count == 0) serverDeck = new SystemDeck().getDeck();
            startDeck[i] = serverDeck.LastOrDefault();
            serverDeck.Remove(serverDeck.LastOrDefault());
        }

        if (table.ContainsKey("playerStartDeck")) table["playerStartDeck"] = startDeck;
        else table.Add("playerStartDeck", startDeck);
        player.SetCustomProperties(table);//table(key,value) "playerStartDeck" = startDeck //(id7,id8,id10,id3);
    }

    public void assignMessageForPlayer(Player player, int cardId)
    {
        // if cardId == -1, assign random message from systemdeck
        if (cardId == -1)
        {
            Debug.Log($"SystemDeck count: {serverDeck.Count}");
            if (serverDeck.Count == 0) serverDeck = new SystemDeck().getDeck();
            cardId = serverDeck.LastOrDefault();
            Debug.Log($"Getting cardId: { cardId }");
        }
        // get message color combination
        int blueColor = Deck[cardId].blue;
        int redColor = Deck[cardId].red;
        int blackColor = Deck[cardId].black;

        Debug.Log($"Assigned {blueColor}, {redColor}, {blackColor}");
        // assign message according to color
        if (blueColor == 1) assignMessage(player, 0);
        if (redColor == 1) assignMessage(player, 1);
        if (blackColor == 1) assignMessage(player, 2);
    }

    // Overload function to assignMessage for specific player
    private void assignMessage(Player player, int i)
    {
        Hashtable table = getPlayerHashTable(player);
        if (!table.ContainsKey(playerMessages[i])) table.Add(playerMessages[i], 1);
        else
        {
            int existingM = (int)table[playerMessages[i]];
            table[playerMessages[i]] = existingM + 1;
        }
        player.SetCustomProperties(table);
    }

    protected void assignMessage(int i)
    {
        Hashtable table = getPlayerHashTable(PhotonNetwork.LocalPlayer);
        if (!table.ContainsKey(playerMessages[i])) table.Add(playerMessages[i], 1);
        else
        {
            int existingM = (int)table[playerMessages[i]];
            table[playerMessages[i]] = existingM + 1;
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(table);
    }

    // 相关信息 Eugene！
    protected bool isPlayerCastAllowed(int type, int subTurn, int currentCardId)//必须在自己的回合 使用的卡片
    {
        if(type==0 || type == 2 || type == 4)//锁定 增援
        {
            if (!((Player)playerSequences[$"{turnCount}"]).IsLocal) return false;
        }
        if(type==3 && subTurn!= (int)playerSequencesByName[$"{PhotonNetwork.LocalPlayer.NickName}"]) return false;
        if (type == 5)
        {
            Debug.Log(currentCardId);
            // return false if there is no sending message card
            if (currentCardId == -1)
            {
                return false;
            }
            // return true if this is other player's turn, false if this is casting player's turn
            return !(((Player)playerSequences[$"{turnCount}"]).IsLocal);
        }
        return true;
    }

    protected bool isCastedPlayerAllowed(int type, int player,int subTurn)//点击用户图片后 系统确认是否可以触发
    {
        if (type == 1 && player == turnCount) return false;//调虎离山不能给发送者
        if (type == 3 && player == subTurn)
        {
            Debug.Log($"cannot cast in [isCastedPlayerAllowed] type: {type} player int : {player}, subTurn: {subTurn}");
            return false;//必须到此人面前 才能使用[转移]
        }

        return checkPriority(type, (Player)playerSequences[$"{player}"]);
    }

    public void resetUserDebuff()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Hashtable table = getPlayerHashTable(player);
            if (table.ContainsKey("locked") && (bool)table["locked"]) table["locked"] = false;
            if (table.ContainsKey("awayed") && (bool)table["awayed"]) table["awayed"] = false;
            if (table.ContainsKey("redirected") && (bool)table["redirected"]) table["redirected"] = false;
            player.SetCustomProperties(table);
        }
    }

    public int getPlayerBlackMessage()
    {
        Hashtable table = getPlayerHashTable(PhotonNetwork.LocalPlayer);
        int numBlacks = table.ContainsKey("playerBlackMessage") ? (int)table["playerBlackMessage"] : 0;
        return numBlacks;
    }

    protected Hashtable getPlayerHashTable(Player player)
    {
        Hashtable table = player.CustomProperties;
        if (table == null) table = new Hashtable();
        return table;
    }

    private bool checkPriority(int type, Player player)//1
    {
        Hashtable table = getPlayerHashTable(player);
        if (type == 0)
        {
            if (table.ContainsKey("redirected") && (bool)table["redirected"]) return false;
        }
        else if (type==1)
        {
            if (table.ContainsKey("redirected") && (bool)table["redirected"]) return false;
            if (table.ContainsKey("locked") && (bool)table["locked"]) return false;
        }
        
        return true;
    }

    public void raiseCertainEvent(byte eventCode, object[] content)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };//Event
        PhotonNetwork.RaiseEvent(eventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}
